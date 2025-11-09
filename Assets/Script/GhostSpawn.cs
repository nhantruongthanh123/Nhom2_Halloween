using System.Collections; 
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab; 
    public float minSpawnDelay = 1f; 
    public float maxSpawnDelay = 3f; 

    public float minY = -4f; 
    public float maxY = 4f;  

    void Start()
    {
        StartCoroutine(SpawnGhostRoutine());
    }

    IEnumerator SpawnGhostRoutine()
    {
       
        while (true)
        {
            
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPosition = new Vector3(10f, randomY, transform.position.z);

            Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        }
    }
}