/*
 * Name: Quinn Farre;;
 * Date: 2/23/2024
 * Desc: Attach to an object to spawn ads across the player's screen. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    [Header("Assets")]
    [Tooltip("All of the ads to spawn")]
    [SerializeField] private GameObject[] ads;

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
        RectTransform[] positions = adHolder.GetComponentsInChildren<RectTransform>();
        RectTransform spawnPosition = null;

        // Choose a random spawn position
        int pos = Random.Range(0, adHolder.transform.childCount);
        
        // Keep searching for a valid spawn pos until all positions have been looped through
        for (int i = 0; i < adHolder.transform.childCount; i++)
        {
            // See if the current position is already occupied
            if (!Physics2D.OverlapCircle(positions[pos].position, 25))
            {
                // If it's not, set the spawn position to that one
                spawnPosition = positions[pos];
                break;
            }
            // Otherwise, incriment the current position by 1
            pos = (pos < adHolder.transform.childCount) ? pos + 1 : 0;
        }

        // If no valid spawn position was found, then give up.
        if (spawnPosition == null)
            return;

        // Choose a random ad
        GameObject toSpawn = ads[Random.Range(0, ads.Length)];

        // Choose a random offset
        Vector3 offset = Random.insideUnitCircle * adSpawnVariation;

        // Spawn the ad with the ad spawn position as a parent
        GameObject instance = Instantiate(toSpawn, adHolder.GetComponentInParent<Canvas>().transform);
        // Position the ad
        instance.transform.position = spawnPosition.transform.position + offset;
    }
}
