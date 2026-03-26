using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] BoxCollider2D platformCollider;
    [SerializeField] float positionOffsetY;
    [SerializeField] float positionOffsetX;
    [SerializeField] float collisionDelay;
    [SerializeField] bool offsetAdjustmentMode;

    Player player;
    Collider2D playerCollider;
    bool falling;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerCollider = player.GetComponent<Collider2D>();
    }

    void Update()
    {
        float playerXPositionAbs = Mathf.Abs(transform.position.x - player.transform.position.x);

        if(playerXPositionAbs >= platformCollider.size.x * 0.5f + positionOffsetX && player.transform.position.y > transform.position.y + positionOffsetY)
        {
            Physics2D.IgnoreCollision(platformCollider, playerCollider, true);

            if(offsetAdjustmentMode)
                GetComponent<SpriteRenderer>().color = Color.red;

            return;
        }

        if(player.transform.position.y < transform.position.y + positionOffsetY)
        {
            Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
            
            if(offsetAdjustmentMode)
                GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {            
            if(player.fallButtonTimer <= 0 && Input.GetAxisRaw("Vertical") < 0 && !falling)
            {
                StartCoroutine(FallCo());
                return;
            }

            if(!falling)
                Physics2D.IgnoreCollision(platformCollider, playerCollider, false);

            if(offsetAdjustmentMode)
                GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    IEnumerator FallCo()
    {
        falling = true;
        Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
        yield return new WaitForSeconds(collisionDelay);
        Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
        falling = false;
    }

    void OnDrawGizmos()
    {
        if(!offsetAdjustmentMode)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + positionOffsetY, 0), 0.2f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + platformCollider.size.x * 0.5f + positionOffsetX, transform.position.y, 0), 0.2f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x - platformCollider.size.x * 0.5f - positionOffsetX, transform.position.y, 0), 0.2f);
    }
}