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
    [SerializeField] PauseMenuButton pauseButton;
    [SerializeField] Animator pauseAnimator;
    [SerializeField] AudioClip pauseSound;
    [SerializeField] Text pausedText;
    [SerializeField] PauseMenuButton[] pauseMenuButtons;

    [Header("Game Over")]
    [SerializeField] Animator gameOverAnimator;
    [SerializeField] AudioClip gameOverSound;

    [Header("Finish Level")]
    [SerializeField] Text levelNameText;

    [Header("Screenshot")]
    [SerializeField] Screenshot screenshotScript;

    public bool paused { get; protected set; }
    public bool canPause { get; set; }
    public static float referenceTime;
    float startDelay = 1;
    Player player;

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

        player.OnPlayerKilled += PlayerKilled;
        player.OnPlayerRespawn += PlayerRespawn;

        Time.timeScale = 1;
        pausedText.text = "Paused\n<size=25>" + SceneManager.GetActiveScene().name + "</size>";
    }

    bool CanPause()
    {
        if(player.dead || !LevelLoadingManager.Instance.done || player.finishedLevel || screenshotScript.frozen)
            return false;
        else
            return true;
    }

    void Update()
    {
        GunInfo.CanShoot(!pauseButton.IsCursorOverButton());

        player.gameHUDPaused = paused;
        player.gameHUDFrozen = screenshotScript.frozen;

        if(startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            pauseButton.SetInteractable(false);
            return;
        }

        if(!CanPause() || paused)
            pauseButton.SetInteractable(false);
        else
            pauseButton.SetInteractable(true);

        if(CanPause())
            if(Input.GetButtonDown("Pause"))
                TogglePause();
    }

    IEnumerator DelayPauseCo()
    {
        canPause = false;
        paused = true;
        referenceTime = 0;
        Time.timeScale = 0;
        
        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].SetInteractable(false);

        AudioManager.Instance.PlaySound2D(pauseSound);
        ScreenEffectsManager.Instance.FadeGrayScale(0, 1, 0.35f);
        AudioManager.Instance.FadeMusicOut(0.35f);
        pauseAnimator.SetBool("Paused", true);
        yield return new WaitForSecondsRealtime(0.45f);
        
        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].SetInteractable(true);

        canPause = true;
    }
    IEnumerator DelayResumeCo()
    {
        canPause = false;
        
        for(int i = 0; i < pauseMenuButtons.Length; i++)
            pauseMenuButtons[i].SetInteractable(false);

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
            else
                StartCoroutine(DelayResumeCo());
        }
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