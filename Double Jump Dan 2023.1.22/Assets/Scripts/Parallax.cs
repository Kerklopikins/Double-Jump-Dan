using UnityEngine;

public class Parallax : MonoBehaviour 
{
    [SerializeField] float amplification;
    [SerializeField] bool onlyAffectXAxis;
    [SerializeField] bool resizeSprite;

    Camera _camera;
    Transform cameraHolder;
    SpriteRenderer spriteRenderer;
    Vector2 startingSize;
    int startingXOffset = -6;

    void Start()
    {
        _camera = Camera.main;
        cameraHolder = _camera.transform.parent;
        
        OffsetBackground();

        if(resizeSprite)

        if(resizeSprite)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            startingSize = GetComponent<SpriteRenderer>().size;        
        }
    }

    public void OffsetBackground()
    {
        if(!onlyAffectXAxis)
        {
            transform.localPosition = new Vector3(cameraHolder.transform.localPosition.x * amplification, cameraHolder.transform.localPosition.y * amplification, 0);
            transform.parent.position = new Vector3(-transform.localPosition.x + startingXOffset, -transform.position.y, 0);
        }
        else
        {
            transform.localPosition = new Vector3(cameraHolder.transform.localPosition.x * amplification, transform.localPosition.y, 0);
            transform.parent.position = new Vector3(-transform.localPosition.x + startingXOffset, transform.parent.position.y, 0);
        }
    }

	void LateUpdate() 
	{
        if(!onlyAffectXAxis)
            transform.localPosition = new Vector3(cameraHolder.transform.localPosition.x * amplification, cameraHolder.transform.localPosition.y * amplification, 0);
        else
            transform.localPosition = new Vector3(cameraHolder.transform.localPosition.x * amplification, transform.localPosition.y, 0);

        if(resizeSprite)
            if(cameraHolder.transform.position.x + _camera.orthographicSize * ((float)Screen.width / Screen.height) > transform.position.x + spriteRenderer.bounds.extents.x * 2)
                spriteRenderer.size = new Vector2(spriteRenderer.size.x + startingSize.x, spriteRenderer.size.y);
    }
}