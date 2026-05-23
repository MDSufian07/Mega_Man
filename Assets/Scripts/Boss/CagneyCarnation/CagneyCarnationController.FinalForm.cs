using System.Collections;
using UnityEngine;

namespace Boss.CagneyCarnation
{
    public partial class CagneyCarnationController
    {
        //============= FINAL FORM ==================
        IEnumerator FinalFormRoutine()
        {
            yield return PlayStateAndWait("CCFinalFormIntro");
            if (mainVinesGameObject != null)
            {
                mainVinesGameObject.SetActive(true);
            }
            StartCoroutine(SubVinesRoutine());

            while (!_isDeath)
            {
                yield return PlayStateAndWait("CCFinalFormIdle");
                if (_isDeath) yield break;

                yield return PlayStateAndWait("CCFireingPollen");
            }
            _animator.Play("CCDeath");
        }

        //=====================VINES=========================
        IEnumerator SubVinesRoutine()
        {
            yield return new WaitForSeconds(1f);
            while (!_isDeath)
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
                _currentActiveVineIndex = -1;
        }
    }
}

