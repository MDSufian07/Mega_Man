using System.Collections;
using UnityEngine;

namespace Combat.Boss
{
    public class StoneBoss : MonoBehaviour
    {
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
        public float moveForce = 4f;

        [Header("Timing")]
        public float introDuration = 2f;
        public float idleDelay = 1f;

        [Header("Environment Shake")]
        public Transform environmentToShake;
        public float shakeDuration = 1f;
        public float shakeAmount = 0.2f;

        // Components
        private Rigidbody2D rb;
        private Animator anim;

        // State
        private bool isGrounded;
        private bool wasGrounded;
        private bool isJumping;

        // FIX: Ignore early ground detection
        private float jumpIgnoreTime = 0.2f;
        private float jumpTimer = 0f;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

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

            // Reduce ignore timer
            if (jumpTimer > 0)
                jumpTimer -= Time.deltaTime;

            HandleLanding();
        }

        // ================= MAIN LOOP =================

        IEnumerator MainLoop()
        {
            anim.Play("Idle");
            yield return new WaitForSeconds(introDuration);

            while (true)
            {
                anim.Play("Idle");
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
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundRadius,
                groundLayer
            );
        }

        // ================= LAND DETECTION =================

        void HandleLanding()
        {
            // Ignore ground right after jump
            if (jumpTimer > 0)
            {
                wasGrounded = isGrounded;
                return;
            }

            // Detect: air → ground AND falling
            if (!wasGrounded && isGrounded && isJumping && rb.linearVelocity.y <= 0)
            {
                OnLand();
            }

            wasGrounded = isGrounded;
        }

        void OnLand()
        {
            StartCoroutine(ShakeEnvironment());
            StartCoroutine(DisablePlayer());

            isJumping = false;
        }

        // ================= JUMP =================

        IEnumerator JumpRoutine()
        {
            if (!isGrounded) yield break;

            isJumping = true;

            anim.SetTrigger("Jump");

            yield return new WaitForSeconds(0.2f);

            float dir = (player.position.x > transform.position.x) ? 1f : -1f;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);

            // Start ignore timer
            jumpTimer = jumpIgnoreTime;
        }

        // ================= THROW =================

        IEnumerator ThrowRoutine()
        {
            anim.SetTrigger("Throw");

            yield return new WaitForSeconds(1.5f);

            Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);
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

            if (move != null) move.enabled = false;
            if (shoot != null) shoot.enabled = false;

            yield return new WaitForSeconds(1f);

            if (move != null) move.enabled = true;
            if (shoot != null) shoot.enabled = true;
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