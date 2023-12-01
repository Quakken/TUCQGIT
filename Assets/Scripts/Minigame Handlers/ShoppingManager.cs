using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShoppingManager : MonoBehaviour
{
    [System.Serializable]
    public struct FoodType
    {
        public GameObject[] foodPrefabs;
    }

    [Header("Config")]
    [Tooltip("How many shelves can exist at one time")]
    [SerializeField] int maxShelves = 8;
    [Tooltip("How often a new shelf is spawned. Try to sync up to their movement speed")]
    [SerializeField] float shelfSpawnRate = 2;
    [Tooltip("The least amount of items a shelf can have")]
    [SerializeField][Range(1, 8)] int minItemsPerShelf = 1;
    [Tooltip("The most amount of items a shelf can have")]
    [SerializeField][Range(1, 8)] int maxItemsPerShelf = 4;

    private bool spawnShelves = true;


    [Header("Assets")]
    [Tooltip("The shelf object to spawn")]
    [SerializeField] GameObject shelfPrefab;
    [Tooltip("Contents of each stage. Each entry should have a different type of food")]
    [SerializeField] FoodType[] aisles;

    private List<GameObject> shelves;
    private List<GameObject> food;
    private int stageIndex;

    [Header("Events")]
    [SerializeField] UnityEvent onStageComplete;
    [SerializeField] UnityEvent onFail;

    // Globally accessable instance of this script
    public static ShoppingManager instance;

    /*--------------------Unity Functions--------------------*/

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Spawn a new shelf every X seconds
        InvokeRepeating("SpawnShelf", shelfSpawnRate, shelfSpawnRate);

        // Clamp the max shelf items to the amount of shelf spaces there are
        // Each shelf space is a child of the shelf prefab, so the getcomponentsinchildren function can be used to determine how many slots there are
        maxItemsPerShelf = Mathf.Clamp(maxItemsPerShelf, minItemsPerShelf, shelfPrefab.GetComponentsInChildren<Transform>().Length - 1);

        // Spawn the first level
        GenerateLevel();
    }

    /*--------------------Level Management Functions--------------------*/

    // Moves on to the next stage
    private void AdvanceStage()
    {
        // Increase the stage index

        stageIndex++;
        
        // Clear all the foods

        for (int i = 0; i < food.Count; i++)
        {
            Destroy(food[i]);
        }

        // Stop spawning shelves

        SetShelfSpawn(false);

        // If it's the last stage, have the player win. Otherwise, start the next level

        if (stageIndex >= aisles.Length)
        {
            print("YOU WIN!");
        }
        else
        {
            onStageComplete.Invoke();
        }
    }

    // Creates a level based on the current food/level. Should only be called when a level is first started
    private void GenerateLevel()
    {
        shelves = new List<GameObject>();
        food = new List<GameObject>();

        // Offset to spawn the shelves at
        float xOffset = -maxShelves / 2 * shelfPrefab.transform.localScale.x;

        // Loop through all the shelves to spawn

        for (int i = 0; i < maxShelves; i++)
        {
            // Spawn the shelf
            shelves.Add(Instantiate(shelfPrefab, new Vector3(xOffset, 0, 0), shelfPrefab.transform.rotation));

            /*// Spawn food on that shelf

            int foodToSpawn = Random.Range(minItemsPerShelf, maxItemsPerShelf);

            for (int f = 0; f < foodToSpawn; f++)
            {
                GameObject foodPrefab = aisles[stageIndex].foodPrefabs[Random.Range(0, aisles[stageIndex].foodPrefabs.Length)];
                // Spawn the food at a random position on the shelf for now ----------------------------------------CHANGE THIS LATER---------------------------------------------------------------------------
                food.Add(Instantiate(foodPrefab, new Vector3(xOffset, shelfPrefab.transform.localScale.y / 2 - f * shelfPrefab.transform.localScale.y / foodToSpawn), foodPrefab.transform.rotation));
            }*/

            // Increase the offset
            xOffset += shelfPrefab.transform.localScale.x;
        }

        // Start automatically spawning shelves
        SetShelfSpawn(true);
    }

    // Spawns a single shelf object at the end of the aisle, filled with a random amount of food
    private void SpawnShelf()
    {
        if (spawnShelves)
        {
            // Get rid of the first of the shelves & shift all the other shelves down in the heirarchy

            Destroy(shelves[0]);
            shelves.RemoveAt(0);

            // Get the distance to spawn the shelf from the most recently spawned shelf

            float xOffset = shelves[shelves.Count - 1].transform.position.x + shelfPrefab.transform.localScale.x;

            // Spawn the next shelf

            shelves.Add(Instantiate(shelfPrefab, new Vector3(xOffset, 0, 0), shelfPrefab.transform.rotation));

            // Spawn food on that shelf

            int foodToSpawn = Random.Range(minItemsPerShelf, maxItemsPerShelf);

            for (int f = 0; f < foodToSpawn; f++)
            {
                GameObject foodPrefab = aisles[stageIndex].foodPrefabs[Random.Range(0, aisles[stageIndex].foodPrefabs.Length)];

                // Spawn the food at a random position on the shelf

                Transform spawnTransform = shelves[shelves.Count - 1].GetComponentsInChildren<Transform>()[Random.Range(0, shelves[shelves.Count - 1].GetComponentsInChildren<Transform>().Length - 1)].transform;

                // This will work sort of, but it will allow foods to overlap each other

                food.Add(Instantiate(foodPrefab, spawnTransform.position, foodPrefab.transform.rotation));
            }
        }
    }

    /*--------------------Utility Functions--------------------*/

    public void CheckFood(Food food)
    {
        // Check to see if the food the player clicks is fake
        if (!food.isFake)
        {
            // If it's not, advance the stage
            AdvanceStage();
        }
        else
        {
            onFail.Invoke();
        }
    }

    // Function that toggles the automatic shelf spawning

    public void SetShelfSpawn(bool val)
    {
        spawnShelves = val;
    }
}
