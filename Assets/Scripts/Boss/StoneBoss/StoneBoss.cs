using System.Collections;
using Player;
using UnityEngine;

namespace Boss.StoneBoss
{
    public class StoneBoss : MonoBehaviour
    {
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Throw = Animator.StringToHash("Throw");
        private static readonly int Fall = Animator.StringToHash("Fall");

        [Header("References")]
        public Transform player;
        public Transform throwPoint;
        public GameObject stonePrefab;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundRadius = 0.2f;
        public LayerMask groundLayer;

        [Header("Jump Settings")]
        public float jumpForce = 8f;
        public float maxMoveForce = 4f;
        public float minMoveForce = -1f;
        

        [Header("Throw Settings")]
        public float throwForce = 15f;
        public float throwUpForce = 5f;

        [Header("Timing")]
        public float introDuration = 2f;
        public float idleDelay = 1f;

        [Header("Environment Shake")]
        public Transform environmentToShake;
        public float shakeDuration = 1f;
        public float shakeAmount = 0.2f;

        private Rigidbody2D _rb;
        private Animator _anim;

        private bool _isGrounded;
        private bool _wasGrounded;
        private bool _isJumping;

        // Fix for fake landing
        private readonly float _jumpIgnoreTime = 0.2f;
        private float _jumpTimer;
        
        private Vector3 _originalScale;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _originalScale = transform.localScale;

            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            StartCoroutine(MainLoop());
        }

        void Update()
        {
            CheckGround();

            if (_jumpTimer > 0)
                _jumpTimer -= Time.deltaTime;

            HandleLanding();
            
            // Always look at player when grounded
            if (_isGrounded)
                LookAtPlayer();
        }

        // ================= MAIN LOOP =================

        IEnumerator MainLoop()
        {
            _anim.Play("Idle");
            yield return new WaitForSeconds(introDuration);

            while (true)
            {
                _anim.Play("Idle");
                yield return new WaitForSeconds(idleDelay);

                int action = Random.Range(0, 2);

                if (action == 0)
                    yield return StartCoroutine(JumpRoutine());
                else
                    yield return StartCoroutine(ThrowRoutine());
            }
        }

        // ================= GROUND =================

        void CheckGround()
        {
            _isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundRadius,
                groundLayer
            );
        }

        // ================= LOOK =================

        void LookAtPlayer()
        {
            if (player == null) return;

            Vector3 scale = _originalScale;

            if (player.position.x > transform.position.x)
                scale.x = Mathf.Abs(_originalScale.x);
            else
                scale.x = -Mathf.Abs(_originalScale.x);

            transform.localScale = scale;
        }

        // ================= LAND DETECTION =================

        void HandleLanding()
        {
            if (_jumpTimer > 0)
            {
                _wasGrounded = _isGrounded;
                return;
            }

            if (!_wasGrounded && _isGrounded && _isJumping && _rb.linearVelocity.y <= 0)
            {
                OnLand();
            }

            _wasGrounded = _isGrounded;
        }

        void OnLand()
        {
            StartCoroutine(ShakeEnvironment());
            StartCoroutine(DisablePlayer());

            _isJumping = false;
        }

        // ================= JUMP =================

        IEnumerator JumpRoutine()
        {
            if (!_isGrounded) yield break;

            _isJumping = true;

            _anim.SetTrigger(Jump);

            yield return new WaitForSeconds(0.2f);

            float dir = (player.position.x > transform.position.x) ? 1f : -1f;
            
            // Random moveForce between -1 and 3
            float randomMoveForce = Random.Range(minMoveForce, maxMoveForce);

            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(new Vector2(dir * randomMoveForce, jumpForce), ForceMode2D.Impulse);

            // Ignore early ground detection
            _jumpTimer = _jumpIgnoreTime;
        }

        // ================= THROW =================

        IEnumerator ThrowRoutine()
        {
            _anim.SetTrigger(Throw);

            // Wait until animation event calls SpawnStone()
            yield return null;
        }
        
        //================== SpawnStone=============
        public void SpawnStone()
        {
            GameObject stone = Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);

            Rigidbody2D srb = stone.GetComponent<Rigidbody2D>();

            if (srb != null && player != null)
            {
                Vector2 dir = (player.position - throwPoint.position).normalized;

                // Add arc
                dir.y += throwUpForce / throwForce;

                srb.linearVelocity = Vector2.zero;
                srb.AddForce(dir * throwForce, ForceMode2D.Impulse);
            }
        }
        // ================= SHAKE =================

        IEnumerator ShakeEnvironment()
        {
            Vector3 originalPos = environmentToShake.position;
            float time = shakeDuration;

            while (time > 0)
            {
                environmentToShake.position =
                    originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;

                time -= Time.deltaTime;
                yield return null;
            }

            environmentToShake.position = originalPos;
        }

        // ================= PLAYER DISABLE =================

        IEnumerator DisablePlayer()
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            var move = p.GetComponent<PlayerMovement>();
            var shoot = p.GetComponent<PlayerShooting>();
            var playerAnim = p.GetComponent<Animator>();

            // Only disable movement and shooting if player is grounded
            if (move != null && move.IsGrounded)
            {
                move.enabled = false;
                if (shoot != null) shoot.enabled = false;

                // Trigger fall animation when boss lands
                if (playerAnim != null)
                {
                    playerAnim.SetTrigger(Fall);
                }

                // Disable player only for the duration of the shake
                yield return new WaitForSeconds(shakeDuration);

                move.enabled = true;
                if (shoot != null) shoot.enabled = true;
            }
        }

        // ================= DEBUG =================

        void OnDrawGizmos()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
            }
        }
    }
}