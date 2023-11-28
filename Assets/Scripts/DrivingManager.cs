/*
 * Name: Quinn Farrell
 * Date: 11/27/2023
 * Desc: A manager class for the driving minigame. Handles things like player movement, car spawning, road spawning, as well as game over and win conditions.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingManager : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("How many lanes the level generates")]
    [SerializeField][Range(2, 10)] int lanes = 2;
    [Tooltip("How many 'rows' of roads to spawn. Pretty much how long the level should be")]
    [SerializeField] int length = 50;

    [Space]
    [Tooltip("How many rows to wait before spawning the first car")]
    [SerializeField][Min(3)] int startDelay = 10;
    [Tooltip("The number of cars to spawn. Increase this number to increase difficulty")]
    [SerializeField] int carsToSpawn = 50;

    [Space]
    [Tooltip("How many paths to spawn that will guarantee a way to the end of the level")]
    [SerializeField][Min(1)] int paths = 2;

    [Header("Assets")]
    [SerializeField] GameObject carPrefab;
    [SerializeField] GameObject roadPrefab;

    GameObject[,] roads;
    GameObject[,] cars;

    /*--------------------Unity Functions--------------------*/

    private void Start()
    {
        // Cap the start delay to the length of the road (when it will not spawn any cars?)
        startDelay = Mathf.Clamp(startDelay, 3, length);

        GenerateLevel();
    }

    private void Update()
    {
        
    }

    /*--------------------Level Management Functions--------------------*/

    private void GenerateLevel()
    {
        // Create the road

        GameObject roadParent = new GameObject("Road Holder");

        roads = new GameObject[length, lanes];

        for (int y = 0; y < length; y++) // Loop through the rows of the road
        {
            for (int x = 0; x < lanes; x++) // Loop through the columns of the road
            {
                // Spawn a road tile
                Vector3 posToSpawn = new Vector3(x * roadPrefab.transform.localScale.x, 0, y * roadPrefab.transform.localScale.y);
                GameObject instance = Instantiate(roadPrefab, posToSpawn, roadPrefab.transform.rotation, roadParent.transform);
                roads[y, x] = instance;
            }
        }

        // Spawn the cars

        cars = new GameObject[length - startDelay, lanes];

        for (int y = startDelay; y < length; y++) // Loop through all the rows of the road that come after the start delay
        {
            for (int x = 0; x < lanes; x++) // Loop through the columns of the road
            {
                // Spawn a car there
                GameObject instance = Instantiate(carPrefab, roads[y, x].transform.position, carPrefab.transform.rotation);
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
    }

    /*--------------------Utility Functions--------------------*/

    // Called when the player runs into an obstacle
    public void OnPlayerCrash()
    {
        Debug.LogError("You lose");
    }
}
