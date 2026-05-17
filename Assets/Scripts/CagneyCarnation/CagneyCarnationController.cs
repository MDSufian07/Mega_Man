using System.Collections;
using Combat;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;


namespace CagneyCarnation
{
    public class CagneyCarnationController : MonoBehaviour
    {
        [SerializeField] private float finalFormHealth = 30f;
        
        [Header("Object Creation")]
        [SerializeField] private Transform[] boomerangTravelPoint;
        [SerializeField] private GameObject boomerangPrefab;
        [SerializeField] private float boomerangSpeed= 5f;

        private Animator _animator;
        private CarnationState _currentState;
        private Health _health;
        private Coroutine _boomerangRoutine;

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
            int ObjectCreationAction = Random.Range(0, 2);

            if (ObjectCreationAction == 0) yield return AcornFire();
            if(ObjectCreationAction == 1) yield return BomerangRoutine();
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

        IEnumerator AcornFire()
        {
            yield return null;
        }

        IEnumerator BomerangRoutine()
        {
            if (boomerangPrefab == null || boomerangTravelPoint == null || boomerangTravelPoint.Length == 0) yield break;

            GameObject boomerang = SpawnBoomerang();
            if (boomerang == null) yield break;

            int index = 0;
            while (boomerang != null && index < boomerangTravelPoint.Length)
            {
                Transform target = boomerangTravelPoint[index];
                if (target == null)
                {
                    index++;
                    continue;
                }

                boomerang.transform.position = Vector3.MoveTowards(boomerang.transform.position, target.position, boomerangSpeed * Time.deltaTime);

                if (Vector3.Distance(boomerang.transform.position, target.position) <= 0.02f)
                {
                    index++;
                }
                yield return null;
            }

            if (boomerang != null) Destroy(boomerang);
            _boomerangRoutine = null;
        }

        // Animation Event: call this from the boomerang spawn frame.
        public void OnBoomerangSpawnEvent()
        {
            if (_boomerangRoutine != null) return;
            _boomerangRoutine = StartCoroutine(BomerangRoutine());
        }

        private GameObject SpawnBoomerang()
        {
            if (boomerangTravelPoint == null || boomerangTravelPoint.Length == 0 || boomerangTravelPoint[0] == null) return null;
            return Instantiate(boomerangPrefab, boomerangTravelPoint[0].position, Quaternion.identity);
        }

    }
}