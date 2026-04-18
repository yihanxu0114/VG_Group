using System.Collections;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    [Header("Spawn No Collision")]
    public float noCollisionTime = 0.7f;
    public int damage = 1;
    private Collider2D[] myCols;

    [Header("Fuse")]
    public float fuseTime = 1.2f;

    [Header("Explosion")]
    public float radius = 2.0f;
    public LayerMask obstacleMask;

    [Header("Explosion Animation")]
    public Sprite[] explosionFrames;
    public float frameRate = 12f;
    public Vector3 explosionScale = Vector3.one;

    private bool exploded = false;

    void Awake()
    {
        myCols = GetComponentsInChildren<Collider2D>(true);
    }

    void Start()
    {
        Invoke(nameof(Explode), fuseTime);
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
        {
            foreach (var pc in playerCols)
            {
                if (mc != null && pc != null)
                {
                    Physics2D.IgnoreCollision(mc, pc, true);
                }
            }
        }

        yield return new WaitForSeconds(seconds);

        foreach (var mc in myCols)
        {
            foreach (var pc in playerCols)
            {
                if (mc != null && pc != null)
                {
                    Physics2D.IgnoreCollision(mc, pc, false);
                }
            }
        }
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        // ===== explosion animation =====
        if (explosionFrames != null && explosionFrames.Length > 0)
        {
            GameObject explosion = new GameObject("Explosion");
            explosion.transform.position = transform.position;
            explosion.transform.localScale = explosionScale;

            SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();

            SpriteRenderer mySr = GetComponent<SpriteRenderer>();
            if (mySr != null)
            {
                sr.sortingLayerName = mySr.sortingLayerName;
                sr.sortingOrder = mySr.sortingOrder + 10;
            }
            else
            {
                sr.sortingOrder = 100;
            }

            ExplosionAnimator animator = explosion.AddComponent<ExplosionAnimator>();
            animator.frames = explosionFrames;
            animator.frameRate = frameRate;
        }

        // ===== damage / destroy =====
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, obstacleMask);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            // skip explosion-immune objects
            if (hit.GetComponentInParent<ExplosionImmune>() != null)
            {
                continue;
            }

            Portal p = hit.GetComponent<Portal>();
            if (p != null)
            {
                Destroy(p.gameObject);
                continue;
            }

            BlockDynamite b = hit.GetComponent<BlockDynamite>();
            if (b != null)
            {
                b.Break();
            }

            PlayerHealth player = hit.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}