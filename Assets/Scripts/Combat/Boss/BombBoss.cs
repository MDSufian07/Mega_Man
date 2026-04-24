using System.Collections;
using UnityEngine;

namespace Combat.Boss
{
    public class BombBoss : MonoBehaviour
    {
        [Header("References")]
        public Transform player;
        public Transform throwPoint;
        public GameObject bombPrefab;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayer;

        [Header("Jump Settings")]
        public float minJumpForce = 5f;
        public float maxJumpForce = 10f;
        public float minMoveForce = 2f;
        public float maxMoveForce = 5f;

        [Header("Throw Settings")]
        public float minThrowForce = 20f;
        public float maxThrowForce = 30f;
        public float minAngle = 30f;
        public float maxAngle = 70f;

        [Header("Timing")]
        public float introDuration = 2f;
        public float idleDelay = 1f;

        private Rigidbody2D rb;
        private Animator animator;

        private bool isGrounded;
        private bool isJumping = false;
        private bool bombActive = false;

        private Vector3 originalScale;
        private float jumpDirection = 1f;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            originalScale = transform.localScale;

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
            if (isJumping && !isGrounded)
            {
                Vector3 scale = originalScale;
                scale.x = Mathf.Abs(originalScale.x) * jumpDirection;
                transform.localScale = scale;
            }
            // When grounded and not jumping, look at player
            else if (isGrounded && !isJumping)
            {
                LookAtPlayer();
            }
        }

        // ================= MAIN LOOP =================

        IEnumerator MainLoop()
        {
            animator.Play("BossIntro");
            yield return new WaitForSeconds(introDuration);

            while (true)
            {
                animator.Play("Idle");
                yield return new WaitForSeconds(idleDelay);

                // Jump 40%, Throw 60%
                int action = Random.Range(0, 5);

                if ((action == 0 || action == 1) && !bombActive)
                {
                    // Jump (40% chance: 2 out of 5)
                    yield return StartCoroutine(JumpRoutine());
                }
                else if ((action == 2 || action == 3 || action == 4) && isGrounded && !bombActive)
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

            Vector3 scale = originalScale;

            if (player.position.x > transform.position.x)
                scale.x = Mathf.Abs(originalScale.x);
            else
                scale.x = -Mathf.Abs(originalScale.x);

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

            if (groundedNow != isGrounded)
            {
                isGrounded = groundedNow;
                animator.SetBool("isGrounded", isGrounded);
            }
        }

        // ================= JUMP =================

        IEnumerator JumpRoutine()
        {
            if (!isGrounded || bombActive) yield break;

            isJumping = true;

            animator.SetTrigger("Jump");

            yield return new WaitForSeconds(0.2f);

            float jumpForce = Random.Range(minJumpForce, maxJumpForce);
            float moveForce = Random.Range(minMoveForce, maxMoveForce);

            float dir = 1f;

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
            jumpDirection = dir;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);

            // Wait while jumping (not grounded)
            yield return new WaitUntil(() => !isGrounded || isGrounded);
            yield return new WaitUntil(() => isGrounded);

            // slight delay to avoid instant flip glitch
            yield return new WaitForSeconds(0.05f);

            isJumping = false;
        }

        // ================= THROW =================

        IEnumerator ThrowRoutine()
        {
            if (!isGrounded || isJumping || bombActive) yield break;

            animator.SetTrigger("Throw");

            yield return new WaitForSeconds(0.4f);

            GameObject bomb = ThrowBomb();

            if (bomb != null)
            {
                bombActive = true;
                yield return new WaitUntil(() => bomb == null);
            }

            bombActive = false;
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