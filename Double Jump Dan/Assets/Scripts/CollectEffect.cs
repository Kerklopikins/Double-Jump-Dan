using UnityEngine;
using System.Collections;

public class CollectEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] animationSprites;
    [SerializeField] float animationSpeed;
    [SerializeField] bool detatchFromParent;

    void OnEnable()
    {
        if(detatchFromParent)
            transform.parent = null;
            
        StartCoroutine(AnimateCollectEffect());
    }

    IEnumerator AnimateCollectEffect()
    {
        for(int i = 0; i < animationSprites.Length; i++)
        {
            spriteRenderer.sprite = animationSprites[i];
            yield return new WaitForSeconds(animationSpeed);
        }

        Destroy(gameObject);
    }
}