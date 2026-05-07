using Combat;
using UnityEngine;

namespace Boss.YellowDevil
{
    public class BlobDamage : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int damage = 1;

        [Header("Targeting")]
        [SerializeField] private string playerTag = "Player";

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(playerTag))
            {
                return;
            }

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
