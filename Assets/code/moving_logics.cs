using UnityEngine;
using System.Collections;

public class moving_logic : MonoBehaviour
{
    [Header("Basic Movement")]
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;

    [Header("Digging Settings")]
    public float digDistance = 1.2f;     // how far the player can dig
    public float digDuration = 0.5f;     // how long the digging action takes (animation time)
    public LayerMask obstacleMask;       

    [Header("References")]
    public Rigidbody2D rb;
    private Collider2D col;
    // animScript
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
        // 1. if digging, lock all other inputs until the digging action is complete
        if (isDiggingAction)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // 2. Only allow digging when grounded (you can change this if you want to allow mid-air digging)
        if (Input.GetKeyDown(KeyCode.J) && isGrounded)
        {
            // If player presses J, check if they are also holding S to determine digging direction
            bool isDiggingDown = Input.GetKey(KeyCode.S);

            Vector2 direction = Vector2.right;
            if (isDiggingDown)
            {
                direction = Vector2.down;
            }
            else
            {
                // get facing direction from the animation script (assuming it has a bool or method to determine facing direction)
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
        isDiggingAction = true; // lock input

        // 1. Start digging animation
        if (animScript != null) animScript.SetDigging(true, isDown);

        yield return new WaitForSeconds(digDuration);

        // 3. Judge if there is an obstacle in the digging direction within digDistance using Raycast
        Debug.DrawRay(transform.position, dir * digDistance, Color.red, 1.0f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, digDistance, obstacleMask);

        if (hit.collider != null)
        {
            Debug.Log("dig something£º" + hit.collider.name);
            // destroy the hit object (you can replace this with a more complex logic if you want, e.g. play a breaking animation, drop items, etc.)
            Destroy(hit.collider.gameObject);
        }
        else
        {
            Debug.Log("Layer Error");
        }

        // 4. digging action complete, reset animation and unlock input
        if (animScript != null) animScript.SetDigging(false, isDown);

        isDiggingAction = false; // ½âËøÊäÈë
    }

    bool CheckGrounded()
    {
        if (col == null) return false;
        return col.Cast(Vector2.down, filter, hitBuffer, groundDistance) > 0;
    }
}