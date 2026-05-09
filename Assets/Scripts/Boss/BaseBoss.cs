using System.Collections;
using UnityEngine;
using Utilities;
// ReSharper disable All

namespace Boss
{
    public abstract class BaseBoss : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Transform player;

        [Header("Ground Check")]
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected float groundCheckRadius = 0.2f;

        [Header("Timing")]
        [SerializeField] protected float introDuration = 2f;
        [SerializeField] protected float idleDelay = 1f;

        protected Rigidbody2D Rb;
        protected Animator Anim;

        protected bool IsGrounded;

        protected Vector3 OriginalScale;

        protected virtual void Start()
        {
            Rb = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();

            OriginalScale = transform.localScale;

            FindPlayer();

            StartCoroutine(MainLoop());
        }

        protected virtual void Update()
        {
            CheckGround();

            if (player != null)
            {
                LookAtPlayer();
            }
        }

        // ================= PLAYER =================

        protected virtual void FindPlayer()
        {
            if (player != null) return;

            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // ================= GROUND =================

        protected virtual void CheckGround()
        {
            IsGrounded = GroundCheckUtility.IsGrounded(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        // ================= LOOK =================

        protected virtual void LookAtPlayer()
        {
            FlipUtility.FlipX(transform, player, OriginalScale);
        }

        // ================= MAIN LOOP =================

        protected virtual IEnumerator MainLoop()
        {
            yield return new WaitForSeconds(introDuration);

            while (true)
            {
                Anim.Play("Idle");

                yield return new WaitForSeconds(idleDelay);

                yield return BossAction();
            }
        }

        protected abstract IEnumerator BossAction();

        // ================= DEBUG =================

        protected virtual void OnDrawGizmos()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}