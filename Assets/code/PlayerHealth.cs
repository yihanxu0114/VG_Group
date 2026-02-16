using UnityEngine;
using UnityEngine.SceneManagement; // restart game

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 1;     // maximum health
    private int currentHealth;

    public bool isDead = false;   

    private moving_logic moveScript;      
    private SpriteRenderer sr;             

    void Start()
    {
        currentHealth = maxHealth;
        moveScript = GetComponent<moving_logic>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.anyKeyDown)
            {
                RestartGame();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        Debug.Log($"current HP: {currentHealth}");

        if (sr != null) sr.color = Color.red;
        Invoke("ResetColor", 0.2f); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ResetColor()
    {
        if (!isDead && sr != null) sr.color = Color.white;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Game Over!");

        // 1. ban moving
        if (moveScript != null)
        {
            moveScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // stop now !!!
            moveScript.enabled = false; 
        }

        // 2. 可以在这里播放死亡动画 (如果有)
        // GetComponent<Animator>().SetTrigger("Die");

        if (sr != null) sr.color = Color.gray;
    }

    // restart game
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void OnGUI()
    {
        if (isDead)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 50;
            style.normal.textColor = Color.red;
            style.alignment = TextAnchor.MiddleCenter;

            // GAME OVER
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "GAME OVER", style);

            style.fontSize = 20;
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 20, 200, 50), "Press Any Key to Restart", style);
        }
    }
}
