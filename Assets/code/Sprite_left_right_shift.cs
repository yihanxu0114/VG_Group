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
    [Tooltip("启动动画的帧数，这些帧只播放一次，之后循环播放剩余帧")]
    public int jetpackIntroFrames = 0; // 例如：如果设为3，则前3帧只播放一次

    // ================= NEW ADDITION: DIGGING ANIMATION =================
    [Header("Digging Animation")]
    public Sprite[] digSideFrames; // from dig_00 to dig_07
    public Sprite[] digDownFrames; 
    public float digFrameRate = 12f;
    // ===================================================================

    private SpriteRenderer sr;
    private const float deadzone = 0.01f;

    private bool isWalking = false;
    private bool isJumping = false;
    private bool isUsingJetpack = false;

    private bool isDigging = false;
    private bool isDiggingDown = false;

    private float frameTimer = 0f;
    private int currentFrame = 0;
    private float jetpackFrameTimer = 0f;
    private int currentJetpackFrame = 0;
    private bool hasPlayedJetpackIntro = false; // 是否已播放完启动动画

    private float digTimer = 0f;
    private int currentDigFrame = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (rightDavid != null)
            sr.sprite = rightDavid;
    }

    void Update()
    {
        // Priority 0: Digging (Highest Priority)
        if (isDigging)
        {
            HandleDigAnimation();
            return;
        }

        // Priority 1: jetpack animation
        if (isUsingJetpack)
        {
            if (jetpackFrames != null && jetpackFrames.Length > 0)
            {
                jetpackFrameTimer += Time.deltaTime;
                if (jetpackFrameTimer >= 1f / jetpackFrameRate)
                {
                    jetpackFrameTimer = 0f;
                    
                    // 如果还在播放启动动画
                    if (!hasPlayedJetpackIntro && jetpackIntroFrames > 0)
                    {
                        currentJetpackFrame++;
                        
                        // 如果启动动画播放完毕
                        if (currentJetpackFrame >= jetpackIntroFrames)
                        {
                            hasPlayedJetpackIntro = true;
                            // 从循环动画的起始帧开始（如果有启动帧）
                            if (jetpackIntroFrames < jetpackFrames.Length)
                            {
                                currentJetpackFrame = jetpackIntroFrames;
                            }
                            else
                            {
                                // 如果启动帧数 >= 总帧数，从头循环
                                currentJetpackFrame = 0;
                            }
                        }
                    }
                    else
                    {
                        // 播放循环动画
                        currentJetpackFrame++;
                        
                        // 循环范围：从 jetpackIntroFrames 到最后一帧
                        int loopStart = Mathf.Min(jetpackIntroFrames, jetpackFrames.Length - 1);
                        if (currentJetpackFrame >= jetpackFrames.Length)
                        {
                            currentJetpackFrame = loopStart;
                        }
                    }
                    
                    sr.sprite = jetpackFrames[currentJetpackFrame];
                }
            }
            return;
        }

        // Priority 2: jumping sprite
        if (isJumping)
        {
            if (jumpSprite != null)
                sr.sprite = jumpSprite;
            return;
        }

        // Priority 3: walking animation
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

    void HandleDigAnimation()
    {
        Sprite[] currentAnim = isDiggingDown ? digDownFrames : digSideFrames;

        if (currentAnim != null && currentAnim.Length > 0)
        {
            digTimer += Time.deltaTime;
            if (digTimer >= 1f / digFrameRate)
            {
                digTimer = 0f;
                currentDigFrame = (currentDigFrame + 1) % currentAnim.Length;
                sr.sprite = currentAnim[currentDigFrame];
            }
        }
    }

    public void SetFacing(float x)
    {
        if (isDigging) return;

        if (Mathf.Abs(x) > deadzone)
        {
            isWalking = true;
        }

        if (x > deadzone)
            sr.flipX = false;
        else if (x < -deadzone)
            sr.flipX = true;
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
    }

    public void SetJetpack(bool usingJetpack)
    {
        // 如果是开始使用喷气背包，重置动画状态
        if (usingJetpack && !isUsingJetpack)
        {
            currentJetpackFrame = 0;
            jetpackFrameTimer = 0f;
            hasPlayedJetpackIntro = false;
            
            // 立即显示第一帧
            if (jetpackFrames != null && jetpackFrames.Length > 0)
                sr.sprite = jetpackFrames[0];
        }
        
        isUsingJetpack = usingJetpack;
    }

    public void SetDigging(bool digging, bool down)
    {
        isDigging = digging;
        isDiggingDown = down;

        // every time we start digging, reset the animation to the first frame and timer
        if (digging)
        {
            currentDigFrame = 0;
            digTimer = 0f;

            // set the sprite to the first frame of the appropriate digging animation
            Sprite[] currentAnim = isDiggingDown ? digDownFrames : digSideFrames;
            if (currentAnim != null && currentAnim.Length > 0)
                sr.sprite = currentAnim[0];
        }
        else
        {
            // when we stop digging, reset to idle sprite if not doing anything else
            if (!isJumping && !isUsingJetpack && !isWalking)
                sr.sprite = rightDavid;
        }
    }
}