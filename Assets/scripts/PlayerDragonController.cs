using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(DragonHealth))]
public class PlayerDragonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    public float rotationSpeed = 4f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.15f;
    public float moveSmoothTime = 0.2f;

    [Header("Mobile Input")]
    public VirtualJoystick joystick;

    [Header("Combat")]
    public Transform attackPoint;
    public float attackRange = 4f;
    public LayerMask enemyLayer;
    public int basicDamage = 30;
    public int clawDamage = 50;
    public int flameAttackDamage = 20;
    public float damageInterval = 0.15f;

    [Header("Heal - H Key")]
    public int healAmount = 30;
    public float healCooldown = 8f;
    public Image healCooldownImage;
    private float nextHealTime;

    [Header("Cooldown")]
    public float basicCooldown = 2f;
    public float heavyCooldown = 4f;
    public float flameCooldown = 6f;
    public float flyCooldown = 3f;
    public Image basicCooldownImage;
    public Image heavyCooldownImage;
    public Image flameCooldownImage;
    public Image flyCooldownImage;

    private float nextBasicTime;
    private float nextHeavyTime;
    private float nextFlameTime;
    private float nextFlyTime;

    [Header("Flame Auto-Lock")]
    public float flameLockRange = 25f;
    public float flameLockAngle = 60f;
    public float lockRotationSpeed = 8f;

    [Header("Mobile Flame")]
    public float holdThreshold = 0.2f;
    public float flameDelay = 0.55f;

    [Header("Flight")]
    public float flightHeight = 0.5f;
    public float flySpeed = 5f;
    public float takeoffLandSpeed = 2f;
    public KeyCode toggleFlightKey = KeyCode.Space;

    [Header("Input")]
    public KeyCode flameAttackKey = KeyCode.J;
    public KeyCode basicAttackKey = KeyCode.F;
    public KeyCode heavyAttackKey = KeyCode.G;
    public KeyCode healKey = KeyCode.H;

    [Header("Animator")]
    public string isWalkingParam = "IsWalking";
    public string isRunningParam = "IsRunning";
    public string basicAttackTrigger = "Attack";
    public string heavyAttackTrigger = "HeavyAttack";
    public string flameAttackTrigger = "SpellAttack";
    public string defendBool = "Defend";
    public string takeOffTrigger = "TakeOffTrigger";
    public string isFlyingBool = "isFlying";
    public string landTrigger = "Land";

    [Header("VFX")]
    public GameObject flameObject;
    public GameObject hitVFX;
    public float hitEffectDuration = 0.6f;
    public float hitEffectSpawnInterval = 0.1f;
    private float nextHitEffectTime;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private DragonHealth health;
    private int pendingAttackDamage;
    private Transform currentFlameTarget;
    private Quaternion originalAttackPointRot;
    private float nextFlameDamageTime;
    private float turnSmoothVelocity;
    private Vector3 currentVelRef;
    private Vector3 moveVelSmooth;
    private bool isFlameButtonHeld = false;
    private bool isFlameActive = false;
    private float holdTimer = 0f;

    public enum FlightState { Grounded, TakingOff, Flying, Landing }
    private FlightState flightState = FlightState.Grounded;
    private float groundY;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<DragonHealth>();
        groundY = transform.position.y;
        if (attackPoint != null) originalAttackPointRot = attackPoint.localRotation;
        if (flameObject != null) flameObject.SetActive(false);
        ForceOffHitVFX();
    }

    void Start()
    {
        if (flameObject != null) flameObject.SetActive(false);
        ForceOffHitVFX();
        if (basicCooldownImage) basicCooldownImage.fillAmount = 0;
        if (heavyCooldownImage) heavyCooldownImage.fillAmount = 0;
        if (flameCooldownImage) flameCooldownImage.fillAmount = 0;
        if (flyCooldownImage) flyCooldownImage.fillAmount = 0;
        if (healCooldownImage) healCooldownImage.fillAmount = 0;
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
        if (health != null && health.IsDead) return;
        HandleFlightInput();
        if (flightState == FlightState.Grounded) HandleGroundMovement();
        else HandleFlightMovement();
        HandleCombatInput();
        HandleFlameLock();
        HandleMobileFlameHold();
        UpdateCooldownUI();
    }

    void UpdateCooldownUI()
    {
        if (basicCooldownImage) basicCooldownImage.fillAmount = Time.time < nextBasicTime ? (nextBasicTime - Time.time) / basicCooldown : 0;
        if (heavyCooldownImage) heavyCooldownImage.fillAmount = Time.time < nextHeavyTime ? (nextHeavyTime - Time.time) / heavyCooldown : 0;
        if (flameCooldownImage) flameCooldownImage.fillAmount = Time.time < nextFlameTime ? (nextFlameTime - Time.time) / flameCooldown : 0;
        if (flyCooldownImage) flyCooldownImage.fillAmount = Time.time < nextFlyTime ? (nextFlyTime - Time.time) / flyCooldown : 0;
        if (healCooldownImage) healCooldownImage.fillAmount = Time.time < nextHealTime ? (nextHealTime - Time.time) / healCooldown : 0;
    }

    bool CanBasic() => Time.time >= nextBasicTime;
    bool CanHeavy() => Time.time >= nextHeavyTime;
    bool CanFlame() => Time.time >= nextFlameTime;
    bool CanFly() => Time.time >= nextFlyTime;
    bool CanHeal() => Time.time >= nextHealTime;

    public void OnFlamePointerDown()
    {
        if (!CanFlame()) return;
        animator.ResetTrigger("GetHit");
        isFlameButtonHeld = true;
        holdTimer = 0f;
        pendingAttackDamage = flameAttackDamage;
        currentFlameTarget = FindClosestEnemyInFront();
        animator.SetTrigger(flameAttackTrigger);
        health.PlayAttackSound(KeyCode.J);
    }

    public void OnFlamePointerUp()
    {
        if (isFlameActive) nextFlameTime = Time.time + flameCooldown;
        isFlameButtonHeld = false;
        isFlameActive = false;
        holdTimer = 0f;
        StopFlameVFX();
        currentFlameTarget = null;
        if (attackPoint != null) attackPoint.localRotation = originalAttackPointRot;
    }

    public void OnFlameButtonDown() => OnFlamePointerDown();
    public void OnFlameButtonUp() => OnFlamePointerUp();

    public void OnFlightButton()
    {
        if (!CanFly()) return;
        if (flightState == FlightState.Grounded)
        {
            groundY = transform.position.y;
            flightState = FlightState.TakingOff;
            animator.SetTrigger(takeOffTrigger);
            nextFlyTime = Time.time + flyCooldown;
        }
        else if (flightState == FlightState.Flying)
        {
            flightState = FlightState.Landing;
            animator.SetTrigger(landTrigger);
            nextFlyTime = Time.time + flyCooldown;
        }
    }

    // HEAL BUTTON - H
    public void OnHealButton()
    {
        if (!CanHeal()) return;
        if (health != null)
        {
            health.Heal(healAmount);
            nextHealTime = Time.time + healCooldown;
        }
    }

    void HandleMobileFlameHold()
    {
        if (!isFlameButtonHeld) return;
        holdTimer += Time.deltaTime;
        if (holdTimer >= flameDelay && !isFlameActive) { isFlameActive = true; PlayFlameVFX(); }
        if (isFlameActive && Time.time >= nextFlameDamageTime)
        {
            if (currentFlameTarget == null) currentFlameTarget = FindClosestEnemyInFront();
            DealDamage(pendingAttackDamage);
            nextFlameDamageTime = Time.time + damageInterval;
        }
    }

    void HandleFlameLock()
    {
        if (attackPoint == null || currentFlameTarget == null || !isFlameActive) return;
        var dh = currentFlameTarget.GetComponent<DragonHealth>();
        if (dh == null || dh.IsDead) { currentFlameTarget = null; return; }
        Vector3 dir = currentFlameTarget.position - attackPoint.position;
        if (dir != Vector3.zero) attackPoint.rotation = Quaternion.Slerp(attackPoint.rotation, Quaternion.LookRotation(dir), lockRotationSpeed * Time.deltaTime);
    }

    Transform FindClosestEnemyInFront()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, flameLockRange, enemyLayer.value == 0 ? ~0 : enemyLayer);
        Transform best = null; float bestDist = Mathf.Infinity;
        foreach (var hit in hits)
        {
            var th = hit.GetComponentInParent<DragonHealth>();
            if (th == null) th = hit.transform.root.GetComponent<DragonHealth>();
            if (th == null || th.IsDead || th.gameObject == gameObject) continue;
            Vector3 dir = hit.transform.position - transform.position;
            if (Vector3.Angle(transform.forward, dir) > flameLockAngle) continue;
            float d = dir.magnitude;
            if (d < bestDist) { bestDist = d; best = hit.transform; }
        }
        return best;
    }

    void HandleFlightInput() { if (!Input.GetKeyDown(toggleFlightKey)) return; OnFlightButton(); }

    void HandleFlightMovement()
    {
        velocity.y = 0f;
        animator.SetBool(isFlyingBool, true);
        float joyH = joystick != null ? joystick.Horizontal : 0f;
        float joyV = joystick != null ? joystick.Vertical : 0f;
        float h = Input.GetAxis("Horizontal") + joyH;
        float v = Input.GetAxis("Vertical") + joyV;
        Vector3 move = new Vector3(h, 0, v);
        if (flightState == FlightState.TakingOff)
        {
            float newY = Mathf.MoveTowards(transform.position.y, groundY + flightHeight, takeoffLandSpeed * Time.deltaTime);
            controller.Move(new Vector3(0f, newY - transform.position.y, 0f));
            if (Mathf.Abs(newY - (groundY + flightHeight)) < 0.1f) { flightState = FlightState.Flying; animator.SetBool(isFlyingBool, true); }
            return;
        }
        if (flightState == FlightState.Landing)
        {
            float newY = Mathf.MoveTowards(transform.position.y, groundY, takeoffLandSpeed * Time.deltaTime);
            controller.Move(new Vector3(0f, newY - transform.position.y, 0f));
            if (Mathf.Abs(newY - groundY) < 0.1f) { flightState = FlightState.Grounded; animator.SetBool(isFlyingBool, false); }
            return;
        }
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), rotationSpeed * Time.deltaTime);
            Vector3 moveXZ = new Vector3(move.normalized.x, 0, move.normalized.z);
            controller.Move(moveXZ * flySpeed * Time.deltaTime);
        }
        float lockedY = groundY + flightHeight;
        if (transform.position.y < lockedY - 0.5f) controller.Move(new Vector3(0f, lockedY - transform.position.y, 0f));
    }

    void HandleGroundMovement()
    {
        float joyH = joystick != null ? joystick.Horizontal : 0f;
        float joyV = joystick != null ? joystick.Vertical : 0f;
        float h = Input.GetAxis("Horizontal") + joyH;
        float v = Input.GetAxis("Vertical") + joyV;
        Vector3 input = new Vector3(h, 0, v);
        bool isMoving = input.magnitude > 0.1f;
        Vector3 targetMove = isMoving ? input.normalized : Vector3.zero;
        moveVelSmooth = Vector3.SmoothDamp(moveVelSmooth, targetMove, ref currentVelRef, moveSmoothTime);
        float joyMagnitude = new Vector2(joyH, joyV).magnitude;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || joyMagnitude > 0.8f;
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;
        float currentSpeed = moveVelSmooth.magnitude * targetSpeed;
        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            controller.Move(moveVelSmooth * currentSpeed * Time.deltaTime);
        }
        animator.SetBool(isWalkingParam, isMoving && !isSprinting);
        animator.SetBool(isRunningParam, isMoving && isSprinting);
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCombatInput()
    {
        if (Input.GetKeyDown(basicAttackKey) && CanBasic()) OnAttackButton();
        if (Input.GetKeyDown(heavyAttackKey) && CanHeavy()) OnHeavyAttackButton();
        if (Input.GetKeyDown(flameAttackKey) && CanFlame()) OnFlameButtonDown();
        if (Input.GetKeyUp(flameAttackKey)) OnFlameButtonUp();
        if (Input.GetKeyDown(healKey) && CanHeal()) OnHealButton(); // H = HEAL
        animator.SetBool(defendBool, Input.GetMouseButton(1));
    }

    void DoBasicDamage() { DealDamage(basicDamage); }
    void DoClawDamage() { DealDamage(clawDamage); }

    public void DealDamage(int dmg)
    {
        Vector3 center = attackPoint != null ? attackPoint.position : transform.position + transform.forward * 2.5f + Vector3.up * 1f;
        LayerMask mask = enemyLayer.value == 0 ? ~0 : enemyLayer;
        Collider[] hits = Physics.OverlapSphere(center, attackRange, mask);
        foreach (var hit in hits)
        {
            if (hit.transform.root == transform.root) continue;
            var th = hit.GetComponentInParent<DragonHealth>();
            if (th == null) th = hit.transform.root.GetComponent<DragonHealth>();
            if (th != null && !th.IsDead) { th.TakeDamage(dmg); SpawnHitOnEnemy(hit.transform); }
        }
    }

    void SpawnHitOnEnemy(Transform enemy)
    {
        if (hitVFX == null) return;
        if (isFlameActive && Time.time < nextHitEffectTime) return;
        nextHitEffectTime = Time.time + hitEffectSpawnInterval;
        DragonHealth dh = enemy.GetComponentInParent<DragonHealth>();
        if (dh != null)
        {
            var enemyAI = dh.GetComponent<EnemyDragonAI>();
            if (enemyAI != null && enemyAI.hitVFX != null) { enemyAI.PlayHitVFXAt(transform); return; }
        }
        PlayHitVFXAt(enemy);
    }

    public void PlayHitVFXAt(Transform attacker)
    {
        if (hitVFX == null) return;
        if (Time.time < nextHitEffectTime) return;
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
        flameObject.SetActive(true);
        var allPs = flameObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in allPs) { ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); ps.Play(true); }
    }

    public void StopFlameVFX()
    {
        if (flameObject == null) return;
        var allPs = flameObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in allPs) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        flameObject.SetActive(false);
    }

    public bool IsCurrentlyFlying() { return flightState != FlightState.Grounded; }
    public bool IsFlameAttacking() { return isFlameButtonHeld || isFlameActive; }

    public void OnAttackButton()
    {
        if (health != null && health.IsDead) return;
        if (!CanBasic()) return;
        animator.ResetTrigger("GetHit");
        animator.SetTrigger(basicAttackTrigger);
        health.PlayAttackSound(KeyCode.F);
        CancelInvoke(nameof(DoBasicDamage));
        Invoke(nameof(DoBasicDamage), 0.35f);
        nextBasicTime = Time.time + basicCooldown;
    }

    public void OnHeavyAttackButton()
    {
        if (health != null && health.IsDead) return;
        if (!CanHeavy()) return;
        animator.ResetTrigger("GetHit");
        animator.SetTrigger(heavyAttackTrigger);
        health.PlayAttackSound(KeyCode.G);
        CancelInvoke(nameof(DoClawDamage));
        Invoke(nameof(DoClawDamage), 0.45f);
        nextHeavyTime = Time.time + heavyCooldown;
    }
}