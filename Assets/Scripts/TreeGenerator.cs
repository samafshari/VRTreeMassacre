using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public GameObject[] treePrefabs;
    int initialSpawnCount = 90;
    float minSpawnTime = 0.04f;
    float maxSpawnTime = 0.3f;
    float amplitude = 50;
    float initZAmplitude = 900;
    float nextSpawn;
    float t;
    float maxSpeedBoost = 3;
    float speedBoost = 1;
    MoveForward terrain;

    // Start is called before the first frame update
    void Start()
    {
        terrain = FindObjectOfType<TerrainController>().GetComponent<MoveForward>();
        for (int i = 0; i < initialSpawnCount; i++)
            Spawn(Random.Range(-initZAmplitude, 0));
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        maxSpawnTime -= Time.deltaTime * 0.01f;
        if (maxSpawnTime < minSpawnTime)
            maxSpawnTime = minSpawnTime;
        if (t > nextSpawn) Spawn();

        speedBoost += Time.deltaTime * 0.01f;
        if (speedBoost > maxSpeedBoost)
            speedBoost = maxSpeedBoost;
    }

    void Spawn(float z = 0)
    {
        var index = Random.Range(0, treePrefabs.Length);
        var prefab = treePrefabs[index];
        var instance = Instantiate(prefab, transform);
        instance.transform.localPosition = new Vector3(
            Random.Range(-amplitude, amplitude),
            0,
            z);
        nextSpawn = Random.Range(minSpawnTime, maxSpawnTime);
        t = 0;

        var tree = instance.GetComponent<MoveForward>();
        tree.velocity *= speedBoost;
        terrain.velocity = tree.velocity;
    }
}
