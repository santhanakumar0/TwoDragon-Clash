using UnityEngine;

public class EnemyDragonAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    private DragonHealth playerHealth, myHealth;
    private PlayerDragonController playerController;
    private Animator playerAnimator;

    [Header("Ground Combat")]
    public float detectionRange = 25f;
    public float attackRange = 6f;
    public float moveSpeed = 4f;
    public float rotationSpeed = 3f;
    public float attackCooldown = 2.5f;
    float lastAttackTime;

    [Header("Attack Chances % - 0 = disable")]
    [Range(0, 100)] public float basicChance = 50f;
    [Range(0, 100)] public float clawChance = 50f;
    [Range(0, 100)] public float flameChance = 30f;

    [Header("Damage")]
    public int basicDamage = 40;
    public int clawDamage = 60;
    public int flameDamage = 10; // LOW per tick for continuous
    public float flameDamageInterval = 0.15f; // CONTINUOUS
    public float flameDuration = 2.5f; // HOW LONG IT FLAMES

    [Header("Low HP Berserk")]
    [Range(0, 100)] public float lowHpPercent = 30f;
    public float lowHpFlyHeight = 12f;
    public float berserkFlameCooldown = 0.7f;
    private bool isBerserk = false;

    [Header("Flight")]
    public float flightHeight = 8f;
    public float flySpeed = 8f;
    public float takeoffLandSpeed = 5f;
    public float flyingChaseDistance = 8f;

    [Header("VFX")]
    public GameObject flameObject;
    public GameObject hitVFX;
    public float hitEffectDuration = 0.6f;
    public float hitEffectSpawnInterval = 0.1f;
    private float nextHitEffectTime;

    private Transform flameMouth;
    private bool flameActive = false;
    private float nextFlameDamageTime;
    private float flameStopTime;

    readonly string isFlyingBool = "isFlying";
    readonly string takeOffTrigger = "TakeOffTrigger";
    readonly string landTrigger = "Land";
    readonly string basicAttackTrigger = "Attack";
    readonly string clawAttackTrigger = "HeavyAttack";
    readonly string flameAttackTrigger = "SpellAttack";

    private Animator animator;
    private CharacterController cc;
    private bool isAttacking;
    private float groundY;

    public enum FlightState { Grounded, TakingOff, Flying, Landing }
    private FlightState flightState = FlightState.Grounded;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
        cc = GetComponent<CharacterController>();
        myHealth = GetComponent<DragonHealth>();
        groundY = transform.position.y;

        if (flameObject != null)
        {
            flameMouth = flameObject.transform.parent;
            flameObject.SetActive(false);
        }
        ForceOffHitVFX();

        if (player == null)
        {
            var all = FindObjectsByType<DragonHealth>();
            foreach (var d in all) if (d.transform != transform) { player = d.transform; playerHealth = d; break; }
        }
        else playerHealth = player.GetComponent<DragonHealth>();

        if (player != null)
        {
            playerController = player.GetComponent<PlayerDragonController>();
            playerAnimator = player.GetComponentInChildren<Animator>();
        }
    }

    void Start()
    {
        if (flameObject != null) flameObject.SetActive(false);
        ForceOffHitVFX();
    }

    void ForceOffHitVFX()
    {
        if (hitVFX == null) return;
        var all = hitVFX.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in all) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        hitVFX.SetActive(false);
    }

    void Update()
    {
        if (myHealth != null && myHealth.IsDead) { StopFlameVFX(); return; }
        if (player == null) return;
        if (playerHealth != null && playerHealth.IsDead) { animator.SetBool("IsWalking", false); StopFlameVFX(); return; }

        CheckLowHpBerserk();
        CheckPlayerFlightSync();

        if (flightState == FlightState.Grounded) HandleGroundAI();
        else HandleFlightAI();

        // CONTINUOUS FLAME DAMAGE
        if (flameActive && Time.time >= nextFlameDamageTime)
        {
            DoFlameDamage();
            nextFlameDamageTime = Time.time + flameDamageInterval;
        }

        if (flameActive && Time.time >= flameStopTime)
        {
            StopFlameVFX();
        }
    }

    void LateUpdate()
    {
        if (!flameActive || flameObject == null || player == null || flameMouth == null) return;
        flameObject.transform.position = flameMouth.position;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 dir = target - flameObject.transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            flameObject.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void CheckLowHpBerserk()
    {
        if (myHealth == null || isBerserk) return;
        float hpPercent = (float)myHealth.CurrentHealth / myHealth.maxHealth * 100f;
        if (hpPercent <= lowHpPercent)
        {
            isBerserk = true;
            flightState = FlightState.TakingOff;
            animator.SetTrigger(takeOffTrigger);
            flightHeight = lowHpFlyHeight;
        }
    }

    void CheckPlayerFlightSync()
    {
        if (isBerserk || player == null) return;
        bool playerIsFlying = false;
        if (playerController != null) playerIsFlying = playerController.IsCurrentlyFlying();
        else if (playerAnimator != null)
        {
            try { playerIsFlying = playerAnimator.GetBool(isFlyingBool); } catch { }
            if (player.position.y > groundY + 3f) playerIsFlying = true;
        }
        if (playerIsFlying && flightState == FlightState.Grounded)
        {
            flightState = FlightState.TakingOff;
            animator.SetTrigger(takeOffTrigger);
            StopFlameVFX();
        }
        else if (!playerIsFlying && flightState == FlightState.Flying)
        {
            flightState = FlightState.Landing;
            animator.SetTrigger(landTrigger);
            StopFlameVFX();
        }
    }

    void HandleGroundAI()
    {
        Vector3 dir = player.position - transform.position; dir.y = 0;
        float dist = dir.magnitude;
        if (dir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);

        if (dist > attackRange)
        {
            if (dist <= detectionRange && !isAttacking)
            {
                animator.SetBool("IsWalking", true);
                cc.Move(transform.forward * moveSpeed * Time.deltaTime);
            }
            else animator.SetBool("IsWalking", false);
        }
        else
        {
            animator.SetBool("IsWalking", false);
            TryGroundAttack();
        }
    }

    void HandleFlightAI()
    {
        animator.SetBool(isFlyingBool, true);
        if (flightState == FlightState.TakingOff)
        {
            float newY = Mathf.MoveTowards(transform.position.y, groundY + flightHeight, takeoffLandSpeed * Time.deltaTime);
            cc.Move(new Vector3(0f, newY - transform.position.y, 0f));
            if (Mathf.Abs(newY - (groundY + flightHeight)) < 0.2f) { flightState = FlightState.Flying; animator.SetBool(isFlyingBool, true); }
            return;
        }
        if (flightState == FlightState.Landing)
        {
            float newY = Mathf.MoveTowards(transform.position.y, groundY, takeoffLandSpeed * Time.deltaTime);
            cc.Move(new Vector3(0f, newY - transform.position.y, 0f));
            if (Mathf.Abs(newY - groundY) < 0.2f) { flightState = FlightState.Grounded; animator.SetBool(isFlyingBool, false); StopFlameVFX(); }
            return;
        }

        float lockedY = groundY + flightHeight;
        if (Mathf.Abs(transform.position.y - lockedY) > 0.5f) cc.Move(new Vector3(0f, (lockedY - transform.position.y) * Time.deltaTime * 3f, 0f));

        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;
        Vector3 dirXZ = new Vector3(dir.x, 0, dir.z);
        if (dirXZ != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirXZ), rotationSpeed * Time.deltaTime);

        if (dist > flyingChaseDistance) { cc.Move(transform.forward * flySpeed * Time.deltaTime); animator.SetBool("IsWalking", true); }
        else { animator.SetBool("IsWalking", false); TryFlyingAttack(); }
    }

    int GetRandomAttackType()
    {
        float total = basicChance + clawChance + flameChance;
        if (total <= 0) return -1;
        float r = Random.Range(0, total);
        if (r < basicChance) return 0;
        r -= basicChance;
        if (r < clawChance) return 1;
        return 2;
    }

    void TryGroundAttack()
    {
        float cd = isBerserk ? berserkFlameCooldown : attackCooldown;
        if (Time.time - lastAttackTime < cd || isAttacking) return;
        int type = GetRandomAttackType();
        if (type == -1) return;
        if (isBerserk) type = 2;

        lastAttackTime = Time.time;
        isAttacking = true;

        if (type == 0) { animator.SetTrigger(basicAttackTrigger); Invoke(nameof(DoBasicDamage), 0.35f); Invoke(nameof(EndAttack), 1.5f); }
        else if (type == 1) { animator.SetTrigger(clawAttackTrigger); Invoke(nameof(DoClawDamage), 0.45f); Invoke(nameof(EndAttack), 1.5f); }
        else { animator.SetTrigger(flameAttackTrigger); PlayFlameVFX(); Invoke(nameof(EndAttack), isBerserk ? 0.8f : flameDuration + 0.5f); }
    }

    void TryFlyingAttack()
    {
        float cd = isBerserk ? berserkFlameCooldown : attackCooldown;
        if (Time.time - lastAttackTime < cd || isAttacking) return;
        int type = GetRandomAttackType();
        if (type == -1) return;
        if (isBerserk) type = 2;

        lastAttackTime = Time.time;
        isAttacking = true;

        if (type == 0) { animator.SetTrigger(basicAttackTrigger); Invoke(nameof(DoBasicDamage), 0.4f); Invoke(nameof(EndAttack), 1.8f); }
        else if (type == 1) { animator.SetTrigger(clawAttackTrigger); Invoke(nameof(DoClawDamage), 0.5f); Invoke(nameof(EndAttack), 1.8f); }
        else { animator.SetTrigger(flameAttackTrigger); PlayFlameVFX(); Invoke(nameof(EndAttack), isBerserk ? 0.8f : flameDuration + 0.5f); }
    }

    void EndAttack() => isAttacking = false;
    void DoBasicDamage() => DoDamage(basicDamage);
    void DoClawDamage() => DoDamage(clawDamage);
    void DoFlameDamage() => DoDamage(flameDamage);

    void DoDamage(int dmg)
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.position) <= attackRange + 12f)
        {
            playerHealth?.TakeDamage(dmg);
            player.GetComponent<PlayerDragonController>()?.PlayHitVFXAt(transform);
        }
    }

    public void PlayHitVFXAt(Transform attacker)
    {
        if (hitVFX == null || Time.time < nextHitEffectTime) return;
        nextHitEffectTime = Time.time + hitEffectSpawnInterval;
        hitVFX.transform.localPosition = new Vector3(0f, 1.2f, 1f);
        hitVFX.transform.localRotation = Quaternion.identity;
        hitVFX.SetActive(true);
        var all = hitVFX.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in all) { ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); ps.Play(true); }
        CancelInvoke(nameof(ForceOffHitVFX));
        Invoke(nameof(ForceOffHitVFX), hitEffectDuration);
    }

    public void PlayFlameVFX()
    {
        if (flameObject == null) return;
        flameActive = true;
        flameStopTime = Time.time + flameDuration;
        nextFlameDamageTime = Time.time + 0.2f; // damage starts instantly after 0.2s
        CancelInvoke(nameof(OffFlame));
        flameObject.SetActive(true);
        var all = flameObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in all)
        {
            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.loop = true;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
    }

    public void StopFlameVFX()
    {
        if (flameObject == null) return;
        flameActive = false;
        var all = flameObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in all) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        CancelInvoke(nameof(OffFlame));
        Invoke(nameof(OffFlame), 0.5f);
    }

    void OffFlame()
    {
        flameActive = false;
        if (flameObject != null) flameObject.SetActive(false);
    }

    public bool IsCurrentlyFlying() => flightState == FlightState.Flying || flightState == FlightState.TakingOff;
}