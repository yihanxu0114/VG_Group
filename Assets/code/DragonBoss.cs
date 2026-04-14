using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DragonBoss : MonoBehaviour
{
    public enum DragonState { Fly, Attack }
    public DragonState currentState = DragonState.Fly;

    [Header("Animation settings")]
    public Sprite[] flyFrames;
    public Sprite[] attackFrames;
    public float frameRate = 0.15f;
    private int currentFrame;
    private float animTimer;
    private SpriteRenderer spriteRenderer;

    [Header("Flight and View Settings")]
    public float flySpeed = 3f;
    public float attackRange = 10f;
    public bool movingLeft = true;

    [Header("Initial Attack Settings")]
    public GameObject missilePrefab;
    public Transform firePoint;
    public float initialAttackCooldown = 2.5f;
    public float initialMissileSpeed = 8f;
    public int fireFrame = 2;

    [Header("Lightning & Evolution Settings")]
    public GameObject lightningPrefab;
    public int lightningCount = 7;
    public float lightningSpacing = 2.5f;
    public float cooldownReduction = 0.4f;
    public float speedIncrease = 2f;

    [Header("Body Collision Settings")]
    public int bodyDamage = 10;
    private float lastBodyDamageTime = -999f;

    private float currentAttackCooldown;
    private float currentMissileSpeed;
    private int nextHealthMilestone;
    private int milestoneAmount;

    private float attackTimer;
    private bool hasFiredThisAnim;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private EnemyHealth healthScript;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<EnemyHealth>();

        currentAttackCooldown = initialAttackCooldown;
        currentMissileSpeed = initialMissileSpeed;

        if (healthScript != null)
        {
            milestoneAmount = Mathf.FloorToInt(healthScript.maxHealth * 0.2f);
            nextHealthMilestone = healthScript.maxHealth - milestoneAmount;
        }

        rb.gravityScale = 0f;
        rb.mass = 10000f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (healthScript != null && healthScript.currentHealth <= nextHealthMilestone && healthScript.currentHealth > 0)
        {
            EvolveBoss();
            nextHealthMilestone -= milestoneAmount;
        }

        attackTimer -= Time.deltaTime;
        CheckState();
        PlayAnimation();
    }

    void EvolveBoss()
    {
        SummonLightningRow();
        lightningCount += 2;
        currentAttackCooldown = Mathf.Max(0.5f, currentAttackCooldown - cooldownReduction);
        currentMissileSpeed += speedIncrease;
    }

    void SummonLightningRow()
    {
        if (lightningPrefab == null) return;
        float startX = transform.position.x - ((lightningCount - 1) * lightningSpacing / 2f);
        Collider2D dragonCol = GetComponent<Collider2D>();

        for (int i = 0; i < lightningCount; i++)
        {
            float spawnX = startX + (i * lightningSpacing);
            Vector3 spawnPos = new Vector3(spawnX, transform.position.y, 0);
            GameObject lightning = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);

            Collider2D lightningCol = lightning.GetComponent<Collider2D>();
            if (dragonCol != null && lightningCol != null)
                Physics2D.IgnoreCollision(dragonCol, lightningCol);
        }
    }

    void PlayAnimation()
    {
        Sprite[] currentAnimArray = (currentState == DragonState.Fly) ? flyFrames : attackFrames;
        if (currentAnimArray == null || currentAnimArray.Length == 0) return;

        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer -= frameRate;
            currentFrame++;
            if (currentFrame >= currentAnimArray.Length)
            {
                currentFrame = 0;
                if (currentState == DragonState.Attack)
                {
                    attackTimer = currentAttackCooldown;
                    ChangeState(DragonState.Fly);
                }
            }
            spriteRenderer.sprite = currentAnimArray[currentFrame];
            spriteRenderer.flipX = movingLeft;

            if (firePoint != null)
            {
                Vector3 localPos = firePoint.localPosition;
                localPos.x = movingLeft ? -Mathf.Abs(localPos.x) : Mathf.Abs(localPos.x);
                firePoint.localPosition = localPos;
            }

            int actualFireFrame = Mathf.Min(fireFrame, currentAnimArray.Length - 1);
            if (currentState == DragonState.Attack && currentFrame == actualFireFrame && !hasFiredThisAnim)
            {
                FireMissile();
                hasFiredThisAnim = true;
            }
        }
    }

    void FireMissile()
    {
        if (missilePrefab == null || firePoint == null) return;
        GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);

        Collider2D dragonCol = GetComponent<Collider2D>();
        Collider2D missileCol = missile.GetComponent<Collider2D>();
        if (dragonCol != null && missileCol != null) Physics2D.IgnoreCollision(dragonCol, missileCol);

        BossMissile bm = missile.GetComponent<BossMissile>();
        if (bm != null)
        {
            bm.speed = currentMissileSpeed;
            bm.direction = (playerTransform.position - firePoint.position).normalized;
        }
    }

    void FixedUpdate()
    {
        if (currentState == DragonState.Fly)
            rb.velocity = new Vector2(movingLeft ? -flySpeed : flySpeed, rb.velocity.y);
        else if (currentState == DragonState.Attack)
            rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void CheckState()
    {
        if (currentState == DragonState.Attack) return;
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        bool isPlayerInFront = (movingLeft && playerTransform.position.x < transform.position.x) || (!movingLeft && playerTransform.position.x > transform.position.x);
        if (isPlayerInFront && distance <= attackRange && attackTimer <= 0) ChangeState(DragonState.Attack);
    }

    void ChangeState(DragonState newState)
    {
        currentState = newState;
        currentFrame = 0;
        animTimer = 0;
        hasFiredThisAnim = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastBodyDamageTime + 1.0f)
            {
                PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
                if (ph != null) ph.TakeDamage(bodyDamage);

                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = Vector2.zero;
                    float pushDirection = (collision.transform.position.x > transform.position.x) ? 1f : -1f;
                    playerRb.AddForce(new Vector2(pushDirection * 15f, 8f), ForceMode2D.Impulse);

                    moving_logic movementScript = collision.gameObject.GetComponent<moving_logic>();
                    if (movementScript != null) movementScript.stunTimer = 0.3f;
                }
                lastBodyDamageTime = Time.time;
            }
            return;
        }

        if (collision.gameObject.GetComponent<BossMissile>() != null ||
            collision.gameObject.GetComponent<LightningBolt>() != null) return;

        if (collision.contacts.Length > 0)
        {
            Vector2 contactNormal = collision.contacts[0].normal;
            if (contactNormal.x > 0.5f && movingLeft) movingLeft = false;
            else if (contactNormal.x < -0.5f && !movingLeft) movingLeft = true;
        }
    }
}