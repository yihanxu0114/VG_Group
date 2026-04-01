using UnityEngine;

public class BulletShootAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 12f;
    public bool loop = false;
    public int maxFrameCount = 0;

    private SpriteRenderer sr;
    private float timer = 0f;
    private int currentFrame = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (frames != null && frames.Length > 0)
        {
            sr.sprite = frames[0];
        }
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame++;

            int limit = (maxFrameCount > 0) ? Mathf.Min(maxFrameCount, frames.Length) : frames.Length;

            if (loop)
            {
                currentFrame %= limit;
            }
            else
            {
                if (currentFrame >= limit)
                {
                    currentFrame = limit - 1;
                }
            }

            sr.sprite = frames[currentFrame];
        }
    }

    public void Init(Sprite[] animFrames, float rate, bool shouldLoop, int frameLimit)
    {
        frames = animFrames;
        frameRate = rate;
        loop = shouldLoop;
        maxFrameCount = frameLimit;

        currentFrame = 0;
        timer = 0f;

        if (frames != null && frames.Length > 0)
        {
            sr.sprite = frames[0];
        }
    }
}