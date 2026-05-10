using System.Collections;
using Player;
using UnityEngine;

namespace Boss.StoneBoss
{
    public class StoneBoss : BaseBoss
    {
        private static readonly int Jump =
            Animator.StringToHash("Jump");

        private static readonly int Throw =
            Animator.StringToHash("Throw");

        private static readonly int Fall =
            Animator.StringToHash("Fall");

        [Header("References")]
        [SerializeField] private Transform throwPoint;
        [SerializeField] private GameObject stonePrefab;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float maxMoveForce = 4f;
        [SerializeField] private float minMoveForce = -1f;

        [Header("Throw Settings")]
        [SerializeField] private float throwForce = 15f;
        [SerializeField] private float throwUpForce = 5f;

        [Header("Environment Shake")]
        [SerializeField] private Transform environmentToShake;
        [SerializeField] private float shakeDuration = 1f;
        [SerializeField] private float shakeAmount = 0.2f;

        private bool _isJumping;
        private bool _wasGrounded;

        protected override void Update()
        {
            base.Update();

            HandleLanding();
        }

        protected override IEnumerator BossAction()
        {
            int action = Random.Range(0, 2);

            if (action == 0)
                yield return JumpRoutine();
            else
                yield return ThrowRoutine();
        }

        // ================= LAND =================

        void HandleLanding()
        {
            if (!_wasGrounded && IsGrounded && _isJumping && Rb.linearVelocity.y <= 0)
            {
                OnLand();
            }
            _wasGrounded = IsGrounded;
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
            if (!IsGrounded || player == null) yield break;

            _isJumping = true;

            Anim.SetTrigger(Jump);

            yield return new WaitForSeconds(0.2f);

            float dir = player.position.x > transform.position.x ? 1f : -1f;

            float moveForce = Random.Range(minMoveForce, maxMoveForce);

            Rb.linearVelocity = Vector2.zero;

            Rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);

            yield return new WaitUntil(() => !IsGrounded);
            yield return new WaitUntil(() => IsGrounded);
        }

        // ================= THROW =================

        IEnumerator ThrowRoutine()
        {
            Anim.SetTrigger(Throw);
            yield return null;
        }

        public void SpawnStone()
        {
            if (stonePrefab == null || throwPoint == null || player == null) return;

            GameObject stone = Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);
            Rigidbody2D stoneRb = stone.GetComponent<Rigidbody2D>();

            if (stoneRb == null) return;
            
            Vector2 dir = (player.position - throwPoint.position).normalized;
            dir.y += throwUpForce / throwForce;

            stoneRb.AddForce(dir * throwForce, ForceMode2D.Impulse);
        }

        // ================= SHAKE =================

        IEnumerator ShakeEnvironment()
        {
            if (environmentToShake == null) yield break;

            Vector3 originalPos = environmentToShake.position;

            float time = shakeDuration;

            while (time > 0)
            {
                environmentToShake.position = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
                time -= Time.deltaTime;

                yield return null;
            }
            environmentToShake.position = originalPos;
        }

        // ================= PLAYER DISABLE =================

        IEnumerator DisablePlayer()
        {
            if (player == null) yield break;

            GameObject p = player.gameObject;

            PlayerMovement move = p.GetComponent<PlayerMovement>();
            PlayerShooting shoot = p.GetComponent<PlayerShooting>();
            Animator playerAnim = p.GetComponent<Animator>();

            if (move == null) yield break;

            if (move.IsGrounded)
                move.enabled = false;

            if (shoot != null)
                shoot.enabled = false;
            
            if (playerAnim != null && move.IsGrounded)
                playerAnim.SetTrigger(Fall);

            yield return new WaitForSeconds(shakeDuration);
                
            move.enabled = true;
            if (shoot != null) shoot.enabled = true;
        }
    }
}
