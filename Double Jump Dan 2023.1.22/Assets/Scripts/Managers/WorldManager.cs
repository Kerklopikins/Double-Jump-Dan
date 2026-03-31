using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    [Header("Time of Day")]
    public Material mainMaterial;
    public Material mainMaterialStencil;
    [SerializeField] SpriteRenderer sky;
    [SerializeField] GameObject sunPivot;
    [SerializeField] GameObject sun;
    [SerializeField] GameObject moon;
    [SerializeField] SpriteRenderer stars;
    [SerializeField] Sprite skyDay;
    [SerializeField] Sprite skySunset;
    [SerializeField] Sprite skyNight;
    [SerializeField] Sprite skySunrise;
    [SerializeField] Sprite skyOvercast;

    [Header("Weather")]
    [SerializeField] Transform clouds;
    [SerializeField] GameObject sunGlow;
    [SerializeField] GameObject dust;
    [SerializeField] Transform snow;

    bool overcast;
    float cloudEmissionOverTime;
    bool snowing;
    bool dusting;
    float cameraSize;
    Camera _camera;
    ParticleSystem cloudParticleSystem;
    ParticleSystemRenderer cloudParticleSystemRenderer;
    LocalWorldManager localWorldManager;
    bool testingMode;
    
    void Awake()
    {
        Instance = this;
        localWorldManager = GameObject.FindWithTag("Level Managers").GetComponent<LocalWorldManager>();
    }

    void OnGUI()
    {
        if(!testingMode)
            return;
        
        float width = 150;
        float height = 500;

        GUILayout.BeginArea(new Rect(10, Screen.height - 500, width, height));

        GUILayout.Label("Time Of Day Changer");
        if(GUILayout.Button("Day"))
        {
            SetDay();
            stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Sunset"))
        {
            SetSunset();
            stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Night"))
        {
            SetNight();
            stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Sunrise"))
        {
            SetSunrise();
            stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Overcast"))
        {
            sky.sprite = skyOvercast;
            sky.color = new Color(1, 1, 1, mainMaterial.color.a);
            mainMaterial.color = new Color(1, 1, 1, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
        if(GUILayout.Button("Toggle Dust"))
        {
            dusting = !dusting;
            dust.SetActive(dusting);
        }
        if(GUILayout.Button("Toggle Snow"))
        {
            snowing = !snowing;
            
            snow.gameObject.SetActive(snowing);
            clouds.gameObject.SetActive(!snowing);

            for(int i = 0; i < snow.childCount; i++)
            {
                snow.GetChild(i).gameObject.SetActive(snowing);
            }

            float cameraSize = GetComponent<Camera>().orthographicSize * ((float)Screen.width / Screen.height);
            snow.GetChild(0).transform.localScale = new Vector2(cameraSize * 2, GetComponent<Camera>().orthographicSize * 2);
            snow.GetChild(1).transform.localScale = new Vector2(cameraSize * 2, GetComponent<Camera>().orthographicSize * 2);

            snow.GetChild(2).GetComponent<ParticleSystem>().Stop();
            snow.GetChild(2).transform.position = new Vector2(cameraSize - cameraSize, GetComponent<Camera>().orthographicSize * 2);
            snow.GetChild(2).GetComponent<ParticleSystem>().Play();
        }
        if(GUILayout.Button("Toggle Clouds"))
        {
            bool cloudsEnabled = clouds.gameObject.activeSelf;
            clouds.gameObject.SetActive(!cloudsEnabled);
        }        

        GUILayout.EndArea();
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
            
            mainMaterial.color = new Color(1, 1, 1, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
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

            float cameraSize = GetComponent<Camera>().orthographicSize * ((float)Screen.width / Screen.height);
            snow.GetChild(0).transform.localScale = new Vector2(cameraSize * 2, GetComponent<Camera>().orthographicSize * 2);
            snow.GetChild(1).transform.localScale = new Vector2(cameraSize * 2, GetComponent<Camera>().orthographicSize * 2);

            snow.GetChild(2).GetComponent<ParticleSystem>().Stop();
            snow.GetChild(2).transform.position = new Vector2(cameraSize - cameraSize, GetComponent<Camera>().orthographicSize * 2);
            snow.GetChild(2).GetComponent<ParticleSystem>().Play();
        }

        if(dusting)
            dust.SetActive(true);
    }

    void SetDay()
    {
        if(!overcast)
        {
            sky.sprite = skyDay;
            sunPivot.transform.eulerAngles = new Vector3(0, 0, 55);
            sun.transform.eulerAngles = new Vector3(0, 0, 0);
            moon.transform.eulerAngles = new Vector3(0, 0, 0);

            mainMaterial.color = new Color(1, 1, 1, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
        else
        {
            sky.sprite = skyOvercast;
            sun.SetActive(false);
            sky.color = new Color(1, 1, 1, mainMaterial.color.a);
            mainMaterial.color = new Color(1, 1, 1, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
    }

    void SetSunset()
    {
        if(!overcast)
        {
            sunGlow.SetActive(true);

            sky.sprite = skySunset;
            sunPivot.transform.localEulerAngles = new Vector3(0, 0, 0);
            sun.transform.eulerAngles = new Vector3(0, 0, 0);
            moon.transform.eulerAngles = new Vector3(0, 0, 0);

            mainMaterial.color = new Color(0.3f, 0.3f, 0.3f, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
        else
        {
            sky.sprite = skyOvercast;
            sun.SetActive(false);
            sky.color = new Color(0.3f, 0.3f, 0.3f, mainMaterial.color.a);
            mainMaterial.color = new Color(0.3f, 0.3f, 0.3f, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
    }

    void SetNight()
    {
        if(!overcast)
        {
            sky.sprite = skyNight;
            sunPivot.transform.localEulerAngles = new Vector3(0, 0, 210);
            sun.transform.eulerAngles = new Vector3(0, 0, 0);
            moon.transform.eulerAngles = new Vector3(0, 0, 0);

            stars.gameObject.SetActive(true);
            mainMaterial.color = new Color(0.3f, 0.3f, 0.3f, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
        else
        {
            sky.sprite = skyOvercast;
            sun.SetActive(false);
            sky.color = new Color(0.3f, 0.3f, 0.3f, mainMaterial.color.a);
            mainMaterial.color = new Color(0.3f, 0.3f, 0.3f, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
    }

    public bool UseWeatherEffects()
    {
        if(GameManager.Instance.weatherEffects)
            return true;
        else
            return false;
    }

    void SetSunrise()
    {
        if(!overcast)
        {
            sky.sprite = skySunrise;
            sunPivot.SetActive(false);

            mainMaterial.color = new Color(0.6f, 0.6f, 0.6f, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
        else
        {
            sky.sprite = skyOvercast;
            sun.SetActive(false);
            sky.color = new Color(0.6f, 0.6f, 0.6f, mainMaterial.color.a);
            mainMaterial.color = new Color(0.6f, 0.6f, 0.6f, mainMaterial.color.a);
            mainMaterialStencil.color = mainMaterial.color;
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            print("Sky resized");
            ReloadSky();
        }

        if(Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.LeftShift))
            testingMode = true;
    }
    public void ReloadSky()
    {
        cameraSize = _camera.orthographicSize * ((float)Screen.width / Screen.height);
        sky.transform.parent.localScale = new Vector3(cameraSize, sky.transform.parent.localScale.y, 1);

        if(localWorldManager.timeOfDay == LocalWorldManager.TimeOfDay.Night)
            stars.size = new Vector2(cameraSize * 2, stars.size.y);
    }
}