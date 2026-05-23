using System.Collections;
using UnityEngine;

namespace Boss.CagneyCarnation
{
    public partial class CagneyCarnationController
    {
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

        IEnumerator PlayStateAndWait(string stateName)
        {
            _animator.Play(stateName);
            yield return null; // allow animator to enter the new state
            float currentAnimationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(currentAnimationLength);
        }
    }
}

