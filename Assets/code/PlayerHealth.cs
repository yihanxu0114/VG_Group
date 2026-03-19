using UnityEngine;
using UnityEngine.SceneManagement; // restart game
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;     // maximum health
    private int currentHealth;

    public bool isDead = false;

    [Header("UI References")]
    public Image healthFill;

    private moving_logic moveScript;      
    private SpriteRenderer sr;             

    void Start()
    {
        currentHealth = maxHealth;
        moveScript = GetComponent<moving_logic>();
        sr = GetComponent<SpriteRenderer>();
        UpdateHealthUI();
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

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log($"current HP: {currentHealth}");

        UpdateHealthUI();

        if (sr != null) sr.color = Color.red;
        Invoke("ResetColor", 0.2f); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void ResetColor()
    {
        if (!isDead && sr != null) sr.color = Color.white;
    }

    void Die()
    {
        if (isDead) return; 
        isDead = true;
        Debug.Log("Game Over!");

        if (moveScript != null)
        {
            moveScript.enabled = false;
        }

        if (sr != null) sr.color = Color.gray;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0, 10f);
        }

        Invoke("RestartGame", 2.5f);
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
