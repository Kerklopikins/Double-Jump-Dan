using UnityEngine;
using UnityEngine.UI;

public class ScreenshotButton : MonoBehaviour
{
    public ScreenshotsMenu screenshotsMenu { get; set; }
    public UIScreenManager uiScreenManager { get; set; }

    Image image;

	void Start() 
	{
        image = GetComponent<Image>();	
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
	}
	
	public void ViewScreenshot() 
	{
        if(screenshotsMenu.screenshotViewerAnimator.transform.GetChild(0).transform.localScale.x > 0 && screenshotsMenu.screenshotViewerAnimator.gameObject.activeInHierarchy)
            return;

        string newName = image.name.Replace(".png", "");

        screenshotsMenu.screenshotViewerText.text = newName;
        screenshotsMenu.screenshotViewerImage.sprite = image.sprite;

        uiScreenManager.OpenPanel(screenshotsMenu.screenshotViewerAnimator);
	}
}