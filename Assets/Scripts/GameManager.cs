using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // timer
    public float totalTime;
    public List<GameObject> plants = new List<GameObject>();
    public int minPlants = 3;
    public GameObject plantPrefab;
    
    void Start()
    {
        foreach (GameObject plant in GameObject.FindGameObjectsWithTag("Plant"))
        {
            plants.Add(plant);
        }
    }
    
    void Update()
    {
        
        plants.RemoveAll(item => item == null);
        float random = Random.Range(0f, 10f);

        if (random >= 9.997f)
        {
            SpawnPlant();
        }
        
        while (plants.Count < minPlants)
        {
            SpawnPlant();
        }
        
    }

    void SpawnPlant()
    {
        Debug.Log("spawning plant");
        Vector2 spawnPosition = new Vector2(Random.Range(-9f, 9f), Random.Range(-9f, 9f));
        GameObject newPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity);
        plants.Add(newPlant);
    }
}
