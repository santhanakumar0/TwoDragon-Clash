using UnityEngine;
using System;

public class DragonHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("Animator")]
    public string getHitTrigger = "GetHit";
    public string dieTrigger = "Die";

    [Header("Anti-Spam")]
    public float hitCooldown = 0.6f;
    private float lastHitTime = -10f;

    [Header("Sounds")]
    public AudioClip hitClip;
    public AudioClip dieClip;
    public AudioClip healClip;
    public AudioClip attackF;
    public AudioClip attackG;
    public AudioClip attackJ;

    private AudioSource audioSource;
    public event Action OnDeath;
    private Animator animator;
    private bool isDead;
    private PlayerDragonController playerCtrl;
    private EnemyDragonAI enemyCtrl;

    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerCtrl = GetComponent<PlayerDragonController>();
        enemyCtrl = GetComponent<EnemyDragonAI>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (hitClip) audioSource.PlayOneShot(hitClip);
        if (currentHealth <= 0) Die();
        else
        {
            if (IsFlying()) return;
            if (playerCtrl != null && playerCtrl.IsFlameAttacking()) return;
            if (animator != null) { animator.ResetTrigger(getHitTrigger); animator.SetTrigger(getHitTrigger); }
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healClip) audioSource.PlayOneShot(healClip);
        Debug.Log(gameObject.name + " HEALED +" + amount + " HP: " + currentHealth);
    }

    public void PlayAttackSound(KeyCode key)
    {
        if (isDead) return;
        if (key == KeyCode.F && attackF) audioSource.PlayOneShot(attackF);
        if (key == KeyCode.G && attackG) audioSource.PlayOneShot(attackG);
        if (key == KeyCode.J && attackJ) audioSource.PlayOneShot(attackJ);
    }

    bool IsFlying()
    {
        if (playerCtrl != null) { try { return playerCtrl.IsCurrentlyFlying(); } catch { } }
        if (enemyCtrl != null) { try { return enemyCtrl.IsCurrentlyFlying(); } catch { } }
        if (animator != null)
        {
            try { if (animator.GetBool("isFlying")) return true; if (animator.GetBool("IsFlying")) return true; } catch { }
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Fly") || state.IsName("Flying") || state.IsName("TakeOff") || state.IsName("Flight")) return true;
        }
        if (transform.position.y > 3f) return true;
        return false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        currentHealth = 0;
        if (dieClip) audioSource.PlayOneShot(dieClip);
        if (playerCtrl != null) playerCtrl.StopFlameVFX();
        if (enemyCtrl != null) enemyCtrl.StopFlameVFX();
        var all = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in all) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (playerCtrl != null && playerCtrl.flameObject != null) playerCtrl.flameObject.SetActive(false);
        if (enemyCtrl != null && enemyCtrl.flameObject != null) enemyCtrl.flameObject.SetActive(false);
        if (animator != null) animator.SetTrigger(dieTrigger);
        OnDeath?.Invoke();
        if (playerCtrl) playerCtrl.enabled = false;
        if (enemyCtrl) enemyCtrl.enabled = false;
        var cc = GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
    }
}