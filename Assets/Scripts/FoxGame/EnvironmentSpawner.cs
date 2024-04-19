using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    public GameObject[] environmentPrefabs; // Prefabs for different environment elements
    //public GameObject leftTurnPrefab;
    //public GameObject rightTurnPrefab;
    //public GameObject sidewaysTurnPrefab;
    public Transform player; // Reference to the player's transform
    public float spawnDistance = 50f; // Distance ahead of the player to spawn new environment elements
    public float spawnPositionZ = 200f;
    //public float turnChance = 0.2f; // Probability of a turn being spawned

    private Vector3 lastSpawnedPosition;

    public int initialPoolSize = 4; // Initial size of the object pool
    private List<GameObject> objectPool = new List<GameObject>();

    void Start()
    {
        lastSpawnedPosition = player.position;
        InitializePool();
    }

    void InitializePool()
    {
        // Initialize environment element pool
        foreach (GameObject prefab in environmentPrefabs)
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                objectPool.Add(obj);
            }
        }

        //// Initialize turn prefabs pool
        //GameObject[] turnPrefabs = { leftTurnPrefab, rightTurnPrefab, sidewaysTurnPrefab };
        //foreach (GameObject prefab in turnPrefabs)
        //{
        //    for (int i = 0; i < initialPoolSize; i++)
        //    {
        //        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        //        obj.SetActive(false);
        //        objectPool.Add(obj);
        //    }
        //}
    }
    void Update()
    {
        // Spawn new environment elements
        if (player.position.z > lastSpawnedPosition.z + spawnDistance)
        {
            SpawnEnvironmentElement();
        }

        // Handle turns
        //if (Random.value < turnChance)
        //{
        //    SpawnTurn();
        //}
    }

    void SpawnEnvironmentElement()
    {
        // Get an object from the pool
        GameObject environmentObj = GetObjectFromPool(environmentPrefabs[Random.Range(0, environmentPrefabs.Length)]);
        if (environmentObj != null)
        {
            // Position the object and activate it
            Vector3 spawnPosition = lastSpawnedPosition + Vector3.forward * spawnPositionZ;
            environmentObj.transform.position = spawnPosition;
            environmentObj.SetActive(true);
            lastSpawnedPosition = spawnPosition;
        }
    }

    GameObject GetObjectFromPool(GameObject prefab)
    {
        // Find an inactive object of the specified prefab from the pool
        foreach (GameObject obj in objectPool)
        {
            if (obj.CompareTag(prefab.tag) && !obj.activeInHierarchy)
            {
                return obj;
            }
        }
        // If no inactive object is found, instantiate a new one and add it to the pool
        GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        newObj.SetActive(false);
        objectPool.Add(newObj);
        return newObj;
    }

    void ReturnObjectToPool(GameObject obj)
    {
        // Deactivate the object and return it to the pool
        obj.SetActive(false);
    }

    //void SpawnTurn()
    //{
    //    GameObject turnPrefab;

    //    // Randomly select a turn type
    //    int turnType = Random.Range(0, 3);
    //    switch (turnType)
    //    {
    //        case 0:
    //            turnPrefab = leftTurnPrefab;
    //            break;
    //        case 1:
    //            turnPrefab = rightTurnPrefab;
    //            break;
    //        case 2:
    //            turnPrefab = sidewaysTurnPrefab;
    //            break;
    //        default:
    //            turnPrefab = leftTurnPrefab; // Default to left turn
    //            break;
    //    }

    //    Vector3 spawnPosition = lastSpawnedPosition + Vector3.forward * spawnDistance;
    //    Instantiate(turnPrefab, spawnPosition, Quaternion.identity);
    //    lastSpawnedPosition = spawnPosition;
    //}
}
