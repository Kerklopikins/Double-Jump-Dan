using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsToggle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] SettingsManager settingsManager;
    public SettingType settingType;
    public enum SettingType { PostProcessing, WeatherEffects, Fullscreen }

	public void OnPointerClick(PointerEventData eventData)
	{
		switch(settingType)
        {
            case SettingType.PostProcessing:
            ToggleColorPP();
            break;
            case SettingType.WeatherEffects:
            ToggleWeatherEffects();
            break;
            case SettingType.Fullscreen:
            ToggleFullScreen();
            break;
        }
	}

    void ToggleColorPP()
    {
        GameManager.Instance.postProcessing = !GameManager.Instance.postProcessing;
        ScreenEffectsManager.Instance.UpdatePostProcessing();
    }

    void ToggleWeatherEffects()
    {
        GameManager.Instance.weatherEffects = !GameManager.Instance.weatherEffects;
        settingsManager.ToggleDust(GameManager.Instance.weatherEffects);
    }

    void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}