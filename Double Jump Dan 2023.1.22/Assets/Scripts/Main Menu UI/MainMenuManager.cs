using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    [Header("Main Menu")]
    public Button playButton;
    public Button shopButton;
    public Button userButton;
    public Button tutorialButton;
    public Button statsButton;

    [Header("Level Select")]
    [SerializeField] ScrollRect levelsScrollRect;

    [Header("Stats Menu")]
    [SerializeField] GameObject statsGameObject;
    [SerializeField] Text statsTitleText;
    [SerializeField] Text enemiesCounterText;
    [SerializeField] Text deathsCounterText;
    [SerializeField] Text gemsCounterText;
    [SerializeField] Text playtimeCounterText;

    [Header("Tampered User File")]
    [SerializeField] Image tamperedUserFileImage;
    [SerializeField] Text tamperedUserFileText;

    public event Action OnLevelButtonsRefresh;
    public event Action OnUsersRefresh;
    
    GameManager gameManager;
    ShopManager shopManager;
    float startDelay = 1.35f;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Time.timeScale = 1;
        gameManager = GameManager.Instance;
        shopManager = GetComponent<ShopManager>(); 

        if(playButton == null || shopButton == null || userButton == null)
            Debug.LogError("Play, Shop, or User button is null");
            
        playButton.onClick.AddListener(RefreshLevels);
        shopButton.onClick.AddListener(shopManager.RefreshShop);
        shopButton.onClick.AddListener(shopManager.RefreshShopScrollRects);
        userButton.onClick.AddListener(RefreshUsers);
        statsButton.onClick.AddListener(RefreshUserStats);
    }

    void Update()
    {
        if(startDelay > 0)
        {
            tutorialButton.interactable = false;
            startDelay -= Time.deltaTime;
        }
        else
        {
            tutorialButton.interactable = true;
        }

        if(statsGameObject.activeSelf)
            playtimeCounterText.text = GetTotalPlaytimeString();
    }

    #region Level Select
    public void RefreshLevels()
    {
        OnLevelButtonsRefresh?.Invoke();
        StartCoroutine(DelayLevelsContentCentering());
    }

    IEnumerator DelayLevelsContentCentering()
    {
        yield return null;
        yield return null;

        levelsScrollRect.verticalNormalizedPosition = 1;
    }

    #endregion

    public void RefreshUsers()
    {
        OnUsersRefresh?.Invoke();
    }

    public void LoadScene(string sceneToLoad)
    {
        if(startDelay > 0)
            return;
            
        LevelLoadingManager.Instance.LoadScene(sceneToLoad);
    }

    #region TamperedUserFile
    public void TamperedUserFile(string text)
    {
        StartCoroutine(TamperedUserFileCo(text));
        StartCoroutine(ForceTamperedUserImageForward());
    }

    IEnumerator ForceTamperedUserImageForward()
    {
        float duration = 0.25f;

        while(duration > 0)
        {
            duration -= Time.deltaTime;
            tamperedUserFileImage.transform.SetAsLastSibling();
            yield return null;
        }
    }

    IEnumerator TamperedUserFileCo(string text)
    {
        tamperedUserFileImage.gameObject.SetActive(true);
        tamperedUserFileText.text = text;

        tamperedUserFileImage.color = new Color(tamperedUserFileImage.color.r, tamperedUserFileImage.color.g, tamperedUserFileImage.color.b, 1);
        tamperedUserFileText.color = new Color(tamperedUserFileText.color.r, tamperedUserFileText.color.g, tamperedUserFileText.color.b, 1);

        yield return new WaitForSeconds(3);

        float inTime = 0;
        float duration = 2;

        while(inTime < duration)
        {
            inTime += Time.deltaTime;
            tamperedUserFileImage.color = new Color(tamperedUserFileImage.color.r, tamperedUserFileImage.color.g, tamperedUserFileImage.color.b, Mathf.Lerp(1, 0, inTime / duration));
            tamperedUserFileText.color = new Color(tamperedUserFileText.color.r, tamperedUserFileText.color.g, tamperedUserFileText.color.b, Mathf.Lerp(1, 0, inTime / duration));
            yield return null;
        } 

        tamperedUserFileImage.gameObject.SetActive(false);
        tamperedUserFileImage.color = new Color(tamperedUserFileImage.color.r, tamperedUserFileImage.color.g, tamperedUserFileImage.color.b, 0);
        tamperedUserFileText.color = new Color(tamperedUserFileText.color.r, tamperedUserFileText.color.g, tamperedUserFileText.color.b, 0);
    }

    #endregion

    #region UserStats
    
    void RefreshUserStats()
    {
        statsTitleText.text = gameManager.currentUserName + "'s Statistics";
        enemiesCounterText.text = gameManager.totalEnemiesKilled.ToString();
        deathsCounterText.text = gameManager.totalDeaths.ToString();
        gemsCounterText.text = gameManager.totalGemsCollected.ToString();
        playtimeCounterText.text = GetTotalPlaytimeString();
    }

    string GetTotalPlaytimeString()
    {
        TimeSpan t = TimeSpan.FromSeconds(gameManager.totalPlaytime);

        string formatted = "";

        if(t.Days > 0)
            formatted += t.Days + "d ";
        
        if(t.Hours > 0)
            formatted += t.Hours + "h ";

        if(t.Minutes > 0)
            formatted += t.Minutes + "m ";
        
        formatted += t.Seconds + "s ";

        return formatted;
    }

    #endregion
    
    public void ExitGame()
    {
        Application.Quit();
    }
}