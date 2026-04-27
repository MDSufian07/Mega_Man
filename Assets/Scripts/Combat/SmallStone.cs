using UnityEngine;

namespace Combat
{
    public class SmallStone : MonoBehaviour
    {
        public int damage = 10;

        [Header("Force Settings")]
        public float extraForce = 2f;
        public float upwardForce = 5f;

        [Header("Lifetime")]
        public float lifeTime = 3f;

        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            // Add extra force in current moving direction
            if (rb != null && rb.linearVelocity != Vector2.zero)
            {
                Vector2 dir = rb.linearVelocity.normalized;
                rb.AddForce(dir * extraForce, ForceMode2D.Impulse);
            }

            // Add upward force
            if (rb != null)
            {
                rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
            }

            // ⏱ Auto destroy after time
            Destroy(gameObject, lifeTime);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                var dmg = collision.collider.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}