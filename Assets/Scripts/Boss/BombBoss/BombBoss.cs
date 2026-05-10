using System.Collections;
using UnityEngine;
using Utilities;

namespace Boss.BombBoss
{
    public class BombBoss : BaseBoss
    {
        [Header("References")]
        [SerializeField] private Transform throwPoint;
        [SerializeField] private GameObject bombPrefab;

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

        private bool _isJumping;
        private bool _bombActive;

        private float _jumpDirection = 1f;

        protected override void Start()
        {
            base.Start();

            Anim.Play(AnimatorHashes.BombBossIntro);
        }

        protected override void Update()
        {
            base.Update();

            // While jumping in air
            if (_isJumping && !IsGrounded)
            {
                Vector3 scale = OriginalScale;

                scale.x = Mathf.Abs(OriginalScale.x) * _jumpDirection;
                
                transform.localScale = scale;
            }
            // When grounded
            else if (IsGrounded && !_isJumping)
                LookAtPlayer();

            Anim.SetBool(AnimatorHashes.IsGrounded, IsGrounded);
        }

        protected override IEnumerator BossAction()
        {
            int action = Random.Range(0, 5);

            // 40% jump
            if ((action == 0 || action == 1) && !_bombActive)
                yield return JumpRoutine();
            // 60% throw
            else if ((action == 2 || action == 3 || action == 4) && !_bombActive && IsGrounded)
                yield return ThrowRoutine();
        }

        // ================= JUMP =================
        IEnumerator JumpRoutine()
        {
            if (!IsGrounded || _bombActive) yield break;

            _isJumping = true;

            Anim.SetTrigger(AnimatorHashes.Jump);

            yield return new WaitForSeconds(0.2f);

            float jumpForce = Random.Range(minJumpForce, maxJumpForce);
            float moveForce = Random.Range(minMoveForce, maxMoveForce);

            float dir;

            if (player != null)
                dir = player.position.x > transform.position.x ? 1f : -1f;
            else
                dir = Random.Range(0, 2) == 0 ? -1f : 1f;

            _jumpDirection = dir;

            Rb.linearVelocity = Vector2.zero;
            Rb.AddForce(new Vector2(dir * moveForce, jumpForce), ForceMode2D.Impulse);

            yield return new WaitUntil(() => !IsGrounded);
            yield return new WaitUntil(() => IsGrounded);
            yield return new WaitForSeconds(0.05f);

            _isJumping = false;
        }

        // ================= THROW =================

        IEnumerator ThrowRoutine()
        {
            if (!IsGrounded || _isJumping || _bombActive) yield break;

            Anim.SetTrigger(AnimatorHashes.Throw);

            yield return new WaitForSeconds(0.4f);

            GameObject bomb = ThrowBomb();

            if (bomb != null)
            {
                _bombActive = true;

                yield return new WaitUntil(() => bomb == null);
            }
            _bombActive = false;
        }

        // ================= BOMB =================

        GameObject ThrowBomb()
        {
            if (bombPrefab == null || throwPoint == null) return null;

            GameObject bomb = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

            if (bombRb != null)
            {
                float distanceToPlayer = float.MaxValue;

                if (player != null)
                    distanceToPlayer = Vector2.Distance(transform.position, player.position);

                float maxDistance = 10f;

                float normalizedDistance = Mathf.Clamp01(distanceToPlayer / maxDistance);
                float angle = Mathf.Lerp(maxAngle, minAngle, normalizedDistance);
                float force = Mathf.Lerp(minThrowForce, maxThrowForce, normalizedDistance);
                float dir = Mathf.Sign(transform.localScale.x);

                angle += Random.Range(-5f, 5f);

                Vector2 throwDir = new Vector2 (Mathf.Cos(angle * Mathf.Deg2Rad) * dir, Mathf.Sin(angle * Mathf.Deg2Rad));

                bombRb.AddForce(throwDir * force, ForceMode2D.Impulse);
            }
            return bomb;
        }
    }
}