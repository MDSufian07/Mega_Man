using System.Collections;
using UnityEngine;
using Combat;

namespace Combat.Boss
{
    public class YellowDevilController : MonoBehaviour
    {
        [Header("Sprite Renderers")]
        public SpriteRenderer rightBaseSR; // Renderer for the Right Side
        public SpriteRenderer leftBaseSR;  // Renderer for the Left Side

        [Header("Sprite Arrays")]
        // Order: Full Body -> 1 part removed -> 2 parts removed ... -> Empty
        public Sprite[] disassemblySprites; 
        // Order: Empty -> 1 part added -> 2 parts added ... -> Full Body
        public Sprite[] reassemblySprites; 

        [Header("Anchors & Prefabs")]
        public GameObject blobPrefab;
        public Transform[] rightPoints; // 19 points on the Right Base
        public Transform[] leftPoints;  // 19 points on the Left Base
        public GameObject rightEyeObject; // Eye child object (Right base)
        public GameObject leftEyeObject;  // Eye child object (Left base)

        [Header("Settings")]
        public float blobSpeed = 18f;
        public float timeBetweenParts = 0.12f;
        public float eyeOpenTime = 2.0f;
        public float introDelay = 2.0f;
        public float eyeJitterRadius = 0.5f;
        [SerializeField] private float blobStartDelay = 0.3f;
        [SerializeField] private float blobArriveDelay = 0.3f;
        [SerializeField] private string blobInitialState = "BlobInitial";
        [SerializeField] private string blobRunState = "BlobRun";
        [SerializeField] private string blobEndState = "BlobEnd";

        [Header("Attack")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private string playerTag = "Player";
        
        [Header("Death Effects")]
        [SerializeField] private GameObject deathEffectPrefab;

        private bool bossIsAtRight = true;
        private Vector3 rightEyeBaseLocalPos;
        private Vector3 leftEyeBaseLocalPos;
        private bool hasPlayedFirstEyeOpen;

        void Start()
        {
            // Initial State: Right base is full, Left base is empty
            rightBaseSR.sprite = disassemblySprites[0];
            leftBaseSR.sprite = null;

            if (rightEyeObject != null)
            {
                rightEyeBaseLocalPos = rightEyeObject.transform.localPosition;
                rightEyeObject.SetActive(false);
            }

            if (leftEyeObject != null)
            {
                leftEyeBaseLocalPos = leftEyeObject.transform.localPosition;
                leftEyeObject.SetActive(false);
            }

            StartCoroutine(BossMasterLoop());
        }

        IEnumerator BossMasterLoop()
        {
            if (introDelay > 0f)
            {
                GameObject introEye = bossIsAtRight ? rightEyeObject : leftEyeObject;
                ShowEye(introEye, bossIsAtRight, false);
                yield return new WaitForSeconds(introDelay);
                HideEyes();
            }

            while (true)
            {
                // PHASE 1: Attack (Show eye on the current active base)
                GameObject activeEye = bossIsAtRight ? rightEyeObject : leftEyeObject;
                bool applyJitter = hasPlayedFirstEyeOpen;
                ShowEye(activeEye, bossIsAtRight, applyJitter);
                hasPlayedFirstEyeOpen = true;

                float halfOpenTime = Mathf.Max(0f, eyeOpenTime * 0.5f);
                float remainingOpenTime = Mathf.Max(0f, eyeOpenTime - halfOpenTime);

                if (halfOpenTime > 0f)
                {
                    yield return new WaitForSeconds(halfOpenTime);
                    FireEyeBullet(activeEye);
                }

                if (remainingOpenTime > 0f)
                {
                    yield return new WaitForSeconds(remainingOpenTime);
                }

                HideEyes();

                // PHASE 2: Sync Move
                if (bossIsAtRight)
                    yield return StartCoroutine(MoveSideToSide(rightBaseSR, leftBaseSR, rightPoints, leftPoints, true));
                else
                    yield return StartCoroutine(MoveSideToSide(leftBaseSR, rightBaseSR, leftPoints, rightPoints, false));

                bossIsAtRight = !bossIsAtRight;
                yield return new WaitForSeconds(1f);
            }
        }

        void ShowEye(GameObject eyeObject, bool isRightSide, bool applyJitter)
        {
            if (eyeObject == null)
            {
                return;
            }

            Vector3 jitterOffset = applyJitter ? (Vector3)(Random.insideUnitCircle * eyeJitterRadius) : Vector3.zero;

            if (isRightSide)
            {
                eyeObject.transform.SetParent(rightBaseSR.transform, false);
                eyeObject.transform.localPosition = rightEyeBaseLocalPos + jitterOffset;
            }
            else
            {
                eyeObject.transform.SetParent(leftBaseSR.transform, false);
                eyeObject.transform.localPosition = leftEyeBaseLocalPos + jitterOffset;
            }

            eyeObject.SetActive(true);
        }

        void HideEyes()
        {
            if (rightEyeObject != null)
            {
                rightEyeObject.SetActive(false);
            }

            if (leftEyeObject != null)
            {
                leftEyeObject.SetActive(false);
            }
        }

        void FireEyeBullet(GameObject eyeObject)
        {
            if (eyeObject == null || bulletPrefab == null)
            {
                return;
            }

            Transform target = playerTarget;
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(playerTag);
                if (player != null)
                {
                    target = player.transform;
                }
            }

            if (target == null)
            {
                return;
            }

            Vector2 direction = (target.position - eyeObject.transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, eyeObject.transform.position, Quaternion.identity);

            YellowDevilBullet yellowDevilBullet = bullet.GetComponent<YellowDevilBullet>();
            if (yellowDevilBullet != null)
            {
                yellowDevilBullet.SetDirection(direction);
                yellowDevilBullet.SetPlayerTag(playerTag);
                return;
            }

            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.SetDirection(direction);
            }
        }

        IEnumerator MoveSideToSide(SpriteRenderer sourceSR, SpriteRenderer targetSR, Transform[] sourcePoints, Transform[] targetPoints, bool movingLeft)
        {
            // Reset the target side to be empty before rebuilding
            targetSR.sprite = null;
            // Ensure flip is correct based on direction
            targetSR.flipX = movingLeft; 

            for (int i = 0; i < sourcePoints.Length; i++)
            {
                // 1. Change source sprite (Visual: Part disappears from current body)
                // Using i + 1 because index 0 is 'Full' and index 1 is '1 part missing'
                if (i + 1 < disassemblySprites.Length)
                    sourceSR.sprite = disassemblySprites[i + 1];
                else
                    sourceSR.sprite = null;

                // 2. Spawn blob at the exact point index
                GameObject blob = Instantiate(blobPrefab, sourcePoints[i].position, Quaternion.identity);

                BlobMover blobMover = blob.GetComponent<BlobMover>();
                if (blobMover != null)
                {
                    int partIndex = i;
                    blobMover.Configure(blobSpeed, blobStartDelay, blobArriveDelay);
                    blobMover.SetAnimationStates(blobInitialState, blobRunState, blobEndState);
                    blobMover.BeginMove(targetPoints[i].position, () =>
                    {
                        if (partIndex < reassemblySprites.Length)
                        {
                            targetSR.sprite = reassemblySprites[partIndex];
                        }
                    });
                }

                // Wait before sending the next part
                yield return new WaitForSeconds(timeBetweenParts);
            }
        }
    }
}
