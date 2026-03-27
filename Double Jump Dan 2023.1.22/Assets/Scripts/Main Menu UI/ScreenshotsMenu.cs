using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class ScreenshotsMenu : MonoBehaviour
{
    [SerializeField] Image screenshotButton;
    [SerializeField] Transform imagesParent;
    [SerializeField] GameObject noScreenshotsText;
    public Animator screenshotViewerAnimator;
    public Text screenshotViewerText;
    public Image screenshotViewerImage;

    UIScreenManager uiScreenManager;
    bool loadedScreenshots;

    void Start()
    {
        uiScreenManager = GetComponent<UIScreenManager>();

        if(!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/Double Jump Dan"))
            noScreenshotsText.SetActive(true);
    }

    private Sprite LoadSprite(string path)
    {
        if(string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is null");
            return null;
        }

        if(!File.Exists(path))
        {
            Debug.LogError("File is null");
            return null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
        ImageConversion.LoadImage(texture, bytes, false);
        texture.filterMode = FilterMode.Bilinear;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        //texture.Apply(false, false);

        return sprite;
    }

    public void MaybeLoadScreenshots()
    {
		if(noScreenshotsText.activeInHierarchy)
			return;
		
        if(loadedScreenshots)
            return;

        DirectoryInfo directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/Double Jump Dan");
        FileInfo[] files = directory.GetFiles("*.jpg");

        if(files.Length == 0)
        {
            noScreenshotsText.SetActive(true);
            return;
        }

        for(int i = 0; i < files.Length; i++)
        {
            var _image = (Image)Instantiate(screenshotButton, Vector3.zero, Quaternion.identity, imagesParent);
            string imageName = files[i].CreationTime.Month + "-" + files[i].CreationTime.Day + "-" + files[i].CreationTime.Year;
            _image.GetComponentInChildren<Text>().text = imageName;
            _image.name = imageName;
            _image.sprite = LoadSprite(files[i].FullName);
            ScreenshotButton button = _image.GetComponent<ScreenshotButton>();
            button.screenshotsMenu = this;
            button.uiScreenManager = uiScreenManager;
            //_image.GetComponentInChildren<Text>().text = files[i].CreationTime.Month + "-" + files[i].CreationTime.Day + "-" + files[i].CreationTime.Year;
            //_image.name = files[i].ToString();
        }

        loadedScreenshots = true;
    }
}