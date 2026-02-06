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
    bool wasGrounded;
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

        wasGrounded = isGrounded;
        isGrounded = CheckGrounded();

        // W键 - 普通跳跃（只能在地面）
        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        
        // 空格键 - 喷气背包（可以在空中使用）
        bool usingJetpack = Input.GetKey(KeyCode.Space);
        if (usingJetpack)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // 更新精灵状态
        if (facing != null)
        {
            facing.SetFacing(x);
            
            // 设置喷气背包状态
            facing.SetJetpack(usingJetpack);
            
            // 设置跳跃状态（只在不使用喷气背包时生效）
            if (!usingJetpack)
            {
                facing.SetJumping(!isGrounded);
            }
            else
            {
                // 使用喷气背包时，关闭普通跳跃状态
                facing.SetJumping(false);
            }
        }
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