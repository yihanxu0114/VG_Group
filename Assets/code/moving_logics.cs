using UnityEngine;
using System.Collections;

public class moving_logic : MonoBehaviour
{
    [Header("Basic Movement")]
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;

    [Header("Status")]
    public float stunTimer = 0f;

    [Header("Digging Settings")]
    public float defaultDigDuration = 0.5f;
    public float digDistance = 1.2f;
    public LayerMask obstacleMask;

    [Header("References")]
    public Rigidbody2D rb;
    private Collider2D col;
    private Sprite_left_right_shift animScript;

    [Header("Ground Detection")]
    public float groundDistance = 0.05f;
    public LayerMask groundMask = ~0;

    bool isGrounded;
    bool wasGrounded;
    bool isDiggingAction = false;

    RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
    ContactFilter2D filter;
    [Header("No-collision at spawn")]
    public float noCollisionTime = 0.3f;

    private Collider2D bombCol;
    private Collider2D playerCol;
    void Awake()
    {
        bombCol = GetComponent<Collider2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animScript = GetComponent<Sprite_left_right_shift>();

        filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(groundMask);
    }

    void Update()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            return;
        }

        if (isDiggingAction)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (Input.GetKeyDown(KeyCode.J) && isGrounded)
        {
            bool isDiggingDown = Input.GetKey(KeyCode.S);

            Vector2 direction = Vector2.right;
            if (isDiggingDown)
            {
                direction = Vector2.down;
            }
            else
            {
                bool isFacingLeft = animScript.GetComponent<SpriteRenderer>().flipX;
                direction = isFacingLeft ? Vector2.left : Vector2.right;
            }

            StartCoroutine(DigRoutine(direction, isDiggingDown));
            return;
        }

        float x = 0f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        wasGrounded = isGrounded;
        isGrounded = CheckGrounded();

        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        bool usingJetpack = Input.GetKey(KeyCode.Space);
        if (usingJetpack)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        if (animScript != null)
        {
            animScript.SetFacing(x);
            animScript.SetJetpack(usingJetpack);
            if (!usingJetpack) animScript.SetJumping(!isGrounded);
            else animScript.SetJumping(false);
        }
    }

    IEnumerator DigRoutine(Vector2 dir, bool isDown)
    {
        isDiggingAction = true;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, digDistance, obstacleMask);

        float actualWaitTime = defaultDigDuration;
        bool targetIsUndiggable = false;

        if (hit.collider != null)
        {
            BlockHardness hardness = hit.collider.GetComponent<BlockHardness>();
            if (hardness != null)
            {
                actualWaitTime = hardness.digTime;
                targetIsUndiggable = hardness.isUndiggable;
            }
        }

        if (animScript != null) animScript.SetDigging(true, isDown);

        yield return new WaitForSeconds(actualWaitTime);

        if (hit.collider != null)
        {
            if (targetIsUndiggable)
            {
                Debug.Log("It's a rock! The pickaxe did nothing.");
            }
            else
            {
                ValuableBlock valuable = hit.collider.GetComponent<ValuableBlock>();
                if (valuable != null)
                {
                    PlayerInventory inventory = GetComponent<PlayerInventory>();
                    if (inventory != null)
                    {
                        inventory.CollectItem(valuable.blockType);
                    }
                }
                if (hit.collider.GetComponent<Portal>() != null)
                {
                    Debug.Log("Portal cannot be dug.");
                }
                else
                {
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Digging successful!");
                }
            }
        }

        if (animScript != null) animScript.SetDigging(false, isDown);
        isDiggingAction = false;
    }

    bool CheckGrounded()
    {
        if (col == null) return false;
        return col.Cast(Vector2.down, filter, hitBuffer, groundDistance) > 0;
    }
 

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerCol = player.GetComponent<Collider2D>();

        if (bombCol != null && playerCol != null)
            StartCoroutine(TempIgnorePlayerCollision());
    }

    IEnumerator TempIgnorePlayerCollision()
    {
        Physics2D.IgnoreCollision(bombCol, playerCol, true);
        yield return new WaitForSeconds(noCollisionTime);
        Physics2D.IgnoreCollision(bombCol, playerCol, false);
    }
}