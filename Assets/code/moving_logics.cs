using UnityEngine;
using System.Collections;

public class moving_logic : MonoBehaviour
{
    [Header("Basic Movement")]
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;

    [Header("Status")]
    public float stunTimer = 0f;

    [Header("Jetpack Settings")]
    public float jetpackMaxTime = 5f;
    private float jetpackTimeLeft;

    [Header("Digging Settings")]
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
    bool lastDigDirectionDown = false;

    RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
    ContactFilter2D filter;

    [Header("No-collision at spawn")]
    public float noCollisionTime = 0.3f;
    private Collider2D bombCol;
    private Collider2D playerCol;

    void Awake()
    {
        jetpackTimeLeft = jetpackMaxTime;

        bombCol = GetComponent<Collider2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animScript = GetComponent<Sprite_left_right_shift>();

        filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(groundMask);
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerCol = player.GetComponent<Collider2D>();

        if (bombCol != null && playerCol != null)
            StartCoroutine(TempIgnorePlayerCollision());
    }

    void Update()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            return;
        }

        float x = 0f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;

        wasGrounded = isGrounded;
        isGrounded = CheckGrounded();

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        bool wantsJetpack = Input.GetKey(KeyCode.Space);
        bool usingJetpack = false;

        if (wantsJetpack && jetpackTimeLeft > 0f)
        {
            usingJetpack = true;
            jetpackTimeLeft -= Time.deltaTime;
            jetpackTimeLeft = Mathf.Max(jetpackTimeLeft, 0f);
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        else if (!wasGrounded && isGrounded)
        {
            jetpackTimeLeft = jetpackMaxTime;
        }

        if (animScript != null)
        {
            animScript.SetFacing(x);
            animScript.SetJetpack(usingJetpack);
            if (!usingJetpack) animScript.SetJumping(!isGrounded);
            else animScript.SetJumping(false);
        }

        bool wantsToDig = Input.GetMouseButton(1) && isGrounded;
        bool actuallyDigging = false;

        if (wantsToDig)
        {
            bool isDiggingDown = Input.GetKey(KeyCode.S);

            Vector2 direction = isDiggingDown ? Vector2.down : (animScript.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, digDistance, obstacleMask);

            if (hit.collider != null)
            {
                BlockHardness hardness = hit.collider.GetComponent<BlockHardness>();
                if (hardness != null && !hardness.isUndiggable)
                {
                    actuallyDigging = true;

                    if (!isDiggingAction || lastDigDirectionDown != isDiggingDown)
                    {
                        isDiggingAction = true;
                        lastDigDirectionDown = isDiggingDown;
                        if (animScript != null) animScript.SetDigging(true, isDiggingDown);
                    }

                    bool destroyed = hardness.TakeDamage(Time.deltaTime);
                    if (destroyed)
                    {
                        ValuableBlock valuable = hit.collider.GetComponent<ValuableBlock>();
                        if (valuable != null)
                        {
                            PlayerInventory inventory = GetComponent<PlayerInventory>();
                            if (inventory != null) inventory.CollectItem(valuable.blockType);
                        }
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }

        if (!actuallyDigging)
        {
            if (isDiggingAction)
            {
                isDiggingAction = false;
                if (animScript != null) animScript.SetDigging(false, false);
            }
        }
    }

    bool CheckGrounded()
    {
        if (col == null) return false;
        return col.Cast(Vector2.down, filter, hitBuffer, groundDistance) > 0;
    }

    IEnumerator TempIgnorePlayerCollision()
    {
        Physics2D.IgnoreCollision(bombCol, playerCol, true);
        yield return new WaitForSeconds(noCollisionTime);
        Physics2D.IgnoreCollision(bombCol, playerCol, false);
    }

    public float GetJetpackFuelRatio()
    {
        return jetpackMaxTime > 0f ? jetpackTimeLeft / jetpackMaxTime : 0f;
    }
}