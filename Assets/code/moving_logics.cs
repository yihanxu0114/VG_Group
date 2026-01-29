using UnityEngine;

public class moving_logic : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpSpeed = 12f;

    public Rigidbody2D rb;
    private Collider2D col;

    // “脚底下面”有多近算落地
    public float groundDistance = 0.05f;

    // 可选：只认这些层为地面（如果你不想用 layer，就留 Default Everything 也行）
    public LayerMask groundMask = ~0; // 全部层

    bool isGrounded;

    // cast 用的缓存（避免每帧 new）
    RaycastHit2D[] hitBuffer = new RaycastHit2D[4];
    ContactFilter2D filter;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.useTriggers = false;      // 不把 trigger 当地面
        filter.SetLayerMask(groundMask); // 地面层（默认全部层）
    }

    void Update()
    {
        // 左右移动（物理）
        float x = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        // 只检测“向下”的地面，侧面墙不会算 grounded
        isGrounded = CheckGrounded();

        // 跳跃：必须真的在地面上，按下那一瞬间
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    bool CheckGrounded()
    {
        if (col == null) return false;

        // 用角色自身 collider 向下 cast 一小段距离
        int hitCount = col.Cast(Vector2.down, filter, hitBuffer, groundDistance);

        // 只要下面有东西就算 grounded
        return hitCount > 0;
    }
}
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
//         // 左右移动
//         float x = 0f;
//         if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
//         if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
//         rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
// 
//         // grounded：检测脚底附近的任何实体 collider（不靠 layer）
//         isGrounded = false;
//         if (groundCheck != null)
//         {
//             Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius);
//             foreach (var h in hits)
//             {
//                 if (h == null) continue;
//                 if (h.isTrigger) continue;                // 不把 trigger 当地面
//                 if (h.attachedRigidbody == rb) continue;  // 排除自己
//                 isGrounded = true;
//                 break;
//             }
//         }
// 
//         // 跳跃：只在按下那一瞬间触发一次
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