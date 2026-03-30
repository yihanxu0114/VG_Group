using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI References")]
    public Slider healthSlider;

    [Header("Visual Feedback")]
    public SpriteRenderer sr;
    private Color originalColor;
    private Coroutine visualCoroutine;

    [Header("Split Settings (只在 Boss 身上勾选)")]
    public bool canSplit = false;              
    public GameObject smallSlimePrefab;       
    public int minSplitCount = 2;             
    public int maxSplitCount = 3;             
    public float splitBurstForce = 5f;        

    private bool isReallyDying = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isReallyDying) return; // 正在播死亡动画时无敌

        currentHealth -= damage;
        
        if (healthSlider != null) healthSlider.value = currentHealth;
        
        if (visualCoroutine != null) StopCoroutine(visualCoroutine);
        visualCoroutine = StartCoroutine(VisualDamage());

        if (currentHealth <= 0)
        {
            StartDeathPerformance();
        }
    }

    void StartDeathPerformance()
    {
        if (isReallyDying) return; 
        isReallyDying = true; 
        
        currentHealth = 0; 
        if (healthSlider != null) healthSlider.value = 0; 
        
        SlimeKingBoss bossAI = GetComponent<SlimeKingBoss>();
        if (bossAI != null && canSplit) 
        {
            bossAI.StartDeathSplitting();
        }
        else
        {
            ExecuteRealDeathAfterAnimation();
        }
    }

    public void ExecuteRealDeathAfterAnimation()
    {
        if (canSplit && smallSlimePrefab != null)
        {
            int numToSpawn = Random.Range(minSplitCount, maxSplitCount + 1); 
            
            for (int i = 0; i < numToSpawn; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.8f, 0.8f), 0.5f, 0f);
                GameObject newSlime = Instantiate(smallSlimePrefab, spawnPos, Quaternion.identity);
                
                Rigidbody2D rb = newSlime.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 burstDir = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                    rb.AddForce(burstDir * splitBurstForce, ForceMode2D.Impulse);
                }
            }
        }

        Destroy(gameObject);
    }

    IEnumerator VisualDamage()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }
}