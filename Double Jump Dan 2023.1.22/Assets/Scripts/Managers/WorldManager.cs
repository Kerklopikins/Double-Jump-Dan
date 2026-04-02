using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    [Header("Time of Day")]
    public Material mainMaterial;
    public Material mainMaterialStencil;
    public SpriteRenderer sky;
    [SerializeField] GameObject sunGlow;
    [SerializeField] GameObject sunPivot;
    [SerializeField] GameObject sun;
    [SerializeField] GameObject moon;
    public SpriteRenderer stars;
    [SerializeField] Sprite skyDay;
    [SerializeField] Sprite skySunset;
    [SerializeField] Sprite skyNight;
    [SerializeField] Sprite skySunrise;
    [SerializeField] Sprite skyOvercast;

    [Header("Weather")]
    public Transform clouds;
    public GameObject dust;
    public Transform snow;

    bool overcast;
    float cloudEmissionOverTime;
    public bool snowing { get; set; }
    public bool dusting { get; set; }
    float cameraSize;
    Camera _camera;
    ParticleSystem cloudParticleSystem;
    ParticleSystemRenderer cloudParticleSystemRenderer;
    LocalWorldManager localWorldManager;

    void Awake()
    {
        Instance = this;
        localWorldManager = GameObject.FindWithTag("Level Managers").GetComponent<LocalWorldManager>();
    }

    void Start()
    {
        _camera = Camera.main;

        if(localWorldManager.world != LocalWorldManager.World.Tutorial && localWorldManager.world != LocalWorldManager.World.MainMenu)
        {
            overcast = localWorldManager.overcast;
            cloudEmissionOverTime = localWorldManager.cloudEmissionOverTime;
            snowing = localWorldManager.snowing;
            dusting = localWorldManager.dusting;

            switch(localWorldManager.timeOfDay)
            {   
                case LocalWorldManager.TimeOfDay.Day:
                    SetDay();
                    break;
                case LocalWorldManager.TimeOfDay.Sunset:
                    SetSunset();
                    break;
                case LocalWorldManager.TimeOfDay.Night:
                    SetNight();
                    break;
                case LocalWorldManager.TimeOfDay.Sunrise:
                    SetSunrise();
                    break;
            }

            ReloadSky();

            cloudParticleSystem = clouds.GetComponent<ParticleSystem>();
            clouds.localPosition = new Vector3(-cameraSize, clouds.localPosition.y, clouds.localPosition.z);

            ParticleSystem.EmissionModule emission = cloudParticleSystem.emission;
            emission.rateOverTime = cloudEmissionOverTime;

            cloudParticleSystemRenderer = clouds.GetComponent<ParticleSystemRenderer>();
            cloudParticleSystemRenderer.material.color = new Color(mainMaterial.color.r, mainMaterial.color.g, mainMaterial.color.b, cloudParticleSystemRenderer.material.color.a);

            cloudParticleSystem.Stop();
            cloudParticleSystem.Play();
    
            ToggleWeatherEffects(GameManager.Instance.weatherEffects);
        }
        else
        {
            sky.gameObject.SetActive(false);
            sunPivot.SetActive(false);
            clouds.gameObject.SetActive(false);
            
            SetSkyAndMaterialsColor(1, 1, 1);
        }
    }

    public void ToggleWeatherEffects(bool isOn)
    {
        if(!isOn)
        {
            snow.gameObject.SetActive(false);
            dust.SetActive(false);
            return;
        }

        if(snowing)
        {
            snow.gameObject.SetActive(true);
            sun.SetActive(false);
            clouds.gameObject.SetActive(false);

            for(int i = 0; i < snow.childCount; i++)
            {
                snow.GetChild(i).gameObject.SetActive(true);
            }

            float cameraSize = _camera.orthographicSize * ((float)Screen.width / Screen.height);
            snow.GetChild(0).transform.localScale = new Vector2(cameraSize * 2, _camera.orthographicSize * 2);
            snow.GetChild(1).transform.localScale = new Vector2(cameraSize * 2, _camera.orthographicSize * 2);

            ParticleSystem snowParticles = snow.GetChild(2).GetComponent<ParticleSystem>();
            
            snowParticles.Stop();
            snow.GetChild(2).transform.position = new Vector2(cameraSize - cameraSize, _camera.orthographicSize * 2);
            snowParticles.Play();
        }

        if(dusting)
            dust.SetActive(true);
    }

    public void SetDay()
    {
        if(!overcast)
        {
            sky.sprite = skyDay;
            sunPivot.transform.eulerAngles = new Vector3(0, 0, 55);
            sun.transform.eulerAngles = new Vector3(0, 0, 0);
            moon.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            SetOvercastSky();
            sun.SetActive(false);
        }

        SetSkyAndMaterialsColor(1, 1, 1);
    }

    public void SetSunset()
    {
        if(!overcast)
        {
            sunGlow.SetActive(true);

            sky.sprite = skySunset;
            sunPivot.transform.localEulerAngles = new Vector3(0, 0, 0);
            sun.transform.eulerAngles = new Vector3(0, 0, 0);
            moon.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            SetOvercastSky();
            sun.SetActive(false);
        }

        SetSkyAndMaterialsColor(0.3f, 0.3f, 0.3f);
    }

    public void SetNight()
    {
        if(!overcast)
        {
            sky.sprite = skyNight;
            sunPivot.transform.localEulerAngles = new Vector3(0, 0, 210);
            sun.transform.eulerAngles = new Vector3(0, 0, 0);
            moon.transform.eulerAngles = new Vector3(0, 0, 0);

            stars.gameObject.SetActive(true);
        }
        else
        {
            SetOvercastSky();
            sun.SetActive(false);
        }

        SetSkyAndMaterialsColor(0.3f, 0.3f, 0.3f);
    }

    public bool UseWeatherEffects()
    {
        if(GameManager.Instance.weatherEffects)
            return true;
        else
            return false;
    }

    public void SetSunrise()
    {
        if(!overcast)
        {
            sky.sprite = skySunrise;
            sunPivot.SetActive(false);
        }
        else
        {
            SetOvercastSky();
            sun.SetActive(false);
        }

        SetSkyAndMaterialsColor(0.6f, 0.6f, 0.6f);
    }
    
    public void SetOvercastSky()
    {
        sky.sprite = skyOvercast;
    }

    void SetSkyAndMaterialsColor(float r, float g, float b)
    {
        if(overcast)
            sky.color = new Color(r, g, b, mainMaterial.color.a);

        mainMaterial.color = new Color(r, g, b, mainMaterial.color.a);
        mainMaterialStencil.color = mainMaterial.color;
    }

    public void ReloadSky()
    {
        cameraSize = _camera.orthographicSize * ((float)Screen.width / Screen.height);
        sky.transform.parent.localScale = new Vector3(cameraSize, sky.transform.parent.localScale.y, 1);

        if(localWorldManager.timeOfDay == LocalWorldManager.TimeOfDay.Night)
            stars.size = new Vector2(cameraSize * 2, stars.size.y);
    }
}