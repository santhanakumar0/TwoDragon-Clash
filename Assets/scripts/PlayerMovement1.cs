using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float rotationSpeed = 10f;
    public float gravity = 9.8f;

    [Header("Sprint")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Attack")]
    public KeyCode attackKey = KeyCode.F;
    public string attackParameter = "BasicAttack";

    [Header("Animation")]
    public string walkingParameter = "walking";
    public string sprintingParameter = "sprinting";

    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // WASD or Arrow Keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        bool isMoving = moveDirection.magnitude >= 0.1f;

        // Sprint
        bool isSprinting = isMoving && Input.GetKey(sprintKey);

        // Walking / Sprinting animations
        if (animator != null)
        {
            animator.SetBool(walkingParameter, isMoving && !isSprinting);
            animator.SetBool(sprintingParameter, isSprinting);

            // Basic Attack with F
            if (Input.GetKeyDown(attackKey))
            {
                animator.SetTrigger(attackParameter);
            }
        }

        // Movement
        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            controller.Move(
                moveDirection * currentSpeed * Time.deltaTime
            );
        }

        // Gravity
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * gravity * Time.deltaTime);
        }
    }
}