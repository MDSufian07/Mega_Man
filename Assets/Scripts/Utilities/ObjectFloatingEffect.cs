using UnityEngine;

namespace Utilities
{
    public class ObjectFloatingEffect  : MonoBehaviour
    {
        [SerializeField] private float floatSpeed = 3f;
        [SerializeField] private float floatHeight = 0.5f;
        
        private Vector3 startPosition;
        
        void Start()
        {
            startPosition = transform.position;
        }
        
        void Update()
        {
            FloatUpDown();
        }
        
        private void FloatUpDown()
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector2(transform.position.x, startPosition.y + yOffset);
        }
    }
}
