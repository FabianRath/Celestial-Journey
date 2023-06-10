using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading;

public class CaveGenerator : MonoBehaviour{
    public Transform spawnPosition; 

    public List<GameObject> objectsToDelete;

    Vector3 spawnPos;

    private int mapSizeX = 8;
    private int mapSizeY = 8;
    private int mapSizeZ = 8;

    private int fillPercentage = 62;

    private int filtering = 11;

    public Material mat;

    private int sizeX;
    private int sizeY;
    private int sizeZ;

    private int[,,] map;

    private List<GameObject> chunksList = new List<GameObject>(); 

    private int posX = -65;
    private int posY = -55;
    private int posZ = 260;

    private int entranceTopLeftY;
    private int entranceTopLeftZ;

    private int exitTopLeftY;
    private int exitTopLeftZ;

    private int counter = 1;

    List<Vector3> vertices = new List<Vector3>();   // stores chunk vertices
    List<int> triangles = new List<int>();          // stores chunk faces

    private int newVolume;

    private void Awake(){
        sizeX = mapSizeX * 8;
        sizeY = mapSizeY * 8;
        sizeZ = mapSizeZ * 8;

        entranceTopLeftY = (sizeY/4) + (sizeY/8);
        entranceTopLeftZ = (sizeZ/4) + (sizeZ/8);

        foreach (GameObject obj in objectsToDelete){
            chunksList.Add(obj);
        }

        StartCoroutine(GenerateChunk());
        StartCoroutine(DeleteChunks());
    }

    IEnumerator GenerateChunk(){
        while(true){
            if(chunksList.Count <= 3){
                exitTopLeftY = Random.Range(10, 38);
                exitTopLeftZ = Random.Range(10, 38);

                yield return StartCoroutine(GenerateMap());

                entranceTopLeftY = exitTopLeftY;
                entranceTopLeftZ = exitTopLeftZ;

                posZ += 255;
                
                //Wait for ship to fly past spawn zone
                if(counter == 1){
                    while(spawnPosition.position.z < 70f){
                        yield return null;
                    }
                }
                counter++;
            }
            yield return null;
        }
    }

   private IEnumerator GenerateMap(){
        map = new int[sizeX, sizeY, sizeZ];

        int[,] entranceExit = generateEntranceExit();

        for (int y = 0; y < sizeY/4; y++){
            for (int z = 0; z < sizeZ/4; z++){
                map[0, y + exitTopLeftY, z + exitTopLeftZ] = entranceExit[y, z];
                map[sizeX - 1, y + entranceTopLeftY, z + entranceTopLeftZ] = entranceExit[y, z];
            }
        }

        // Initialize map
        for (int x = 0; x < sizeX; x++){
            for (int y = 0; y < sizeY; y++){
                for (int z = 0; z < sizeZ; z++){
                    if (map[x, y, z] == 0){ //keeping entrance and exit holes open
                        if (x == 0 || x == sizeX - 1 || y == 0 || y == sizeY - 1 || z == 0 || z == sizeZ - 1){
                            map[x, y, z] = 1; // Edges to solid
                        }
                        else{
                            map[x, y, z] = Random.Range(0, 100) < fillPercentage ? 1 : 0; // Random fill (1 or 0)
                        }
                    }
                }
            }    
        }

        int[,,] filterMap = new int[sizeX, sizeY, sizeZ];
        for (int x = 0; x < sizeX; x++){
            for (int y = 0; y < sizeY; y++){
                for (int z = 0; z < sizeZ; z++){
                    filterMap[x, y, z] = map[x, y, z];
                }
            }
        }

        for (int i = 0; i < filtering; i++){
            for (int x = 1; x < sizeX - 1; x++){
                for (int y = 1; y < sizeY - 1; y++){
                    for (int z = 1; z < sizeZ - 1; z++){
                        int count = 0;
                        for (int xn = -1; xn < 2; xn++){
                            for (int yn = -1; yn < 2; yn++){
                                for (int zn = -1; zn < 2; zn++){
                                    if (xn == 0 && yn == 0 && zn == 0){
                                        continue;
                                    }
                                    else{
                                        count += map[x + xn, y + yn, z + zn] == 1 ? 1 : 0;
                                    }
                                }
                            }
                        }
                        filterMap[x, y, z] = count > 15 ? 1 : 0;
                    }
                }
                if(counter != 1 && x % 5 == 0 && PlayerPrefs.GetInt("Booster") != 0){
                    yield return null;
                }
            }

            for (int x = 0; x < sizeX; x++){
                for (int y = 0; y < sizeY; y++){
                    for (int z = 0; z < sizeZ; z++){
                        map[x, y, z] = filterMap[x, y, z];
                    }
                }
                if(counter != 1 && x % 10 == 0){
                    yield return null;
                }
            }
        }

        int cur = 2;
        int max = 2;
        int volume = 0;

        for (int x = 0; x < sizeX; x++){
            for (int y = 0; y < sizeY; y++){
                for (int z = 0; z < sizeZ; z++){
                    if (map[x, y, z] == 0){
                        StartCoroutine(FloodFill(x, y, z, cur));
                        if (newVolume > volume){
                            volume = newVolume;
                            max = cur;
                        }
                        cur++;
                    }
                }
            }
            if(counter != 1 && x % 10 == 0){
                yield return null;
            }
        }

        for (int x = 0; x < sizeX; x++){
            for (int y = 0; y < sizeY; y++){
                for (int z = 0; z < sizeZ; z++){
                    if (map[x, y, z] != 1 && map[x, y, z] != max){
                        map[x, y, z] = 1;
                    }
                    else if (map[x, y, z] == max){
                        map[x, y, z] = 0;
                    }
                }
            }
            if(counter != 1 && x % 10 == 0){
                yield return null;
            }
        }

        // Wait for both methods to finish
        yield return StartCoroutine(BuildMap());
    }

