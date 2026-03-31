using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [SerializeField] Transform levelStart;
    [SerializeField] Transform levelFinish;
    [SerializeField] Player player;


    [Header("Finish Level")]
    [SerializeField] float maxDetectionHeight;

    public int gems { get; set; }
    public Vector2 currentSpawnPoint { get; private set; }
    bool playerEntered;
    float xDistance = 1.5f;
    GameManager gameManager;
    int currentScene;
    bool finishedLevel;
    LocalWorldManager localWorldManager;

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
                if(currentScene >= gameManager.levelsCompleted)
                    gameManager.levelsCompleted = currentScene + 1;
            }

            GameHUD.Instance.FinishLevel();
            gameManager.SaveUserData();
            
            playerEntered = true;
        }
    }
    public void AddGems(int gemsToGive)
    {
        gems += gemsToGive;
        GameManager.Instance.gems += gemsToGive;
        GameManager.Instance.totalGemsCollected += gemsToGive;
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
        Gizmos.color = new Color(0, 0, 1, 0.5f);
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

        //Gizmos.DrawSphere(new Vector3(transform.position.x - xDistance, transform.position.y, 0), 0.1f);
        //Gizmos.DrawSphere(new Vector3(transform.position.x + xDistance, transform.position.y, 0), 0.1f);
    #endif
    }
}