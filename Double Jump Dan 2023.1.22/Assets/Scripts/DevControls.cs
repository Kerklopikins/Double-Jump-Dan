using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevControls : MonoBehaviour
{
    public bool enableTestingMode;

    WorldManager worldManager;
    List<Parallax> parallaxes;
    bool enteredDevMode;
    ItemManager itemManager;

    void Update()
    {   
        if(!enableTestingMode)
            return;

        if(Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.LeftShift) && !enteredDevMode)
        {
            worldManager = WorldManager.Instance;
            parallaxes = new List<Parallax>(FindObjectsByType<Parallax>(FindObjectsSortMode.None));
            itemManager = GetComponent<ItemManager>();

            enteredDevMode = true;
        }

        if(enteredDevMode)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                print("Objects Refreshed");
                worldManager.ReloadSky();
                LevelLoadingManager.Instance.ResizeFadeBackground();
                StatsHUD.Instance.PositionUI();

                foreach(var parallax in parallaxes)
                    parallax.OffsetBackground();
            }

            if(Input.GetKeyDown(KeyCode.Z))
            {
                GameObject oldGun = GameObject.Find("Arm 2").GetComponentInChildren<GunInfo>().gameObject;
                GameObject.Find("Arm 2").transform.localEulerAngles = new Vector3(0, 0, 90);
                oldGun.SetActive(false);
                
                Item randomItem = itemManager.guns[Random.Range(0, itemManager.guns.Count)];
                itemManager.InstantiateGun(randomItem);
            }

            if(Input.GetKeyDown(KeyCode.X))
            {
                Transform body = GameObject.Find("Body").transform;

                for(int i = 0; i < body.childCount; i++)
                    if(body.GetChild(i).GetComponent<Item>() != null)
                        if(body.GetChild(i).GetComponent<Item>().itemType == Item.ItemType.Hat)
                            body.GetChild(i).gameObject.SetActive(false);

                Item randomItem = itemManager.hats[Random.Range(0, itemManager.hats.Count)];
                itemManager.InstantiateHat(randomItem);
            }

            if(Input.GetKeyDown(KeyCode.V))
            {
                Item randomItem = itemManager.skins[Random.Range(0, itemManager.skins.Count)];
                itemManager.ChangeSkin(randomItem);
            }
        }
    }

    void OnGUI()
    {
        if(!enteredDevMode)
            return;
        
        float width = 300;
        float height = 500;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 500, width, height));

        GUILayout.Label("Dev Mode");
        GUILayout.TextField("Q to resize things");
        GUILayout.TextField("Z to Instantiate new gun");
        GUILayout.TextField("X to Instantiate new hat");
        GUILayout.TextField("V to change skin");

        if(GUILayout.Button("Day"))
        {
            worldManager.SetDay();
            worldManager.stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Sunset"))
        {
            worldManager.SetSunset();
            worldManager.stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Night"))
        {
            worldManager.SetNight();
            worldManager.stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Sunrise"))
        {
            worldManager.SetSunrise();
            worldManager.stars.gameObject.SetActive(false);
        }
        if(GUILayout.Button("Overcast"))
        {
            worldManager.SetOvercastSky();
        }
        if(GUILayout.Button("Toggle Dust"))
        {
            worldManager.dusting = !worldManager.dusting;
            worldManager.dust.SetActive(worldManager.dusting);
        }
        if(GUILayout.Button("Toggle Snow"))
        {
            worldManager.snowing = !worldManager.snowing;
            
            worldManager.snow.gameObject.SetActive(worldManager.snowing);
            worldManager.clouds.gameObject.SetActive(!worldManager.snowing);

            for(int i = 0; i < worldManager.snow.childCount; i++)
            {
                worldManager.snow.GetChild(i).gameObject.SetActive(worldManager.snowing);
            }

            float cameraSize = GetComponent<Camera>().orthographicSize * ((float)Screen.width / Screen.height);
            worldManager.snow.GetChild(0).transform.localScale = new Vector2(cameraSize * 2, GetComponent<Camera>().orthographicSize * 2);
            worldManager.snow.GetChild(1).transform.localScale = new Vector2(cameraSize * 2, GetComponent<Camera>().orthographicSize * 2);

            worldManager.snow.GetChild(2).GetComponent<ParticleSystem>().Stop();
            worldManager.snow.GetChild(2).transform.position = new Vector2(cameraSize - cameraSize, GetComponent<Camera>().orthographicSize * 2);
            worldManager.snow.GetChild(2).GetComponent<ParticleSystem>().Play();
        }
        if(GUILayout.Button("Toggle Clouds"))
        {
            bool cloudsEnabled = worldManager.clouds.gameObject.activeSelf;
            worldManager.clouds.gameObject.SetActive(!cloudsEnabled);
        }        

        GUILayout.EndArea();
    }
}