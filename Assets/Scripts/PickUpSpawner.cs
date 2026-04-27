using UnityEngine;
using System.Collections.Generic;

public class PickUpSpawner : MonoBehaviour
{
    public GameObject pickUpPrefab;
    public int count = 12;
    public float rangeX = 8f;
    public float rangeZ = 8f;
    public float minSpacing = 2.5f;

    void Start()
    {
        List<Vector3> spawnedPositions = new List<Vector3>();
        int maxAttempts = 100;

        for (int i = 0; i < count; i++)
        {
            Vector3 candidate = Vector3.zero;
            bool found = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                float x = Random.Range(-rangeX, rangeX);
                float z = Random.Range(-rangeZ, rangeZ);
                candidate = new Vector3(x, 0.5f, z);

                bool tooClose = false;
                foreach (Vector3 existing in spawnedPositions)
                {
                    if (Vector3.Distance(candidate, existing) < minSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                spawnedPositions.Add(candidate);
                Instantiate(pickUpPrefab, candidate, Quaternion.identity, transform);
            }
        }

        if (GameManager.instance != null)
            GameManager.instance.totalPickUps = spawnedPositions.Count;
    }
}
