using System.Collections;
using UnityEngine;

namespace Combat.Boss
{
    public class BombBoss2D : MonoBehaviour
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
        public float minJumpForce = 6f;
        public float maxJumpForce = 10f;
        public float minMoveForce = 2f;
        public float maxMoveForce = 5f;

        [Header("Throw Settings")]
        public float throwForce = 8f;
        public float minAngle = 30f;
        public float maxAngle = 70f;

        [Header("Timing")]
        public float introDuration = 2f;
        public float idleDelay = 1.5f;

        private Rigidbody2D rb;
        private Animator animator;

        private bool isGrounded;
        private bool isJumping = false;
        private bool bombActive = false;

        private Vector3 originalScale;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            originalScale = transform.localScale;

            StartCoroutine(MainLoop());
        }

        void Update()
        {
            CheckGround();
        }

        // ================= MAIN LOOP =================

        IEnumerator MainLoop()
        {
            // INTRO
            animator.Play("BossIntro");
            yield return new WaitForSeconds(introDuration);

            while (true)
            {
                // IDLE
                animator.Play("Idle");
                yield return new WaitForSeconds(idleDelay);

                LookAtPlayer();

                int action = Random.Range(0, 2);

                // Jump only if no bomb
                if (action == 0 && !bombActive)
                {
                    yield return StartCoroutine(JumpRoutine());
                }
                // Throw only if grounded
                else if (action == 1 && isGrounded && !bombActive)
                {
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

        // ================= GROUND CHECK (BEST METHOD) =================

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
            float dir = Random.Range(0, 2) == 0 ? -1f : 1f;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);

            // wait until landed
            yield return new WaitUntil(() => isGrounded);

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

                // wait until bomb destroyed
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
                float angle = Random.Range(minAngle, maxAngle);
                float direction = Mathf.Sign(transform.localScale.x);

                Vector2 dir = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * direction,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                bombRb.AddForce(dir * throwForce, ForceMode2D.Impulse);
            }

            return bomb;
        }

        // ================= DEBUG VISUAL =================

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