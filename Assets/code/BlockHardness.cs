using UnityEngine;

public class BlockHardness : MonoBehaviour
{
    [Header("Digging Setting")]
    public float digTime = 0.5f;        
    public bool isUndiggable = false; 

    [Header("Visual Feedback")]
    public Sprite[] damageFrames;

    private float currentDigTime;
    private SpriteRenderer sr;

    void Start()
    {
        currentDigTime = digTime;
        sr = GetComponent<SpriteRenderer>();
    }

    public bool TakeDamage(float damageAmount)
    {
        if (isUndiggable) return false; 

        currentDigTime -= damageAmount;

        if (currentDigTime <= 0)
        {
            return true;
        }

        if (sr != null && damageFrames != null && damageFrames.Length > 0)
        {
            float damagePercent = 1f - (currentDigTime / digTime);

            int frameIndex = Mathf.Clamp(Mathf.FloorToInt(damagePercent * damageFrames.Length), 0, damageFrames.Length - 1);

            sr.sprite = damageFrames[frameIndex];
        }

        return false; 
    }
}