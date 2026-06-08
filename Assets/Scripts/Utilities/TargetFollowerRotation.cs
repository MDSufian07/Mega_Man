using UnityEngine;

namespace Utilities
{
    public class TargetFollowerRotation : MonoBehaviour
    {
    
        [SerializeField] private GameTags targeTag = GameTags.Player;
        [SerializeField] private float rotationSpeed = 80f;
        [SerializeField] private float intialRotation = 180f;

        private Transform target;

        void Start()
        {
           GameObject player = GameObject.FindGameObjectWithTag(targeTag.ToString());

           if (player != null)
           {
               target = player.transform;
           }
        }
        
        void Update()
        {
            RotateTowardsTarget();
        }
        
        private void RotateTowardsTarget()
        {
            if(target == null) return;
            
            Vector2 direction = target.position - transform.position;
            
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + intialRotation;
            
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        
    }
}
