using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 10;

    [Header("Spawn No Collision")]
    public float noCollisionTime = 0.3f;
    private Collider2D[] myCols;

    [Header("Animation Settings")]
    public Sprite[] frames;
    public int fps = 12;
    public bool loop = true;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float frameDuration;

    private int enemyLayer;
    private int projectileLayer;

    void Awake()
    {
        myCols = GetComponentsInChildren<Collider2D>(true);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        enemyLayer = LayerMask.NameToLayer("Enemy");
        projectileLayer = gameObject.layer;

        if (rb != null)
        {
            rb.velocity = transform.right * speed;
        }

        frameDuration = (fps > 0) ? 1f / fps : 0.1f;

        if (frames != null && frames.Length > 0 && sr != null)
        {
            sr.sprite = frames[0];
        }

        Destroy(gameObject, lifeTime);
    }

    public void InitIgnorePlayer(Collider2D[] playerCols, float seconds)
    {
        if (playerCols == null || playerCols.Length == 0) return;
        if (myCols == null || myCols.Length == 0) return;

        StartCoroutine(IgnoreRoutine(playerCols, seconds));
    }

    private IEnumerator IgnoreRoutine(Collider2D[] playerCols, float seconds)
    {
        foreach (var mc in myCols)
            foreach (var pc in playerCols)
                if (mc != null && pc != null)
                    Physics2D.IgnoreCollision(mc, pc, true);

        yield return new WaitForSeconds(seconds);

        foreach (var mc in myCols)
            foreach (var pc in playerCols)
                if (mc != null && pc != null)
                    Physics2D.IgnoreCollision(mc, pc, false);
    }

    void Update()
    {
        PlayAnimation();
    }

    void PlayAnimation()
    {
        if (frames == null || frames.Length == 0 || sr == null) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                    currentFrame = 0;
                else
                    currentFrame = frames.Length - 1;
            }

            sr.sprite = frames[currentFrame];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObj = collision.gameObject;
        int hitLayer = hitObj.layer;

        if (hitLayer == projectileLayer)
        {
            return;
        }

        if (hitLayer == enemyLayer)
        {
            EnemyHealth enemy = hitObj.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}