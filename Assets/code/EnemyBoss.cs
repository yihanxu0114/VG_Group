using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public enum BossState { Patrol, Chase, Attack }
    public BossState currentState = BossState.Patrol;

    [Header("Anim Setting")]
    public Sprite[] walkFrames;
    public Sprite[] runFrames;
    public Sprite[] attackFrames;
    public float frameRate = 0.15f;
    private int currentFrame;
    private float animTimer;
    private SpriteRenderer spriteRenderer;

    [Header("Moving & Vision Setting")]
    public float patrolSpeed = 2f;    
    public float chaseSpeed = 5.5f;  
    public float detectionRange = 8f;
    public float attackRange = 5f;   
    private bool movingLeft = true;

    [Header("Missile Setting")]
    public GameObject missilePrefab;  
    public Transform firePoint;       
    public float attackCooldown = 2f; 
    public int fireFrame = 3;         
    private float attackTimer;
    private bool hasFiredThisAnim;

    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        attackTimer -= Time.deltaTime;
        CheckState();    
        PlayAnimation(); 
    }

    void FixedUpdate()
    {
        if (currentState == BossState.Patrol)
        {
            Move(patrolSpeed);
        }
        else if (currentState == BossState.Chase)
        {
            Move(chaseSpeed);
        }
        else if (currentState == BossState.Attack)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void CheckState()
    {
        if (currentState == BossState.Attack) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        bool isPlayerInFront = (movingLeft && playerTransform.position.x < transform.position.x) ||
                               (!movingLeft && playerTransform.position.x > transform.position.x);

        if (isPlayerInFront && distance <= attackRange && attackTimer <= 0)
        {
            ChangeState(BossState.Attack);
        }
        else if (isPlayerInFront && distance <= detectionRange)
        {
            if (currentState != BossState.Chase) ChangeState(BossState.Chase);
        }
        else
        {
            if (currentState != BossState.Patrol) ChangeState(BossState.Patrol);
        }
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
        currentFrame = 0;
        animTimer = 0;
        hasFiredThisAnim = false;
    }

    void Move(float currentSpeed)
    {
        if (movingLeft)
        {
            rb.velocity = new Vector2(-currentSpeed, rb.velocity.y);
            spriteRenderer.flipX = true;
        }
        else
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            spriteRenderer.flipX = false;
        }
    }

    void PlayAnimation()
    {
        Sprite[] currentAnimArray = walkFrames;
        if (currentState == BossState.Chase) currentAnimArray = runFrames;
        else if (currentState == BossState.Attack) currentAnimArray = attackFrames;

        if (currentAnimArray == null || currentAnimArray.Length == 0) return;

        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer -= frameRate;
            currentFrame++;

            if (currentFrame >= currentAnimArray.Length)
            {
                currentFrame = 0;
                if (currentState == BossState.Attack)
                {
                    attackTimer = attackCooldown;
                    ChangeState(BossState.Patrol);
                }
            }

            spriteRenderer.sprite = currentAnimArray[currentFrame];

            if (currentState == BossState.Attack && currentFrame == fireFrame && !hasFiredThisAnim)
            {
                FireMissile();
                hasFiredThisAnim = true;
            }
        }
    }

    void FireMissile()
    {
        if (missilePrefab != null && firePoint != null && playerTransform != null)
        {
            GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
            BossMissile bm = missile.GetComponent<BossMissile>();

            if (bm != null)
            {
                Vector2 aimDirection = (playerTransform.position - firePoint.position).normalized;
                bm.direction = aimDirection;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        Vector2 contactNormal = collision.contacts[0].normal;
        if (contactNormal.x > 0.5f && movingLeft) movingLeft = false;
        else if (contactNormal.x < -0.5f && !movingLeft) movingLeft = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}