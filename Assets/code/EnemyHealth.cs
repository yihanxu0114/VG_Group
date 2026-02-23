using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Setting")]
    public int maxHealth = 30;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;     
    private Material whiteFlashMaterial;   

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
            whiteFlashMaterial = new Material(Shader.Find("GUI/Text Shader"));
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " be danmaged£¡get " + damageAmount + " danmage£¡");

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashWhite());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashWhite()
    {
        spriteRenderer.material = whiteFlashMaterial;
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null)
        {
            spriteRenderer.material = originalMaterial;
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died£¡");
        Destroy(gameObject);
    }
}