using UnityEngine;
using System.Collections;

public class ShockWaveAnimation : MonoBehaviour
{
    [SerializeField] float shockWaveTime;
    public PlayMode playMode;
    public enum PlayMode { PlayOnStart, PlayOnShoot }
    public GunInfo gunInfo;
    SpriteRenderer spriteRenderer;
    float shockWaveSpeed;
    bool canAnimate = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shockWaveSpeed = 1 / shockWaveTime;
        
        if(playMode == PlayMode.PlayOnShoot)
            gunInfo.OnShoot += AnimateShockWave;
        else if(playMode == PlayMode.PlayOnStart)
            AnimateShockWave();
    }

    public void AnimateShockWave()
    {
        if(canAnimate)
            StartCoroutine(AnimateShockWaveCo());
    }

    IEnumerator AnimateShockWaveCo()
    {
        canAnimate = false;

        float percent = 0;
        
        while(percent < 1)
        {
            percent += Time.deltaTime * shockWaveSpeed;

            float scale = Mathf.Lerp(0, 1, percent);
            float alpha = Mathf.Lerp(1, 0, percent);

            transform.localScale = Vector3.one * scale;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        canAnimate = true;
    }
}