using System;
using Combat;
using UnityEngine;

namespace Boss.YellowDevil
{
    public class YellowDevilBullet : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifeTime = 2f;
        [SerializeField] private int damage = 10;

        [Header("Targeting")]
        [SerializeField] private string playerTag = "Player";

        [Header("VFX")]
        [SerializeField] private GameObject bulletExplosionPrefab;

        private Vector2 _direction;

        public event Action<GameObject> OnHit;

        // Set direction from shooter
        public void SetDirection(Vector2 dir)
        {
            _direction = dir.normalized;
        }

        public void SetPlayerTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                playerTag = tag;
            }
        }

        void Start()
        {
            // Auto destroy after time
            Destroy(gameObject, lifeTime);
        }

        void Update()
        {
            // Move bullet
            transform.Translate(_direction * (speed * Time.deltaTime));
        }

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

            OnHit?.Invoke(collision.gameObject);

            if (bulletExplosionPrefab != null)
            {
                Instantiate(bulletExplosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
