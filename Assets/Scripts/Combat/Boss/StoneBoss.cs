using System.Collections;
using UnityEngine;

namespace Combat.Boss
{
    public class StoneBoss : MonoBehaviour
    {
        [Header("Refs")]
        public Transform player;
        public Transform throwPoint;
        public GameObject stonePrefab;

        [Header("Ground")]
        public Transform groundCheck;
        public float groundRadius;
        public LayerMask groundLayer;

        [Header("Jump")]
        public float jumpForce = 8f;
        public float moveForce = 4f;

        [Header("Timing")]
        public float introTime = 2f;
        public float idleTime = 1f;

        [Header("Environment")]
        public Transform environmentToShake;

        private Rigidbody2D rb;
        private Animator anim;

        private bool isGrounded;
        private bool isJumping;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player").transform;

            StartCoroutine(MainLoop());
        }

        void Update()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        }

        IEnumerator MainLoop()
        {
            anim.Play("Idle");
            yield return new WaitForSeconds(introTime);

            while (true)
            {
                anim.Play("Idle");
                yield return new WaitForSeconds(idleTime);

                int action = Random.Range(0, 2);

                if (action == 0)
                    yield return JumpRoutine();
                else
                    yield return ThrowRoutine();
            }
        }

        IEnumerator JumpRoutine()
        {
            if (!isGrounded) yield break;

            isJumping = true;
            anim.SetTrigger("Jump");

            yield return new WaitForSeconds(0.2f);

            float dir = (player.position.x > transform.position.x) ? 1 : -1;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);

            yield return new WaitUntil(() => isGrounded);

            // LAND EFFECT
            StartCoroutine(ShakeEnvironment());
            StartCoroutine(DisablePlayer());

            isJumping = false;
        }

        IEnumerator ThrowRoutine()
        {
            anim.SetTrigger("Throw");

            yield return new WaitForSeconds(1.5f);

            Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);
        }

        IEnumerator ShakeEnvironment()
        {
            float time = 1f;
            Vector3 original = environmentToShake.position;

            while (time > 0)
            {
                environmentToShake.position = original + (Vector3)Random.insideUnitCircle * 0.2f;
                time -= Time.deltaTime;
                yield return null;
            }

            environmentToShake.position = original;
        }

        IEnumerator DisablePlayer()
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            var move = p.GetComponent<PlayerMovement>();
            var shoot = p.GetComponent<PlayerShooting>();

            move.enabled = false;
            shoot.enabled = false;

            yield return new WaitForSeconds(1f);

            move.enabled = true;
            shoot.enabled = true;
        }
    }
}