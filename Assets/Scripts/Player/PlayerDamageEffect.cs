using System.Collections;
using Combat;
using UnityEngine;

namespace Player
{
    public class PlayerDamageEffect : MonoBehaviour
    {
        [SerializeField] private float flickerDuration = 1f;
        [SerializeField] private float flickerSpeed = 0.1f;

        private SpriteRenderer _sr;
        private Health _health;

        private bool _isInvincible;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _health = GetComponent<Health>();
        }

        void Start()
        {
            if (_health != null)
            {
                _health.OnHealthChanged += OnDamaged;
            }
        }

        void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnHealthChanged -= OnDamaged;
            }
        }

        void OnDamaged(int currentHealth)
        {
            if (!_isInvincible)
            {
                StartCoroutine(Flicker());
            }
        }

        IEnumerator Flicker()
        {
            _health.SetInvincible(true);
            _isInvincible = true;

            float elapsed = 0f;

            while (elapsed < flickerDuration)
            {
                Color c = _sr.color;
                c.a = (Mathf.Approximately(c.a, 1f)) ? 0.2f : 1f;
                _sr.color = c;

                yield return new WaitForSeconds(flickerSpeed);
                elapsed += flickerSpeed;
            }

            // Reset
            Color reset = _sr.color;
            reset.a = 1f;
            _sr.color = reset;

            _isInvincible = false;
            _health.SetInvincible(false);
        }
    }
}