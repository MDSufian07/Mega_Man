using UnityEngine;

namespace Boss.CagneyCarnation
{
    public class SeedToVine : MonoBehaviour
    {
        [SerializeField] private GameObject[] vinePrefabs;

        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

        private Animator animator;
        private Rigidbody2D rb;

        [SerializeField] private float vineYaxisSpawnPoint = -1;

        private bool hasLanded;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasLanded) return;

            if (other.CompareTag("Ground"))
            {
                hasLanded = true;

                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                animator.SetBool(IsGrounded, true);

                Invoke(nameof(SpawnVine), 1f);
            }
        }

        private void SpawnVine()
        {
            int randomIndex = Random.Range(0, vinePrefabs.Length);
            
            Vector3 spawnPos = new Vector3(transform.position.x, vineYaxisSpawnPoint, transform.position.z);

            Instantiate(vinePrefabs[randomIndex], spawnPos, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}