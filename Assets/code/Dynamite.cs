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
        Debug.Log($"炸弹启动，{fuseTime}秒后爆炸");
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        Debug.Log("💥 爆炸！");

        // 播放爆炸动画
        if (explosionFrames == null || explosionFrames.Length == 0)
        {
            Debug.LogError("❌ 没有设置爆炸帧！请在Inspector中拖入图片");
        }
        else
        {
            Debug.Log($"✅ 开始播放爆炸动画，共 {explosionFrames.Length} 帧");
            
            GameObject explosion = new GameObject("Explosion");
            explosion.transform.position = transform.position;
            explosion.transform.localScale = explosionScale;
            
            SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
            
            // 获取炸弹的SpriteRenderer设置
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
            
            Debug.Log($"爆炸对象创建在位置: {transform.position}");
        }

        // 破坏逻辑
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, obstacleMask);
        Debug.Log($"爆炸范围内检测到 {hits.Length} 个对象");
        
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