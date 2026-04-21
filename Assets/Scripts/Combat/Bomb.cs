using UnityEngine;

namespace Combat
{
    public class Bomb2D : MonoBehaviour
    {
        public float explodeDelay = 2f;
        public float radius = 2f;
        public int damage = 20;
        public GameObject explosionPrefab;

        void Start()
        {
            
            Invoke(nameof(Explode), explodeDelay);
        }

        void Explode()
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
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

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}