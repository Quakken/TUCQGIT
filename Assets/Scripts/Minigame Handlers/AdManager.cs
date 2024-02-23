/*
 * Name: Quinn Farre;;
 * Date: 2/23/2024
 * Desc: Attach to an object to spawn ads across the player's screen. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    [Header("Assignables")]
    [Tooltip("Game object that holds all ad positions")]
    [SerializeField] private GameObject adHolder;

    [Header("Config")]
    [Tooltip("The minimum amount of time between ad spawns")]
    [SerializeField][Min(0)] private float minSpawnDelay;
    [Tooltip("The maximum amount of time between ad spawns")]
    [SerializeField][Min(0)] private float maxSpawnDelay;
    [Tooltip("How far from the center of each ad position an ad can spawn. Increase for more variation")]
    [SerializeField][Range(0, 50)] private float adSpawnVariation = 25f;

    // Timer that counts down the time between ad spanws
    private float spawnTimer;

    private void Start()
    {
        // Clamp spawn delay
        if (minSpawnDelay > maxSpawnDelay)
            maxSpawnDelay = minSpawnDelay;

        // Initialize spawn timer
        spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void Update()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);
            SpawnAd();
        }
    }

    private void SpawnAd()
    {
        // Choose a random spawn position
        RectTransform[] positions = adHolder.GetComponentsInChildren<RectTransform>();
        // Keep searching for a valid spawn pos until all positions have been looped through
        RectTransform spawnPosition = null;
        for (int i = 0; i < positions.Length - 1; i++)
        {
            spawnPosition = positions[Random.Range(1, positions.Length - 1)];
        }

        // Choose a random ad
        GameObject toSpawn = ads[Random.Range(0, ads.Length)];

        // Choose a random offset
        Vector3 offset = Random.insideUnitCircle * adSpawnVariation;

        // Spawn the ad with the ad holder's canvas as a parent
        GameObject instance = Instantiate(toSpawn, adHolder.GetComponentInParent<Canvas>().transform);
        // Position the ad
        instance.GetComponent<RectTransform>().position = spawnPosition.position + offset;
    }

    [Header("Assets")]
    [Tooltip("All of the ads to spawn")]
    [SerializeField] private GameObject[] ads;
}
