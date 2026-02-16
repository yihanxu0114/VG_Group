using UnityEngine;

public class Dynamite : MonoBehaviour
{
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

    void Start()
    {
        Invoke(nameof(Explode), fuseTime);
       
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        

        // 1. boom anim 
        if (explosionFrames == null || explosionFrames.Length == 0)
        {
            Debug.LogError("No explosion frames assigned.");
        }
        else
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

        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
  
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

            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(1);
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
