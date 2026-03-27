using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Sprite[] gemSprites;
    [SerializeField] Material mainMaterial;
    [SerializeField] Material mainMaterialStencil;

    //Game Data
    [HideInInspector] public List<int> users = new List<int>();
    [HideInInspector] public List<string> userNames = new List<string>();
    [HideInInspector] public int currentUser;
    [HideInInspector] public string currentUserName;
    [HideInInspector] public float sfxVolume = 1;
    [HideInInspector] public float musicVolume = 1;
	[HideInInspector] public int screenResolution = -1;
    
    //User Data
    [HideInInspector] public int gems;
    [HideInInspector] public List<int> ownedHats = new List<int>();
    [HideInInspector] public List<int> ownedGuns = new List<int>();
	[HideInInspector] public List<int> ownedSkins = new List<int>();
    [HideInInspector] public int hatID;
    [HideInInspector] public int gunID;
	[HideInInspector] public int skinID;
	[HideInInspector] public int levelsCompleted;
	
    string folderPath;
    bool started;
    public SpriteRenderer centralizedGem { get; set; }
    LocalWorldManager localWorldManager;
    public static bool died;

    void Awake()
    {
        Instance = this;

		//folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Double Jump Dan";
        folderPath = Application.persistentDataPath;

		if(!Directory.Exists(folderPath))   
			Directory.CreateDirectory(folderPath);
        
        LoadData();
        LoadUserData();
        
        localWorldManager = GameObject.FindWithTag("Local World Manager").GetComponent<LocalWorldManager>();

        if(localWorldManager.world != LocalWorldManager.World.MainMenu)
            centralizedGem = transform.Find("Centralized Gem").GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        if(!started)
            StartCoroutine(SetStartedToTrueDelayed());
    }
    
    IEnumerator SetStartedToTrueDelayed()
    {
        yield return new WaitForEndOfFrame();
        started = true;
        SaveData();
    }
    
    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(folderPath + "/GameData.djd");
        GameData gameData = new GameData();

        //Game Data Here
        gameData.started = started;
        gameData.users = users;
        gameData.userNames = userNames;
        gameData.currentUser = currentUser;
        gameData.currentUserName = currentUserName;
        gameData.sfxVolume = sfxVolume;
        gameData.musicVolume = musicVolume;
		gameData.screenResolution = screenResolution;

        bf.Serialize(file, gameData);
        file.Close();
    }

    public void LoadData()
    {
		if(File.Exists(folderPath + "/GameData.djd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(folderPath + "/GameData.djd", FileMode.Open);
            GameData gameData = (GameData)bf.Deserialize(file);
            file.Close();

            //Game Data Here
            started = gameData.started;
            users = gameData.users;
            userNames = gameData.userNames;
            currentUser = gameData.currentUser;
            currentUserName = gameData.currentUserName;
            sfxVolume = gameData.sfxVolume;
            musicVolume = gameData.musicVolume;
			screenResolution = gameData.screenResolution;
        }
    }

    public void SaveUserData()
    {
        BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(folderPath + "/UserData" + currentUser + ".djd");
        UserData userData = new UserData();

        //User Data Here
        userData.gems = gems;
        userData.ownedHats = ownedHats;
        userData.ownedGuns = ownedGuns;
		userData.ownedSkins = ownedSkins;
        userData.hatID = hatID;
        userData.gunID = gunID;
		userData.skinID = skinID;
		userData.levelsCompleted = levelsCompleted;

        bf.Serialize(file, userData);
        file.Close();
    }

    public void LoadUserData()
    {
		if(File.Exists(folderPath + "/UserData" + currentUser + ".djd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(folderPath + "/UserData" + currentUser + ".djd", FileMode.Open);
            UserData userData = (UserData)bf.Deserialize(file);
            file.Close();

            //User Data Here
            gems = userData.gems;
            ownedHats = userData.ownedHats;
            ownedGuns = userData.ownedGuns;
			ownedSkins = userData.ownedSkins;
            hatID = userData.hatID;
            gunID = userData.gunID;
			skinID = userData.skinID;
			levelsCompleted = userData.levelsCompleted;
        }
    }

    public void DeleteUserData(int user)
    {
		if(File.Exists(folderPath + "/UserData" + user + ".djd"))
			File.Delete(folderPath + "/UserData" + user + ".djd");
    }

    public void ResetCurrentUserData()
    {
        UserData userData = new UserData();

        gems = userData.gems;
        ownedHats = userData.ownedHats;
        ownedGuns = userData.ownedGuns;
		ownedSkins = userData.ownedSkins;
		levelsCompleted = userData.levelsCompleted;
    }

    public int LoadUserFileSize(int _currentUser)
    {
		FileStream file = File.Open(folderPath + "/UserData" + _currentUser + ".djd", FileMode.Open);
        int fileSize = (int)file.Length;
        file.Close();

        return fileSize;
    }

    public void ResetGame()
    {
        for(int i = 0; i < users.Count; i++)
        {
			if(File.Exists(folderPath + "/UserData" + users[i] + ".djd"))
				File.Delete(folderPath + "/UserData" + users[i] + ".djd");
        }

		if(File.Exists(folderPath + "/GameData.djd"))
			File.Delete(folderPath + "/GameData.djd");
    }

    void OnApplicationQuit()
    {
        mainMaterial.color = new Color(1, 1, 1, mainMaterial.color.a);
        mainMaterialStencil.color = mainMaterial.color;
    }
}

[System.Serializable]
public class GameData
{
    public bool started;
    public List<int> users = new List<int>();
    public List<string> userNames = new List<string>();
    public int currentUser;
    public string currentUserName;
    public float sfxVolume;
    public float musicVolume;
	public int screenResolution = -1;
}

[System.Serializable]
public class UserData
{
    public int gems;
    public List<int> ownedHats = new List<int>();
    public List<int> ownedGuns = new List<int>();
	public List<int> ownedSkins = new List<int>();
    public int hatID;
    public int gunID;
	public int skinID;
	public int levelsCompleted;
}