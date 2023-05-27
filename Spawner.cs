using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour{
    public Transform spawnPosition; // The position to spawn the objects around
    private List<GameObject> spawnedObjects = new List<GameObject>(); // List of currently spawned objects

    void Start() {
        // Start spawning objects in a circular pattern around SpawnPosition
        StartCoroutine(SpawnObjects());

        // Check for spawned objects that have passed SpawnPosition and delete them
        StartCoroutine(DeleteObjectsPassedSpawnPosition());
    }

    IEnumerator SpawnObjects(){
        float spawnPosChange = 0;
        while (true) 
        {
            Vector3 spawnPos = spawnPosition.position;
            if(spawnPos.z >= spawnPosChange + 100){
                spawnPosChange = spawnPos.z;
                spawnPos.x = Random.Range(-38f, 38f);
                spawnPos.y = Random.Range(12f, 18f);
                spawnPos.z += 120f;

                Collider[] colliders = Physics.OverlapSphere(spawnPos, 2f);
                if (colliders.Length > 0) {
                    // If there are colliders, skip spawning a new object and try again in the next frame
                    yield return null;
                    continue;
                }

                var prefabRing = Resources.Load("Ring") as GameObject;
                GameObject newRingObject = Instantiate(prefabRing, spawnPos, Quaternion.Euler(0f, 90f, 0f));
                spawnedObjects.Add(newRingObject);
            }
            yield return new WaitForSeconds(0.1f); // Wait for 1 second before spawning the next object
        }
    }

    IEnumerator DeleteObjectsPassedSpawnPosition(){
        while (true) {
            // Filter the list of spawned objects to find those that have passed SpawnPosition on the z-axis
            List<GameObject> objectsToDelete = spawnedObjects.Where(obj => obj.transform.position.z < spawnPosition.position.z-51).ToList();

            // Destroy all objects that have passed SpawnPosition on the z-axis and remove them from the list of spawned objects
            foreach (GameObject obj in objectsToDelete){
                spawnedObjects.Remove(obj);
                Destroy(obj);
            }
            yield return null;
        }
    }
}