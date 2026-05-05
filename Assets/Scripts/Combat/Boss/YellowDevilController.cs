using System.Collections;
using UnityEngine;

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
        public GameObject eyeObject;    // Eye child object (attach to the active base)

        [Header("Settings")]
        public float blobSpeed = 18f;
        public float timeBetweenParts = 0.12f;
        public float eyeOpenTime = 2.0f;

        private bool bossIsAtRight = true;

        void Start()
        {
            // Initial State: Right base is full, Left base is empty
            rightBaseSR.sprite = disassemblySprites[0];
            leftBaseSR.sprite = null;
            eyeObject.SetActive(false);

            StartCoroutine(BossMasterLoop());
        }

        IEnumerator BossMasterLoop()
        {
            while (true)
            {
                // PHASE 1: Attack (Show eye on the current active base)
                GameObject activeEye = bossIsAtRight ? eyeObject : null; // Logic to handle eye position
                if(bossIsAtRight) {
                    eyeObject.transform.SetParent(rightBaseSR.transform, false);
                    eyeObject.SetActive(true);
                } else {
                    eyeObject.transform.SetParent(leftBaseSR.transform, false);
                    eyeObject.SetActive(true);
                }

                yield return new WaitForSeconds(eyeOpenTime);
                eyeObject.SetActive(false);

                // PHASE 2: Sync Move
                if (bossIsAtRight)
                    yield return StartCoroutine(MoveSideToSide(rightBaseSR, leftBaseSR, rightPoints, leftPoints, true));
                else
                    yield return StartCoroutine(MoveSideToSide(leftBaseSR, rightBaseSR, leftPoints, rightPoints, false));

                bossIsAtRight = !bossIsAtRight;
                yield return new WaitForSeconds(1f);
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
            
                // 3. Move blob and update target base when it arrives
                StartCoroutine(MoveAndRebuild(blob, targetPoints[i].position, targetSR, i));

                // Wait before sending the next part
                yield return new WaitForSeconds(timeBetweenParts);
            }
        }

        IEnumerator MoveAndRebuild(GameObject blob, Vector3 destination, SpriteRenderer targetSR, int partIndex)
        {
            while (Vector3.Distance(blob.transform.position, destination) > 0.05f)
            {
                blob.transform.position = Vector3.MoveTowards(blob.transform.position, destination, blobSpeed * Time.deltaTime);
                yield return null;
            }

            // 4. Blob arrived: Destroy blob and update target sprite
            Destroy(blob);
        
            // Visual: Part appears on the target body
            // index 0 of reassemblySprites is '1 part added'
            if (partIndex < reassemblySprites.Length)
                targetSR.sprite = reassemblySprites[partIndex];
        }
    }
}