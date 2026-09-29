using UnityEngine;
using UnityEngine.UI;

// Attach this script to each dragon (Dragonblue, DragonRed).
// It manages HP and drives a World Space Slider that sits above/below the dragon.
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthSlider;      // drag the Slider (child Canvas) here
    public GameObject healthBarRoot; // drag the Canvas GameObject here (for hide/show)
    public bool hideWhenFull = false;

    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateBar();
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateBar();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateBar();
    }

    void UpdateBar()
    {
        if (healthSlider != null)
            healthSlider.value = (float)currentHealth / maxHealth;

        if (healthBarRoot != null && hideWhenFull)
            healthBarRoot.SetActive(currentHealth < maxHealth);
    }

    void Die()
    {
        // Hook animation/AI-disable logic here before destroying.
        Destroy(gameObject);
    }
}
