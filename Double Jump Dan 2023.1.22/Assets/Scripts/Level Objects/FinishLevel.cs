using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    [SerializeField] GameObject levelCompleteUI;
    [SerializeField] float maxDetectionHeight;

    public bool levelComplete { get; protected set; }
    Player player;
    bool playerEntered;
    float xDistance = 1.5f;
    GameManager gameManager;
	GameHUD gameHUD;
    int currentScene;
    void Start()
    {
        gameManager = GameManager.Instance;
        gameHUD = GameHUD.Instance;
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if(SceneManager.GetActiveScene().buildIndex > 2)
        {
            string levelName = SceneManager.GetActiveScene().name;
            currentScene = int.Parse(levelName.Replace("Level ", ""));
        }
    }

    void Update()
    {
        float playerXPositionAbs = Mathf.Abs(transform.position.x - player.transform.position.x);

        if(playerXPositionAbs < xDistance && player.transform.position.y < transform.position.y + maxDetectionHeight && !playerEntered)
        {
            if(SceneManager.GetActiveScene().name == "Tutorial")
            {
                LevelLoadingManager.Instance.LoadScene("Main Menu");
                playerEntered = true;
                return;
            }
           
            if(currentScene >= gameManager.levelsCompleted)
                gameManager.levelsCompleted = currentScene + 1;

            if(levelCompleteUI != null)
                levelCompleteUI.SetActive(true);
            else
                gameHUD.LoadNextScene();

            levelComplete = true;
            player.finishedLevel = true;

            gameManager.SaveUserData();
            playerEntered = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, 0), Vector3.up + new Vector3(0, maxDetectionHeight - 1, 0));

        //Gizmos.DrawSphere(new Vector3(transform.position.x - xDistance, transform.position.y, 0), 0.1f);
        //Gizmos.DrawSphere(new Vector3(transform.position.x + xDistance, transform.position.y, 0), 0.1f);
    }
}