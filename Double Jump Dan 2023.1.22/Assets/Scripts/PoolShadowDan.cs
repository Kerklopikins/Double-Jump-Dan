using System.Collections.Generic;
using UnityEngine;
public class PoolShadowDan : MonoBehaviour, IPoolable
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform playerArmOne;
	[SerializeField] Transform playerArmTwo;
    [SerializeField] List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    
    float inTime;
    float duration = 0.45f;
    void Start()
    {
        WorldManager worldManager = WorldManager.Instance;

        foreach(var sprite in sprites)
            sprite.color = new Color(worldManager.mainMaterial.color.r, worldManager.mainMaterial.color.g, worldManager.mainMaterial.color.b, sprite.color.a);
    }
    public void OnObjectReuse(object data)
    {
        inTime = 0;
        ShadowDanProperties properties = (ShadowDanProperties)data;
        spriteRenderer.sprite = properties.sprite;
        transform.position = properties.position;
        transform.localScale = properties.scale;
        playerArmOne.rotation = properties.armOneRotation;
        playerArmTwo.rotation = properties.armTwoRotation;
    }

    void Update()
    {
        if(inTime < duration)
        {
            inTime += Time.deltaTime;

            foreach(var sprite in sprites)
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(1, 0, inTime / duration));
        }
        else
            gameObject.SetActive(false);
    }
}

public struct ShadowDanProperties
{
    public Sprite sprite;
    public Vector2 position;
    public Quaternion armOneRotation;
    public Quaternion armTwoRotation;
    public Vector2 scale;
}