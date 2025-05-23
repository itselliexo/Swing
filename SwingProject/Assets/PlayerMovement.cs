using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Buttons")]
   /* [SerializeField] private char forwardButton = 'W';
    [SerializeField] private char leftButton = 'A';
    [SerializeField] private char rightButton = 'D';
    [SerializeField] private char backButton = 'S'; */
    //[SerializeField] private int swingButton = 1;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float airControlMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float damping;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector3 moveDirection;

    public Transform orientation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = orientation.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = orientation.right;
        right.y = 0f;
        right.Normalize();

        moveDirection = forward * vertical + right * horizontal;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        AdjustDrag();
        if (isGrounded)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / moveSpeed + 1);

        float accelerationMultiplier = 1.1f - speedRatio;
        Vector3 forceToAdd = moveDirection * moveSpeed * 10f * accelerationMultiplier;

        rb.AddForce(forceToAdd, ForceMode.Force);
    }

    private void Jump()
    {
        float newJumpForce = jumpForce;
        float currentSpeed = rb.linearVelocity.magnitude;
        newJumpForce = jumpForce + Mathf.Log(currentSpeed + 1) * 2f;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * newJumpForce, ForceMode.Impulse);
    }

    private void AdjustDrag()
    {
        Vector3 velocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed > 0.1f && isGrounded)
        {
            float dampingStrength = Mathf.Lerp(0f, damping, speed / moveSpeed);

            Vector3 dampingForce = -horizontalVelocity.normalized * dampingStrength;

            rb.AddForce(dampingForce, ForceMode.Acceleration);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
