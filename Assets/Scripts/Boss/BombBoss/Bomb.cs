using System.Collections;
using Combat;
using UnityEngine;

namespace Boss.BombBoss
{
    public class Bomb2D : MonoBehaviour
    {
        public float explodeDelay = 2f;
        public float damageDelay = 0.1f;
        public float radius = 2f;
        public int damage = 20;
        public GameObject explosionPrefab;
        public SpriteRenderer spriteRenderer;
        public Collider2D col;

        void Start()
        {
            Invoke(nameof(StartExplosion), explodeDelay);
        }

        void StartExplosion()
        {
            // Hide bomb instantly
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;

            if (col != null)
                col.enabled = false;

            // Play explosion effect
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Start delayed damage
            StartCoroutine(ApplyDamageAfterDelay());
        }

        IEnumerator ApplyDamageAfterDelay()
        {
            yield return new WaitForSeconds(damageDelay);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damage);
                        Debug.Log("Player Hit AFTER delay! Damage: " + damage);
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