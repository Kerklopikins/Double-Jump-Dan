using UnityEngine;

public class LocalWorldManager : MonoBehaviour
{
	[Header("World Properties")]
	public TimeOfDay timeOfDay;
    public World world;

    [Header("Weather")]
    public float cloudEmissionOverTime;
    public bool overcast;
	public bool snowing;
    public bool dusting;
    
	public enum TimeOfDay { Day, Sunset, Night, Sunrise };
    public enum World { SplashScreen, Tutorial, MainMenu, Archipelago, City, Desert, Forest, Jungle, Tundra };
}