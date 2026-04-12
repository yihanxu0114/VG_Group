using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class SlimeKingBoss : MonoBehaviour
{
    public enum State { Walk, Attack, Split }
    public State currentState = State.Walk;

    [Header("Animation Settings")]
    public Sprite[] walkFrames;
    public Sprite[] attackFrames;
    public Sprite[] splitFrames;

    public float frameRate = 0.15f;
    public float splitFrameRate = 0.08f;

    private int currentFrame;
    private float animTimer;
    private SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool movingLeft = true;

    [Header("Attack Settings")]
    public float attackRange = 3.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 25;
    public int damageFrame = 2;

    [Header("Knockback Settings")]
    public float knockbackForceX = 15f;
    public float knockbackForceY = 8f;

    [Header("Split Shrink Effect")]
    public float targetSplitScale = 0.4f; 
    private Vector3 originalScale;       
    private float splitTimer = 0f;     
    private float splitDuration = 1f;     

    private float attackTimer;
    private bool hasDamagedThisAnim;
    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (currentState == State.Split)
        {
            splitTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(splitTimer / splitDuration);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * targetSplitScale, progress);

            PlayAnimation();
            return;
        }

        if (playerTransform == null) return;

        attackTimer -= Time.deltaTime;
        CheckState();
        PlayAnimation();
    }

    void FixedUpdate()
    {
        if (currentState == State.Walk)
        {
            rb.velocity = new Vector2(movingLeft ? -moveSpeed : moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void StartDeathSplitting()
    {
        if (currentState == State.Split) return;

        currentState = State.Split;
        currentFrame = 0;
        animTimer = 0;

        if (splitFrames != null && splitFrames.Length > 0)
        {
            splitDuration = splitFrames.Length * splitFrameRate;
        }
        splitTimer = 0f;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    void CheckState()
    {
        if (currentState == State.Attack || currentState == State.Split) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        bool isPlayerInFront = (movingLeft && playerTransform.position.x < transform.position.x) ||
                               (!movingLeft && playerTransform.position.x > transform.position.x);

        if (isPlayerInFront && distance <= attackRange && attackTimer <= 0)
        {
            ChangeState(State.Attack);
        }
    }

    void ChangeState(State newState)
    {
        if (currentState == State.Split) return;
        currentState = newState;
        currentFrame = 0;
        animTimer = 0;
        hasDamagedThisAnim = false;
    }

    void PlayAnimation()
    {
        Sprite[] currentAnimArray;
        float currentFrameRate = frameRate;

        if (currentState == State.Walk)
        {
            currentAnimArray = walkFrames;
        }
        else if (currentState == State.Attack)
        {
            currentAnimArray = attackFrames;
        }
        else
        {
            currentAnimArray = splitFrames;
            currentFrameRate = splitFrameRate;
        }

        if (currentAnimArray == null || currentAnimArray.Length == 0) return;

        animTimer += Time.deltaTime;
        if (animTimer >= currentFrameRate)
        {
            animTimer -= currentFrameRate;
            currentFrame++;

            if (currentFrame >= currentAnimArray.Length)
            {
                if (currentState == State.Split)
                {
                    EnemyHealth healthScript = GetComponent<EnemyHealth>();
                    if (healthScript != null)
                    {
                        healthScript.ExecuteRealDeathAfterAnimation();
                    }
                    return;
                }

                currentFrame = 0;
                if (currentState == State.Attack)
                {
                    attackTimer = attackCooldown;
                    ChangeState(State.Walk);
                }
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = currentAnimArray[currentFrame];

                if (currentState != State.Split)
                {
                    spriteRenderer.flipX = !movingLeft;
                }
            }

            int actualDamageFrame = Mathf.Min(damageFrame, currentAnimArray.Length - 1);
            if (currentState == State.Attack && currentFrame == actualDamageFrame && !hasDamagedThisAnim)
            {
                ExecuteAttack();
                hasDamagedThisAnim = true;
            }
        }
    }

    void ExecuteAttack()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        bool isPlayerInFront = (movingLeft && playerTransform.position.x < transform.position.x) ||
                               (!movingLeft && playerTransform.position.x > transform.position.x);

        if (isPlayerInFront && distance <= attackRange)
        {
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(attackDamage);

            Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                float pushDirection = movingLeft ? -1f : 1f;
                Vector2 knockbackVector = new Vector2(pushDirection * knockbackForceX, knockbackForceY);
                playerRb.AddForce(knockbackVector, ForceMode2D.Impulse);

                moving_logic movementScript = playerTransform.GetComponent<moving_logic>();
                if (movementScript != null)
                {
                    movementScript.stunTimer = 0.3f;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            Vector2 contactNormal = collision.contacts[0].normal;
            if (contactNormal.x > 0.5f && movingLeft)
            {
                movingLeft = false;
            }
            else if (contactNormal.x < -0.5f && !movingLeft)
            {
                movingLeft = true;
            }
        }
    }
}