using UnityEngine;

namespace Combat
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifeTime = 2f;

        private Vector2 direction;

        public void SetDirection(Vector2 dir)
        {
            direction = dir.normalized;
        }

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        void Update()
        {
            transform.Translate(direction * (speed * Time.deltaTime));
        }
        
        
    }
}