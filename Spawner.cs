using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour{
    public Transform spawnPosition; // The position to spawn the objects around
    public float distance = 50f; // Distance from SpawnPosition to spawn the left and right wall objects
    public float SpawnRate; // The rate at which to spawn objects in seconds
    public List<GameObject> objectsToDelete; // Objects already in scene to be Deleted

    private List<GameObject> spawnedObjects = new List<GameObject>(); // List of currently spawned objects

    void Start() {
        foreach (GameObject obj in objectsToDelete){
            spawnedObjects.Add(obj);
        }

        // Start spawning objects in a circular pattern around SpawnPosition
        StartCoroutine(SpawnObjects());

        // Check for spawned objects that have passed SpawnPosition and delete them
        StartCoroutine(DeleteObjectsPassedSpawnPosition());
    }

    private const string ObjectPrefix = "CaveRockPack1_";

    private class ObjectInfo {
        public string Name { get; set; }
        public float Scale { get; set; }
        public float Height { get; set; }
    }

    private List<ObjectInfo> objectList = new List<ObjectInfo>() {
        new ObjectInfo() { Name = "CaveRockPack1_High1", Scale = 0.5f, Height = 22f },
        new ObjectInfo() { Name = "CaveRockPack1_High2", Scale = 1f, Height = 22f },
        new ObjectInfo() { Name = "CaveRockPack1_High3", Scale = 0.7f, Height = 15f },
        new ObjectInfo() { Name = "CaveRockPack1_High4", Scale = 1f, Height = 21f },
        new ObjectInfo() { Name = "CaveRockPack1_High5", Scale = 1f, Height = 22f },
        new ObjectInfo() { Name = "CaveRockPack1_Low1", Scale = 0.7f, Height = 10f },
        new ObjectInfo() { Name = "CaveRockPack1_Low2", Scale = 1f, Height = 8f },
        new ObjectInfo() { Name = "CaveRockPack1_Low3", Scale = 0.5f, Height = 10f },
        new ObjectInfo() { Name = "CaveRockPack1_Low4", Scale = 1f, Height = 10f },
        new ObjectInfo() { Name = "CaveRockPack1_Low5", Scale = 1f, Height = 8f }
    };

    IEnumerator SpawnObjects(){
        float spawnPosChange1 = 0;
        float spawnPosChange2 = 0;
        while (true) 
        {
            // Calculate the spawn position using the angle and distance from the spawn position
            Vector3 spawnPos = spawnPosition.position;
            if (spawnPos.z >= spawnPosChange1 + 50){
                spawnPosChange1 = spawnPos.z;
                spawnPos.x = 0f;
                spawnPos.y = 0f;
                spawnPos.z += 120f;

                var prefab1 = Resources.Load("floor") as GameObject;
                // Create a new object instance at the specified position
                Vector3 floorSpawnPos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
                GameObject newFloorObject = Instantiate(prefab1, floorSpawnPos, Quaternion.identity);
                spawnedObjects.Add(newFloorObject);

                var prefab2 = Resources.Load("ceiling") as GameObject;
                // Create a new object instance at the specified position for ceiling object
                Vector3 ceilingSpawnPos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
                GameObject newCeilingObject = Instantiate(prefab2, ceilingSpawnPos, Quaternion.identity);
                spawnedObjects.Add(newCeilingObject);

                var prefab3 = Resources.Load("leftWall") as GameObject;
                // Create new object instances for left and right walls
                Vector3 leftSpawnPos = new Vector3(-distance, spawnPos.y, spawnPos.z);
                GameObject newLeftWallObject = Instantiate(prefab3, leftSpawnPos, Quaternion.identity);
                spawnedObjects.Add(newLeftWallObject);

                var prefab4 = Resources.Load("rightWall") as GameObject;
                Vector3 rightSpawnPos = new Vector3(distance, spawnPos.y, spawnPos.z);
                GameObject newRightWallObject = Instantiate(prefab4, rightSpawnPos, Quaternion.identity);
                spawnedObjects.Add(newRightWallObject);
            }
            if(spawnPos.z >= spawnPosChange2 + 100){
                spawnPosChange2 = spawnPos.z;
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

            // Only spawn a new object if the number of spawned objects is below a certain threshold
            if (spawnedObjects.Count < 35){
                float xCoord = Random.Range(-50f, 50f);
                float zCoord = Random.Range(90f, 200f);
                // Calculate the spawn position using the angle and distance from the spawn position
                spawnPos = spawnPosition.position; // Reuse the 'spawnPos' variable
                spawnPos.x = xCoord;
                spawnPos.z += zCoord;
                
                // Check if there are any colliders within a certain radius of the spawn position
                Collider[] colliders = Physics.OverlapSphere(spawnPos, 2f);
                if (colliders.Length > 0){
                    // If there are colliders, skip spawning a new object and try again in the next frame
                    yield return null;
                    continue;
                }
                
                int randomInt = Random.Range(1, 6);
                string highLow = (Random.Range(1, 6) > 2 ? "High" : "Low");

                // Get a random object name from the list
                string objectName = "CaveRockPack1_" + highLow + randomInt;
                
                // Load the prefab from the resources folder
                var prefab = Resources.Load(objectName) as GameObject;

                // Create a new object instance at the specified position with a random rotation
                GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.Euler(Random.Range(0f, 5f), Random.Range(0f, 360f), 0f));

                // Get the object info from the object list based on the object's name
                ObjectInfo objectInfo = objectList.Find(obj => obj.Name == objectName);

                // Adjust the scale and height of the new object based on the object info
                if (objectInfo != null){
                    newObject.transform.localScale *= objectInfo.Scale;
                    newObject.transform.position = new Vector3(newObject.transform.position.x, objectInfo.Height, newObject.transform.position.z);
                }

                // Add the new object to the list of spawned objects
                spawnedObjects.Add(newObject);
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

            // Debug the number of objects that have passed SpawnPosition on the z-axis
            int objectsPassed = spawnedObjects.Count(obj => obj.transform.position.z < spawnPosition.position.z-60);

            yield return null;
        }
    }
}