using System.Collections;
using Combat;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Boss.YellowDevil
{
    public class YellowDevilController : MonoBehaviour
    {
        [Header("Sprite Renderers")]
        [FormerlySerializedAs("rightBaseSR")] 
        [SerializeField] private SpriteRenderer rightBaseSr; 
        [FormerlySerializedAs("leftBaseSR")] 
        [SerializeField] private SpriteRenderer leftBaseSr;  // Renderer for the Left Side

        [Header("Sprite Arrays")]
        // Order: Full Body -> 1 part removed -> 2 parts removed ... -> Empty
        [SerializeField] private Sprite[] disassemblySprites; 
        // Order: Empty -> 1 part added -> 2 parts added ... -> Full Body
        [SerializeField] private Sprite[] reassemblySprites; 

        [Header("Anchors & Prefabs")]
        [SerializeField] GameObject blobPrefab;
        [SerializeField] Transform[] rightPoints; // 19 points on the Right Base
        [SerializeField] Transform[] leftPoints;  // 19 points on the Left Base
        [SerializeField] GameObject rightEyeObject; // Eye child object (Right base)
        [SerializeField] GameObject leftEyeObject;  // Eye child object (Left base)

        [Header("Settings")]
        [SerializeField] float blobSpeed = 18f;
        [SerializeField] float timeBetweenParts = 0.12f;
        [SerializeField] float eyeOpenTime = 2.0f;
        [SerializeField] float introDelay = 2.0f;
        [SerializeField] float eyeJitterRadius = 0.5f;
        [SerializeField] private float blobStartDelay = 0.3f;
        [SerializeField] private float blobArriveDelay = 0.3f;

        [Header("Attack")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private string playerTag = GameTags.Player;
        
        [Header("Death Effects")]
        [SerializeField] private GameObject deathEffectPrefab;

        private bool _bossIsAtRight = true;
        private Vector3 _rightEyeBaseLocalPos;
        private Vector3 _leftEyeBaseLocalPos;
        private bool _hasPlayedFirstEyeOpen;

        void Start()
        {
            // Initial State: Right base is full, Left base is empty
            rightBaseSr.sprite = disassemblySprites[0];
            leftBaseSr.sprite = null;

            if (rightEyeObject != null)
            {
                _rightEyeBaseLocalPos = rightEyeObject.transform.localPosition;
                rightEyeObject.SetActive(false);
            }

            if (leftEyeObject != null)
            {
                _leftEyeBaseLocalPos = leftEyeObject.transform.localPosition;
                leftEyeObject.SetActive(false);
            }

            StartCoroutine(BossMasterLoop());
        }

        IEnumerator BossMasterLoop()
        {
            if (introDelay > 0f)
            {
                GameObject introEye = _bossIsAtRight ? rightEyeObject : leftEyeObject;
                ShowEye(introEye, _bossIsAtRight, false);
                yield return new WaitForSeconds(introDelay);
                HideEyes();
            }

            while (true)
            {
                // PHASE 1: Attack (Show eye on the current active base)
                GameObject activeEye = _bossIsAtRight ? rightEyeObject : leftEyeObject;
                bool applyJitter = _hasPlayedFirstEyeOpen;
                ShowEye(activeEye, _bossIsAtRight, applyJitter);
                _hasPlayedFirstEyeOpen = true;

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
                if (_bossIsAtRight)
                    yield return StartCoroutine(MoveSideToSide(rightBaseSr, leftBaseSr, rightPoints, leftPoints, true));
                else
                    yield return StartCoroutine(MoveSideToSide(leftBaseSr, rightBaseSr, leftPoints, rightPoints, false));

                _bossIsAtRight = !_bossIsAtRight;
                yield return new WaitForSeconds(1f);
            }
        }

        void ShowEye(GameObject eyeObject, bool isRightSide, bool applyJitter)
        {
            if (eyeObject == null)
            {
                return;
            }

            Vector3 jitterOffset = applyJitter ? Random.insideUnitCircle * eyeJitterRadius : Vector3.zero;

            if (isRightSide)
            {
                eyeObject.transform.SetParent(rightBaseSr.transform, false);
                eyeObject.transform.localPosition = _rightEyeBaseLocalPos + jitterOffset;
            }
            else
            {
                eyeObject.transform.SetParent(leftBaseSr.transform, false);
                eyeObject.transform.localPosition = _leftEyeBaseLocalPos + jitterOffset;
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

        IEnumerator MoveSideToSide(SpriteRenderer sourceSr, SpriteRenderer targetSr, Transform[] sourcePoints, Transform[] targetPoints, bool movingLeft)
        {
            // Reset the target side to be empty before rebuilding
            targetSr.sprite = null;
            // Ensure flip is correct based on direction
            targetSr.flipX = movingLeft; 

            for (int i = 0; i < sourcePoints.Length; i++)
            {
                // 1. Change source sprite (Visual: Part disappears from current body)
                // Using i + 1 because index 0 is 'Full' and index 1 is '1 part missing'
                sourceSr.sprite = i + 1 < disassemblySprites.Length ? disassemblySprites[i + 1] : null;

                // 2. Spawn blob at the exact point index
                GameObject blob = Instantiate(blobPrefab, sourcePoints[i].position, Quaternion.identity);

                BlobMover blobMover = blob.GetComponent<BlobMover>();
                if (blobMover != null)
                {
                    int partIndex = i;
                    blobMover.Configure(blobSpeed, blobStartDelay, blobArriveDelay);
                    blobMover.BeginMove(targetPoints[i].position, () =>
                    {
                        if (targetSr != null && partIndex < reassemblySprites.Length)
                        {
                            targetSr.sprite = reassemblySprites[partIndex];
                        }
                    });
                }

                // Wait before sending the next part
                yield return new WaitForSeconds(timeBetweenParts);
            }
        }
    }
}
