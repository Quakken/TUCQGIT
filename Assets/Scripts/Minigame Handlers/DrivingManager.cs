/*
 * Name: Quinn Farrell, Jack Zhao
 * Date: 11/27/2023
 * Desc: A manager class for the driving minigame. Handles things like player movement, car spawning, road spawning, as well as game over and win conditions.
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public class DrivingManager : MonoBehaviour
{
    [Header("Road Config")]
    [Tooltip("How many lanes the level generates")]
    [SerializeField][Range(2, 10)] int lanes = 2;
    [Tooltip("How many lanes of rainslicked concreet the level generates")]
    [SerializeField] int floor = 10;
    [Tooltip("How many 'rows' of roads to spawn. Pretty much how long the level should be")]
    [SerializeField] int length = 50;

    [Header("Car Config")]
    [Tooltip("How many rows to wait before spawning the first car")]
    [SerializeField][Min(3)] int startDelay = 10;
    [Tooltip("The number of cars to spawn. Increase this number to increase difficulty")]
    [SerializeField] int carsToSpawn = 50;

    [Header("Scenery Config")]
    [Tooltip("How far from the edge of the road the scenery can spawn")]
    [SerializeField] float xOffsetVariance;
    [Tooltip("How big of a gap there should be between all scenery pieces")]
    [SerializeField][Range(0, 50)] float minSceneryGap;
    [Tooltip("How big the gap between scenery pieces CAN be")]
    [SerializeField][Range(0, 100)] float maxSceneryGap;
    [Tooltip("How many rows to generate")]
    [SerializeField] int rows = 3;
    [Tooltip("How much distance between rows")]
    [SerializeField] float rowGap = 3;

    [Space]
    [Tooltip("How many paths to spawn that will guarantee a way to the end of the level")]
    [SerializeField][Min(1)] int paths = 2;

    [Header("Assets")]
    [SerializeField] GameObject[] carPrefabs;
    [SerializeField] GameObject roadPrefab;
    [SerializeField] GameObject floorPrefab;
    [Tooltip("Objects to spawn along the side of the road")]
    [SerializeField] GameObject[] scenery;
    [Tooltip("Objects to spawn farther away")]
    [SerializeField] GameObject[] sceneryfar;
    [Tooltip("minimum offset of far objects")]
    [SerializeField] float farOffsetNear = 30;
    [Tooltip("maximum offset of far objects")]
    [SerializeField] float farOffsetFar = 150;

    GameObject[,] roads;
    GameObject[,] cars;

    [Header("Events")]
    [SerializeField] UnityEvent onCrash;
    [SerializeField] UnityEvent onWin;

    /*--------------------Unity Functions--------------------*/

    private void Start()
    {
        // Cap the start delay to the length of the road (when it will not spawn any cars?)
        startDelay = Mathf.Clamp(startDelay, 3, length);

        // Cap the scenery gap stuff
        maxSceneryGap = Mathf.Clamp(maxSceneryGap, minSceneryGap, float.MaxValue);

        GenerateRoad();

        GenerateScenery();
    }

    /*--------------------Level Management Functions--------------------*/

    private void GenerateRoad()
    {
        // Create the road

        GameObject roadParent = new GameObject("Road Holder");

        roads = new GameObject[length, lanes];

        for (int y = 0; y < length; y++) // Loop through the rows of the road
        {
            for (int x = 0; x < lanes; x++) // Loop through the columns of the road
            {
                // Spawn a road tile
                Vector3 posToSpawn = new Vector3(x * roadPrefab.transform.lossyScale.x, 0, y * roadPrefab.transform.lossyScale.y);
                GameObject instance = Instantiate(roadPrefab, posToSpawn, roadPrefab.transform.rotation, roadParent.transform);
                roads[y, x] = instance;
            }
        }

        // Add floor

        float buildLocation = 0;
        while (buildLocation < length * roadPrefab.transform.lossyScale.x)
        {
            Instantiate(floorPrefab, new Vector3(0, 0, buildLocation), floorPrefab.transform.rotation);
            buildLocation += floorPrefab.transform.lossyScale.z;
        }

        // Spawn the cars

        GameObject carParent = new GameObject("Car Holder");

        cars = new GameObject[length - startDelay, lanes];

        for (int y = startDelay; y < length; y++) // Loop through all the rows of the road that come after the start delay
        {
            for (int x = 0; x < lanes; x++) // Loop through the columns of the road
            {
                // Spawn a car there
                GameObject toSpawn = carPrefabs[Random.Range(0, carPrefabs.Length)];

                GameObject instance = Instantiate(toSpawn, roads[y, x].transform.position, toSpawn.transform.rotation, carParent.transform);
                cars[y - startDelay, x] = instance;
            }
        }

        // Clear a path(s) to the end

        for (int p = 0; p < paths; p++)
        {
            // Start a walker out at a random position at the start of the wall of cars

            int lane = Random.Range(0, lanes);

            // Destroy that car

            if (cars[0, lane] != null)
                Destroy(cars[0, lane]);

            // Walk down the massive wall of cars, deleting anything in the way

            for (int y = 0; y < length - startDelay; y++)
            {
                // Clear out the car at the current position

                Destroy(cars[y, lane]);

                // Choose a direction to move from here
                // 0 = left
                // 1 = straight ahead
                // 2 = right
                int dir = 1;

                // Clamp the direction so that the walker can't walk off the road
                if (lane <= 0)
                    dir = Random.Range(1, 3);
                else if (lane >= lanes - 1)
                    dir = Random.Range(0, 2);
                else
                    dir = Random.Range(0, 3);

                switch (dir)
                {
                    case 0:
                        // Move left next time
                        lane -= 1;
                        Destroy(cars[y, lane]);
                        break;
                    case 1:
                        // Do nothing
                        break;
                    case 2:
                        // Move right next time
                        lane += 1;
                        Destroy(cars[y, lane]);
                        break;
                    default:
                        Debug.LogError("ERROR: How did this even happen? Check the driving manager script, this shouldn't be possible");
                        break;
                }
            }
        }

        // Count up all cars still left on the road

        int carCount = 0;

        for (int y = 0; y < length - startDelay; y++)
        {
            for (int x = 0; x < lanes; x++)
            {
                if (cars[y, x] != null)
                    carCount += 1;
            }
        }

        // If the amount of cars on the road is greater than the amount of cars to spawn, then get rid of cars at random until the target has been reached
        
        while (carCount - 1 > carsToSpawn)
        {
            int x = Random.Range(0, lanes);
            int y = Random.Range(0, length - startDelay);

            if (cars[y, x] != null)
            {
                DestroyImmediate(cars[y, x]);
                carCount -= 1;
            }
        }

        // Add boundaries to both sides of the road
        GameObject wall1 = new GameObject("Barrier");
        wall1.AddComponent<BoxCollider>();
        wall1.transform.position = new Vector3(-5 - (roadPrefab.transform.lossyScale.x * 0.5f), 5, length * roadPrefab.transform.lossyScale.z / 2 - 3);
        wall1.transform.localScale = new Vector3(10, 10, length * roadPrefab.transform.lossyScale.z);

        GameObject wall2 = new GameObject("Barrier2");
        wall2.AddComponent<BoxCollider>();
        wall2.transform.position = new Vector3(roadPrefab.transform.lossyScale.x * (lanes - 0.5f) + 5, 5, length * roadPrefab.transform.lossyScale.z / 2 - 3);
        wall2.transform.localScale = new Vector3(10, 10, length * roadPrefab.transform.lossyScale.z);
    }

    private void GenerateScenery()
    {
        // Create the scenery for the left side of the road

        GameObject sceneryParent = new GameObject("Scenery Holder");


        float zOffset = 0;
        // Keep spawning scenery until it reaches the end of the road
        for (int side = 0; side < 2; side++)
        {
            for (int r = 0; r < rows; r++)
            {
                zOffset = 0;
                while (zOffset < length * roadPrefab.transform.lossyScale.z)
                {
                    // Choose a random object to spawn

                    GameObject toSpawn = scenery[Random.Range(0, scenery.Length)];

                    // Add a random amount to the spawn offset

                    zOffset += Random.Range(minSceneryGap, maxSceneryGap);

                    // Get the x offset

                    float xOffset = Random.Range(-xOffsetVariance, 0);

                    if (sceneryfar.Contains(toSpawn))
                        xOffset -= Random.Range(farOffsetNear, farOffsetFar);
                    // Spawn the scenery
                    int scale = 1;
                    if (side == 1)
                        scale = -1;
                    GameObject spawned = Instantiate(toSpawn, new Vector3(scale * (xOffset - (r * rowGap) - toSpawn.transform.localScale.x  -(lanes * roadPrefab.transform.lossyScale.x / 2)) + (lanes * roadPrefab.transform.lossyScale.x / 2), 0, zOffset), 
                        toSpawn.transform.rotation, sceneryParent.transform); // spawns the scene object
                    
                    if (Random.Range(0, 2) == 1) // coin flipper
                        spawned.transform.Rotate(0,180,0);

                    /*GameObject mirrored = Instantiate(spawned, new Vector3(scale * (xOffset - (r * rowGap) - toSpawn.transform.localScale.x - (lanes * roadPrefab.transform.lossyScale.x / 2)) + (lanes * roadPrefab.transform.lossyScale.x / 2), 0, zOffset), 
                        spawned.transform.rotation, sceneryParent.transform); // spawns mirror version of object;
                    mirrored.transform.Rotate(180, 0, 0);
                    mirrored.GetComponent<SpriteRenderer>().sortingOrder -= 4;*/
                }
            }
        }

        // Then do it for the other side of the road

    }

    /*--------------------Utility Functions--------------------*/

    // Called when the player runs into an obstacle
    public void OnPlayerCrash()
    {
        Debug.LogError("You lose");
        onCrash.Invoke();
    }

    // Called when the player survives :D
    public void OnPlayerWin()
    {
        onWin.Invoke();
    }
}
