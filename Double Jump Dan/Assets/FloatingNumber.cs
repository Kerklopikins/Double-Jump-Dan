
using UnityEngine;
using System.Collections;

public class FloatingNumber : MonoBehaviour
{
    [SerializeField] float floatHeight;
    [SerializeField] float scaleDuration;
    [SerializeField] float floatDuration;
    [SerializeField] float hangDuration;

    [SerializeField] SpriteRenderer plusMinusSprite;
    [SerializeField] Sprite[] plusMinusSprites;
    [SerializeField] Transform numbersHolder;
    [SerializeField] SpriteRenderer[] numberSprites;
    [SerializeField] Sprite[] numbers;
    [SerializeField] float[] spacing;
    
    Vector2 numbersHolderStartPosition;

    void Start()
    {
        transform.localScale = Vector2.zero;
        SetNumber(1, transform.position, true);
    }

    IEnumerator ScaleNumbers(Vector3 from, Vector3 to)
    {
        float inTime = 0;
        transform.localScale = from;

        while(inTime < scaleDuration)
        {
            inTime += Time.deltaTime;
            float t = inTime / scaleDuration;
            float smoothT = 1 - Mathf.Pow(1 - t, 4);

            transform.localScale = Vector3.Lerp(from, to, smoothT);

            yield return null;
        }

        transform.localScale = to;
    }

    IEnumerator FloatNumbers(Vector2 startPosition)
    {
        StartCoroutine(ScaleNumbers(Vector3.zero, Vector3.one));
        
        float inTime = 0;

        while(inTime < floatDuration)
        {
            inTime += Time.deltaTime;
            float t = inTime / floatDuration;
            float smoothT = 1 - Mathf.Pow(1 - t, 4);

            transform.position = Vector2.Lerp(startPosition, new Vector2(startPosition.x, startPosition.y + floatHeight), smoothT);

            yield return null;
        }

        transform.position = new Vector2(startPosition.x, startPosition.y + floatHeight);
        yield return new WaitForSeconds(hangDuration); 
        
        StartCoroutine(ScaleNumbers(Vector3.one, Vector3.zero));
    }

    void SetNumber(int number, Vector2 position, bool plusOrMinus)
    {
        transform.position = position;
        plusMinusSprite.sprite = plusOrMinus == true ? plusMinusSprites[0] : plusMinusSprites[1];
        StartCoroutine(FloatNumbers(position));

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
            numbersHolder.localPosition = new Vector3(numbersHolderStartPosition.x + spacing[index], numbersHolder.localPosition.y, 0);

            temp %= divisor;
            divisor /= 10;
            index++;
        }
    }
}