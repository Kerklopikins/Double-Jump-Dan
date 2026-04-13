using UnityEngine;
using System.Collections;
using System;

public class Snail : MonoBehaviour
{
    public float speed;
    public Transform wallCheck;
    public Transform edgeCheck;
    public Sprite[] snailSprites;
    public float animationSpeed;
public LayerMask collisionMask;
        bool notAtEdge;
    bool hittingWall;
    int direction;
    Health health;
    SpriteRenderer spriteRenderer;
    int animationIndex = -1;
    bool hittingEnemy;
    void Start()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        direction = -1;

        StartCoroutine(Animation());
    }

    void Update()
    {
        if(health.Dead())
            return;

        hittingWall = Physics2D.OverlapCircle(wallCheck.position, 0.2f, collisionMask);
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, 0.2f, 1 << LayerMask.NameToLayer("Collisions"));
        transform.Translate(new Vector2(speed * Time.deltaTime * direction, 0), Space.Self);

        if(!notAtEdge || hittingWall || hittingEnemy)
            direction = -direction;

        transform.localScale = new Vector2(-direction, 1);
    }

    IEnumerator Animation()
    {
        while(true)
        {
            animationIndex++;

            if(animationIndex > snailSprites.Length - 1)
                animationIndex = 0;

            spriteRenderer.sprite = snailSprites[animationIndex];
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}