using UnityEngine;

public class Dynamite : MonoBehaviour
{
    [Header("Fuse")]
    public float fuseTime = 1.2f;

    [Header("Explosion")]
    public float radius = 2.0f;
    public LayerMask obstacleMask;   
    public GameObject explosionVfx;  
    public float vfxLife = 1.0f;

    private bool exploded = false;

    void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, obstacleMask);
       

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
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}