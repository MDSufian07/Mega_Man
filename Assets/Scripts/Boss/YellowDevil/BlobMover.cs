using System.Collections;
using UnityEngine;

namespace Boss.YellowDevil
{
    public class BlobMover : MonoBehaviour
    {
        [Header("Movement")]
        private float _moveSpeed;
        private float _startDelay;
        private float _arriveDelay;

        [Header("Animation State Names")]
        [SerializeField] private string initialState = "BlobInitial";
        [SerializeField] private string runState = "BlobRun";
        [SerializeField] private string endState = "BlobEnd";

        private Animator _blobAnimator;
        private Vector3 _destination;
        private bool _hasDestination;
        private System.Action _onArrive;

        void Awake()
        {
            _blobAnimator = GetComponent<Animator>();
        }

        public void Configure(float speed, float startDelaySeconds, float arriveDelaySeconds)
        {
            _moveSpeed = speed;
            _startDelay = startDelaySeconds;
            _arriveDelay = arriveDelaySeconds;
        }

        public void BeginMove(Vector3 targetPosition, System.Action arrivedCallback = null)
        {
            _destination = targetPosition;
            _onArrive = arrivedCallback;
            _hasDestination = true;
            StopAllCoroutines();
            StartCoroutine(MoveRoutine());
        }

        private IEnumerator MoveRoutine()
        {
            PlayState(initialState);

            if (_startDelay > 0f)
            {
                yield return new WaitForSeconds(_startDelay);
            }

            PlayState(runState);

            while (_hasDestination && Vector3.Distance(transform.position, _destination) > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, _destination, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = _destination;

            PlayState(endState);

            if (_arriveDelay > 0f)
            {
                yield return new WaitForSeconds(_arriveDelay);
            }

            if (_onArrive != null)
            {
                _onArrive.Invoke();
            }

            gameObject.SetActive(false);
        }

        private void PlayState(string stateName)
        {
            if (_blobAnimator == null || string.IsNullOrEmpty(stateName))
            {
                return;
            }

            _blobAnimator.Play(stateName, 0, 0f);
        }
    }
}
