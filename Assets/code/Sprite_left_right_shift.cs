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
    
    [Header("jackpet")]
    public Sprite jetpackSprite;  
    
    private SpriteRenderer sr;
    private const float deadzone = 0.01f;
    
    private bool isWalking = false;
    private bool isJumping = false;
    private bool isUsingJetpack = false;  
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
  

        if (isUsingJetpack)
        {
            if (jetpackSprite != null)
                sr.sprite = jetpackSprite;
            return;
        }
        
  
        if (isJumping)
        {
            if (jumpSprite != null)
                sr.sprite = jumpSprite;
            return;
        }
        
     
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
    

    public void SetJetpack(bool usingJetpack)
    {
        isUsingJetpack = usingJetpack;
        
        if (!usingJetpack && !isJumping && !isWalking)
        {
      
            sr.sprite = rightDavid;
        }
    }
}