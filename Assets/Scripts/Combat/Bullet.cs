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

        [Header("Targeting")]
        [SerializeField] private bool isPlayerBullet = true;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private string enemyTag = "Enemy";

        private Vector2 direction;

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
            if (isPlayerBullet && collision.CompareTag(playerTag))
            {
                // Ignore the shooter side so player bullets don't disappear on player contact.
                return;
            }

            bool canDamageThisTarget = !isPlayerBullet || collision.CompareTag(enemyTag);
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (canDamageThisTarget && damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // Invoke hit event
            OnHit?.Invoke(collision.gameObject);

            // Destroy bullet on any valid hit except ignored friendly collision.
            Destroy(gameObject);
        }
    }
}