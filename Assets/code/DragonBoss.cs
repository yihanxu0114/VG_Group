using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DragonBoss : MonoBehaviour
{
    public enum DragonState { Fly, Attack }
    public DragonState currentState = DragonState.Fly;

    [Header("Animation settings (two groups of pictures)")]
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

    [Header("Attack settings")]
    public GameObject missilePrefab;
    public Transform firePoint;
    public float attackCooldown = 2.5f;
    public int fireFrame = 2;

    private float attackTimer;
    private bool hasFiredThisAnim;

    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.mass = 10000f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

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
        if (currentState == DragonState.Fly)
        {
            rb.velocity = new Vector2(movingLeft ? -flySpeed : flySpeed, rb.velocity.y);
        }
        else if (currentState == DragonState.Attack)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    void CheckState()
    {
        if (currentState == DragonState.Attack) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        bool isPlayerInFront = (movingLeft && playerTransform.position.x < transform.position.x) ||
                               (!movingLeft && playerTransform.position.x > transform.position.x);

        if (isPlayerInFront && distance <= attackRange && attackTimer <= 0)
        {
            Debug.Log("【Flying Dragon Brain】: Lock in the player, stop and prepare to unleash fire!");
            ChangeState(DragonState.Attack);
        }
    }

    void ChangeState(DragonState newState)
    {
        currentState = newState;
        currentFrame = 0;
        animTimer = 0;
        hasFiredThisAnim = false;
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
                    attackTimer = attackCooldown;
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
        if (missilePrefab == null)
        {
            Debug.LogError("🔴 【Dragon Flight Error】: You forgot to drag the Missile Prefab (missile) into the panel of Dragon Flight!");
            return;
        }
        if (firePoint == null)
        {
            Debug.LogError("🔴 【Dragon Flight Error】: You forgot to drag the Fire Point (fire point) into the panel of Dragon Flight!");
            return;
        }

        GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);

        BossMissile bm = missile.GetComponent<BossMissile>();
        if (bm != null)
        {
            Vector2 aimDirection = (playerTransform.position - firePoint.position).normalized;
            bm.direction = aimDirection;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        movingLeft = !movingLeft;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        int segments = 20;
        float startAngle = movingLeft ? 90f : -90f;
        Vector3 previousPoint = transform.position + new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad), Mathf.Sin(startAngle * Mathf.Deg2Rad), 0) * attackRange;

        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + (180f * i / segments);
            Vector3 currentPoint = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * attackRange;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
        Gizmos.DrawLine(transform.position + new Vector3(0, attackRange, 0), transform.position + new Vector3(0, -attackRange, 0));
    }
}