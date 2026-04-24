using UnityEngine;

namespace Combat
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireCooldown = 0.5f;
        
        [SerializeField] private float startDelay = 1.5f;

        private InputHandler input;
        private float nextFireTime;
        
        private bool canFire;
        public bool CanFire => canFire;

        void Awake()
        {
            input = GetComponent<InputHandler>();
        }

        void Start()
        {
            Invoke(nameof(EnableFire), startDelay);
        }

        void EnableFire()
        {
            canFire = true;
        }

        void Update()
        {
            if(!canFire) return;
            
            if (input.FirePressed && Time.time >= nextFireTime)
            {
                Shoot();
            }
        }

        void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            bullet.GetComponent<Bullet>().SetDirection(direction);
            nextFireTime = Time.time + fireCooldown;
        }
    }
}