using System;
using UnityEngine;

namespace Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;
        
        private bool canTakeDamage = true;
        
        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        
        public event Action<int> OnHealthChanged;
        public event Action OnDeath;


        void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (!canTakeDamage) return;

            currentHealth -= damage;

            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void SetInvincible(bool value)
        {
            canTakeDamage = !value;
        }

        private void Die()
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
        
    }
}
