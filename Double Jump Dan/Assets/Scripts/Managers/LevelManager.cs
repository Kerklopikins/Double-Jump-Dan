using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [SerializeField] Transform levelStart;
    [SerializeField] Transform levelFinish;
    [SerializeField] Player player;

    [Header("Finish Level")]
    [SerializeField] float maxDetectionHeight;

    [Header("Random Object Placement")]
    [SerializeField] List<SpawnType> spawnTypes = new List<SpawnType>();

    public int gems { get; set; }
    public Vector2 currentSpawnPoint { get; private set; }
    bool playerEntered;
    float xDistance = 1.5f;
    GameManager gameManager;
    int currentScene;
    bool finishedLevel;
    LocalWorldManager localWorldManager;
    int objectSpawnProbability;
    int randomObjectIndex;

    void Awake()
    {
        Instance = this;

        if(levelStart != null)
            currentSpawnPoint = levelStart.position;
        else
            currentSpawnPoint = player.transform.position;

        localWorldManager = GetComponent<LocalWorldManager>();
        Respawn();
    }

    void Start()
    {
        gameManager = GameManager.Instance;

        if(SceneManager.GetActiveScene().buildIndex > 2)
        {
            string levelName = SceneManager.GetActiveScene().name;
            currentScene = int.Parse(levelName.Replace("Level ", ""));
        }

        if(spawnTypes.Count > 0)
        {
            foreach(var spawnType in spawnTypes)
            {
                for(int i = 0; i < spawnType.objectSpawnPoints.Count; i++)
                {
                    if(spawnType.objectsParent == null)
                    {
                        GameObject objectHolder = new GameObject(spawnType.name);
                        spawnType.objectsParent = objectHolder.transform;
                        spawnType.objectsParent.parent = GameObject.FindWithTag("Level Objects").transform;
                    }
                        
                    randomObjectIndex = UnityEngine.Random.Range(0, spawnType.objects.Count);
                    objectSpawnProbability = UnityEngine.Random.Range(0, 100);

                    if(objectSpawnProbability <= spawnType.objects[randomObjectIndex].placementProbability)
                        Instantiate(spawnType.objects[randomObjectIndex].prefab, (Vector2)spawnType.objectSpawnPoints[i].position + spawnType.objects[randomObjectIndex].spawnOffset, Quaternion.identity, spawnType.objectsParent);
                }
            }
        }  
    }
    void Update()
    {
        float playerXPositionAbs = Mathf.Abs(levelFinish.position.x - player.transform.position.x);

        if(playerXPositionAbs < xDistance && player.transform.position.y > levelFinish.position.y - 1 && !playerEntered)
        {
            if(player.transform.position.y > levelFinish.position.y - 1 + maxDetectionHeight)
                return;

            finishedLevel = true;

            if(localWorldManager.world != LocalWorldManager.World.Tutorial)
            {
                if(currentScene >= gameManager.currentUser.levelsCompleted)
                    gameManager.currentUser.levelsCompleted = currentScene + 1;
            }

            GameHUD.Instance.FinishLevel();
            gameManager.SaveUserData();
            
            playerEntered = true;
        }
    }
    public void AddGems(int gemsToGive)
    {
        gems += gemsToGive;
        GameManager.Instance.currentUser.gems += gemsToGive;
        GameManager.Instance.currentUser.totalGemsCollected += gemsToGive;
        StatsHUD.Instance.UpdateGemsCounter(gems);
    }

    public bool FinishedLevel()
    {
        return finishedLevel;
    }
    
    public void Respawn()
    {
        player.transform.position = new Vector2(currentSpawnPoint.x, currentSpawnPoint.y - 0.125f);
    }

    public void UpdateSpawnPoint(Vector2 position)
    {
        currentSpawnPoint = position;
    }

    void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(levelStart.position, new Vector3(2, 2, 1));
        Gizmos.DrawCube(levelFinish.position, new Vector3(2, 2, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(levelFinish.position.x, levelFinish.position.y - 1, 0), Vector3.up + new Vector3(0, maxDetectionHeight - 1, 0));
        
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        Handles.Label(new Vector2(levelStart.position.x, levelStart.position.y + 2), "Level Start", style);
        Handles.Label(new Vector2(levelFinish.position.x, levelFinish.position.y + 2), "Level Finish", style);

        Gizmos.color = new Color(0, 1, 1, 0.5f);
        GUIStyle smallStyle = new GUIStyle();
        smallStyle.normal.textColor = Color.white;
        smallStyle.fontSize = 8;
        smallStyle.alignment = TextAnchor.MiddleCenter;
        smallStyle.fontStyle = FontStyle.Bold;

        if(spawnTypes.Count > 0)
        {
            foreach(var spawnType in spawnTypes)
            {
                if(spawnType.objectSpawnPoints.Count > 0)
                {
                    for(int i = 0; i < spawnType.objectSpawnPoints.Count; i++)
                    {
                        Handles.Label(new Vector2(spawnType.objectSpawnPoints[i].position.x, spawnType.objectSpawnPoints[i].position.y + 2), spawnType.name + " Spawn Point", smallStyle);
                        Gizmos.DrawCube(spawnType.objectSpawnPoints[i].position, new Vector3(2, 2, 1));
                    }
                }
            }
        }  

        //Gizmos.DrawSphere(new Vector3(transform.position.x - xDistance, transform.position.y, 0), 0.1f);
    #endif
    }

    [Serializable]
    public class SpawnType
    {
        public string name;
        public List<Transform> objectSpawnPoints = new List<Transform>();
        public List<LevelObject> objects = new List<LevelObject>();
        public Transform objectsParent;
    }

    [Serializable]
    public class LevelObject
    {
        public int placementProbability;
        public GameObject prefab;
        public Vector2 spawnOffset;
    }
}