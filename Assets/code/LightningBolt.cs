using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    public enum State { Hover, Fall, Explode }
    public State currentState = State.Hover;

    [Header("Hover & Fall Settings")]
    public Sprite[] idleFrames;      
    public float idleFrameRate = 0.1f;
    public float hoverTime = 3f;    
    public float fallSpeed = 25f;    
    public string groundTag = "Ground";

    [Header("Explosion Settings")]
    public Sprite[] explosionFrames; 
    public float explosionFrameRate = 0.05f;
    public int damageFrame = 4;    
    public int damage = 30;

    [Header("Hitbox Settings (Damage Range)")]
    public Vector2 strikeBoxSize = new Vector2(2f, 2f);
    public Vector2 strikeBoxOffset = new Vector2(0f, 0f);

    private SpriteRenderer sr;
    private float stateTimer = 0f;
    private float animTimer = 0f;
    private int currentFrame = 0;
    private bool hasDamaged = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1, 1, 1, 0.5f);
    }

    void Update()
    {
        if (currentState == State.Hover)
        {
            PlayAnimFrames(idleFrames, idleFrameRate);

            stateTimer += Time.deltaTime;
            if (stateTimer >= hoverTime)
            {
                currentState = State.Fall;
                if (sr != null) sr.color = Color.white; 
            }
        }
        else if (currentState == State.Fall)
        {
            PlayAnimFrames(idleFrames, idleFrameRate);

            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

            if (transform.position.y < -50f) Destroy(gameObject);
        }
        else if (currentState == State.Explode)
        {
            PlayExplosionAnimation();
        }
    }

    void PlayAnimFrames(Sprite[] frames, float frameRate)
    {
        if (frames == null || frames.Length == 0) return;
        animTimer += Time.deltaTime;
        if (animTimer >= frameRate)
        {
            animTimer -= frameRate;
            currentFrame = (currentFrame + 1) % frames.Length;
            sr.sprite = frames[currentFrame];
        }
    }

    void PlayExplosionAnimation()
    {
        if (explosionFrames == null || explosionFrames.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        animTimer += Time.deltaTime;
        if (animTimer >= explosionFrameRate)
        {
            animTimer -= explosionFrameRate;
            currentFrame++;

            if (currentFrame >= explosionFrames.Length)
            {
                Destroy(gameObject);
                return;
            }

            sr.sprite = explosionFrames[currentFrame];

            if (currentFrame == damageFrame && !hasDamaged)
            {
                ExecuteDamage();
                hasDamaged = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState != State.Fall) return;

        if (collision.CompareTag("Player") || collision.CompareTag(groundTag))
        {
            StartExplosion();
        }
    }

    void StartExplosion()
    {
        currentState = State.Explode;
        currentFrame = 0;
        animTimer = 0f;
        hasDamaged = false;
    }

    void ExecuteDamage()
    {
        Vector2 boxCenter = (Vector2)transform.position + strikeBoxOffset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, strikeBoxSize, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null) ph.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 boxCenter = (Vector2)transform.position + strikeBoxOffset;
        Gizmos.DrawWireCube(boxCenter, strikeBoxSize);
    }
}