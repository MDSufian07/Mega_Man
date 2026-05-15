using System.Collections;
using UnityEngine;
using Utilities;


namespace CagneyCarnation
{
    public class CagneyCarnationController : MonoBehaviour
    {
        [SerializeField] private float introTime = 2f;
        [SerializeField] private float idleTime = 1f;
        
        private Animator animator;
        void Start()
        {
            animator = GetComponent<Animator>();
            StartCoroutine(CagneyCarnationMasterLoop());
        }

        IEnumerator CagneyCarnationMasterLoop()
        {
            yield return new WaitForSeconds(introTime);

            while (true)
            {
                animator.Play(AnimatorHashes.CagneyCarnationIdle);
                yield return new WaitForSeconds(idleTime);
                StartCoroutine(MainLoop());
            }

        }

        IEnumerator MainLoop()
        {
            int action = Random.Range(0, 2);

            if (action == 0)
                yield return FiringSeeds();
            else if (action == 1)
                yield return ObjectCreation();
        }

        IEnumerator FiringSeeds()
        {
            animator.Play(AnimatorHashes.CagneyCarnationFiringSeeds);
            yield return new WaitForSeconds(idleTime);
            yield return null;
        }

        IEnumerator ObjectCreation()
        {
            animator.Play(AnimatorHashes.CagneyCarnationCreatingObject);
            yield return new WaitForSeconds(introTime);
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
