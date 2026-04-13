using System;
using UnityEngine;

namespace Combat
{
    public class Bullet : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifeTime = 2f;
        [SerializeField] private int damage = 10;

        private Vector2 direction;

        // EVENT (optional use)
        public event Action<GameObject> OnHit;

        // Set direction from shooter
        public void SetDirection(Vector2 dir)
        {
            direction = dir.normalized;
        }

        void Start()
        {
            // Auto destroy after time
            Destroy(gameObject, lifeTime);
        }

        void Update()
        {
            // Move bullet
            transform.Translate(direction * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 🔹 Try to damage object
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // Invoke hit event
            OnHit?.Invoke(collision.gameObject);

            // Destroy bullet on hit
            Destroy(gameObject);
        }
    }
}