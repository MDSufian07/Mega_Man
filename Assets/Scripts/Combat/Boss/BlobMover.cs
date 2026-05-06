using System.Collections;
using UnityEngine;

namespace Combat.Boss
{
    public class BlobMover : MonoBehaviour
    {
        [Header("Movement")]
        private float moveSpeed;
        private float startDelay;
        private float arriveDelay;

        [Header("Animation State Names")]
        [SerializeField] private string initialState = "BlobInitial";
        [SerializeField] private string runState = "BlobRun";
        [SerializeField] private string endState = "BlobEnd";

        private Animator blobAnimator;
        private Vector3 destination;
        private bool hasDestination;
        private System.Action onArrive;

        void Awake()
        {
            blobAnimator = GetComponent<Animator>();
        }

        public void Configure(float speed, float startDelaySeconds, float arriveDelaySeconds)
        {
            moveSpeed = speed;
            startDelay = startDelaySeconds;
            arriveDelay = arriveDelaySeconds;
        }

        public void BeginMove(Vector3 targetPosition, System.Action arrivedCallback)
        {
            destination = targetPosition;
            onArrive = arrivedCallback;
            hasDestination = true;
            StopAllCoroutines();
            StartCoroutine(MoveRoutine());
        }

        public void BeginMove(Vector3 targetPosition)
        {
            BeginMove(targetPosition, null);
        }

        private IEnumerator MoveRoutine()
        {
            PlayState(initialState);

            if (startDelay > 0f)
            {
                yield return new WaitForSeconds(startDelay);
            }

            PlayState(runState);

            while (hasDestination && Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = destination;

            PlayState(endState);

            if (arriveDelay > 0f)
            {
                yield return new WaitForSeconds(arriveDelay);
            }

            if (onArrive != null)
            {
                onArrive.Invoke();
            }

            gameObject.SetActive(false);
        }

        private void PlayState(string stateName)
        {
            if (blobAnimator == null || string.IsNullOrEmpty(stateName))
            {
                return;
            }

            blobAnimator.Play(stateName, 0, 0f);
        }
    }
}
