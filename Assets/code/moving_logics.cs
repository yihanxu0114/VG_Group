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
        isDiggingAction = true; // Lock movement input

        // 1. Raycast to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, digDistance, obstacleMask);

        // 2. Calculate digging duration based on block hardness
        float actualWaitTime = defaultDigDuration;
        bool targetIsUndiggable = false; // Flag: is the target impossible to dig?

        if (hit.collider != null)
        {
            BlockHardness hardness = hit.collider.GetComponent<BlockHardness>();
            if (hardness != null)
            {
                actualWaitTime = hardness.digTime; // Get custom dig time
                targetIsUndiggable = hardness.isUndiggable; // Check if this block is undiggable
            }
        }

        // 3. [ANIMATION] Start the digging animation regardless of block type
        // This allows the player to swing at rocks without them breaking
        if (animScript != null) animScript.SetDigging(true, isDown);

        // 4. Wait for the animation to play (simulate the effort of digging)
        yield return new WaitForSeconds(actualWaitTime);

        // 5. Time's up! Decide whether to destroy the block
        if (hit.collider != null)
        {
            // If it is marked as undiggable (like your Rock)
            if (targetIsUndiggable)
            {
                // Do nothing to the GameObject.
                Debug.Log("It's a rock! The pickaxe did nothing.");
            }
            else
            {
                // If it's a normal block (Dirt/Gold/Diamond), destroy it.
                Destroy(hit.collider.gameObject);
                Debug.Log("Digging successful!");
            }
        }

        // 6. Stop the animation and unlock movement
        if (animScript != null) animScript.SetDigging(false, isDown);
        isDiggingAction = false;
    }

    bool CheckGrounded()
    {
        if (col == null) return false;
        return col.Cast(Vector2.down, filter, hitBuffer, groundDistance) > 0;
    }
}