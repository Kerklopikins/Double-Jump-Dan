using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    public Button playButton;
    public Button shopButton;

    [SerializeField] Slider[] volumeSliders;
    [SerializeField] GameObject resettedGameImage;
    public ConfirmPurchase confirmPurchase;
	[SerializeField] Slider screenResolutionSlider;
	[SerializeField] Text screenResolutionText;
	[SerializeField] Toggle fullscreenToggle;
	[SerializeField] Text gemsText;
    [SerializeField] AudioClip tooExpensiveSound;

    public ItemManager itemManager;

    public event Action OnShopItemsChanged;
    public event Action OnLevelButtonsRefresh;
    GameManager gameManager;
	List<Vector2> screenResolutions = new List<Vector2>();
    float startDelay = 1;

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
			
		screenResolutionSlider.maxValue = screenResolutions.Count - 1;
		screenResolutionSlider.value = gameManager.screenResolution;

		fullscreenToggle.isOn = Screen.fullScreen;

		if(gameManager.screenResolution == -1)
		{
			Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
			gameManager.screenResolution = screenResolutions.Count;
		}
		else if(gameManager.screenResolution > screenResolutions.Count)
		{
			gameManager.screenResolution = screenResolutions.Count;
		}

		gameManager.SaveData();

        if(gameManager.gems != 1)
            RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems");
		else
            RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGem");

        if(playButton == null || shopButton == null)
            Debug.LogError("Play or Shop button is null");
            
        playButton.onClick.AddListener(RefreshLevels);
        shopButton.onClick.AddListener(RefreshShop);
    }

    void Update()
    {
        if(startDelay > 0)
            startDelay -= Time.deltaTime;
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
        if(gameManager.gems != 1)
            RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems");
		else
            RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGem");
        
        OnShopItemsChanged?.Invoke();
    }

    public void RefreshLevels()
    {
        OnLevelButtonsRefresh?.Invoke();
    }

    public void LoadScene(string sceneToLoad)
    {
        if(startDelay > 0)
            return;
            
        LevelLoadingManager.Instance.LoadScene(sceneToLoad);
    }

    public void SaveSfxVolume()
    {
        AudioManager.Instance.sfxVolumePercent = volumeSliders[0].value;
        AudioManager.Instance.SetVolume(volumeSliders[0].value, AudioManager.AudioChannel.Sfx);
        gameManager.sfxVolume = volumeSliders[0].value;
        gameManager.SaveData();
    }

    public void SaveMusicVolume()
    {
        AudioManager.Instance.musicVolumePercent = volumeSliders[1].value;
        AudioManager.Instance.SetVolume(volumeSliders[1].value, AudioManager.AudioChannel.Music);
        gameManager.musicVolume = volumeSliders[1].value;
        gameManager.SaveData();
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

    public void RefreshGemsText(string text)
    {
        gemsText.text = text;
    }

    public void GetOneThousandGems()
    {
        gameManager.gems += 1000;
        RefreshGemsText("<color=yellow>" + gameManager.gems.ToString() + "</color>" + "\nGems");
        gameManager.SaveUserData();

		RefreshShop();
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