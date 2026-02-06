using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Sprite_left_right_shift : MonoBehaviour
{
    [Header("静止状态精灵")]
    public Sprite rightDavid;  // 静止时的精灵（面朝右）
    
    [Header("行走动画")]
    public Sprite[] walkingFrames;  // 行走动画帧数组（面朝右）
    public float frameRate = 10f;   // 动画帧率（每秒显示多少帧）
    
    [Header("跳跃精灵")]
    public Sprite jumpSprite;  // 跳跃时的精灵（面朝右）
    
    [Header("喷气背包精灵")]
    public Sprite jetpackSprite;  // 使用喷气背包时的精灵（面朝右）
    
    private SpriteRenderer sr;
    private const float deadzone = 0.01f;
    
    private bool isWalking = false;
    private bool isJumping = false;
    private bool isUsingJetpack = false;  // 新增：喷气背包状态
    private float frameTimer = 0f;
    private int currentFrame = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (rightDavid != null) 
            sr.sprite = rightDavid;
    }

    void Update()
    {
        // 优先级：喷气背包 > 跳跃 > 行走 > 静止
        
        // 1. 喷气背包优先级最高
        if (isUsingJetpack)
        {
            if (jetpackSprite != null)
                sr.sprite = jetpackSprite;
            return;
        }
        
        // 2. 跳跃精灵
        if (isJumping)
        {
            if (jumpSprite != null)
                sr.sprite = jumpSprite;
            return;
        }
        
        // 3. 行走动画
        if (isWalking && walkingFrames != null && walkingFrames.Length > 0)
        {
            frameTimer += Time.deltaTime;
            
            if (frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % walkingFrames.Length;
                sr.sprite = walkingFrames[currentFrame];
            }
        }
    }

    public void SetFacing(float x)
    {
        // 判断是否在移动
        if (Mathf.Abs(x) > deadzone)
        {
            isWalking = true;
            
            // 设置朝向
            if (x > deadzone) 
                sr.flipX = false;
            else if (x < -deadzone) 
                sr.flipX = true;
        }
        else
        {
            // 停止时切换回静止精灵（如果不在跳跃或使用喷气背包）
            isWalking = false;
            if (!isJumping && !isUsingJetpack)
            {
                sr.sprite = rightDavid;
            }
            currentFrame = 0;
            frameTimer = 0f;
        }
    }
    
    // 设置跳跃状态
    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
        
        if (!jumping && !isWalking && !isUsingJetpack)
        {
            // 落地且不在移动且不使用喷气背包时，回到静止精灵
            sr.sprite = rightDavid;
        }
    }
    
    // 新增：设置喷气背包状态
    public void SetJetpack(bool usingJetpack)
    {
        isUsingJetpack = usingJetpack;
        
        if (!usingJetpack && !isJumping && !isWalking)
        {
            // 停止使用喷气背包且不在跳跃或移动时，回到静止精灵
            sr.sprite = rightDavid;
        }
    }
}