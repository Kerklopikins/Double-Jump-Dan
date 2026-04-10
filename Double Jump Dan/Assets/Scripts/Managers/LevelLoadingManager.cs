using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoadingManager : MonoBehaviour
{
    public static LevelLoadingManager Instance;
    [SerializeField] Sprite[] loadingDanSprites;
    [SerializeField] Sprite[] gameOverLoadingDanSprites;
    [SerializeField] bool startClear;

    public bool animationFinished { get; set; }
    public bool levelStarted { get; set; }
    public bool loading { get; set; }
    public bool done { get; set; }
	public SpriteRenderer fadeSprite { get; set; }
    SpriteRenderer loadingDan;
    Transform loadingSpritesTransform;
    GameObject loadingCircle;
    SpriteRenderer loadingCircleSprite;
    bool loaded;
    bool fadingIn;
    float levelDelay = 0.125f;
    GameObject eventSystem;
    GameHUD gameHUD;
    LocalWorldManager localWorldManager;
    Player player;
    GameObject levelLoadingGameObject;
    Camera _camera;

    void Awake()
    {
        Instance = this;

        levelLoadingGameObject = GameObject.Find("Level Loading");
        
        fadeSprite = levelLoadingGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        loadingSpritesTransform = levelLoadingGameObject.transform.GetChild(1).transform;

        loadingDan = loadingSpritesTransform.GetChild(0).GetComponent<SpriteRenderer>();
        loadingCircle = loadingDan.transform.GetChild(0).gameObject;
        loadingCircleSprite = loadingCircle.GetComponent<SpriteRenderer>();
        _camera = Camera.main;

        if(!startClear)
            fadeSprite.color = Color.black;
    }

    bool IsNormalLevel()
    {
        if(localWorldManager.world == LocalWorldManager.World.MainMenu || localWorldManager.world == LocalWorldManager.World.SplashScreen)
            return false;
        else
            return true;
    }

	void Start()
	{
		ResizeFadeBackground();

        localWorldManager = GameObject.FindWithTag("Level Managers").GetComponent<LocalWorldManager>();

        if(IsNormalLevel())
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            gameHUD = GameHUD.Instance;
            
            if(GameManager.died)
            {
                fadeSprite.color = new Color(1, 0, 0, fadeSprite.color.a);
                loadingCircleSprite.color = Color.black;
            }
        }

        if(localWorldManager.world != LocalWorldManager.World.SplashScreen)
            eventSystem = GameObject.Find("Event System");    

        if(localWorldManager.world == LocalWorldManager.World.MainMenu)
            levelDelay = 0.5f;
        else if(IsNormalLevel())
            levelDelay = 0.125f;
    }

    void Update()
    {
        if(loading || loaded)
            loadingCircle.transform.Rotate(Vector3.forward, -200 * Time.unscaledDeltaTime);

        if(animationFinished && !levelStarted)
        {
            animationFinished = false;
            levelStarted = true;
        }

        if(loaded)
        {
            loadingSpritesTransform.localScale -= Vector3.one * 1.5f * Time.unscaledDeltaTime;
            loadingSpritesTransform.localScale = new Vector3(Mathf.Clamp(loadingSpritesTransform.localScale.x, 0, 1), Mathf.Clamp(loadingSpritesTransform.localScale.y, 0, 1), Mathf.Clamp(loadingSpritesTransform.localScale.z, 0, 1));
        }

        if(fadingIn && !loaded)
        {
            if(eventSystem != null && eventSystem.activeInHierarchy == true)
                eventSystem.SetActive(false);
            
            if(player != null)
            {
                if(player.lives > 0)
                {
                    loadingCircleSprite.color = Color.white;
                    fadeSprite.color = new Color(0, 0, 0, fadeSprite.color.a);
                }
                else
                {
                    loadingCircleSprite.color = Color.black;
                    fadeSprite.color = new Color(1, 0, 0, fadeSprite.color.a);
                    GameManager.died = true;
                }
            }
            else
            {
                loadingCircleSprite.color = Color.white;
                fadeSprite.color = new Color(0, 0, 0, fadeSprite.color.a);
            }
            
            fadeSprite.color += new Color(0, 0, 0, 1.5f * Time.unscaledDeltaTime);
            
            if(localWorldManager.world != LocalWorldManager.World.SplashScreen)
            {
                loadingSpritesTransform.localScale += Vector3.one * 1.5f * Time.unscaledDeltaTime;
                loadingSpritesTransform.localScale = new Vector3(Mathf.Clamp(loadingSpritesTransform.localScale.x, 0, 1), Mathf.Clamp(loadingSpritesTransform.localScale.y, 0, 1), Mathf.Clamp(loadingSpritesTransform.localScale.z, 0, 1));
            } 
            
            fadeSprite.color = new Vector4(Mathf.Clamp(fadeSprite.color.r, 0, 1), Mathf.Clamp(fadeSprite.color.g, 0, 1), Mathf.Clamp(fadeSprite.color.b, 0, 1), Mathf.Clamp(fadeSprite.color.a, 0, 1));

            if(fadeSprite.color.a >= 1)
                animationFinished = true;
        }

        if(levelDelay > 0)
        {
            levelDelay -= Time.deltaTime;
            return;
        }

        FadeOut();
    }
    
    void FadeOut()
    {
        if(!levelStarted)
        {
            fadeSprite.color -= new Color(0, 0, 0, 1.5f * Time.unscaledDeltaTime);
            loadingSpritesTransform.localScale = Vector3.zero;

            fadeSprite.color = new Vector4(Mathf.Clamp(fadeSprite.color.r, 0, 1), Mathf.Clamp(fadeSprite.color.g, 0, 1), Mathf.Clamp(fadeSprite.color.b, 0, 1), Mathf.Clamp(fadeSprite.color.a, 0, 1));

            if(fadeSprite.color.a <= 0)
            {
                levelStarted = true;

                if(IsNormalLevel())
                    GameManager.died = false;
            }
        }

        if(fadeSprite.color.a <= 0)
            done = true;
        else
            done = false;
    }

    public void ResizeFadeBackground()
    {
        float cameraWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);
        
        if(Camera.main.orthographic)
			fadeSprite.transform.localScale = new Vector2(cameraWidth + 0.2f, _camera.orthographicSize + 0.2f);
		else
			fadeSprite.transform.localScale = new Vector2(cameraWidth + 0.2f, _camera.orthographicSize + 0.2f);
    }

    public void SetAnimationFinishedToTrue()
    {
        animationFinished = true;
    }

    public void LoadScene(int sceneToLoad)
    {
        fadingIn = true;
        loading = true;

        if(localWorldManager.world != LocalWorldManager.World.SplashScreen)
            GameManager.Instance.SaveUserData();

        ResizeFadeBackground();		
        StartCoroutine(LoadSceneSlowly(sceneToLoad));
    }

    public void LoadScene(string sceneToLoad)
    {
        fadingIn = true;
        loading = true;

        if(localWorldManager.world != LocalWorldManager.World.SplashScreen)
            GameManager.Instance.SaveUserData();
            
        ResizeFadeBackground();		
        StartCoroutine(LoadSceneSlowly(sceneToLoad));
    }

    IEnumerator LoadSceneSlowly(int sceneToLoad)
    {
        AnimateDanAndMusicFade();

        while(!animationFinished)
            yield return null;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while(!asyncLoad.isDone)
        {
            asyncLoad.allowSceneActivation = false;

            if(asyncLoad.progress == 0.9f)
                loaded = true;

            if(loadingSpritesTransform.localScale.x <= 0)
                asyncLoad.allowSceneActivation = true;

            yield return null;
        }
    }

    IEnumerator LoadSceneSlowly(string sceneToLoad)
    {
        AnimateDanAndMusicFade();

        while(!animationFinished)
            yield return null;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while(!asyncLoad.isDone)
        {
            asyncLoad.allowSceneActivation = false;

            if(asyncLoad.progress == 0.9f)
                loaded = true;

            if(loadingSpritesTransform.transform.localScale.x <= 0)
                asyncLoad.allowSceneActivation = true;

            yield return null;
        }
    }

    void AnimateDanAndMusicFade()
    {
        if(localWorldManager.world == LocalWorldManager.World.SplashScreen)
            return;
            
        if(IsNormalLevel())
        {
            if(!gameHUD.paused)
                AudioManager.Instance.FadeMusicOut(1);
        }
        else
        {
            if(AudioManager.Instance != null)
                AudioManager.Instance.FadeMusicOut(1);
        }

        if(IsNormalLevel())
        {
            if(GameManager.died)
                StartCoroutine(LoadingDanAnimation());
            else
                StartCoroutine(LoadingDanAnimation());
        }
        else
        {
            StartCoroutine(LoadingDanAnimation());
        }
    }

    IEnumerator LoadingDanAnimation()
    {
        while(true)
        {
            if(IsNormalLevel() && player.lives <= 0)
            {
                loadingDan.sprite = gameOverLoadingDanSprites[0];
                yield return new WaitForSecondsRealtime(0.075f);
                loadingDan.sprite = gameOverLoadingDanSprites[1];
                yield return new WaitForSecondsRealtime(0.075f);
                loadingDan.sprite = gameOverLoadingDanSprites[2];
                yield return new WaitForSecondsRealtime(0.075f);
                loadingDan.sprite = gameOverLoadingDanSprites[1];
                yield return new WaitForSecondsRealtime(0.075f);
            }
            else
            {
                loadingDan.sprite = loadingDanSprites[0];
                yield return new WaitForSecondsRealtime(0.075f);
                loadingDan.sprite = loadingDanSprites[1];
                yield return new WaitForSecondsRealtime(0.075f);
                loadingDan.sprite = loadingDanSprites[2];
                yield return new WaitForSecondsRealtime(0.075f);
                loadingDan.sprite = loadingDanSprites[1];
                yield return new WaitForSecondsRealtime(0.075f);
            }
        }
    }
}