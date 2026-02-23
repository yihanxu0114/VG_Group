using UnityEngine;

public class EnemyMonster : MonoBehaviour
{
    [Header("Anim Setting")]
    public Sprite[] walkFrames;      // walk animation frames
    public Sprite[] attackFrames;    // attack animation frames
    public float frameRate = 0.15f;  // animation frame rate (time per frame)

    private int currentFrame;
    private float animTimer;
    private SpriteRenderer spriteRenderer;

    // if true, play attack animation and stop moving; if false, play walk animation and move
    private bool isAttacking = false;

    [Header("Moving Setting")]
    public float speed = 2f;
    private bool movingLeft = true;

    [Header("Attack Setting")]
    public int damageAmount = 10;
    public float knockbackForceX = 10f;
    public float knockbackForceY = 5f;
    public float attackRange = 1.5f;

    [Header("Attack Speed")]
    public int damageFrame = 2;         
    private bool hasDealtDamage = false; 
    private GameObject targetPlayer;    

    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayAnimation();
    }

    void FixedUpdate()
    {
        // if currently attacking, stop horizontal movement; otherwise, keep moving
        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else // if not attacking, keep moving
        {
            Move();
        }
    }

    void Move()
    {
        if (movingLeft)
        {
            // left walk
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            spriteRenderer.flipX = true;
        }
        else
        {
            // right walk
            rb.velocity = new Vector2(speed, rb.velocity.y);
            spriteRenderer.flipX = false;
        }
    }

    void PlayAnimation()
    {
        Sprite[] currentAnimArray = isAttacking ? attackFrames : walkFrames;
        if (currentAnimArray == null || currentAnimArray.Length == 0) return;

        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer -= frameRate;
            currentFrame++;

            if (currentFrame >= currentAnimArray.Length)
            {
                if (isAttacking)
                {
                    isAttacking = false;
                    currentFrame = 0;
                    targetPlayer = null; 
                }
                else
                {
                    currentFrame = 0;
                }
            }

            spriteRenderer.sprite = currentAnimArray[currentFrame];

            if (isAttacking && currentFrame == damageFrame && !hasDealtDamage)
            {
                if (targetPlayer != null)
                {
                    // 1. calc distance
                    float distance = Vector2.Distance(transform.position, targetPlayer.transform.position);

                    // 2. calc direction (is player in front?)
                    bool isPlayerInFront = false;
                    if (movingLeft && targetPlayer.transform.position.x < transform.position.x)
                    {
                        isPlayerInFront = true;
                    }
                    else if (!movingLeft && targetPlayer.transform.position.x > transform.position.x)
                    {
                        isPlayerInFront = true;
                    }

                    // 3. calc height difference (is player on top?)
                    float heightDifference = targetPlayer.transform.position.y - transform.position.y;
                    bool isNotOnTop = heightDifference < 0.8f;
                    if (distance <= attackRange && isPlayerInFront && isNotOnTop)
                    {
                        DealDamageToPlayer();
                    }
                    else
                    {
                        Debug.Log("Monster's Attack Miss£¡");
                    }
                }

                hasDealtDamage = true;
            }
        }
    }
    void DealDamageToPlayer()
    {
        if (targetPlayer == null) return;

        // 1. deal damage
        PlayerHealth playerHealth = targetPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null) playerHealth.TakeDamage(damageAmount);

        // 2. handle knockback: get the Rigidbody2D component from the target player object and apply a knockback force if it exists
        Rigidbody2D playerRb = targetPlayer.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            float pushDirection = 1f;
            if (targetPlayer.transform.position.x < transform.position.x) pushDirection = -1f;

            Vector2 knockbackVector = new Vector2(pushDirection * knockbackForceX, knockbackForceY);
            playerRb.AddForce(knockbackVector, ForceMode2D.Impulse);

            moving_logic movementScript = targetPlayer.GetComponent<moving_logic>();
            if (movementScript != null) movementScript.stunTimer = 0.3f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. hit the player (only trigger attack if player is in front and not on top)
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            bool isPlayerInFront = false;
            if (movingLeft && player.transform.position.x < transform.position.x)
            {
                isPlayerInFront = true;
            }
            else if (!movingLeft && player.transform.position.x > transform.position.x)
            {
                isPlayerInFront = true;
            }
            float heightDifference = player.transform.position.y - transform.position.y;
            bool isNotOnTop = heightDifference < 0.8f;
            if (isPlayerInFront && isNotOnTop)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    currentFrame = 0;
                    animTimer = 0f;
                    hasDealtDamage = false;
                    targetPlayer = player;
                }
            }
            else
            {
                Debug.Log("Monster's Attack Miss£¡");
            }
        }
        // 2. hit a wall or obstacle, reverse direction
        else
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}