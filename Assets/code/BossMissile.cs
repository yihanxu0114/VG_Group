using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [Header("Missile Setting")]
    public float speed = 8f;
    public int damage = 20;
    [HideInInspector] public Vector2 direction;

    [Header("Animation Setting")]
    public Sprite[] frames;          // 拖入动画帧（按顺序）
    public int fps = 12;             // 每秒播放帧数（可自由设置）
    public bool loop = true;         // 是否循环播放

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float frameDuration;     // 每帧持续时间

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        Destroy(gameObject, 5f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 根据 fps 计算每帧时长
        frameDuration = (fps > 0) ? 1f / fps : 0.1f;

        // 初始化第一帧
        if (frames != null && frames.Length > 0)
            sr.sprite = frames[0];
    }

    void Update()
    {
        PlayAnimation();
    }

    void FixedUpdate()
    {
        rb.velocity = direction * speed;
    }

    void PlayAnimation()
    {
        if (frames == null || frames.Length == 0) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                    currentFrame = 0;       // 循环：回到第一帧
                else
                    currentFrame = frames.Length - 1; // 不循环：停在最后一帧
            }

            sr.sprite = frames[currentFrame];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null) player.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}