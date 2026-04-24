using System.Collections;
using UnityEngine;
using Combat;

public class PlayerDamageEffect : MonoBehaviour
{
    [SerializeField] private float flickerDuration = 1f;
    [SerializeField] private float flickerSpeed = 0.1f;

    private SpriteRenderer sr;
    private Health health;

    private bool isInvincible;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
    }

    void Start()
    {
        if (health != null)
        {
            health.OnHealthChanged += OnDamaged;
        }
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthChanged -= OnDamaged;
        }
    }

    void OnDamaged(int currentHealth)
    {
        if (!isInvincible)
        {
            StartCoroutine(Flicker());
        }
    }

    IEnumerator Flicker()
    {
        health.SetInvincible(true);
        isInvincible = true;

        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            Color c = sr.color;
            c.a = (Mathf.Approximately(c.a, 1f)) ? 0.2f : 1f;
            sr.color = c;

            yield return new WaitForSeconds(flickerSpeed);
            elapsed += flickerSpeed;
        }

        // Reset
        Color reset = sr.color;
        reset.a = 1f;
        sr.color = reset;

        isInvincible = false;
        health.SetInvincible(false);
    }
}