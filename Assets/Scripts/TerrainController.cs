using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    public float terrainSize = 1000;
    public GameObject[] terrainPrefabs;

    int i = 1;
    Queue<GameObject> currentTerrains = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            currentTerrains.Enqueue(transform.GetChild(i).gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.z < -i * terrainSize)
        {
            i++;
            SpawnNewTerrain();
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SpawnNewTerrain();
    }

    void SpawnNewTerrain()
    {
        var position = new Vector3(-500, 0, i * terrainSize);
        var terrain = Instantiate(GetRandomTerrain(), transform);
        if (currentTerrains.Count > 0) Destroy(currentTerrains.Dequeue());
        currentTerrains.Enqueue(terrain);
        terrain.transform.localPosition = position;
    }

    GameObject GetRandomTerrain()
    {
        var index = Random.Range(0, terrainPrefabs.Length);
        return terrainPrefabs[index];
    }
}
