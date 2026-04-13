using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu")]
    public Button playButton;
    public Button shopButton;
    public Button settingsButton;
    public Button userButton;
    public Button tutorialButton;
    public Button statsButton;

    [Header("Tampered User File")]
    [SerializeField] Image tamperedUserFileImage;
    [SerializeField] Text tamperedUserFileText;

    ShopManager shopManager;
    UserMenu userMenu;
    UserStatsMenu userStatsMenu;
    LevelSelectMenu levelSelectMenu;
    SettingsManager settingsManager;

    float startDelay = 1.35f;

    void Start()
    {
        Time.timeScale = 1;
        shopManager = GetComponent<ShopManager>(); 
        userMenu = GetComponent<UserMenu>();
        userStatsMenu = GetComponent<UserStatsMenu>();
        levelSelectMenu = GetComponent<LevelSelectMenu>();
        settingsManager = GetComponent<SettingsManager>();

        if(playButton == null || shopButton == null || userButton == null || statsButton == null || settingsButton == null)
            Debug.LogError("Main Menu button is null");
            
        playButton.onClick.AddListener(levelSelectMenu.RefreshLevels);
        shopButton.onClick.AddListener(shopManager.RefreshShop);
        shopButton.onClick.AddListener(shopManager.RefreshShopScrollRects);
        userButton.onClick.AddListener(userMenu.RefreshUserByteSizes);
        statsButton.onClick.AddListener(userStatsMenu.RefreshUserStats);
        settingsButton.onClick.AddListener(settingsManager.RefreshFullscreenToggle);
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
    }

    public void LoadTutorial()
    {
        if(startDelay > 0)
            return;
            
        LevelLoadingManager.Instance.LoadScene("Tutorial");
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
    
    public void ExitGame()
    {
        Application.Quit();
    }
}