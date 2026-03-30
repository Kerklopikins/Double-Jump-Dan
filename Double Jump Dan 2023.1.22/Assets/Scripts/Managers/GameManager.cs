using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Sprite[] gemSprites;
    public Material mainMaterial;
    public Material mainMaterialStencil;

    //Game Data
    public List<int> users = new List<int>();
    public List<string> userNames = new List<string>();
    public List<int> userColorIndexes = new List<int>();
    public int currentUser;
    public string currentUserName;
    public float sfxVolume = 1;
    public float musicVolume = 1;
	public int screenResolution = -1;
    
    //User Data
    public int gems;
    public List<int> ownedHats = new List<int>();
    public List<int> ownedGuns = new List<int>();
	public List<int> ownedSkins = new List<int>();
    public int hatID;
    public int gunID;
	public int skinID;
	public int levelsCompleted;
    public int totalEnemiesKilled;
    public int totalDeaths;
    public int totalGemsCollected;
    public double totalPlaytime;
	
    string folderPath;
    public SpriteRenderer centralizedGem { get; set; }
    LocalWorldManager localWorldManager;
    public static bool died;
    bool inMainMenu;
    MainMenuManager mainMenuManager;

    void Awake()
    {
        Instance = this;
        folderPath = Application.persistentDataPath;

        if(SceneManager.GetActiveScene().name == "Main Menu")
        {
            inMainMenu = true;
            mainMenuManager = GameObject.FindWithTag("Main Menu").GetComponent<MainMenuManager>();
        }
        
        if(!File.Exists(folderPath + "/GameData.json"))
        {
            SaveData();
        }
        else
        {
            LoadData();
            LoadUserData();
        }        
        
        localWorldManager = GameObject.FindWithTag("Local World Manager").GetComponent<LocalWorldManager>();

        if(localWorldManager.world != LocalWorldManager.World.MainMenu)
            centralizedGem = transform.Find("Centralized Gem").GetComponent<SpriteRenderer>();     
    }

    void Update()
    {
        if(users.Count > 0)
            totalPlaytime += Time.unscaledDeltaTime;
    }
    
    #region Encryption
    public static string Encrypt(string plainText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32)); // 256-bit key

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                return Convert.ToBase64String(aes.IV) + ":" + Convert.ToBase64String(encrypted);
            }
        }
    }

    public static string Decrypt(string cipherText, string key)
    {
        var parts = cipherText.Split(':');

        byte[] iv = Convert.FromBase64String(parts[0]);
        byte[] data = Convert.FromBase64String(parts[1]);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor())
            {
                byte[] decrypted = decryptor.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }

    #endregion

    #region GameData
    public void SaveData()
    {
        GameData gameData = new GameData();

        //Game Data
        gameData.users = users;
        gameData.userNames = userNames;
        gameData.userColorIndexes = userColorIndexes;
        gameData.currentUser = currentUser;
        gameData.currentUserName = currentUserName;
        gameData.sfxVolume = sfxVolume;
        gameData.musicVolume = musicVolume;
		gameData.screenResolution = screenResolution;

        string json = JsonUtility.ToJson(gameData);
        string encrypted = Encrypt(json, "5a82be8ec0fdafa41013f6ac33b109");
        File.WriteAllText(folderPath + "/GameData.json", encrypted);
    }

    public void LoadData()
    {
		if(File.Exists(folderPath + "/GameData.json"))
        {
            string encrypted = File.ReadAllText(folderPath + "/GameData.json");
            string json = Decrypt(encrypted, "5a82be8ec0fdafa41013f6ac33b109");
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            //Game Data
            users = gameData.users;
            userNames = gameData.userNames;
            userColorIndexes = gameData.userColorIndexes;
            currentUser = gameData.currentUser;
            currentUserName = gameData.currentUserName;
            sfxVolume = gameData.sfxVolume;
            musicVolume = gameData.musicVolume;
			screenResolution = gameData.screenResolution;
        }
    }
    #endregion

    #region UserData
    public void SaveUserData()
    {
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
        userData.hash = (userData.gems * 17 + 9).ToString();
        userData.totalEnemiesKilled = totalEnemiesKilled;
        userData.totalDeaths = totalDeaths;
        userData.totalGemsCollected = totalGemsCollected;
        userData.totalPlaytime = totalPlaytime;

        string json = JsonUtility.ToJson(userData);
        string encrypted = Encrypt(json, "5a82be8ec0fdafa41013f6ac33b109");

        File.WriteAllText(folderPath + "/UserData" + currentUser + ".json", encrypted);
        File.WriteAllText(folderPath + "/UserData" + currentUser + ".bak", encrypted);
    }

    public void LoadUserData()
    {
		if(File.Exists(folderPath + "/UserData" + currentUser + ".json"))
        {            
            string encrypted = File.ReadAllText(folderPath + "/UserData" + currentUser + ".json");
            string json = Decrypt(encrypted, "5a82be8ec0fdafa41013f6ac33b109");
            UserData userData = JsonUtility.FromJson<UserData>(json);

            if(userData.hash == (userData.gems * 17 + 9).ToString())
            {
                gems = userData.gems;
                ownedHats = userData.ownedHats;
                ownedGuns = userData.ownedGuns;
                ownedSkins = userData.ownedSkins;
                hatID = userData.hatID;
                gunID = userData.gunID;
                skinID = userData.skinID;
                levelsCompleted = userData.levelsCompleted;
                totalEnemiesKilled = userData.totalEnemiesKilled;
                totalDeaths = userData.totalDeaths;
                totalGemsCollected = userData.totalGemsCollected;
                totalPlaytime = userData.totalPlaytime;
                return;
            }
        }

        if(File.Exists(folderPath + "/UserData" + currentUser + ".bak"))
        {
            string encryptedBak = File.ReadAllText(folderPath + "/UserData" + currentUser + ".bak");
            string jsonBak = Decrypt(encryptedBak, "5a82be8ec0fdafa41013f6ac33b109");
            UserData userDataBak = JsonUtility.FromJson<UserData>(jsonBak);

            if(userDataBak.hash == (userDataBak.gems * 17 + 9).ToString())
            {
                if(inMainMenu)
                    mainMenuManager.TamperedUserFile("User file tampered with, loading backup...");

                gems = userDataBak.gems;
                ownedHats = userDataBak.ownedHats;
                ownedGuns = userDataBak.ownedGuns;
                ownedSkins = userDataBak.ownedSkins;
                hatID = userDataBak.hatID;
                gunID = userDataBak.gunID;
                skinID = userDataBak.skinID;
                levelsCompleted = userDataBak.levelsCompleted;
                totalEnemiesKilled = userDataBak.totalEnemiesKilled;
                totalDeaths = userDataBak.totalDeaths;
                totalGemsCollected = userDataBak.totalGemsCollected;
                totalPlaytime = userDataBak.totalPlaytime;
                SaveUserData();
                return;
            }
            else
            {
                if(inMainMenu)
                    mainMenuManager.TamperedUserFile("All user files tampered with, creating new file...");
                
                LoadDefaultUserData();
                SaveUserData();
            }              
        }
    }

    public void DeleteUserData(int user)
    {
		if(File.Exists(folderPath + "/UserData" + user + ".json"))
			File.Delete(folderPath + "/UserData" + user + ".json");

        if(File.Exists(folderPath + "/UserData" + user + ".bak"))
			File.Delete(folderPath + "/UserData" + user + ".bak");
    }

    public void LoadDefaultUserData()
    {
        gems = 0;
        ownedHats.Clear();
        ownedGuns.Clear();
		ownedSkins.Clear();
		levelsCompleted = 1;
        totalEnemiesKilled = 0;
        totalDeaths = 0;
        totalGemsCollected = 0;
        totalPlaytime = 0;

        ownedHats.Add(1111);
        hatID = 1111;
        ownedGuns.Add(1111);
        gunID = 1111;
        ownedSkins.Add(1111);
        skinID = 1111;
    }

    public int LoadUserFileSize(int _currentUser)
    {
		FileStream file = File.Open(folderPath + "/UserData" + _currentUser + ".json", FileMode.Open);
        int fileSize = (int)file.Length;
        file.Close();
        return fileSize;
    }
    #endregion

    public void ResetGame()
    {
        for(int i = 0; i < users.Count; i++)
        {
			if(File.Exists(folderPath + "/UserData" + users[i] + ".json"))
				File.Delete(folderPath + "/UserData" + users[i] + ".json");
            
            if(File.Exists(folderPath + "/UserData" + users[i] + ".bak"))
				File.Delete(folderPath + "/UserData" + users[i] + ".bak");
        }

		if(File.Exists(folderPath + "/GameData.json"))
			File.Delete(folderPath + "/GameData.json");
    }

    void OnApplicationQuit()
    {        
        if(users.Count > 0)
        {
            SaveData();
            SaveUserData();
        }
        
        mainMaterial.color = new Color(1, 1, 1, mainMaterial.color.a);
        mainMaterialStencil.color = mainMaterial.color;
    }
}

[System.Serializable]
public class GameData
{
    public List<int> users = new List<int>();
    public List<string> userNames = new List<string>();
    public List<int> userColorIndexes = new List<int>();
    public int currentUser;
    public string currentUserName;
    public float sfxVolume;
    public float musicVolume;
	public int screenResolution;
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
    public string hash;
    public int totalEnemiesKilled;
    public int totalDeaths;
    public int totalGemsCollected;
    public double totalPlaytime;
}