using UnityEngine;

namespace Utilities
{
    public class LinearMovement : MonoBehaviour
    {

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float lifeTime = 5f;
        
        [SerializeField] private Vector2 moveDirection = Vector2.left;
        
    
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }
        
        void Update()
        {
            MoveLeft();
        }
        
        private void MoveLeft()
        {
            transform.Translate(moveDirection * (moveSpeed * Time.deltaTime));
        }
        
    }
}
