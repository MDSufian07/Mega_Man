using UnityEngine;

namespace Combat
{
    public class SmallStone : MonoBehaviour
    {
        public int damage = 10;

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