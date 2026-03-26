using System;
using UnityEngine;

public class CrorgnAI : MonoBehaviour
{
    [Header("Main Stuff")]
    [SerializeField] float speed;
    [SerializeField] SpriteRenderer eye;
    [SerializeField] Sprite[] eyeSprites;
    [SerializeField] Transform bodyPivot;
    [SerializeField] Transform wallCheck;
    [SerializeField] Transform edgeCheck;
    [SerializeField] bool useRigidBody;

    [Header("Player Detection")]
    [SerializeField] float playerRange = 10;
    
    public Rigidbody2D rb2D { get; protected set; }
    public Player player { get; protected set; }
    int moveDirection;
    bool notAtEdge;
    bool hittingWall;
    int layerMask;
    EnemyGun enemyGun;
    float wallHitDistance;
    float _speed;
    Animator animator;
    Health health;
    Collider2D _collider;
    void Start()
    {
        moveDirection = -1;
        rb2D = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        enemyGun = GetComponent<EnemyGun>();
        animator = GetComponent<Animator>();
        layerMask = (1 << LayerMask.NameToLayer("Collisions")) | (1 << LayerMask.NameToLayer("Player"));
        health = GetComponent<Health>();
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if(health.Dead())
        {
            animator.enabled = false;
            _collider.enabled = false;
            enemyGun.enabled = false;
            
            if(rb2D != null)
                rb2D.simulated = false;

            this.enabled = false;
            return;
        }

        bool playerInRange = false;

        for(int i = 0; i < 13; i++)
        {
            float angle = i * 15;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.right;
            RaycastHit2D fieldOfViewRays = Physics2D.Raycast(transform.position, direction, playerRange, layerMask);
            
            if(fieldOfViewRays && fieldOfViewRays.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                playerInRange = true;
            //if(fieldOfViewRays && fieldOfViewRays.transform.gameObject.layer == LayerMask.NameToLayer("Collisions"))
            //{
                //Debug.DrawRay(transform.position, direction * playerRange, Color.blue);
            //}
           
                //Debug.DrawRay(transform.position, direction * playerRange, Color.green);
            //else
            //{
                //Debug.DrawRay(transform.position, direction * playerRange, Color.red);
            //}
        }

        hittingWall = Physics2D.Raycast(wallCheck.position, Vector3.left * transform.localScale.x, wallHitDistance, 1 << LayerMask.NameToLayer("Collisions"));
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, 0.1f, 1 << LayerMask.NameToLayer("Collisions"));

        animator.SetBool("Player In Range", playerInRange);
        
        if(playerInRange)
        {
            eye.sprite = eyeSprites[0];

            Vector3 shootDirection = new Vector3();

            if(transform.lossyScale.x < 0)
                shootDirection = enemyGun.bulletFireLocations[0].right;
            else if(transform.lossyScale.x > 0)
                shootDirection = -enemyGun.bulletFireLocations[0].right;

            RaycastHit2D hit;
            Ray2D ray = new Ray2D(enemyGun.bulletFireLocations[0].position, shootDirection);

            hit = Physics2D.Raycast(ray.origin, ray.direction, playerRange, layerMask);

            Vector3 difference = player.transform.position - transform.position;
            difference.Normalize();

            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            float rotZOffset = rotZ;

            if(transform.lossyScale.x < 0)
            {
                if(player.transform.position.x > transform.position.x)
                    RotateBody(rotZOffset, 0, 45, 0);
                else
                    RotateBody(0, 0, 0, 0);
            }
            else if(transform.lossyScale.x > 0)
            {
                if(player.transform.position.x < transform.position.x)
                    RotateBody(rotZOffset, 135, 180, 180);
                else
                    RotateBody(0, 0, 0, 0);
            }

            if(hit && hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                enemyGun.Shoot();

            wallHitDistance = 4;

            if(!notAtEdge)
                _speed = 0;
            else
                _speed = hittingWall ? _speed = 0 : _speed = speed;

            float xDistance = Mathf.Abs(player.transform.position.x - transform.position.x);

            if(xDistance > 1 && PlayerBehind())
                moveDirection = -moveDirection;

            Move();
        }
        else
        {
            wallHitDistance = 0.25f;
            _speed = speed;
            Move();

            if(hittingWall || !notAtEdge)
                moveDirection = -moveDirection;

            eye.sprite = eyeSprites[1];
            RotateBody(0, 0, 0, 0);
        }

        transform.localScale = new Vector3(-moveDirection, transform.localScale.y, transform.localScale.z);
    }

    void Move()
    {
        if(useRigidBody)
            rb2D.velocity = new Vector2(moveDirection * _speed, rb2D.velocity.y);
        else
            transform.Translate(moveDirection * _speed * Time.deltaTime, 0, 0);

        animator.SetInteger("Speed", (int)_speed);
    }

    bool PlayerBehind()
    {
        if(player.transform.position.x > transform.position.x && moveDirection == -1 || player.transform.position.x < transform.position.x && moveDirection == 1)
            return true;
        else 
            return false;
    }

    void RotateBody(float rotZOffset, float minClamp, float maxClamp, float rotationOffset)
    {
        rotZOffset = Math.Clamp(rotZOffset, minClamp, maxClamp);
        bodyPivot.rotation = Quaternion.Slerp(bodyPivot.rotation, Quaternion.Euler(0, 0, rotZOffset + rotationOffset), Time.deltaTime * 10);
        //bodyPivot.rotation = Quaternion.Euler(0, 0, );
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, playerRange);
    }
}