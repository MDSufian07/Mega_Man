using UnityEngine;

namespace Combat
{
    public class DamageRelayToParent : MonoBehaviour, IDamageable
    {
        [SerializeField] private Health targetHealth;

        void Awake()
        {
            if (targetHealth == null)
            {
                targetHealth = GetComponentInParent<Health>();
            }
        }

        public void TakeDamage(int damage)
        {
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }
    }
}

