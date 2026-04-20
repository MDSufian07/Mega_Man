using UnityEngine;

namespace Combat
{
    public class Bomb2D : MonoBehaviour
    {
        public float explodeDelay = 2f;
        public float radius = 2f;
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
                    Debug.Log("Player Hit!");
                }
            }

            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}