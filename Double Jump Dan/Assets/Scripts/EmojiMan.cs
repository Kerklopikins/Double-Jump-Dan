using System.Collections;
using UnityEngine;

public class EmojiMan : MonoBehaviour
{
	[SerializeField] Sprite[] emojis;
	public float transformRate;

	SpriteRenderer spriteRenderer;
	float timer;
	
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		timer = transformRate;
	}

	void Update()
	{
		if(timer > 0)
		{
			timer -= Time.deltaTime;
		}
		else
		{
			StartCoroutine(ChangeEmoji());
			timer = transformRate;
		}
	}
	IEnumerator ChangeEmoji()
	{
		float inTime = 0;
        float duration = 0.25f;

        while(inTime < duration)
        {
            inTime += Time.deltaTime;
            
            float t = inTime / duration;
            float smoothT = t * t * (3 - 2 * t);

            transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, smoothT);
			transform.localEulerAngles = Vector3.Lerp(Vector2.zero, new Vector3(0, 0, -180), smoothT);
            yield return null;
        }

		spriteRenderer.sprite = emojis[Random.Range(0, emojis.Length)];

		float outTime = 0;

        while(outTime < duration)
        {
            outTime += Time.deltaTime;
            
            float t = outTime / duration;
            float smoothT = t * t * (3 - 2 * t);

            transform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, smoothT);
			transform.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, -180), new Vector3(0, 0, -360), smoothT);
            yield return null;
        }

		transform.localScale = Vector3.one;
		transform.localEulerAngles = Vector3.zero;
	}
}