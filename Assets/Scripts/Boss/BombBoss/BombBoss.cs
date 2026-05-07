using System.Collections;
using UnityEngine;

namespace Boss.BombBoss
{
    public class BombBoss : MonoBehaviour
    {
        private static readonly int Throw = Animator.StringToHash("Throw");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        private static readonly int Jump = Animator.StringToHash("Jump");

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform throwPoint;
        [SerializeField] private GameObject bombPrefab;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Jump Settings")]
        [SerializeField] private float minJumpForce = 5f;
        [SerializeField] private float maxJumpForce = 10f;
        [SerializeField] private float minMoveForce = 2f;
        [SerializeField] private float maxMoveForce = 5f;

        [Header("Throw Settings")]
        [SerializeField] private float minThrowForce = 20f;
        [SerializeField] private float maxThrowForce = 30f;
        [SerializeField] private float minAngle = 30f;
        [SerializeField] private float maxAngle = 70f;

        [Header("Timing")]
        [SerializeField] private float introDuration = 2f;
        [SerializeField] private float idleDelay = 1f;

        private Rigidbody2D _rb;
        private Animator _animator;

        private bool _isGrounded;
        private bool _isJumping;
        private bool _bombActive;

        private Vector3 _originalScale;
        private float _jumpDirection = 1f;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _originalScale = transform.localScale;

            // Auto find player
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

            // While jumping and in air, look at jump direction
            if (_isJumping && !_isGrounded)
            {
                Vector3 scale = _originalScale;
                scale.x = Mathf.Abs(_originalScale.x) * _jumpDirection;
                transform.localScale = scale;
            }
            // When grounded and not jumping, look at player
            else if (_isGrounded && !_isJumping)
            {
                LookAtPlayer();
            }
        }

        // ================= MAIN LOOP =================

        IEnumerator MainLoop()
        {
            _animator.Play("BossIntro");
            yield return new WaitForSeconds(introDuration);

            while (true)
            {
                _animator.Play("Idle");
                yield return new WaitForSeconds(idleDelay);

                // Jump 40%, Throw 60%
                int action = Random.Range(0, 5);

                if ((action == 0 || action == 1) && !_bombActive)
                {
                    // Jump (40% chance: 2 out of 5)
                    yield return StartCoroutine(JumpRoutine());
                }
                else if ((action == 2 || action == 3 || action == 4) && _isGrounded && !_bombActive)
                {
                    // Throw (60% chance: 3 out of 5)
                    yield return StartCoroutine(ThrowRoutine());
                }
            }
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

        // ================= GROUND =================

        void CheckGround()
        {
            bool groundedNow = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

            if (groundedNow != _isGrounded)
            {
                _isGrounded = groundedNow;
                _animator.SetBool(IsGrounded, _isGrounded);
            }
        }

        // ================= JUMP =================

        IEnumerator JumpRoutine()
        {
            if (!_isGrounded || _bombActive) yield break;

            _isJumping = true;

            _animator.SetTrigger(Jump);

            yield return new WaitForSeconds(0.2f);

            float jumpForce = Random.Range(minJumpForce, maxJumpForce);
            float moveForce = Random.Range(minMoveForce, maxMoveForce);

            float dir;

            if (player != null)
            {
                // Always jump towards player
                dir = (player.position.x > transform.position.x) ? 1f : -1f;
            }
            else
            {
                dir = Random.Range(0, 2) == 0 ? -1f : 1f;
            }

            // Store jump direction for Update() to use
            _jumpDirection = dir;

            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);

            // Wait while jumping (not grounded)
            yield return new WaitUntil(() => !_isGrounded || _isGrounded);
            yield return new WaitUntil(() => _isGrounded);

            // slight delay to avoid instant flip glitch
            yield return new WaitForSeconds(0.05f);

            _isJumping = false;
        }

        // ================= THROW =================

        IEnumerator ThrowRoutine()
        {
            if (!_isGrounded || _isJumping || _bombActive) yield break;

            _animator.SetTrigger(Throw);

            yield return new WaitForSeconds(0.4f);

            GameObject bomb = ThrowBomb();

            if (bomb != null)
            {
                _bombActive = true;
                yield return new WaitUntil(() => bomb == null);
            }

            _bombActive = false;
        }

        GameObject ThrowBomb()
        {
            if (bombPrefab == null || throwPoint == null) return null;

            GameObject bomb = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

            if (bombRb != null)
            {
                // Calculate distance to player for aggressive behavior
                float distanceToPlayer = float.MaxValue;
                if (player != null)
                {
                    distanceToPlayer = Vector2.Distance(transform.position, player.position);
                }

                // Normalize distance to 0-1 range
                float maxDistance = 10f;
                float normalizedDistance = Mathf.Clamp01(distanceToPlayer / maxDistance);
                
                // Close = high angle + low force, Far = low angle + high force
                float angle = Mathf.Lerp(maxAngle, minAngle, normalizedDistance);
                float force = Mathf.Lerp(minThrowForce, maxThrowForce, normalizedDistance);
                float dir = Mathf.Sign(transform.localScale.x);

                // Add randomness to angle
                angle += Random.Range(-5f, 5f);

                Vector2 throwDir = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * dir,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                bombRb.AddForce(throwDir * force, ForceMode2D.Impulse);
            }

            return bomb;
        }


        // ================= DEBUG =================

        void OnDrawGizmos()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}