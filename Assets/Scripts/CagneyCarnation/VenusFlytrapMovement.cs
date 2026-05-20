using UnityEngine;

namespace CagneyCarnation
{
    public class VenusFlytrapMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float rotationInterval = 1f;
        [SerializeField] private float rotationDuration = 0.2f;
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private float growthTime = 1f;

        private float yAxisRotation;

        private float timer;
        private float elapsedTime;
        private float rotationTimeLeft;
        private int rotationDirection;

        private void Start()
        {
            PickInitialDirection();
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            Move();
            UpdateRandomRotation();
        }

        private void Move()
        {
            if (elapsedTime >= growthTime)
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }

        private void PickInitialDirection()
        {
            yAxisRotation = Random.Range(0, 2) == 0 ? 0f : 180f;
            transform.rotation = Quaternion.Euler(0, yAxisRotation, 0);
        }

        private void UpdateRandomRotation()
        {
            timer += Time.deltaTime;

            if (timer >= rotationInterval)
            {
                timer = 0f;
                RandomRotation();
            }

            if (rotationTimeLeft > 0f && rotationDirection != 0)
            {
                float rotationAmount = rotationDirection * rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, 0f, rotationAmount);
                rotationTimeLeft = Mathf.Max(0f, rotationTimeLeft - Time.deltaTime);
            }
        }

        private void RandomRotation()
        {
            int random = Random.Range(0, 3);

            switch (random)
            {
                case 0:
                    rotationDirection = 1;
                    rotationTimeLeft = rotationDuration;
                    break;
                case 1:
                    rotationDirection = -1;
                    rotationTimeLeft = rotationDuration;
                    break;
                case 2:
                    rotationDirection = 0;
                    rotationTimeLeft = 0f;
                    break;
            }
        }
    }
}