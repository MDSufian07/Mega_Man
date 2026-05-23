using UnityEngine;

namespace Boss.CagneyCarnation
{
    public class VinesToFlyTrap : MonoBehaviour
    {
       
        [SerializeField] private GameObject flyTrapPrefab;
        [SerializeField] Transform flyTrapSpawnPoint;

        [SerializeField] private float vinesActiveTime = 3f;

        void OnSpawnedFlyTrapEvent()
        {
            SpawnedFlyTrap();
        }

        void SpawnedFlyTrap()
        {
            Instantiate(flyTrapPrefab, flyTrapSpawnPoint.position, Quaternion.identity);
             Invoke(nameof(DeactivateVines), vinesActiveTime);
        }

        void DeactivateVines()
        {
            Destroy(gameObject);
        }
    }
}
