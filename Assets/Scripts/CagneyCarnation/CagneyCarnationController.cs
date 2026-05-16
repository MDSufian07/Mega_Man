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
        
        private Animator _animator;
        private CarnationState _currentState;
        private Health _health;

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

    }
}