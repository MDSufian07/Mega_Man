using UnityEngine;

namespace Combat
{
    public class BigStone : MonoBehaviour
    {
        public int damage = 20;
        public GameObject smallStonePrefab;
        public int splitCount = 4;

        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                var dmg = collision.collider.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(damage);
            }

            SplitStone();
        }

        void SplitStone()
        {
            Vector2 baseDir = rb.linearVelocity.normalized;

            for (int i = 0; i < splitCount; i++)
            {
                float angle = Random.Range(-45f, 45f);
                Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

                GameObject small = Instantiate(smallStonePrefab, transform.position, Quaternion.identity);
                Rigidbody2D srb = small.GetComponent<Rigidbody2D>();

                if (srb != null)
                    srb.AddForce(dir * Random.Range(3f, 6f), ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
    }
}