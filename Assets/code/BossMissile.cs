using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BossMissile : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 8f;
    public int damage = 15;
    public Vector2 direction;

    [Header("Flight Animation")]
    public Sprite[] flightFrames;
    public float flightFrameRate = 0.1f;

    [Header("Explosion Animation")]
    public Sprite[] explosionFrames;
    public float explosionFrameRate = 0.08f;
    public Vector3 explosionScale = new Vector3(1.5f, 1.5f, 1f);

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isExploding = false;

    private int currentFlightFrame = 0;
    private float flightAnimTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;

        if (direction.x < 0)
        {
            sr.flipX = true;
        }
        else if (direction.x > 0)
        {
            sr.flipX = false;
        }

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (!isExploding && flightFrames != null && flightFrames.Length > 0)
        {
            flightAnimTimer += Time.deltaTime;
            if (flightAnimTimer >= flightFrameRate)
            {
                flightAnimTimer -= flightFrameRate;
                currentFlightFrame = (currentFlightFrame + 1) % flightFrames.Length;
                sr.sprite = flightFrames[currentFlightFrame];
            }
        }
    }

    void FixedUpdate()
    {
        if (!isExploding)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    void HandleCollision(GameObject hitObject)
    {
        if (isExploding) return;

        if (hitObject.CompareTag("Enemy") || hitObject.name.Contains("DragonBoss")) return;

        if (hitObject.GetComponent<LightningBolt>() != null) return;
        if (hitObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hitObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        StartCoroutine(ExplodeRoutine());
    }

    IEnumerator ExplodeRoutine()
    {
        isExploding = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        sr.flipX = false;
        transform.rotation = Quaternion.identity;
        transform.localScale = explosionScale;

        if (explosionFrames != null && explosionFrames.Length > 0)
        {
            foreach (Sprite frame in explosionFrames)
            {
                sr.sprite = frame;
                yield return new WaitForSeconds(explosionFrameRate);
            }
        }

        Destroy(gameObject);
    }
}