using UnityEngine;

public class ExplosionAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 12f;

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (frames == null || frames.Length == 0)
        {
            Debug.LogError("ExplosionAnimator: 没有动画帧！");
            Destroy(gameObject);
            return;
        }
        
        Debug.Log($"ExplosionAnimator 开始播放，共 {frames.Length} 帧，帧率 {frameRate}");
        spriteRenderer.sprite = frames[0];
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        float frameDuration = 1f / frameRate;

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                Debug.Log("动画播放完毕，销毁");
                Destroy(gameObject);
                return;
            }

            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}