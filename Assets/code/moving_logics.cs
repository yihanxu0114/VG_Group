// One time jump logic
using UnityEngine;

public class moving_logic : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;

    public Rigidbody2D rb;
    private Collider2D col;

    public float groundDistance = 0.05f;

    public LayerMask groundMask = ~0;

    bool isGrounded;
    Sprite_left_right_shift facing;
    RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
    ContactFilter2D filter;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        col = GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(groundMask);
        facing = GetComponent<Sprite_left_right_shift>();
    }

    void Update()
    {
        float x = 0f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        isGrounded = CheckGrounded();

        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        if (facing != null)
            facing.SetFacing(x);
    }

    bool CheckGrounded()
    {
        if (col == null) return false;

        int hitCount = col.Cast(
            Vector2.down,
            filter,
            hitBuffer,
            groundDistance
        );

        return hitCount > 0;
    }
}

// below is continuous jumping logics. 
//using UnityEngine;
// 
// public class moving_logic : MonoBehaviour
// {
//     public float moveSpeed = 6f;
//     public float jumpSpeed = 7f;
// 
//     public Rigidbody2D rb;
//     public Transform groundCheck;
//     public float groundRadius = 0.15f;
// 
//     bool isGrounded;
// 
//     void Awake()
//     {
//         if (rb == null)
//             rb = GetComponent<Rigidbody2D>();
//     }
// 
//     void Update()
//     {
//         // right left shifting 
//         float x = 0f;
//         if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
//         if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
// 
//         rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
//         // jump
//         if (Input.GetKey(KeyCode.Space))
//         {
//             rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
//         }
//     }
// 
//     void OnDrawGizmosSelected()
//     {
//         if (groundCheck == null) return;
//         Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
//     }
// }