    private IEnumerator BuildMap(){
        GameObject chunks = new GameObject("Chunks");   // chunk parent
        Transform chunksTransform = chunks.transform;
        
        chunks.transform.parent = gameObject.transform;

        // loops for chunk count
        for (int cx = 0; cx < (sizeX / 8); cx++){
            for (int cy = 0; cy < sizeY / 8; cy++){
                for (int cz = 0; cz < sizeZ / 8; cz++){
                    GameObject chunk = new GameObject("Chunk " + cx + "." + cy + "." + cz); // chunk child
                    chunk.transform.parent = chunks.transform;

                    // Applying tags for later collision detection
                    if (cy == 0 || cy == sizeY / 8 - 1 || cz == 0 || cz == sizeZ / 8 - 1){
                        chunk.tag = "Boundary";
                    }
                    else{
                        chunk.tag = "Rock";
                    }

                    // adds required components
                    chunk.AddComponent<MeshFilter>();
                    chunk.AddComponent<MeshRenderer>();
                    MeshCollider chunkCollider = chunk.AddComponent<MeshCollider>();
                    chunkCollider.convex = true;
                    chunkCollider.isTrigger = true;

                    Mesh mesh = new Mesh();

                    vertices.Clear();
                    triangles.Clear();

                    int i = 0;  // last vertice added

                    // loops for chunk size
                    for (int x = cx * 8; x < cx * 8 + 8; x++){
                        for (int y = cy * 8; y < cy * 8 + 8; y++){
                            for (int z = cz * 8; z < cz * 8 + 8; z++){
                                // only creates mesh where blocks exist
                                if (map[x, y, z] == 1){
                                    // checks if x face is drawn
                                    if (x - 1 >= 0){
                                        if (map[x - 1, y, z] == 0){
                                            vertices.Add(new Vector3(x, y, z));             // 0
                                            vertices.Add(new Vector3(x, y + 1, z));         // 1
                                            vertices.Add(new Vector3(x, y + 1, z + 1));     // 2
                                            vertices.Add(new Vector3(x, y, z + 1));         // 3

                                            triangles.Add(i + 2);   // -x
                                            triangles.Add(i + 1);
                                            triangles.Add(i);

                                            triangles.Add(i);       // -x
                                            triangles.Add(i + 3);
                                            triangles.Add(i + 2);

                                            i += 4; // new vertice index
                                        }
                                    }

                                    if (x + 1 < sizeX){
                                        if (map[x + 1, y, z] == 0){
                                            vertices.Add(new Vector3(x + 1, y, z + 1));     // 0
                                            vertices.Add(new Vector3(x + 1, y + 1, z + 1)); // 1
                                            vertices.Add(new Vector3(x + 1, y + 1, z));     // 2
                                            vertices.Add(new Vector3(x + 1, y, z));         // 3

                                            triangles.Add(i + 3);   // x
                                            triangles.Add(i + 2);
                                            triangles.Add(i + 1);

                                            triangles.Add(i + 1);   // x
                                            triangles.Add(i);
                                            triangles.Add(i + 3);

                                            i += 4; // new vertice index
                                        }
                                    }

                                    // checks if y face is drawn
                                    if (y - 1 >= 0){
                                        if (map[x, y - 1, z] == 0){
                                            vertices.Add(new Vector3(x, y, z));             // 0
                                            vertices.Add(new Vector3(x, y, z + 1));         // 1
                                            vertices.Add(new Vector3(x + 1, y, z + 1));     // 2
                                            vertices.Add(new Vector3(x + 1, y, z));         // 3

                                            triangles.Add(i + 1);   // -y
                                            triangles.Add(i);
                                            triangles.Add(i + 3);

                                            triangles.Add(i + 3);   // -y
                                            triangles.Add(i + 2);
                                            triangles.Add(i + 1);

                                            i += 4; // new vertice index
                                        }
                                    }

                                    if (y + 1 < sizeY){
                                        if (map[x, y + 1, z] == 0){
                                            vertices.Add(new Vector3(x, y + 1, z));         // 0
                                            vertices.Add(new Vector3(x, y + 1, z + 1));     // 1
                                            vertices.Add(new Vector3(x + 1, y + 1, z + 1)); // 2
                                            vertices.Add(new Vector3(x + 1, y + 1, z));     // 3

                                            triangles.Add(i + 1);   // y
                                            triangles.Add(i + 2);
                                            triangles.Add(i + 3);

                                            triangles.Add(i + 3);   // y
                                            triangles.Add(i);
                                            triangles.Add(i + 1);

                                            i += 4; // new vertice index
                                        }
                                    }

                                    // checks if z face is drawn
                                    if (z + 1 < sizeZ){
                                        if (map[x, y, z + 1] == 0){
                                            vertices.Add(new Vector3(x, y + 1, z + 1));     // 0
                                            vertices.Add(new Vector3(x, y, z + 1));         // 1
                                            vertices.Add(new Vector3(x + 1, y, z + 1));     // 2
                                            vertices.Add(new Vector3(x + 1, y + 1, z + 1)); // 3

                                            triangles.Add(i);   // z
                                            triangles.Add(i + 1);
                                            triangles.Add(i + 2);

                                            triangles.Add(i + 2);   // z
                                            triangles.Add(i + 3);
                                            triangles.Add(i);

                                            i += 4; // new vertice index
                                        }
                                    }

                                    if (z - 1 >= 0){
                                        if (map[x, y, z - 1] == 0){
                                            vertices.Add(new Vector3(x, y, z));             // 0
                                            vertices.Add(new Vector3(x, y + 1, z));         // 1
                                            vertices.Add(new Vector3(x + 1, y + 1, z));     // 2
                                            vertices.Add(new Vector3(x + 1, y, z));         // 3

                                            triangles.Add(i + 3);   // -z
                                            triangles.Add(i);
                                            triangles.Add(i + 1);

                                            triangles.Add(i + 1);   // -z
                                            triangles.Add(i + 2);
                                            triangles.Add(i + 3);

                                            i += 4; // new vertice index
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // finalizes mesh
                    mesh.Clear();

                    mesh.SetVertices(vertices);
                    mesh.SetTriangles(triangles, 0);

                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();

                    chunk.GetComponent<MeshFilter>().sharedMesh = mesh;
                    chunk.GetComponent<Renderer>().material = mat;
                    chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
                }
            }
        }
        chunksTransform.rotation = Quaternion.Euler(0, 90, 0);
        
        chunksTransform.localScale = new Vector3(4, 2, 2);

        chunksTransform.position = new Vector3(posX, posY, posZ);

        chunksList.Add(chunks);
        yield return null;
    }

    IEnumerator DeleteChunks(){
        while(true){
            // Delete chunks only when they are 300 behind the spaceship
            List<GameObject> chunksDelete = chunksList.Where(obj => obj.transform.position.z < spawnPosition.position.z - 300).ToList();

            foreach (GameObject obj in chunksDelete){
                Destroy(obj);
                chunksList.Remove(obj);
            }
            yield return null;
        }
    }

    IEnumerator FloodFill(int x, int y, int z, int fill){
        // volume of room
        int volume = 1;

        // uses BFS flood fill algorithm to fill each room in cave with the room index.

        List<Vector3Int> list = new List<Vector3Int>();

        list.Add(new Vector3Int(x, y, z));

        while(list.Count != 0){
            Vector3Int node = list[list.Count - 1];

            list.RemoveAt(list.Count - 1);

            int a = node.x;
            int b = node.y;
            int c = node.z;

            map[a, b, c] = fill;

            for (int nx = -1; nx < 2; nx++){
                for (int ny = -1; ny < 2; ny++){
                    for (int nz = -1; nz < 2; nz++){
                        if (Mathf.Abs(nx) + Mathf.Abs(ny) + Mathf.Abs(nz) != 1){
                            continue;
                        }
                        if (map[a + nx, b + ny, c + nz] == 0){
                            list.Add(new Vector3Int(a + nx, b + ny, c + nz));
                            volume++;
                        }
                    }
                }
            }
        }

        // returns the volume of the room filled
        newVolume = volume;
        if(counter != 1){
            yield return null;
        }
    }

    private int[,] generateEntranceExit(){
        int[,] array = new int[sizeY/4, sizeZ/4];

        int centerX = 8;
        int centerY = 8;
        int radius = 7;

        CreateCircle(array, centerX, centerY, radius);

        return array;
    }

    static void CreateCircle(int[,] array, int centerX, int centerY, int radius){
        int x = radius;
        int y = 0;
        int decisionOver2 = 1 - x;

        while (y <= x){
            for (int i = centerX - x; i <= centerX + x; i++){
                SetPixel(array, i, centerY + y);
                SetPixel(array, i, centerY - y);
            }

            for (int i = centerX - y; i <= centerX + y; i++){
                SetPixel(array, i, centerY + x);
                SetPixel(array, i, centerY - x);
            }

            y++;
            if (decisionOver2 <= 0){
                decisionOver2 += 2 * y + 1;
            }
            else{
                x--;
                decisionOver2 += 2 * (y - x) + 1;
            }
        }
    }

    static void SetPixel(int[,] array, int x, int y){
        if (x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1)){
            array[x, y] = 2;
        }
    }
}