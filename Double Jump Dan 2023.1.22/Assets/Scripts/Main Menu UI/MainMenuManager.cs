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

    [Header("Level Select")]
    [SerializeField] ScrollRect levelsScrollRect;
    
    [Header("Settings")]
    [SerializeField] Slider[] volumeSliders;
    [SerializeField] GameObject resettedGameImage;
	[SerializeField] Slider screenResolutionSlider;
	[SerializeField] Text screenResolutionText;
	[SerializeField] Toggle fullscreenToggle;
    
    [Header("Shop")]
    public ConfirmPurchase confirmPurchase;
	[SerializeField] Text gemsText;
    [SerializeField] AudioClip tooExpensiveSound;
    [SerializeField] ScrollRect gunScrollRect;
    [SerializeField] ScrollRect hatScrollRect;
    [SerializeField] ScrollRect skinScrollRect;

    [Header("Tampered User File")]
    [SerializeField] Image tamperedUserFileImage;
    [SerializeField] Text tamperedUserFileText;

    public ItemManager itemManager;

    public event Action OnShopItemsChanged;
    public event Action OnLevelButtonsRefresh;
    public event Action OnUsersRefresh;
    public RectTransform currentGunRect { get; set; }
    public RectTransform currentHatRect { get; set; }
    public RectTransform currentSkinRect { get; set; }
    GameManager gameManager;
	List<Vector2> screenResolutions = new List<Vector2>();
    float startDelay = 1.35f;
    CurrentShopTab currentShopTab = CurrentShopTab.Guns;
    public enum CurrentShopTab { Guns, Hats, Skins }

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Time.timeScale = 1;
        gameManager = GameManager.Instance;
        
        volumeSliders[0].value = gameManager.sfxVolume;
        volumeSliders[1].value = gameManager.musicVolume;
        
		for(int i = 0; i < Screen.resolutions.Length; i++)
		{
			if(Screen.resolutions[i].width >= 512)
				if(!screenResolutions.Contains(new Vector2(Screen.resolutions[i].width, Screen.resolutions[i].height)))
					screenResolutions.Add(new Vector2(Screen.resolutions[i].width, Screen.resolutions[i].height));
		}

		if(gameManager.screenResolution == -1)
		{
			Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
			gameManager.screenResolution = screenResolutions.Count;
		}
		else if(gameManager.screenResolution > screenResolutions.Count)
		{
			gameManager.screenResolution = screenResolutions.Count;
		}

        screenResolutionSlider.maxValue = screenResolutions.Count - 1;
		screenResolutionSlider.value = gameManager.screenResolution;

		fullscreenToggle.isOn = Screen.fullScreen;

		gameManager.SaveData();

        RefreshGemsText();

        if(playButton == null || shopButton == null || userButton == null)
            Debug.LogError("Play, Shop, or User button is null");
            
        playButton.onClick.AddListener(RefreshLevels);
        shopButton.onClick.AddListener(RefreshShop);
        shopButton.onClick.AddListener(RefreshShopScrollRects);
        userButton.onClick.AddListener(RefreshUsers);
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

	public void UpdateScreenResolution()
	{
		gameManager.screenResolution = (int)screenResolutionSlider.value;
		Screen.SetResolution((int)screenResolutions[gameManager.screenResolution].x, (int)screenResolutions[gameManager.screenResolution].y, Screen.fullScreen);
		gameManager.SaveData();
	}

	public void UpdateScreenResolutionText()
	{
		screenResolutionText.text = screenResolutions[(int)screenResolutionSlider.value].x.ToString() + "x" + screenResolutions[(int)screenResolutionSlider.value].y.ToString();
	}

    public void EquipItem(ShopItem shopItem)
	{
        itemManager.EquipItem(shopItem);
        OnShopItemsChanged?.Invoke();
    }

    public void RefreshShop()
    {
        RefreshGemsText();
        OnShopItemsChanged?.Invoke();  
    }

    public void RefreshShopScrollRects()
    {
        StartCoroutine(DelayShopContentCentering());
    }
    IEnumerator DelayShopContentCentering()
    {
        yield return null;
        yield return null;
        
        switch(currentShopTab)
        {   
            case CurrentShopTab.Guns:
                CenterShopScrollRect(gunScrollRect, currentGunRect);
                break;
            case CurrentShopTab.Hats:
                CenterShopScrollRect(hatScrollRect, currentHatRect);
                break;
            case CurrentShopTab.Skins:
                CenterShopScrollRect(skinScrollRect, currentSkinRect);
                break;
        }
    }
    public void CenterShopScrollRect(ScrollRect scrollRect, RectTransform currentRect)
    {
        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;

        float itemPosition = Mathf.Abs(currentRect.anchoredPosition.x);
        float targetPosition = itemPosition - (viewportWidth / 2);

        float normalized = Mathf.Clamp01(targetPosition / (contentWidth - viewportWidth));
        scrollRect.horizontalNormalizedPosition = normalized;
    }
    
    public void SwitchToGunTab()
    {
        currentShopTab = CurrentShopTab.Guns;
        RefreshShopScrollRects();
    }

    public void SwitchToSkinTab()
    {
        currentShopTab = CurrentShopTab.Skins;
        RefreshShopScrollRects();
    }

    public void SwitchToHatTab()
    {
        currentShopTab = CurrentShopTab.Hats;
        RefreshShopScrollRects();
    }
    
    public void RefreshGemsText()
    {
        if(gameManager.gems != 1)
            gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems";
		else
            gemsText.text = "<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGem";
    }
    public void RefreshGemsText(string text)
    {
        gemsText.text = text;
    }

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

    public void SaveSoundSettings()
    {
        gameManager.musicVolume = volumeSliders[1].value;
        gameManager.sfxVolume = volumeSliders[0].value;
        gameManager.SaveData();
    }

    public void AdjustSfxVolume()
    {
        AudioManager.Instance.sfxVolumePercent = volumeSliders[0].value;
        AudioManager.Instance.SetVolume(volumeSliders[0].value, AudioManager.AudioChannel.Sfx);
    }

    public void AdjustMusicVolume()
    {
        AudioManager.Instance.musicVolumePercent = volumeSliders[1].value;
        AudioManager.Instance.SetVolume(volumeSliders[1].value, AudioManager.AudioChannel.Music);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResetGame()
    {
        resettedGameImage.SetActive(true);
        gameManager.ResetGame();
		PlayerPrefs.DeleteAll();
        StartCoroutine(ResetGameCo());
    }

    IEnumerator ResetGameCo()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Main Menu");
    }

    public void GetOneThousandGems()
    {
        gameManager.gems += 1000;
        RefreshGemsText();
        gameManager.SaveUserData();

		RefreshShop();
    }

    public void GetEverything()
    {
        foreach(var hat in itemManager.hats)
        {
            if(!gameManager.ownedHats.Contains(hat.itemID))            
                gameManager.ownedHats.Add(hat.itemID);
        }

        foreach(var gun in itemManager.guns)
        {
            if(!gameManager.ownedGuns.Contains(gun.itemID))            
                gameManager.ownedGuns.Add(gun.itemID);
        }

		foreach(var skin in itemManager.skins)
        {
            if(!gameManager.ownedSkins.Contains(skin.itemID))            
                gameManager.ownedSkins.Add(skin.itemID);
        }
		
        RefreshShop();
		gameManager.SaveUserData();
    }

	public void FlashGemsText()
	{
		AudioManager.Instance.PlaySound2D(tooExpensiveSound);
		StartCoroutine(FlashGemsTextCo());
	}

	IEnumerator FlashGemsTextCo()
	{
		for(int i = 0; i < 3; i++)
		{
			if(gameManager.gems != 1)
				RefreshGemsText("<color=red>" + gameManager.gems.ToString() + "</color>" + "<color=red>\nGems</color>");
			else
				RefreshGemsText("<color=red>" + gameManager.gems.ToString() + "</color>" + "<color=red>\nGem</color>");

			yield return new WaitForSeconds(0.07f);

			if(gameManager.gems != 1)
				RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "<color=white>\nGems</color>");
			else
				RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "<color=white>\nGem</color>");

			yield return new WaitForSeconds(0.07f);
		}
	}
}