
using UnityEngine;
using System.Collections;
public class FloatingNumbers : MonoBehaviour
{
    public float floatHeight;
    public float scaleDuration;
    public float floatDuration;
    public float hangDuration;

    public Transform numbersHolder;
    public SpriteRenderer[] numberSprites;
    public Sprite[] numbers;
    public float[] spacing;
    Vector2 startPosition;

    void Start()
    {
        startPosition = numbersHolder.localPosition;
        transform.localScale = Vector2.zero;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
            SetNumber(Random.Range(0, 9999), transform.position);
    }

    IEnumerator AnimateNumbers()
    {
        float inTime = 0;

        while(inTime < scaleDuration)
        {
            inTime += Time.deltaTime;
            float t = inTime / scaleDuration;
            float smoothT = 1 - Mathf.Pow(1 - t, 4);

            transform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, smoothT);

            yield return null;
        }

        transform.localScale = Vector2.one;

        float middleTime = 0;

        while(middleTime < floatDuration)
        {
            middleTime += Time.deltaTime;
            float t = middleTime / floatDuration;
            float smoothT = 1 - Mathf.Pow(1 - t, 4);

            numbersHolder.transform.localPosition = Vector2.Lerp(startPosition, new Vector2(startPosition.x, startPosition.y + floatHeight), smoothT);

            yield return null;
        }

        numbersHolder.transform.localPosition = new Vector2(startPosition.x, startPosition.y + floatHeight);
        yield return new WaitForSeconds(hangDuration); 
        
        float endTime = 0;

        while(endTime < scaleDuration)
        {
            endTime += Time.deltaTime;
            float t = endTime / scaleDuration;
            float smoothT = 1 - Mathf.Pow(1 - t, 4);

            transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, smoothT);

            yield return null;
        }

        transform.localScale = Vector2.zero;
    }

    void SetNumber(int number, Vector2 position)
    {
        transform.position = position;

        StartCoroutine(AnimateNumbers());

        int value = Mathf.Abs(number);
        
        for(int i = 0; i < numberSprites.Length; i++)
            numberSprites[i].gameObject.SetActive(false);

        if(value == 0)
        {
            numberSprites[0].gameObject.SetActive(true);
            numberSprites[0].sprite = numbers[0];
            return;
        }

        int temp = value;
        int divisor = 1;

        while(temp / divisor >= 10)
            divisor *= 10;

        int index = 0;

        while(divisor > 0 && index < numberSprites.Length)
        {
            int digit = temp / divisor;
            
            numberSprites[index].gameObject.SetActive(true);
            numberSprites[index].sprite = numbers[digit];
            numbersHolder.localPosition = new Vector3(startPosition.x + spacing[index], numbersHolder.localPosition.y, 0);

            temp %= divisor;
            divisor /= 10;
            index++;
        }
    }
}