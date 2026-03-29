using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player: MonoBehaviour
{
    [Header("Main Stuff")]
    [SerializeField] float speed;
    [SerializeField] float accelerationTimeGrounded;
    [SerializeField] float accelerationTimeInAir;
    [SerializeField] float jumpHeight;
    [SerializeField] float coyoteTime;
    public int health;
    [SerializeField] bool invincible;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] Vector2 groundCheckSize = new Vector2(0, 0.125f);
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] Transform levelStart;
    [SerializeField] Transform armTwo;
    public Transform aimPoint;

    [Header("Effects")]
    [SerializeField] GameObject destroyedEffect;
    [SerializeField] GameObject hurtEffect;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip destroyedSound;
    [SerializeField] ParticleSystem doubleJumpParticles;
    [SerializeField] ParticleSystem walkParticles;
    [SerializeField] ParticleSystem landParticles;
    public List<SpriteRenderer> spriteMaterials = new List<SpriteRenderer>();
    [Header("Camera Shake")]
    public CameraManager.Properties properties;

    public event Action OnPlayerHurt;
    public event Action OnPlayerKilled;
    public event Action OnPlayerRespawn;
    public event Action OnPlayerHealthChange;
    public event Action OnPlayerTeleported;
    public bool canFollow { get; set; }
    public bool grounded { get; private set; }
    public bool dead { get; private set; }
    public Transform spawnPoint { get; set; }
    public int _health { get; private set; }
    public bool finishedLevel { get; set; }
    public bool handleInput { get; set; }
    public bool canFall { get; set; }
    public bool respawned { get; set; }
    public float fallButtonTimer { get; set; }
    public int lives { get; set; }
    public bool gameHUDPaused { get; set; }
    public bool gameHUDFrozen { get; set; }

    bool doubleJump;
    Rigidbody2D rb2D;
    Animator animator;
    Collider2D _collider2D;
    float velocityXSmoothing;
    GameObject parts;
    ShadowDanTracer shadowDanTracer;
    float finishLevelJumpTimer;
    Camera _camera;
    float _coyoteTime;
    float doubleJumpTimer;
    bool canJump;
    float fallButtonDelay = 0.1f;
    bool isGrounded;
    float _InputDelayTimer;
    bool teleporting = false;
    Vector2 input;
    Vector3 lastAimDirection;
    float hurtTimer = 0.125f;
    float _hurtTimer;
    int direction = 1;
    bool wasGroundedLastFrame;
    float previousVelocityY;

    void Awake()
    {
        lives = 3;

        if(levelStart != null)
            spawnPoint = levelStart;
	}

    void Start()
    {
        _health = health;
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        canFollow = true;
        parts = transform.Find("Legs").gameObject;
        _camera = Camera.main;
        _coyoteTime = coyoteTime;       
        doubleJumpTimer = 0.15f;

        if(spawnPoint != null)
            transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y - 0.125f, 0);

        shadowDanTracer = GetComponent<ShadowDanTracer>();
        shadowDanTracer.enabled = false;
    }

    void Update()
    {
        _InputDelayTimer -= Time.deltaTime;
        _InputDelayTimer = Mathf.Clamp(_InputDelayTimer, 0, 5);

        if(_hurtTimer > 0)
            _hurtTimer -= Time.deltaTime;    

        if(grounded)
        {
            _coyoteTime = coyoteTime;
            doubleJump = false;
        }
        else
        {
            _coyoteTime -= Time.deltaTime;
            _coyoteTime = Mathf.Clamp(_coyoteTime, 0, coyoteTime);
        }
        
        if(!dead && !gameHUDPaused && !gameHUDFrozen && !finishedLevel && _InputDelayTimer <= 0)
        {
            handleInput = true;
            HandleInput();

            float velocityXAbs = Mathf.Abs(rb2D.velocity.x);
            bool isWalkingOnGround = grounded && velocityXAbs > 0.5f;

            ParticleSystem.EmissionModule emission = walkParticles.emission;
            float emmisionRate = isWalkingOnGround ? Mathf.Lerp(10, 30, velocityXAbs / 8) : 0;

            emission.rateOverTime = emmisionRate;

            if(velocityXAbs > 0.3f)
                animator.SetFloat("Speed", velocityXAbs);
            else
                animator.SetFloat("Speed", 0);
                
            animator.SetBool("Grounded", grounded);
        }
        else
        {
            handleInput = false;
        }

        if(finishedLevel)
        {
            invincible = true;
            canFollow = false;

            animator.SetBool("Grounded", grounded);
            animator.SetFloat("Speed", 0);

            rb2D.velocity = new Vector2(Mathf.SmoothDamp(rb2D.velocity.x, 0, ref velocityXSmoothing, (grounded) ? accelerationTimeGrounded : accelerationTimeInAir), rb2D.velocity.y);

            if(grounded)
            {
                finishLevelJumpTimer -= Time.deltaTime;

                if(finishLevelJumpTimer <= -0)
                    rb2D.velocity = new Vector2(rb2D.velocity.x, jumpHeight);
            }
            else
            {
                finishLevelJumpTimer = 0.5f;
            }
        }
    }
    void FixedUpdate()
    {
        if(rb2D.velocity.y <= -25)
            rb2D.velocity = new Vector2(rb2D.velocity.x, -25);

        float targetVelocityX = input.x * speed;
        float smoothedX = Mathf.SmoothDamp(rb2D.velocity.x, targetVelocityX, ref velocityXSmoothing, (grounded) ? accelerationTimeGrounded : accelerationTimeInAir);

        if(float.IsNaN(velocityXSmoothing))
            velocityXSmoothing = 0;

        if(float.IsNaN(smoothedX))
            smoothedX = 0;

        if(!teleporting)
            rb2D.velocity = new Vector2(smoothedX, rb2D.velocity.y);

        Collider2D hit = Physics2D.OverlapBox(groundCheck.position + groundCheckOffset, groundCheckSize, 0, collisionMask);
        float ySpeed = Mathf.Abs(rb2D.velocityY);
        SetGrounded(hit);

        if(ySpeed >= 0.01 && hit != null && hit.CompareTag("One Way Platform"))
        {
            grounded = false;
            canJump = false;
        }
        else if(ySpeed < 0.01 && hit != null && hit.CompareTag("One Way Platform"))
        {
            grounded = true;
            canJump = true;
        }
        else
        {
            canJump = true;
            grounded = hit;
        }

        if (isGrounded && !wasGroundedLastFrame)
        {
            float impactSpeed = Mathf.Abs(previousVelocityY);
            
            if (impactSpeed > 1.2f)
            {
                float t = Mathf.Clamp01((impactSpeed - 1f) / 24f);

                int burstAmount = Mathf.RoundToInt(Mathf.Lerp(1, 15, t));

                landParticles.Emit(burstAmount);

                var shape = landParticles.shape;
                shape.radius = Mathf.Lerp(0.5f, 1, t);
            }
        }

        previousVelocityY = rb2D.velocity.y;
        wasGroundedLastFrame = isGrounded;
    }

    ///IEnumerator Flip(float from, float to)
   /// {
      //  canFlip = false;
       // float duration = 0.125f;
       // float t = 0;

        //while(t < duration)
        //{
            
           // t += Time.deltaTime;
            //transform.localScale = new Vector2(Mathf.Lerp(from, to, t / duration), 1);
           // yield return null;
       // }
       // canFlip = true;

    //}

    public void HandleInput()
    {
        //UVA 434-924-4357
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(!Input.GetButton("Strafe"))
        {
            if(input.x > 0)
                direction = 1;
            else if(input.x < 0)
                direction = -1;

            transform.localScale = new Vector3(direction, 1, 1);
        }

        RotateArm(true);
        Jump();
    }

    void Jump()
    {
        if(fallButtonTimer > 0)
            fallButtonTimer -= Time.deltaTime;

        if(grounded && fallButtonTimer <= 0 && Input.GetAxisRaw("Vertical") < 0)
            fallButtonTimer = 0;
        else if(!grounded)
            fallButtonTimer = fallButtonDelay;

        if(rb2D.velocityY < 0)
            fallButtonTimer = fallButtonDelay;

        if(!canJump)
            return;

        if(Input.GetButtonDown("Jump") && _coyoteTime > 0)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpHeight);
            doubleJumpTimer = 0.15f;
        }

        doubleJumpTimer -= Time.deltaTime;
        doubleJumpTimer = Mathf.Clamp(doubleJumpTimer, 0, 0.15f);

        if(Input.GetButtonUp("Jump") && !grounded)
            _coyoteTime = 0;

        if(Input.GetButtonDown("Jump") && !doubleJump && !grounded && _coyoteTime <= 0 && doubleJumpTimer <= 0)
        {
            doubleJumpParticles.Play();
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpHeight);
            doubleJump = true;
        }
    }

    void RotateArm(bool slerp)
    {
        Vector3 realMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        float mouseDistance = Vector2.Distance(transform.position, realMousePosition);
        Vector3 difference;

        if(mouseDistance > 2)
        {
            difference = realMousePosition - aimPoint.position;
            difference.Normalize();
            
            lastAimDirection = difference;
        }
        else
        {
            difference = lastAimDirection;
        }

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if(slerp)
            armTwo.rotation = Quaternion.Slerp(armTwo.rotation, Quaternion.Euler(0, 0, rotZ + 90), Time.deltaTime * 10);    
        else
            armTwo.rotation = Quaternion.Euler(0, 0, rotZ + 90);    
    }

    public void ResetDownTimer()
    {
        fallButtonTimer = fallButtonDelay;
    }
    public void SetGrounded(bool _grounded)
    {
        if(!isGrounded && _grounded)
            fallButtonTimer = fallButtonDelay;

        isGrounded = _grounded;
    }

    public void GiveHealth(int healthToGive)
    {
        _health = _health + healthToGive;

        if(_health >= health)
            _health = health;

        if(OnPlayerHealthChange != null)
            OnPlayerHealthChange();
    }

    public void TakeDamage(int damage, float inputDelay, bool giveKnockBack, int xKnockBack, int yKnockBack, float xOffset, Transform otherTransform, bool rotationBasedKnockBack, int knockBack, float rotationOffset)
    {
        if(invincible || _hurtTimer > 0)
            return;
        
        if(giveKnockBack)
        {
            _InputDelayTimer = inputDelay;
            rb2D.velocity = Vector2.zero;

            if(rotationBasedKnockBack)
            {
                Quaternion _rotationOffset = Quaternion.Euler(0, 0, rotationOffset);

                Vector2 knockBackDirection = _rotationOffset * otherTransform.up;
                rb2D.AddForce(knockBackDirection * knockBack, ForceMode2D.Impulse);

                doubleJump = true;
            }
            else
            {
                float xDistance = Mathf.Abs(otherTransform.position.x - transform.position.x);

                if(xDistance <= xOffset)
                {
                    rb2D.AddForce(new Vector2(0, yKnockBack * 1.5f), ForceMode2D.Impulse);
                    doubleJump = true;
                }
                else
                {
                    doubleJump = true;

                    if(otherTransform.position.x > transform.position.x)
                        rb2D.AddForce(new Vector2(-xKnockBack, yKnockBack), ForceMode2D.Impulse);
                    else if(otherTransform.position.x < transform.position.x)
                        rb2D.AddForce(new Vector2(xKnockBack, yKnockBack), ForceMode2D.Impulse);
                }
            }
        }

        Instantiate(hurtEffect, transform.position, Quaternion.identity);

        _health -= damage;

        OnPlayerHurt?.Invoke();

        if(properties.strength > 0)
            CameraManager.Instance.Shake(properties);

        AudioManager.Instance.PlaySound2D(hurtSound);
        StartCoroutine(Flash());

        if(_health <= 0 && !dead)
            Kill();

        _hurtTimer = hurtTimer;
    }

    public void Kill()
    {
        dead = true;        
        _collider2D.enabled = false;
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.velocity = Vector2.zero;
        canFollow = false;
        animator.enabled = false;
        _health = 0;
        AudioManager.Instance.PlaySound2D(destroyedSound);
        Instantiate(destroyedEffect, transform.position, Quaternion.identity);
        lives -= 1;

        walkParticles.Stop();

        OnPlayerKilled?.Invoke();

        GameManager.Instance.totalDeaths += 1;
        
        StartCoroutine(KillCo());
    }

    IEnumerator KillCo()
    {
        foreach(SpriteRenderer spriteMaterial in spriteMaterials)
            spriteMaterial.material.SetFloat("_FlashAmount", 1);

        float inTime = 0;
        float duration = 1.25f;
        float alpha = 1;

        while(inTime < duration)
        {
            rb2D.velocity = Vector2.zero;
            inTime += Time.unscaledDeltaTime;
            
            float t = inTime / duration;
            float smoothT = Mathf.Pow(inTime / duration, 3);

            Vector3 from = new Vector3(direction, 1, 1);
            Vector3 to = new Vector3(3 * direction, 3, 1);

            transform.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -125) * direction, smoothT);
            transform.localScale = Vector2.Lerp(from, to, smoothT);
            alpha = Mathf.Lerp(1, 0, smoothT);

            foreach(SpriteRenderer spriteMaterial in spriteMaterials)
                spriteMaterial.material.SetColor("_Color", new Color(spriteMaterial.material.color.r, spriteMaterial.material.color.g, spriteMaterial.material.color.b, alpha));

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1);

        if(lives > 0)
            Respawn();
    }

    public void Respawn()
    {
        direction = 1;
        transform.localScale = new Vector2(1, 1);
        transform.localEulerAngles = Vector3.zero;

        foreach(SpriteRenderer spriteMaterial in spriteMaterials)
        {
            spriteMaterial.material.SetFloat("_FlashAmount", 0);
            spriteMaterial.material.SetColor("_Color",new Color(spriteMaterial.material.color.r, spriteMaterial.material.color.g, spriteMaterial.material.color.b, 1));
        }

        RotateArm(false);
        
        dead = false;
        animator.enabled = true;
        _InputDelayTimer = 0;
        respawned = true;
        _collider2D.enabled = true;
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.velocity = Vector2.zero;
        canFollow = true;
        parts.SetActive(true);
        walkParticles.Play();

        _health = health;

        transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y - 0.125f, 0);

        OnPlayerRespawn?.Invoke();
    }

    public void Teleporting()
    {
        teleporting = true;
        invincible = true;
    }

    public void Teleported(Vector3 transportPoint)
    {
        transform.position = transportPoint;

        OnPlayerTeleported?.Invoke();

        invincible = false;
        teleporting = false;
    }
    IEnumerator Flash()
    {
        for(int i = 0; i < 4; i++)
        {
            foreach(SpriteRenderer spriteMaterial in spriteMaterials)
                spriteMaterial.material.SetFloat("_FlashAmount", 1);

            yield return new WaitForSeconds(0.07f);

            foreach(SpriteRenderer spriteMaterial in spriteMaterials)
                if(!dead)
                    spriteMaterial.material.SetFloat("_FlashAmount", 0);

            yield return new WaitForSeconds(0.07f);
        }
    }
    public void EnableShadowDanTracer()
    {
        if(!shadowDanTracer.enabled)
            shadowDanTracer.enabled = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(groundCheck.position + groundCheckOffset, groundCheckSize);
    }
}