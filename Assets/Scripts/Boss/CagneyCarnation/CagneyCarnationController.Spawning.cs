using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Boss.CagneyCarnation
{
    public partial class CagneyCarnationController
    {
        // ====================== Creating Object ===================
        public void OnObjectSpawnEvent()
        {
            int randomIndex = Random.Range(0, 2);
            if (randomIndex == 0)
                SpawnBoomerang();
            else
                StartCoroutine(SpawnAcorn());
        }

        //================== Boomerang ==================
        private GameObject SpawnBoomerang()
        {
            if (boomerangPrefab == null || boomerangWaypoints == null || boomerangWaypoints.Length == 0) return null;

            // Spawn at first waypoint
            GameObject boomerang = Instantiate(boomerangPrefab, boomerangWaypoints[0].position, Quaternion.identity);

            // Set waypoints on the prefab
            ProjectilePathFollower follower = boomerang.GetComponent<ProjectilePathFollower>();
            if (follower != null)
            {
                follower.SetWaypointPath(boomerangWaypoints);
            }
            return boomerang;
        }

        //=================== Acorn =========================
        private IEnumerator SpawnAcorn()
        {
            if (acornPrefab == null || acornSpawnPoint == null) yield break;

            List<LinearMovement> acorns = new List<LinearMovement>();

            // Spawn all acorns
            foreach (Transform spawnPoint in acornSpawnPoint)
            {
                GameObject acorn = Instantiate(acornPrefab, spawnPoint.position, Quaternion.identity);

                LinearMovement linearMovement = acorn.GetComponent<LinearMovement>();

                if (linearMovement != null)
                {
                    linearMovement.enabled = false;
                    acorns.Add(linearMovement);
                }
            }

            // Enable one by one
            foreach (LinearMovement linearMovement in acorns)
            {
                yield return new WaitForSeconds(acornSpawnInterval);

                if (linearMovement != null)
                    linearMovement.enabled = true;
            }
        }

        //======================POLLEN========================
        public void OnPollenSpawnEvent()
        {
            SpawnPollen();
        }

        private GameObject SpawnPollen()
        {
            GameObject pollen = Instantiate(pollenWaypointPrefab, pollenSpawnPoint.position, Quaternion.identity);
            return pollen;
        }

        IEnumerator SpawnSeedsRoutine()
        {
            if (seedPrefabs == null || seedPrefabs.Length == 0 || seedSpawnPoint == null) yield break;

            float elapsedTime = 0f;

            yield return new WaitForSeconds(1f);

            while (elapsedTime < seedFiringActiveTime)
            {
                int randomIndex = Random.Range(0, seedPrefabs.Length);
                GameObject seed = Instantiate(seedPrefabs[randomIndex], seedSpawnPoint.position, Quaternion.identity);
                float randomX = Random.Range(minRangeX, maxRangeX);
                seed.transform.position += new Vector3(randomX, 0f, 0f);
                Destroy(seed, 5f); // Destroy seeds after 5 seconds to clean up

                // Wait for spawn interval before spawning next seed
                yield return new WaitForSeconds(seedSpawnInterval);
                elapsedTime += seedSpawnInterval;
            }
        }

        public void OnSeedsFiringEvent()
        {
            StartCoroutine(SpawnSeedsRoutine());
        }
    }
}

