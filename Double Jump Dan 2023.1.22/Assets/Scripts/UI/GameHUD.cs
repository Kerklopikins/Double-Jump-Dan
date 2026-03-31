using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameHUD: MonoBehaviour
{
    public static GameHUD Instance;
    
    [Header("Pause Menu")]
    [SerializeField] Button pauseButton;
    [SerializeField] Animator pauseAnimator;
    [SerializeField] AudioClip pauseSound;
    [SerializeField] Text pausedText;
    [SerializeField] Button[] pauseMenuButtons;

    [Header("Game Over")]
    [SerializeField] Animator gameOverAnimator;
    [SerializeField] AudioClip gameOverSound;

    [Header("Finish Level")]
    [SerializeField] Text levelNameText;
    [SerializeField] GameObject levelCompleteUI;

    [Header("Screenshot")]
    [SerializeField] Screenshot screenshotScript;

    [Header("Settings")]
    [SerializeField] GameObject settings;

    public bool paused { get; protected set; }
    public bool canPause { get; set; }
    public static float referenceTime;
    float startDelay = 1;
    Player player;
    LocalWorldManager localWorldManager;
    RectTransform pauseButtonRect;
    bool inSettings;
    bool canToggleSettings;
    
    void Awake()
    {
        Instance = this;
        referenceTime = 1;
    }

    void Start()
    {
        canPause = true;
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        levelNameText.text = SceneManager.GetActiveScene().name + " Completed";
        pauseButtonRect = pauseButton.GetComponent<RectTransform>();

        player.OnPlayerKilled += PlayerKilled;
        player.OnPlayerRespawn += PlayerRespawn;

        Time.timeScale = 1;
        pausedText.text = "Paused\n<size=25>" + SceneManager.GetActiveScene().name + "</size>";
        localWorldManager = GameObject.FindWithTag("Level Managers").GetComponent<LocalWorldManager>();
    }

    bool CanPause()
    {
        if(player.dead || !LevelLoadingManager.Instance.done || LevelManager.Instance.FinishedLevel() || screenshotScript.frozen || inSettings)
            return false;
        else
            return true;
    }

    void Update()
    {
        if(!LevelManager.Instance.FinishedLevel())
            GunInfo.CanShoot(!IsCursorOverPauseButton());

        player.gameHUDPaused = paused;
        player.gameHUDFrozen = screenshotScript.frozen;

        if(startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            pauseButton.interactable = false;
            return;
        }

        if(!CanPause() || paused)
            pauseButton.interactable = false;
        else
            pauseButton.interactable = true;

        if(CanPause())
            if(Input.GetButtonDown("Pause"))
                TogglePause();

        if(Input.GetButtonDown("Pause"))
            ToggleSettings(false);
    }

    public bool IsCursorOverPauseButton()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(pauseButtonRect, Input.mousePosition, Camera.main);
    }

    IEnumerator DelayPauseCo()
    {
        canPause = false;
        paused = true;
        referenceTime = 0;
        Time.timeScale = 0;
        
        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].interactable = false;

        AudioManager.Instance.PlaySound2D(pauseSound);
        ScreenEffectsManager.Instance.FadeGrayScale(0, 1, 0.35f);
        AudioManager.Instance.FadeMusicOut(0.35f);
        pauseAnimator.SetBool("Paused", true);
        yield return new WaitForSecondsRealtime(0.45f);
        
        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].interactable = true;

        canPause = true;
        canToggleSettings = true;
    }
    IEnumerator DelayResumeCo()
    {
        canPause = false;
        canToggleSettings = false;

        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].interactable = false;

        AudioManager.Instance.PlaySound2D(pauseSound);
        ScreenEffectsManager.Instance.FadeGrayScale(1, 0, 0.35f);
        AudioManager.Instance.FadeMusicIn(0.35f);
        pauseAnimator.SetBool("Paused", false);
        yield return new WaitForSecondsRealtime(0.45f);
        referenceTime = 1;
        Time.timeScale = 1;
        paused = false;
        canPause = true;
    }

    public void TogglePause()
    {
        if(startDelay > 0)
            return;

        if(canPause)
        {
            if(!paused)
                StartCoroutine(DelayPauseCo());
            else if(paused && !inSettings)
                StartCoroutine(DelayResumeCo());
        }
    }  

    public void FinishLevel()
    {
        if(localWorldManager.world != LocalWorldManager.World.Tutorial)
            levelCompleteUI.SetActive(true);
        else
            LoadMainMenu();
    }

    public void LoadScene(string sceneToLoad)
    {
        if(canPause)
            LevelLoadingManager.Instance.LoadScene(sceneToLoad);
    }

    public void LoadNextScene()
    {        
        if(canPause)
        {
            int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

            if(nextScene + 1 > SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log("Scene doesn't exist in build index, loading main menu instead...");
                LevelLoadingManager.Instance.LoadScene("Main Menu");
            }
            else
            {
                LevelLoadingManager.Instance.LoadScene(nextScene);
            }
        }
    }

    public void Restart()
    {
        if(canPause)
            LevelLoadingManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        if(canPause)
            LevelLoadingManager.Instance.LoadScene("Main Menu");
    }

    public void ToggleSettings(bool open)
    {
        if(open && !inSettings)
        {
            if(canToggleSettings)
                StartCoroutine(DelaySettingsOpen());
        }
        else if(!open && inSettings)
        {
            if(canToggleSettings)
                StartCoroutine(DelaySettingsClose());
        }
    }
    IEnumerator DelaySettingsOpen()
    {
        canToggleSettings = false;
        canPause = false;
        settings.SetActive(true);
        pauseAnimator.SetBool("Settings", true);
        inSettings = true;
        AudioManager.Instance.PlaySound2D(pauseSound);

        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].interactable = false;   

        yield return new WaitForSecondsRealtime(0.45f);
        canToggleSettings = true;
    }

    IEnumerator DelaySettingsClose()
    {
        canToggleSettings = false;
        pauseAnimator.SetBool("Settings", false);
        AudioManager.Instance.PlaySound2D(pauseSound);

        yield return new WaitForSecondsRealtime(0.45f);

        settings.SetActive(false);
        inSettings = false;
        canPause = true;
        canToggleSettings = true;

        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].interactable = true;
    }

    public void PlayerKilled()
    {
        if(player.lives == 0)
            GameOver();
        else if(player.lives != 0)
            StartCoroutine(KillCo(0, 1, 1));
    }

    public void PlayerRespawn()
    {
        Time.timeScale = 1;

        ScreenEffectsManager.Instance.AdjustGrayScale(0);
        AudioManager.Instance.sfxSource.pitch = 1;
        AudioManager.Instance.musicSource.pitch = 1;    
    }

    IEnumerator KillCo(float from, float to, float duration)
    {
        float inTime = 0;

        while(inTime < duration)
        {
            inTime += Time.unscaledDeltaTime;

            ScreenEffectsManager.Instance.AdjustGrayScale(Mathf.Lerp(from, to, inTime / duration));
            AudioManager.Instance.sfxSource.pitch = Mathf.Lerp(to, 0.125f, inTime / duration);
            AudioManager.Instance.musicSource.pitch = Mathf.Lerp(to, 0.125f, inTime / duration);

            Time.timeScale = Mathf.Lerp(1, 0.2f, inTime / duration);
            yield return null;
        }
    }

    public void GameOver()
    {
		AudioManager.Instance.PlaySound2D(gameOverSound);
		gameOverAnimator.enabled = true;
    }
}