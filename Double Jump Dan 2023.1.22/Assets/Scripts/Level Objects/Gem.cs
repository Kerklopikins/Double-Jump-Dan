using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] int gemsToGivePlayer;
    [SerializeField] GameObject effect;
    [SerializeField] AudioClip collectSound;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer gemShine;

    float magnetDuration = 0.75f;
    SpriteRenderer centralizedGem;
    bool collected;
    float inTime = 0;
    Vector3 startPosition;

	void Start()
	{
        centralizedGem = GameManager.Instance.centralizedGem;
        spriteRenderer.sprite = GameManager.Instance.gemSprites[Random.Range(0, GameManager.Instance.gemSprites.Length)];
        startPosition = transform.position;
    }

	void Update()
	{
        gemShine.sprite = centralizedGem.sprite;

        if(collected)
        {
            gemShine.enabled = false;
            
            if(inTime < magnetDuration)
            {
                inTime += Time.deltaTime;
                float t = Mathf.Clamp01(inTime / magnetDuration);

                float easedT = t * t;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(1, 0, easedT));
                transform.localEulerAngles += Vector3.forward * 1000 * Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, StatsHUD.Instance.gemIcon.position, easedT);
            }
            else
            {
                LevelManager.Instance.AddGems(gemsToGivePlayer);
                Destroy(gameObject);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && !collected)
        {   
            AudioManager.Instance.PlaySound2D(collectSound, 0.985f, 1);
            Instantiate(effect, transform.position, Quaternion.identity);
            collected = true;
        }
    }
}