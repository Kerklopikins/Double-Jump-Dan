
using UnityEngine;

public class gesss : MonoBehaviour
{
    public float moveSpeedX;
    public float moveSpeedY;
    public float moveTimer = 0.5f;
    float inTime = 0;
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    Vector3 startPosition;
    bool startted;
    Transform endPoint;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        endPoint = GameObject.FindWithTag("Stats HUD").GetComponent<StatsHUD>().gemIcon;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        inTime += Time.deltaTime;
        // gemShine.enabled = false;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(1, 0, inTime / 1));
        //transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, inTime / 0.6f);
        transform.position = Vector3.Lerp(startPosition, endPoint.position, inTime / 1);
        inTime += Time.deltaTime;
        transform.localEulerAngles += Vector3.forward * 1000 * Time.deltaTime;
    
        if(inTime > 1.35f)
        {
            Destroy(gameObject);

        }
    }  
}