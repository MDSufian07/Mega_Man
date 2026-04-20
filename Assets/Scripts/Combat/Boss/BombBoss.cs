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

        [Header("Jump Settings")]
        public float jumpForce = 8f;
        public float moveForce = 3f;

        [Header("Throw Settings")]
        public float throwForce = 8f;
        public float minAngle = 30f;
        public float maxAngle = 70f;

        [Header("Timing")]
        public float actionDelay = 2.5f;

        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer sr;

        private bool isGrounded = true;
        private bool isBusy = false;

        private Vector3 originalScale;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();

            // store original scale (your 4,4,4)
            originalScale = transform.localScale;

            InvokeRepeating(nameof(BossLoop), 1f, actionDelay);
        }

        void BossLoop()
        {
            if (isBusy) return;

            isBusy = true;

            LookAtPlayer();

            int action = Random.Range(0, 2);

            if (action == 0)
                StartCoroutine(JumpRoutine());
            else
                StartCoroutine(ThrowRoutine());
        }

        // ================= LOOK =================

        void LookAtPlayer()
        {
            if (player == null) return;

            Vector3 scale = originalScale;

            if (player.position.x > transform.position.x)
                scale.x = Mathf.Abs(originalScale.x);   // face right
            else
                scale.x = -Mathf.Abs(originalScale.x);  // face left

            transform.localScale = scale;
        }

        // ================= JUMP =================

        IEnumerator JumpRoutine()
        {
            animator.SetTrigger("Jump");

            yield return new WaitForSeconds(0.2f);

            Jump();

            yield return new WaitForSeconds(1.2f);

            isBusy = false;
        }

        void Jump()
        {
            if (!isGrounded) return;

            isGrounded = false;
            animator.SetBool("isGrounded", false);

            float dir = Random.Range(0, 2) == 0 ? -1f : 1f;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);
        }

        // ================= THROW =================

        IEnumerator ThrowRoutine()
        {
            animator.SetTrigger("Throw");

            // You can replace this with Animation Event
            yield return new WaitForSeconds(0.4f);

            ThrowBomb();

            yield return new WaitForSeconds(1f);

            isBusy = false;
        }

        void ThrowBomb()
        {
            if (bombPrefab == null || throwPoint == null) return;

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
        }

        // ================= GROUND CHECK =================

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.contacts[0].normal.y > 0.5f)
            {
                isGrounded = true;
                animator.SetBool("isGrounded", true);
            }
        }

        // ================= ANIMATION EVENT =================
        // Call this from animation if you want perfect timing

        public void Animation_ThrowBomb()
        {
            ThrowBomb();
        }
    }
}