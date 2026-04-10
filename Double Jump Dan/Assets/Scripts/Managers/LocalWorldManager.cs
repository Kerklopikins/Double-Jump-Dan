using UnityEngine;

public class LocalWorldManager : MonoBehaviour
{
	[Header("World Properties")]
	public TimeOfDay timeOfDay;
    public World world;
    
    [Header("Weather")]
    public WeatherType weatherType;
    [Range(0, 1)]
    public float cloudEmissionOverTime;
    [Range(0, 1)]
    public float dustAlpha;

    [Header("Distortion Effects")]
    public DistortionType distortionType;
    [Range(0, 30)]
    public float heatWaveDistortionIntensity;

    [Header("Post Precessing")]
    public Color dayTintColor;
    public Color sunsetTintColor;
    public Color nightTintColor;
    public Color sunriseTintColor;
    public Color overcastTintColor;
    public Color snowingTintColor;
    
    public enum DistortionType { None, HeatWave }
	public enum TimeOfDay { Day, Sunset, Night, Sunrise, Overcast }
    public enum World { SplashScreen, Tutorial, MainMenu, Archipelago, City, Desert, Forest, Jungle, Tundra }
    public enum WeatherType { None, Dusting, Snowing, Raining }
}