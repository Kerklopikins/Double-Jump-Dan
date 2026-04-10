using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsToggle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] SettingsManager settingsManager;
    [SerializeField] SettingType settingType;
    
    public enum SettingType { PostProcessing, DistortionEffects, WeatherEffects, Fullscreen }
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }
    
	public void OnPointerClick(PointerEventData eventData)
	{
		switch(settingType)
        {
            case SettingType.PostProcessing:
            ToggleColorPP();
            break;
            case SettingType.DistortionEffects:
            ToggleDistortionEffects();
            break;
            case SettingType.WeatherEffects:
            ToggleWeatherEffects();
            break;
            case SettingType.Fullscreen:
            ToggleFullscreen();
            break;
        }
	}

    void ToggleColorPP()
    {
        gameManager.postProcessing = !gameManager.postProcessing;
        ScreenEffectsManager.Instance.UpdatePostProcessing();
    }

    void ToggleDistortionEffects()
    {
        gameManager.distortionEffects = !gameManager.distortionEffects;

        if(settingsManager.mainMenuManager == null)
            WorldManager.Instance.UpdateDistortionEffects(gameManager.distortionEffects);
        else
            settingsManager.mainMenuManager.UpdateHeatWave(gameManager.distortionEffects);
    }
    
    void ToggleWeatherEffects()
    {
        gameManager.weatherEffects = !gameManager.weatherEffects;

        if(settingsManager.mainMenuManager == null)
            WorldManager.Instance.UpdateWeatherEffects(gameManager.weatherEffects);
        else
            settingsManager.mainMenuManager.UpdateDust(gameManager.weatherEffects);
    }

    void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}