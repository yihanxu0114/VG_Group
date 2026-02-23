using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    [Header("Anim Setting")]
    public Sprite[] animFrames;
    public float frameRate = 0.15f;
    private int currentFrame;
    private float animTimer;
    private SpriteRenderer spriteRenderer;

    [Header("Jump & Moving Setting")]
    public float hopForceX = 3f;
    public float hopForceY = 5f;
    public int jumpFrame = 2;
    private bool movingLeft = true;

    [Header("Danmage Setting")]
    public int damageAmount = 10;

    [Header("Hitting Back Setting")]
    public float knockbackForceX = 10f; // horizontal knockback force when hitting the player
    public float knockbackForceY = 5f;  // upward knockback force when hitting the player

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
    void Hop()
    {
        rb.velocity = Vector2.zero;

        float actualForceX = hopForceX * rb.mass;
        float actualForceY = hopForceY * rb.mass;

        if (movingLeft)
        {
            rb.AddForce(new Vector2(-actualForceX, actualForceY), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(actualForceX, actualForceY), ForceMode2D.Impulse);
        }
    }

    void PlayAnimation()
    {
        if (animFrames == null || animFrames.Length == 0) return;
        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer -= frameRate;
            currentFrame++;
            if (currentFrame >= animFrames.Length) currentFrame = 0;
            spriteRenderer.sprite = animFrames[currentFrame];
            if (currentFrame == jumpFrame)
            {
                Hop();
            }
        }
    }

    void Flip()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // deal damage to the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(damageAmount);

            // hit the player back with knockback
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;

                float pushDirection = 1f;
                if (collision.transform.position.x < transform.position.x) pushDirection = -1f;

                Vector2 knockbackVector = new Vector2(pushDirection * knockbackForceX, knockbackForceY);
                playerRb.AddForce(knockbackVector, ForceMode2D.Impulse);

                moving_logic movementScript = collision.gameObject.GetComponent<moving_logic>();
                if (movementScript != null)
                {
                    movementScript.stunTimer = 0.3f; 
                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 2. if it's not the player, we want to check if it's a wall and adjust direction accordingly
        if (!collision.gameObject.CompareTag("Player"))
        {
            // get the normal of the collision to determine which side the wall is on
            Vector2 contactNormal = collision.contacts[0].normal;

            // if the wall is on the left (wall's push is to the right, X > 0.5), and the slime is currently moving left
            if (contactNormal.x > 0.5f && movingLeft)
            {
                movingLeft = false; // let it move right
                Flip();
            }
            // if the wall is on the right (wall's push is to the left, X < -0.5), and the slime is currently moving right
            else if (contactNormal.x < -0.5f && !movingLeft)
            {
                movingLeft = true; // let it move left
                Flip();
            }
        }
    }
}