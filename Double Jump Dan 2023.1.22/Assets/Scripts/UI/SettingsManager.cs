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
        
        if(inMainMenu)
        {
            for(int i = 0; i < Screen.resolutions.Length; i++)
            {
                if(Screen.resolutions[i].width >= 512)
                    if(!screenResolutions.Contains(new Vector2(Screen.resolutions[i].width, Screen.resolutions[i].height)))
                        screenResolutions.Add(new Vector2(Screen.resolutions[i].width, Screen.resolutions[i].height));
            }

            if(gameManager.screenResolution == -1)
            {
                Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
                LevelLoadingManager.Instance.ResizeFadeBackground();
                gameManager.screenResolution = screenResolutions.Count;
                gameManager.SaveData();
            }
            else if(gameManager.screenResolution > screenResolutions.Count)
            {
                gameManager.screenResolution = screenResolutions.Count;
                gameManager.SaveData();
            }

            screenResolutionSlider.maxValue = screenResolutions.Count - 1;
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
        Screen.SetResolution((int)screenResolutions[gameManager.screenResolution].x, (int)screenResolutions[gameManager.screenResolution].y, Screen.fullScreen);
        gameManager.SaveData();
    }

    public void UpdateScreenResolutionText()
    {
        screenResolutionText.text = screenResolutions[(int)screenResolutionSlider.value].x.ToString() + "x" + screenResolutions[(int)screenResolutionSlider.value].y.ToString();
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