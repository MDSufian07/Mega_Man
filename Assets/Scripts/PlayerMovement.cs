using Combat;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private InputHandler input;

    private PlayerShooting shooting;

    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = GetComponent<InputHandler>();
        
        shooting = GetComponent<PlayerShooting>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (input.JumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        anim.SetBool("isRunning", input.MoveInput != 0);
        anim.SetBool("isJumping", !isGrounded);
        anim.SetBool("isFiring", shooting.CanFire && input.FirePressed);

        if (input.MoveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (input.MoveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(input.MoveInput * moveSpeed, rb.linearVelocity.y);
    }
}