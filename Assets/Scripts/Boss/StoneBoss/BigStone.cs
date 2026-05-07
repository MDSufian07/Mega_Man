using Combat;
using UnityEngine;

namespace Boss.StoneBoss
{
    public class BigStone : MonoBehaviour
    {
    [SerializeField] private int damage = 20;
    [SerializeField] private GameObject smallStonePrefab;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField]  private int splitCount = 4;
    [SerializeField] private LayerMask destructibleLayers;

    private Rigidbody2D _rb;
    private bool _hasAlreadySplit;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Prevent multiple splits
        if (_hasAlreadySplit) return;

        // Only destroy when hitting objects on destructible layers
        if (((1 << collision.gameObject.layer) & destructibleLayers) != 0)
        {
            if (collision.CompareTag("Player"))
            {
                var dmg = collision.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(damage);
            }

            _hasAlreadySplit = true;
            SplitStone();
        }
    }

        void SplitStone()
        {
            // Spawn effect and apply slight velocity from big stone
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
                Rigidbody2D effectRb = effect.GetComponent<Rigidbody2D>();
                
                // Apply reduced velocity for subtle movement
                if (effectRb != null && _rb != null)
                {
                    effectRb.linearVelocity = _rb.linearVelocity * 0.3f;
                }
            }

            Vector2 baseDir = _rb.linearVelocity.normalized;

            for (int i = 0; i < splitCount; i++)
            {
                float angle = Random.Range(-45f, 45f);
                Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

                GameObject small = Instantiate(smallStonePrefab, transform.position, Quaternion.identity);
                Rigidbody2D srb = small.GetComponent<Rigidbody2D>();

                if (srb != null)
                    srb.AddForce(dir * Random.Range(3f, 6f), ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
    }
}