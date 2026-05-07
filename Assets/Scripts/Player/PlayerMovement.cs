using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int IsFiring = Animator.StringToHash("isFiring");
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D _rb;
        private Animator _anim;
        private InputHandler _input;

        private PlayerShooting _shooting;

        private bool _isGrounded;

        public bool IsGrounded => _isGrounded;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _input = GetComponent<InputHandler>();
        
            _shooting = GetComponent<PlayerShooting>();
        }

        void Update()
        {
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

            if (_input.JumpPressed && _isGrounded)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            }

            _anim.SetBool(IsRunning, _input.MoveInput != 0);
            _anim.SetBool(IsJumping, !_isGrounded);
            _anim.SetBool(IsFiring, _shooting.CanFire && _input.FirePressed);

            if (_input.MoveInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (_input.MoveInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        void FixedUpdate()
        {
            _rb.linearVelocity = new Vector2(_input.MoveInput * moveSpeed, _rb.linearVelocity.y);
        }
    }
}