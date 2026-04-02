using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] Slider[] volumeSliders;
    [SerializeField] Toggle postProcessingToggle;
    [SerializeField] Toggle weatherEffectsToggle;

    [Header("Main Menu Only Settings")]
    [SerializeField] GameObject resettedGameImage;
	[SerializeField] Slider screenResolutionSlider;
	[SerializeField] Text screenResolutionText;
	[SerializeField] Toggle fullscreenToggle;
    [SerializeField] GameObject dust;

    [Header("Pause Menu Only")]
    [SerializeField] Animator settingsAnimator;
    [SerializeField] GameObject settingsMenu;
    
    List<Vector2> screenResolutions = new List<Vector2>();
    List<Vector2> validResolutions = new List<Vector2>();
    GameManager gameManager;
    MainMenuManager mainMenuManager;
    bool inMainMenu;

    void Start()
    {
        gameManager = GameManager.Instance;

        if(GetComponent<MainMenuManager>() != null)
        {
            mainMenuManager = GetComponent<MainMenuManager>();
            inMainMenu = true;
        }

        volumeSliders[0].value = gameManager.sfxVolume;
        volumeSliders[1].value = gameManager.musicVolume;
        
        ///Beautiful Pixel Perfect Resolutions
        screenResolutions.Add(new Vector2(640, 360));
        screenResolutions.Add(new Vector2(960, 540));
        screenResolutions.Add(new Vector2(1280, 720));
        screenResolutions.Add(new Vector2(1600, 900));
        screenResolutions.Add(new Vector2(1920, 1080));
        screenResolutions.Add(new Vector2(2560, 1440));
        screenResolutions.Add(new Vector2(3840, 2160));

        if(inMainMenu)
        {
            foreach(var resolution in screenResolutions)
                if(resolution.x <= Screen.resolutions[Screen.resolutions.Length - 1].width && resolution.y <= Screen.resolutions[Screen.resolutions.Length - 1].height)
                    validResolutions.Add(resolution);
            
            if(gameManager.screenResolution == -1)
            {
                Screen.SetResolution((int)validResolutions[validResolutions.Count - 1].x, (int)validResolutions[validResolutions.Count - 1].y, true);
                LevelLoadingManager.Instance.ResizeFadeBackground();
                gameManager.screenResolution = validResolutions.Count - 1;
                gameManager.SaveData();
            }
            else if(gameManager.screenResolution > validResolutions.Count - 1)
            {
                gameManager.screenResolution = validResolutions.Count - 1;
                gameManager.SaveData();
            }

            screenResolutionSlider.maxValue = validResolutions.Count - 1;
            screenResolutionSlider.value = gameManager.screenResolution;

            fullscreenToggle.isOn = Screen.fullScreen;

            ToggleDust(gameManager.weatherEffects);
        }
		
        postProcessingToggle.isOn = gameManager.postProcessing;
        weatherEffectsToggle.isOn = gameManager.weatherEffects;
    }

    public void UpdateScreenResolution()
    {
        gameManager.screenResolution = (int)screenResolutionSlider.value;
        Screen.SetResolution((int)validResolutions[gameManager.screenResolution].x, (int)validResolutions[gameManager.screenResolution].y, Screen.fullScreen);
        gameManager.SaveData();
    }

    public void UpdateScreenResolutionText()
    {
        screenResolutionText.text = validResolutions[(int)screenResolutionSlider.value].x.ToString() + "x" + validResolutions[(int)screenResolutionSlider.value].y.ToString();
    }
    public void SaveSettings()
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

    public void ToggleDust(bool isOn)
    {
        if(inMainMenu)
            dust.SetActive(isOn);
        else
            WorldManager.Instance.ToggleWeatherEffects(isOn);
    }

    public void EnablePauseMenuSettings()
    {
        settingsMenu.gameObject.SetActive(true);
        settingsAnimator.SetBool("Open", true);
    }
}