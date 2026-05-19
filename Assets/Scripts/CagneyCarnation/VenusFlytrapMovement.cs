using UnityEngine;

namespace CagneyCarnation
{
    public class VenusFlytrapMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float directionChangeTime = 0.5f;
        [SerializeField] private float lifeTime = 5f;

        private float yAxisRotation;

        private float timer;

        private int rotationDirection;

        private void Start()
        {
            PickInitialDirection();
            ApplyInitialRotation();

            PickRandomRotation();

            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            Move();
            SlitherRotation();
        }

        private void Move()
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }

        private void SlitherRotation()
        {
            timer += Time.deltaTime;

            if (timer >= directionChangeTime)
            {
                timer = 0f;

                // Change between + and -
                rotationDirection *= -1;
            }

            float rotationAmount = rotationDirection * rotationSpeed * Time.deltaTime;

            // Reverse if facing right
            if (yAxisRotation == 180f)
            {
                rotationAmount *= -1;
            }

            transform.Rotate(0, 0, rotationAmount);
        }

        private void PickInitialDirection()
        {
            yAxisRotation = Random.Range(0, 2) == 0 ? 0f : 180f;
        }

        private void ApplyInitialRotation()
        {
            transform.rotation = Quaternion.Euler(0, yAxisRotation, 0);
        }

        private void PickRandomRotation()
        {
            rotationDirection = Random.Range(0, 2) == 0 ? -1 : 1;
        }
    }
}