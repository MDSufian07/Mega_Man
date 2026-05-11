using System;
using UnityEngine;

namespace Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100;
        private int _currentHealth;
        
        private bool _canTakeDamage = true;
        
        public int MaxHealth => maxHealth;
        public int CurrentHealth => _currentHealth;
        
        public event Action<int> OnHealthChanged;
        public event Action OnDeath;


        void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (!_canTakeDamage) return;

            _currentHealth -= damage;

            OnHealthChanged?.Invoke(_currentHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void SetInvincible(bool value)
        {
            _canTakeDamage = !value;
        }

        private void Die()
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
        
    }
}
