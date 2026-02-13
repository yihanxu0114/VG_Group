using UnityEngine;
using System.Collections;

public class moving_logic : MonoBehaviour
{
    [Header("Basic Movement")]
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;

    [Header("Digging Settings")]
    public float defaultDigDuration = 0.5f; // digTime is the time it takes to dig through a block without a BlockHardness component. If a block has BlockHardness, we will use that value instead.
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

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animScript = GetComponent<Sprite_left_right_shift>();

        filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(groundMask);
    }

    void Update()
    {
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

        // Movement (WASD)
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

        // 1. first, we do a raycast in the direction we want to dig to see if there's a block there and how long it takes to dig through it
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, digDistance, obstacleMask);

        // 2. determine how long we need to wait based on the block's hardness. If there's no block, we can just use the default dig duration.
        float actualWaitTime = defaultDigDuration;

        if (hit.collider != null)
        {
            // judge if the block has a BlockHardness component, if it does, we use that value instead of the default dig duration
            BlockHardness hardness = hit.collider.GetComponent<BlockHardness>();
            if (hardness != null)
            {
                actualWaitTime = hardness.digTime;
                Debug.Log($"need time {actualWaitTime} s");
            }
        }

        // 3. start the digging animation (if we have one)
        if (animScript != null) animScript.SetDigging(true, isDown);

        // 4. wait for the required time to simulate digging
        yield return new WaitForSeconds(actualWaitTime);

        // 5. time's up, we check again if the block is still there (it might have been removed by another player or something). If it is, we destroy it.
        if (hit.collider != null)
        {
            Destroy(hit.collider.gameObject);
            Debug.Log("Sucessfully Digging£¡");
        }

        // 6. end the digging animation
        if (animScript != null) animScript.SetDigging(false, isDown);
        isDiggingAction = false;
    }

    bool CheckGrounded()
    {
        if (col == null) return false;
        return col.Cast(Vector2.down, filter, hitBuffer, groundDistance) > 0;
    }
}