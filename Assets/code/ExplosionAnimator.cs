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
            Debug.LogError("ExplosionAnimator: No animation frames assigned.");
            Destroy(gameObject);
            return;
        }
        
        Debug.Log($"ExplosionAnimator started. {frames.Length} frames at {frameRate} FPS.");
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
                Debug.Log("Explosion animation finished. Cleaning up.");
                Destroy(gameObject);
                return;
            }

            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}
