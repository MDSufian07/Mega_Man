using Combat;
using UnityEngine;
using Utilities;

namespace Boss.StoneBoss
{
    public class SmallStone : MonoBehaviour
    {
    public int damage = 10;

    [Header("Force Settings")]
    [SerializeField] private float extraForce = 2f;
    [SerializeField] private float upwardForce = 5f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 3f;
    
        private Rigidbody2D _rb;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();

            // Add extra force in current moving direction
            if (_rb != null && _rb.linearVelocity != Vector2.zero)
            {
                Vector2 dir = _rb.linearVelocity.normalized;
                _rb.AddForce(dir * extraForce, ForceMode2D.Impulse);
            }

            // Add upward force
            if (_rb != null)
            {
                _rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
            }

            //  Auto destroy after time
            Destroy(gameObject, lifeTime);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(GameTags.Player))
            {
                var dmg = collision.collider.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}