using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Slider[] volumeSliders;
    [SerializeField] GameObject resettedGameImage;
    public ConfirmPurchase confirmPurchase;
	[SerializeField] Slider screenResolutionSlider;
	[SerializeField] Text screenResolutionText;
	[SerializeField] Toggle fullscreenToggle;
    //[Header("Preloading")]
    //public GameObject[] menuPanels;

    public ItemManager itemManager;

    GameManager gameManager;
	List<Vector2> screenResolutions = new List<Vector2>();
    float startDelay = 1;
    //bool preloaded;
    ///Animator mainMenuAnimator;

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

        //mainMenuAnimator = menuPanels[0].GetComponent<Animator>();
        //StartCoroutine(Preload());
    }

    //IEnumerator Preload()
    //{
        //for(int i = 0; i < menuPanels.Length; i++)
        //{
            //menuPanels[i].SetActive(true);
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //menuPanels[i].SetActive(false);
        //}

        //menuPanels[0].SetActive(true);
        //mainMenuAnimator.SetBool("Open", true);
    //}

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
}