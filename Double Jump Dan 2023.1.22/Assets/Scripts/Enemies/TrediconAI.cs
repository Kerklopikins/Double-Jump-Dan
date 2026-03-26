using UnityEngine;

public class TrediconAI : MonoBehaviour
{
    [Header("Main Stuff")]
    [SerializeField] float speed;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] Transform wallCheck;
    [SerializeField] Transform edgeCheck;
    [SerializeField] float playerRange = 10;
    [SerializeField] float secondsToPauseWhenSeesPlayer;
    [SerializeField] float angrySpeed;
    [SerializeField] TrediconRocketLauncher rocketLauncher;
    [SerializeField] AudioClip transformSound;
    [SerializeField] WatchOutTrigger watchOutTrigger;
    [SerializeField] Collider2D[] colliders;
    float _secondsToPauseWhenSeesPlayer;
    bool seesPlayer;
    Rigidbody2D rb2D;
    Player player;
    bool playerInRange;
    float interval = 1;
    float timer;
    Vector2 direction;
    bool notAtEdge;
    Animator animator;
    bool hittingWall;
    float _angrySpeed;
    bool sawPlayer;
    float playerDistance;
    Health health;
    void Start()
    {
        direction = new Vector2(-transform.localScale.x, 0);
        _secondsToPauseWhenSeesPlayer = secondsToPauseWhenSeesPlayer;
        rb2D = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        timer = interval;
        _angrySpeed = angrySpeed;
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        rocketLauncher.player = player;
    }

    void Update()
    {
        if(health.Dead())
        {
            animator.enabled = false;
            watchOutTrigger.gameObject.SetActive(false);
            rocketLauncher.laser.gameObject.SetActive(false);

            for(int i = 0; i < colliders.Length; i++)
                colliders[i].enabled = false;

            rocketLauncher.enabled = false;
            
            if(rb2D != null)
                rb2D.simulated = false;

            this.enabled = false;
            return;
        }

        playerDistance = Vector3.Distance(player.transform.position, transform.position);

        timer -= Time.deltaTime;

        if(playerRange > 0)
        {
            if(!player.dead)
            {
                if(playerDistance <= playerRange)
                    playerInRange = true;
                else
                    playerInRange = false;

                if(playerInRange)
                    seesPlayer = true;
                else
                    seesPlayer = false;
            }
            else
            {
                playerInRange = false;
                seesPlayer = false;
            }
        }

        hittingWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, collisionMask);

        if(hittingWall && direction.x < 0)
            hittingWall = true;
        else if(!hittingWall && direction.x > 0)
            hittingWall = false;

        if(edgeCheck != null)
        {
            notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, 0.1f, collisionMask);
            
            if(seesPlayer)
                if(!notAtEdge)
                    _angrySpeed = 0;

            if(notAtEdge)
                _angrySpeed = angrySpeed;

            if((direction.x < 0 && hittingWall) || (direction.x > 0 && hittingWall || !notAtEdge))
            {
                direction = -direction;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if((direction.x < 0 && hittingWall) || (direction.x > 0 && hittingWall))
            {
                direction = -direction;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        if(!seesPlayer)
            rb2D.velocity = new Vector2(direction.x * speed, rb2D.velocity.y);

        if(timer <= 0)
            timer = interval;

        if(playerInRange)
            _secondsToPauseWhenSeesPlayer -= Time.deltaTime;
        else
            _secondsToPauseWhenSeesPlayer = secondsToPauseWhenSeesPlayer;

        if(seesPlayer && _secondsToPauseWhenSeesPlayer > 0)
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);

            if(!sawPlayer)
            {
                if(player.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                    direction = new Vector2(1, 0);
                }
                else if(player.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                    direction = new Vector2(-1, 0);
                }
                  
                sawPlayer = true;
				watchOutTrigger.Activate();
            }
        }

        if(seesPlayer && _secondsToPauseWhenSeesPlayer <= 0)
        {
            rocketLauncher.shooting = true;

            float xDistance = Mathf.Abs(player.transform.position.x - transform.position.x);

            if(xDistance > 1)
            {
                if(player.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                    direction = new Vector2(1, 0);
                }
                else if(player.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                    direction = new Vector2(-1, 0);
                }

                if(hittingWall)
                    _angrySpeed = 0;

                rb2D.velocity = new Vector2(direction.x * _angrySpeed, rb2D.velocity.y);
            }
            else
            {
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            }
        }
        else
        {
            rocketLauncher.shooting = false;
        }

        if(rb2D.velocity.x == 0)
            animator.SetBool("Stopped", true);
        else
            animator.SetBool("Stopped", false);

        animator.SetBool("Player In Range", seesPlayer);

        if(!seesPlayer)
            sawPlayer = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, playerRange);
    }
}