using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Sprite_left_right_shift : MonoBehaviour
{
    [Header("still")]
    public Sprite rightDavid; 
    
    [Header("walking animation")]
    public Sprite[] walkingFrames;  
    public float frameRate = 10f;   
    
    [Header("jumping animation")]
    public Sprite jumpSprite;  
    
    [Header("jetpack animation")]
    public Sprite[] jetpackFrames;  
    public float jetpackFrameRate = 10f;  
    
    private SpriteRenderer sr;
    private const float deadzone = 0.01f;
    
    private bool isWalking = false;
    private bool isJumping = false;
    private bool isUsingJetpack = false;  
    private float frameTimer = 0f;
    private int currentFrame = 0;
    private float jetpackFrameTimer = 0f;
    private int currentJetpackFrame = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (rightDavid != null) 
            sr.sprite = rightDavid;
    }

    void Update()
    {
        // pirority 1: jetpack animation
        if (isUsingJetpack)
        {
            if (jetpackFrames != null && jetpackFrames.Length > 0)
            {
                jetpackFrameTimer += Time.deltaTime;
                
                if (jetpackFrameTimer >= 1f / jetpackFrameRate)
                {
                    jetpackFrameTimer = 0f;
                    currentJetpackFrame = (currentJetpackFrame + 1) % jetpackFrames.Length;
                    sr.sprite = jetpackFrames[currentJetpackFrame];
                }
            }
            return;
        }
        
        // pirority 2: jumping sprite
        if (isJumping)
        {
            if (jumpSprite != null)
                sr.sprite = jumpSprite;
            return;
        }
        
        // pirority 3: walking animation
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
    
        if (Mathf.Abs(x) > deadzone)
        {
            isWalking = true;
            
      
            if (x > deadzone) 
                sr.flipX = false;
            else if (x < -deadzone) 
                sr.flipX = true;
        }
        else
        {
         
            isWalking = false;
            if (!isJumping && !isUsingJetpack)
            {
                sr.sprite = rightDavid;
            }
            currentFrame = 0;
            frameTimer = 0f;
        }
    }
    
  
    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
        
        if (!jumping && !isWalking && !isUsingJetpack)
        {
        
            sr.sprite = rightDavid;
        }
    }
    // set jetpack state
    public void SetJetpack(bool usingJetpack)
    {
        // if state changed, reset animation
        if (isUsingJetpack != usingJetpack)
        {
            currentJetpackFrame = 0;
            jetpackFrameTimer = 0f;
        }
        
        isUsingJetpack = usingJetpack;
        
        if (!usingJetpack && !isJumping && !isWalking)
        {
            // back to still sprite
            sr.sprite = rightDavid;
        }
    }
}