using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gemtest : MonoBehaviour
{
    [SerializeField] int gemsToGive;
    [SerializeField] GameObject Gem;

    [SerializeField] float spreadRadius;
    [SerializeField] float popHeight;
    [SerializeField] float duration;
    float magnetDuration = 0.75f;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            ExplodeGems(transform.position);
            StartCoroutine(FlingAnimation(gameObject));
        }
    }
    #region GiveGems
    void ExplodeGems(Vector3 originalPosition)
    {
        StartCoroutine(InstaintiateGems(originalPosition));
    }

    Vector3 GetRandomOffset()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(0.5f, spreadRadius);
        return new Vector3(randomCircle.x, randomCircle.y, 0);
    }
    
    IEnumerator InstaintiateGems(Vector3 originalPosition)
    {
        for(int i = 0; i < gemsToGive; i++)
        {
            GameObject gem = Instantiate(Gem, originalPosition, Quaternion.identity);
            SpriteRenderer gemSprite = gem.GetComponent<SpriteRenderer>();

            gemSprite.sprite = GameManager.Instance.gemSprites[Random.Range(0, GameManager.Instance.gemSprites.Length)];
            Vector3 targetOffset = GetRandomOffset();
            StartCoroutine(PopGem(gem, originalPosition, originalPosition + targetOffset, gemSprite));
            yield return new WaitForSeconds(0.0125f);
        }
    }

    IEnumerator PopGem(GameObject gem, Vector3 start, Vector3 end, SpriteRenderer gemSprite)
    {
        float elapsed = 0;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float easedT = EaseBackOut(t);
            float arcY = Mathf.Sin(Mathf.Pow(t, 0.8f) * Mathf.PI) * popHeight;

            Vector3 flatPos = Vector3.Lerp(start, end, easedT);
            flatPos.y += arcY;

            float tScale = elapsed / duration * 1.5f;
            gem.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, tScale);

            gem.transform.position = flatPos;
            yield return null;
        }

        gem.transform.position = end;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MagnetToPoint(gem, gemSprite));
    }

    IEnumerator MagnetToPoint(GameObject gem, SpriteRenderer gemSprite)
    {
        float elapsed = 0;
        Vector3 start = gem.transform.position;

        while(elapsed < magnetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / magnetDuration);

            float easedT = t * t;

            gemSprite.color = new Color(gemSprite.color.r, gemSprite.color.g, gemSprite.color.b, Mathf.Lerp(1, 0, easedT));
            gem.transform.localEulerAngles += Vector3.forward * 1000 * Time.deltaTime;
            gem.transform.position = Vector3.Lerp(start, StatsHUD.Instance.gemIcon.position, easedT);
            yield return null;
        }

        StatsHUD.Instance.AddGems(1);
        Destroy(gem);
    }

    float EaseBackOut(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1f + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t- 1, 2);
    }
    #endregion

    #region EnemyFling
    IEnumerator FlingAnimation(GameObject enemy)
    {
        float direction = Random.value > 0.5f ? 1 : -1;
        Vector3 startPosition = enemy.transform.position;
        float flingHeight = Random.Range(7, 10);
        //Vector3 flingEnd = startPosition + new Vector3(direction * Random.Range(3, 5), Random.Range(1, 3), 0);

        Vector3 flingEnd = startPosition + new Vector3(direction * Random.Range(8, 12), -10, 0);
        
        float flingDuration = 1.5f;
        float elapsed = 0;

        while(elapsed < flingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / flingDuration);

            float easedT = t * t;

             //float arcY = (Mathf.Sin(t * Mathf.PI * 0.5f) * flingHeight;)
            float arcY = (1 - Mathf.Pow(t * 2 - 1, 2)) * flingHeight;

            Vector3 position = Vector3.Lerp(startPosition, flingEnd, easedT);
            position.y = startPosition.y + arcY + Mathf.Lerp(0, flingEnd.y - startPosition.y, t * t);
            enemy.transform.position = position;

            enemy.transform.Rotate(0, 0, direction * 500 * Time.deltaTime);
            yield return null;
        }
        //Destroy(enemy);


      //  while(elapsed < flingDuration)
//{
          // / elapsed += Time.deltaTime;
           // float t = Mathf.Clamp01(elapsed / flingDuration);

           // float easedT = 1 - Mathf.Pow(1 - t, 3);

             //float arcY = (Mathf.Sin(t * Mathf.PI * 0.5f) * flingHeight;)
          //  float arcY = (1 - Mathf.Pow(t * 2 - 1, 2)) * flingHeight;

          //  Vector3 position = Vector3.Lerp(startPosition, flingEnd, easedT);
           // position.y = startPosition.y + arcY;
          //  enemy.transform.position = position;

        //    enemy.transform.Rotate(0, 0, direction * 500 * Time.deltaTime);
          //  yield return null;
      //  }

        //float fallDuration = 1.5f;
        //elapsed = 0;
       // Vector3 fallStart = enemy.transform.position;
       // Vector3 fallEnd = fallStart + new Vector3(direction * Random.Range(3, 5), -25, 0);

       // while(elapsed < fallDuration)
        //{
           // elapsed += Time.deltaTime;
          //  float t = Mathf.Clamp01(elapsed / fallDuration);

          //  float easedT = t * t * t;

          //  enemy.transform.position = Vector3.Lerp(fallStart, fallEnd, easedT);
          //  enemy.transform.Rotate(0, 0, direction * (500 + 600 * t) * Time.deltaTime);
          //  yield return null;
       // }

        //Destroy(enemy);
    }
    #endregion
}
