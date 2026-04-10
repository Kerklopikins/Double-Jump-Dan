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
    [SerializeField] MeshRenderer[] dustMeshes;
    public Transform snow;

    [Header("Distortion Effects")]
    [SerializeField] GameObject heatWave;

    float cloudEmissionOverTime;
    Camera _camera;
    ParticleSystem cloudParticleSystem;
    ParticleSystemRenderer cloudParticleSystemRenderer;
    LocalWorldManager localWorldManager;
    ScreenEffectsManager screenEffectsManager;
    MeshRenderer heatWaveMesh;

    void Awake()
    {
        Instance = this;
        localWorldManager = GameObject.FindWithTag("Level Managers").GetComponent<LocalWorldManager>();
    }

    void Start()
    {
        _camera = Camera.main;
        screenEffectsManager = GetComponent<ScreenEffectsManager>();

        if(localWorldManager.world != LocalWorldManager.World.Tutorial && localWorldManager.world != LocalWorldManager.World.MainMenu)
        {
            cloudEmissionOverTime = localWorldManager.cloudEmissionOverTime;
            
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
                case LocalWorldManager.TimeOfDay.Overcast:
                    SetOvercast();
                    break;
            }

            ReloadSky();

            cloudParticleSystem = clouds.GetComponent<ParticleSystem>();
            clouds.localPosition = new Vector3(-CameraWidth(), clouds.localPosition.y, clouds.localPosition.z);

            ParticleSystem.EmissionModule emission = cloudParticleSystem.emission;
            emission.rateOverTime = cloudEmissionOverTime;

            cloudParticleSystemRenderer = clouds.GetComponent<ParticleSystemRenderer>();
            cloudParticleSystemRenderer.material.color = new Color(mainMaterial.color.r, mainMaterial.color.g, mainMaterial.color.b, cloudParticleSystemRenderer.material.color.a);

            cloudParticleSystem.Stop();
            cloudParticleSystem.Play();
    
            UpdateWeatherEffects(GameManager.Instance.weatherEffects);
            UpdateDistortionEffects(GameManager.Instance.distortionEffects);
        }
        else
        {
            sky.gameObject.SetActive(false);
            sunPivot.SetActive(false);
            clouds.gameObject.SetActive(false);
            
            SetSkyAndMaterialsColor(1, 1, 1, 0);
        }
    }

    Vector2 BackgroundWeatherScale()
    {
        return new Vector2(CameraWidth() * 2 * 3, _camera.orthographicSize * 2 * 3);
    }

    public void UpdateWeatherEffects(bool isOn)
    {
        if(!isOn)
        {
            snow.gameObject.SetActive(false);
            dust.SetActive(false);
            return;
        }

        if(localWorldManager.weatherType == LocalWorldManager.WeatherType.Snowing)
        {
            screenEffectsManager.SetTintColor(localWorldManager.snowingTintColor);

            snow.gameObject.SetActive(true);
            sun.SetActive(false);
            clouds.gameObject.SetActive(false);

            snow.GetChild(0).transform.localScale = BackgroundWeatherScale();

            ParticleSystem snowParticles = snow.GetChild(1).GetComponent<ParticleSystem>();
            ParticleSystem.ShapeModule shapeModule = snowParticles.shape;
            shapeModule.radius = CameraWidth() * 2 / 5;

            snowParticles.Stop();
            snow.GetChild(1).transform.localPosition = new Vector3(-CameraWidth(), _camera.orthographicSize, 20);
            snowParticles.Play();
        }

        if(localWorldManager.weatherType == LocalWorldManager.WeatherType.Dusting)
        {
            dust.transform.localScale = BackgroundWeatherScale();
            dust.gameObject.SetActive(true);
        }
    }

    public void UpdateDistortionEffects(bool isOn)
    {
        if(!isOn)
        {
            heatWave.SetActive(false);
            return;
        }

        if(localWorldManager.distortionType == LocalWorldManager.DistortionType.HeatWave)
        {
            heatWaveMesh = heatWave.GetComponent<MeshRenderer>();
            heatWave.transform.localScale = new Vector3(CameraWidth() / 5 + 0.2f, 1, _camera.orthographicSize / 5 + 0.2f);
            heatWave.SetActive(true);
            heatWaveMesh.material.SetFloat("Distortion", localWorldManager.heatWaveDistortionIntensity);
        }
    }

    float CameraWidth()
    {
        return _camera.orthographicSize * ((float)Screen.width / Screen.height);
    }

    public void SetDay()
    {
        sunGlow.SetActive(false);

        sunPivot.transform.eulerAngles = new Vector3(0, 0, 55);
        sun.transform.eulerAngles = new Vector3(0, 0, 0);
        moon.transform.eulerAngles = new Vector3(0, 0, 0);
        
        if(localWorldManager.weatherType != LocalWorldManager.WeatherType.Snowing)
            screenEffectsManager.SetTintColor(localWorldManager.dayTintColor);                
        
        SetSkyAndMaterialsColor(1, 1, 1, 0);
    }

    public void SetSunset()
    {
        sunGlow.SetActive(true);

        sunPivot.transform.localEulerAngles = new Vector3(0, 0, 0);
        sun.transform.eulerAngles = new Vector3(0, 0, 0);
        moon.transform.eulerAngles = new Vector3(0, 0, 0);

        if(localWorldManager.weatherType != LocalWorldManager.WeatherType.Snowing)
            screenEffectsManager.SetTintColor(localWorldManager.sunsetTintColor);

        SetSkyAndMaterialsColor(0.3f, 0.3f, 0.3f, 1);
    }

    public void SetNight()
    {
        sunPivot.transform.localEulerAngles = new Vector3(0, 0, 210);
        sun.transform.eulerAngles = new Vector3(0, 0, 0);
        moon.transform.eulerAngles = new Vector3(0, 0, 0);

        stars.gameObject.SetActive(true);

        if(localWorldManager.weatherType != LocalWorldManager.WeatherType.Snowing)
            screenEffectsManager.SetTintColor(localWorldManager.nightTintColor);

        SetSkyAndMaterialsColor(0.3f, 0.3f, 0.3f, 2);
    }

    public void SetSunrise()
    {
        sunPivot.SetActive(false);

        if(localWorldManager.weatherType != LocalWorldManager.WeatherType.Snowing)
            screenEffectsManager.SetTintColor(localWorldManager.sunriseTintColor);

        SetSkyAndMaterialsColor(0.6f, 0.6f, 0.6f, 3);
    }
    
    public void SetOvercast()
    {
        sun.SetActive(false);

        if(localWorldManager.weatherType != LocalWorldManager.WeatherType.Snowing)
            screenEffectsManager.SetTintColor(localWorldManager.overcastTintColor);

        SetSkyAndMaterialsColor(0.6f, 0.6f, 0.6f, 4);
    }

    void SetSkyAndMaterialsColor(float r, float g, float b, int skyIndex)
    {
        if(localWorldManager.weatherType == LocalWorldManager.WeatherType.Snowing || localWorldManager.timeOfDay == LocalWorldManager.TimeOfDay.Overcast)
        {
            sky.color = new Color(r, g, b, mainMaterial.color.a);
            sky.sprite = skyOvercast;
        }
        else
        {
            if(skyIndex == 0)
                sky.sprite = skyDay;
            else if(skyIndex == 1)
                sky.sprite = skySunset;
            else if(skyIndex == 2)
                sky.sprite = skyNight;
            else if(skyIndex == 3)
                sky.sprite = skySunrise;
            else if(skyIndex == 4)
                sky.sprite = skyOvercast;
        }

        if(localWorldManager.weatherType == LocalWorldManager.WeatherType.Dusting)
            foreach(MeshRenderer mesh in dustMeshes)
                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, localWorldManager.dustAlpha);
            
        mainMaterial.color = new Color(r, g, b, mainMaterial.color.a);
        mainMaterialStencil.color = mainMaterial.color;
    }

    public void ReloadSky()
    {
        sky.transform.localScale = new Vector2(CameraWidth() + 0.2f, _camera.orthographicSize / 17.1875f + 0.01f);
        stars.size = new Vector2(CameraWidth() * 2 + 0.5f, _camera.orthographicSize * 2 + 0.5f);
    }

    public bool UseWeatherEffects()
    {
        if(GameManager.Instance.weatherEffects)
            return true;
        else
            return false;
    }
}