using System.Collections;
using Combat;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;


namespace CagneyCarnation
{
    public class CagneyCarnationController : MonoBehaviour
    {
        [SerializeField] private float finalFormHealth = 30f;
        
        [Header("Boomerang Settings")]
        [SerializeField] private GameObject boomerangPrefab;
        [SerializeField] private Transform[] boomerangWaypoints;

        [Header("FinalForm Settings")] 
        [SerializeField] private GameObject mainVinesGameObject;
        [SerializeField] private GameObject[] subVinesGameObjects;
        [SerializeField] private float subVinesActiveTime = 2f;
        [SerializeField] private float subVinesSpawnInterval = 1f;
        
        [Header("Pollen Settings")]
        [SerializeField] GameObject pollenWaypointPrefab;
        [SerializeField] Transform pollenSpawnPoint;
        
        [Header("Seed Firing Settings")]
        [SerializeField] private GameObject[]  seedPrefabs;
        [SerializeField] private float  seedFiringActiveTime = 6f;
        [SerializeField] private float  seedSpawnInterval = .5f;
        [SerializeField] private Transform seedSpawnPoint;
        [SerializeField] private float minRangeX = -5;
        [SerializeField] private float maxRangeX = 5;
        
        private Animator _animator;
        private CarnationState _currentState;
        private Health _health;
        private int _currentActiveVineIndex = -1;

        void Awake()
        {
            foreach (GameObject subVines in subVinesGameObjects)
            {
                subVines.SetActive(false);
            }
            mainVinesGameObject.SetActive(false);
        }
        void Start()
        {
            _animator = GetComponent<Animator>();
            StartCoroutine(MainLoop());
            _health = GetComponent<Health>();
        }

        IEnumerator MainLoop()
        {
            float introTime= _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(introTime);

            while (_health.CurrentHealth >=finalFormHealth)
            {
                yield return PlayStateAndWait("CCIdle");
                yield return BossAction();
            }

            yield return FinalFormRoutine();
        }

        IEnumerator BossAction()
        {
            int action = Random.Range(0, 4);

            switch (action)
            {
                case 0:
                    yield return FiringSeedsRoutine();
                    break;
                
                case 1:
                    yield return CreatingObstacleRoutine();
                    break;
                
                case 2:
                    yield return FaceAttackHighRoutine();
                    break;
                
                case 3:
                    yield return FaceAttackLowRoutine();
                    break;
            }
        }

        IEnumerator FiringSeedsRoutine()
        {
            yield return PlayStateAndWait("CCFiringSeeds");
        }

        IEnumerator CreatingObstacleRoutine()
        {
            yield return PlayStateAndWait("CCCreatingObject");
        }

        IEnumerator FaceAttackHighRoutine()
        {
            yield return PlayStateAndWait("CCHighFaceAttack");
        }

        IEnumerator FaceAttackLowRoutine()
        {
            yield return PlayStateAndWait("CCLowFaceAttack");
        }

        IEnumerator FinalFormRoutine()
        {
            yield return PlayStateAndWait("CCFinalFormIntro");
            if(mainVinesGameObject!=null)mainVinesGameObject.SetActive(true);
            
            // Start sub vines routine WITHOUT yield return - runs in PARALLEL
            StartCoroutine(SubVinesRoutine());
            
             while (true)
             {
                 yield return PlayStateAndWait("CCFinalFormIdle");
                 yield return PlayStateAndWait("CCFireingPollen");
             }
        }

        IEnumerator PlayStateAndWait(string stateName)
        {
            _animator.Play(stateName);
            yield return null; // allow animator to enter the new state
            float currentAnimationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(currentAnimationLength);
        }

        // ====================== BOOMERANG===================
        public void OnBoomerangSpawnEvent()
        {
            SpawnBoomerang();
        }

        private GameObject SpawnBoomerang()
        {
            if (boomerangPrefab == null ||boomerangWaypoints == null || boomerangWaypoints.Length == 0) return null;

            // Spawn at first waypoint
            GameObject boomerang = Instantiate(boomerangPrefab, boomerangWaypoints[0].position, Quaternion.identity);

            // Set waypoints on the prefab
            ProjectilePathFollower follower = boomerang.GetComponent<ProjectilePathFollower>();
            if (follower != null)
            {
                follower.SetWaypointPath(boomerangWaypoints);
            }

            return boomerang;
        }

        //=====================VINES=========================
        IEnumerator SubVinesRoutine()
        {
            yield return new WaitForSeconds(1f);
            while (true)
            {
                RandomEnableSubVines();
                yield return new WaitForSeconds(subVinesSpawnInterval);
            }
        }
        
        private void RandomEnableSubVines()
        {
            if (subVinesGameObjects == null || subVinesGameObjects.Length == 0) return;

            int randomIndex;
            
            // Keep trying until we get an index that's not currently active
            do
            {
                randomIndex = Random.Range(0, subVinesGameObjects.Length);
            } while (randomIndex == _currentActiveVineIndex);

            _currentActiveVineIndex = randomIndex;
            GameObject selectedVines = subVinesGameObjects[randomIndex];
          
            selectedVines.SetActive(true);
            StartCoroutine(DisableSubVinesAfterTime(selectedVines, randomIndex));
        }

        private IEnumerator DisableSubVinesAfterTime(GameObject selectedVines, int vineIndex)
        {
            yield return new WaitForSeconds(subVinesActiveTime);
            selectedVines.SetActive(false);
            
            // Clear the active vine index when disabled
            if (_currentActiveVineIndex == vineIndex)
            {
                _currentActiveVineIndex = -1;
            }
        }
        
        //======================POLLEN========================
        public void OnPollenSpawnEvent()
        {
            SpawnPollen();
        }

        private GameObject SpawnPollen()
        {
            GameObject pollen = Instantiate(pollenWaypointPrefab, pollenSpawnPoint.position, Quaternion.identity);
            return pollen;
        }

        IEnumerator SpawnSeedsRoutine()
        {
            if(seedPrefabs == null || seedPrefabs.Length == 0 || seedSpawnPoint == null) yield break;
            
            float elapsedTime = 0f;
            
            yield return new WaitForSeconds(1f);
            
            while (elapsedTime < seedFiringActiveTime)
            {
                int randomIndex = Random.Range(0, seedPrefabs.Length);
                GameObject seed = Instantiate(seedPrefabs[randomIndex], seedSpawnPoint.position, Quaternion.identity);
                float randomX = Random.Range(minRangeX, maxRangeX);
                seed.transform.position += new Vector3(randomX, 0f, 0f);
                Destroy(seed, 5f); // Destroy seeds after 5 seconds to clean up
                
                // Wait for spawn interval before spawning next seed
                yield return new WaitForSeconds(seedSpawnInterval);
                elapsedTime += seedSpawnInterval;
            }
        }

        public void OnSeedsFiringEvent()
        {
            // Start coroutine immediately (no delay needed if animation event triggers at right time)
            StartCoroutine(SpawnSeedsRoutine());
        }

    }
}
