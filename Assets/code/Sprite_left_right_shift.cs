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
    public int jetpackIntroFrames = 0;

    [Header("Digging Animation")]
    public Sprite[] digSideFrames;
    public Sprite[] digDownFrames;
    public float digFrameRate = 12f;

    // ================= SHOOTING ANIMATION =================
    [Header("Shooting Animation")]
    [Tooltip("发射动画帧，三种子弹共用，只播放一遍")]
    public Sprite[] shootFrames;
    public float shootFrameRate = 20f;
    // ======================================================

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
    private bool hasPlayedJetpackIntro = false;

    private float digTimer = 0f;
    private int currentDigFrame = 0;

    // ---- shooting state ----
    private bool isShooting = false;
    private float shootTimer = 0f;
    private int currentShootFrame = 0;
    // ------------------------

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

        // Priority 1: Shooting (play once, then auto-exit)
        if (isShooting)
        {
            HandleShootAnimation();
            return;
        }

        // Priority 2: jetpack animation
        if (isUsingJetpack)
        {
            if (jetpackFrames != null && jetpackFrames.Length > 0)
            {
                jetpackFrameTimer += Time.deltaTime;
                if (jetpackFrameTimer >= 1f / jetpackFrameRate)
                {
                    jetpackFrameTimer = 0f;

                    if (!hasPlayedJetpackIntro && jetpackIntroFrames > 0)
                    {
                        currentJetpackFrame++;
                        if (currentJetpackFrame >= jetpackIntroFrames)
                        {
                            hasPlayedJetpackIntro = true;
                            currentJetpackFrame = jetpackIntroFrames < jetpackFrames.Length
                                ? jetpackIntroFrames
                                : 0;
                        }
                    }
                    else
                    {
                        currentJetpackFrame++;
                        int loopStart = Mathf.Min(jetpackIntroFrames, jetpackFrames.Length - 1);
                        if (currentJetpackFrame >= jetpackFrames.Length)
                            currentJetpackFrame = loopStart;
                    }

                    sr.sprite = jetpackFrames[currentJetpackFrame];
                }
            }
            return;
        }

        // Priority 3: jumping sprite
        if (isJumping)
        {
            if (jumpSprite != null)
                sr.sprite = jumpSprite;
            return;
        }

        // Priority 4: walking animation
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

    // ── 每帧推进射击动画，播完最后一帧后自动结束 ──
    void HandleShootAnimation()
    {
        if (shootFrames == null || shootFrames.Length == 0)
        {
            isShooting = false;
            return;
        }

        shootTimer += Time.deltaTime;
        if (shootTimer >= 1f / shootFrameRate)
        {
            shootTimer = 0f;
            currentShootFrame++;

            if (currentShootFrame >= shootFrames.Length)
            {
                // 动画播完，退出射击状态
                isShooting = false;
                currentShootFrame = 0;
                return;
            }

            sr.sprite = shootFrames[currentShootFrame];
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

    // ── 外部调用：触发一次射击动画 ──
    public void TriggerShoot()
    {
        if (isWalking || isJumping || isUsingJetpack || isDigging) return;

        isShooting = true;
        currentShootFrame = 0;
        shootTimer = 0f;
        sr.sprite = shootFrames[0]; // 立即显示第一帧
    }

    public void SetFacing(float x)
    {
        if (Mathf.Abs(x) > deadzone)
            isWalking = true;

        if (x > deadzone)
            sr.flipX = false;
        else if (x < -deadzone)
            sr.flipX = true;
        else
        {
            isWalking = false;
            if (!isJumping && !isUsingJetpack && !isDigging && !isShooting)
                sr.sprite = rightDavid;
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
        if (usingJetpack && !isUsingJetpack)
        {
            currentJetpackFrame = 0;
            jetpackFrameTimer = 0f;
            hasPlayedJetpackIntro = false;

            if (jetpackFrames != null && jetpackFrames.Length > 0)
                sr.sprite = jetpackFrames[0];
        }

        isUsingJetpack = usingJetpack;
    }

    public void SetDigging(bool digging, bool down)
    {
        isDigging = digging;
        isDiggingDown = down;

        if (digging)
        {
            currentDigFrame = 0;
            digTimer = 0f;

            Sprite[] currentAnim = isDiggingDown ? digDownFrames : digSideFrames;
            if (currentAnim != null && currentAnim.Length > 0)
                sr.sprite = currentAnim[0];
        }
        else
        {
            if (!isJumping && !isUsingJetpack && !isWalking && !isShooting)
                sr.sprite = rightDavid;
        }
    }
}