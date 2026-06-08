using System.Collections;
using CagneyCarnation;
using Combat;
using UnityEngine;

namespace Boss.CagneyCarnation
{
    public partial class CagneyCarnationController : MonoBehaviour
    {
        [SerializeField] private float finalFormHealth = 30f;
        
        [Header("Boomerang Settings")]
        [SerializeField] private GameObject boomerangPrefab;
        [SerializeField] private Transform[] boomerangWaypoints;
        
        [Header("Acorn Settings")]
        [SerializeField] private GameObject acornPrefab;
        [SerializeField] private Transform[] acornSpawnPoint;
        [SerializeField] private float acornSpawnInterval = 0.5f;

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
        private static bool _isDeath;
        
        public static bool IsDeath => _isDeath;


        void Awake()
        {
            _animator = GetComponent<Animator>();
            _health = GetComponent<Health>();
            
            foreach (GameObject subVines in subVinesGameObjects)
            {
                subVines.SetActive(false);
            }
            mainVinesGameObject.SetActive(false);
        }
        void OnEnable()
        {
            _health.OnDeath += () => _isDeath = true;
        }

        void Start()
        {
            _isDeath = false;
            StartCoroutine(MainLoop());
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
            
            if (_isDeath)
            {
                _animator.Play("CCDeath");
                
            }
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
    }
}
