using UnityEngine;

namespace Utilities
{
    public class TargetFollowerRotation : MonoBehaviour
    {
    
        [SerializeField] private string targeTag = GameTags.Player;
        [SerializeField] private float rotationSpeed = 80f;
        
        private Transform target;

        void Start()
        {
            target = GameObject.FindGameObjectWithTag(targeTag).transform;
        }
        
        void Update()
        {
            RotateTowardsTarget();
        }
        
        private void RotateTowardsTarget()
        {
            if(target == null) return;
            
            Vector2 direction = target.position - transform.position;
            
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
            
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        
    }
}
