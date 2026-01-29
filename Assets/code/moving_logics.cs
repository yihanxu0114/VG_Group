using UnityEngine;

public class moving_logic : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;
    public Rigidbody2D rb;
    private Collider2D col;
    public float groundDistance = 0.05f;
    // Consider which layer is the ground 
    public LayerMask groundMask = ~0; // default is all layer, changing depending on future map.
    bool isGrounded;
    RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
    ContactFilter2D filter;
    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        filter = new ContactFilter2D();
        filter.useTriggers = false;     
        filter.SetLayerMask(groundMask);
    }

    void Update()
    {
        // left and right moving logics
        float x = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        // detect bottom collidor
        isGrounded = CheckGrounded();

        // jumping logics
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }
    //check whether the player character is on the ground or not 
    bool CheckGrounded()
    {
        if (col == null) return false;
        int hitCount = col.Cast(Vector2.down, filter, hitBuffer, groundDistance);
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
//         if (rb == null) rb = GetComponent<Rigidbody2D>();
//     }
// 
//     void Update()
//     {
//      
//         float x = 0f;
//         if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
//         if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
//         rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
// 
//         
//         isGrounded = false;
//         if (groundCheck != null)
//         {
//             Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius);
//             foreach (var h in hits)
//             {
//                 if (h == null) continue;
//                 if (h.isTrigger) continue;                
//                 if (h.attachedRigidbody == rb) continue;  
//                 isGrounded = true;
//                 break;
//             }
//         }
// 
//         
//         if (isGrounded && Input.GetKeyDown(KeyCode.Space))
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