using Combat;
using UnityEngine;

namespace Player
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireCooldown = 0.5f;
        
        [SerializeField] private float startDelay = 1.5f;

        private InputHandler _input;
        private float _nextFireTime;
        
        private bool _canFire;
        public bool CanFire => _canFire;

        void Awake()
        {
            _input = GetComponent<InputHandler>();
        }

        void Start()
        {
            Invoke(nameof(EnableFire), startDelay);
        }

        void EnableFire()
        {
            _canFire = true;
        }

        void Update()
        {
            if(!_canFire) return;
            
            if (_input.FirePressed && Time.time >= _nextFireTime)
            {
                Shoot();
            }
        }

        void Shoot()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            bullet.GetComponent<Bullet>().SetDirection(direction);
            _nextFireTime = Time.time + fireCooldown;
        }
    }
}