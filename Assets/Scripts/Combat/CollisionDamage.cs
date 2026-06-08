using UnityEngine;
using Utilities;
using Boss.CagneyCarnation;

namespace Combat
{
    public class CollisionDamage : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int damage = 1;

        [SerializeField] private bool destroyOnHit;

        [Header("Targeting")]
        [SerializeField] private GameTags playerTag = GameTags.Player;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(playerTag.ToString()))
            {
                return;
            }

            if (CagneyCarnationController.IsDeath)
            {
                return;
            }

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            if (destroyOnHit)
                Destroy(gameObject);
        }
    }
}
