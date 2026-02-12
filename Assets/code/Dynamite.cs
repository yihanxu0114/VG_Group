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
        Debug.Log($"Dynamite lit. Going off in {fuseTime} seconds.");
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        Debug.Log("Boom! Explosion triggered.");

        // Play explosion animation
        if (explosionFrames == null || explosionFrames.Length == 0)
        {
            Debug.LogError("No explosion frames assigned. Drop them into the Inspector.");
        }
        else
        {
            Debug.Log($"Starting explosion animation with {explosionFrames.Length} frames.");
            
            GameObject explosion = new GameObject("Explosion");
            explosion.transform.position = transform.position;
            explosion.transform.localScale = explosionScale;
            
            SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
            
            // Match sorting settings with the dynamite sprite
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
            
            Debug.Log($"Explosion object spawned at position: {transform.position}");
        }

        // Destruction logic
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, obstacleMask);
        Debug.Log($"Found {hits.Length} objects inside explosion radius.");
        
        foreach (var hit in hits)
        {
            BlockDynamite b = hit.GetComponent<BlockDynamite>();
            if (b != null) b.Break();
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
