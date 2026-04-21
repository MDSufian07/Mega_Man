using UnityEngine;

namespace Combat
{
    public class Bomb2D : MonoBehaviour
    {
        public float explodeDelay = 2f;
        public float radius = 2f;
        public int damage = 20;
        public GameObject explosionEffect;

        void Start()
        {
            Invoke(nameof(Explode), explodeDelay);
        }

        void Explode()
        {
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damage);
                        Debug.Log("Player Hit! Damage: " + damage);
                    }
                }
            }

            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}