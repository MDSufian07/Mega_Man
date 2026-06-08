using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class ProjectilePathFollower : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private Transform[] waypointPath;
        [SerializeField] private bool destroyOnComplete = true;
        
        void Start()
        {
            StartCoroutine(FollowPath());
        }
        
        public void SetWaypointPath(Transform[] waypoints)
        {
            waypointPath = waypoints;
        }

        private IEnumerator FollowPath()
        {
            // Validate waypoints are set 
            if (waypointPath == null || waypointPath.Length == 0)
            {
                Destroy(gameObject);
                yield break;
            }

            int index = 0;
            Transform projectile = transform;

            while (index < waypointPath.Length)
            {
                projectile.position = Vector3.MoveTowards(projectile.position, waypointPath[index].position, speed * Time.deltaTime);

                if (Vector3.Distance(projectile.position, waypointPath[index].position) < 0.1f)
                {
                    index++;
                }
                
                yield return null;
            }
            
            if (destroyOnComplete)
            {
                Destroy(gameObject);
            }
        }
        
    }
}

