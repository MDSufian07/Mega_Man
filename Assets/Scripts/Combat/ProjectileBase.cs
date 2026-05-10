using System;
using UnityEngine;

namespace Combat
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifeTime = 2f;
        [SerializeField] private int damage = 10;

        [Header("VFX")]
        [SerializeField] private GameObject bulletExplosionPrefab;

        private Vector2 _direction;

        public event Action<GameObject> OnHit;

        // Set direction from shooter
        public void SetDirection(Vector2 dir)
        {
            _direction = dir.normalized;
        }

        protected virtual void Start()
        {
            // Auto destroy after time
            Destroy(gameObject, lifeTime);
        }

        protected virtual void Update()
        {
            // Move projectile
            transform.Translate(_direction * (speed * Time.deltaTime));
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (ShouldIgnore(collision))
            {
                return;
            }

            bool canDamageThisTarget = CanDamageTarget(collision);
            IDamageable damageable = collision.GetComponent<IDamageable>();

            // Fallback: check parent if not found on child
            if (damageable == null)
            {
                damageable = collision.GetComponentInParent<IDamageable>();
            }

            if (canDamageThisTarget && damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            OnHit?.Invoke(collision.gameObject);

            if (bulletExplosionPrefab != null)
            {
                Instantiate(bulletExplosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

        protected abstract bool ShouldIgnore(Collider2D collision);
        protected abstract bool CanDamageTarget(Collider2D collision);
    }
}

