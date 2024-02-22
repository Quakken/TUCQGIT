using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ShoppingManager : MonoBehaviour
{
    [System.Serializable]
    public struct FoodType
    {
        [Tooltip("The image of the food the player is looking for")]
        public Sprite realFoodSprite;
        [Tooltip("How long the player has to find the right food")]
        public float length;
        [Tooltip("All of the food objects to spawn. Increase/duplicate fake foods to increase their spawn rate")]
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
    [Tooltip("How far down from the bottom of the screen the shoppers can spawn")]
    [SerializeField][Range(0, 5)] float minShopperDist = 0f;
    [Tooltip("How far up from the bottom of the screen the shoppers can spawn")]
    [SerializeField][Range(0, 5)] float maxShopperDist = 0f;
    [Tooltip("How likely it is that a shopper will spawn when a shelf is spawned")]
    [SerializeField][Range(0, 1)] float shopperSpawnChance = 0.5f;
    [Space]
    [SerializeField] TimerCountdown timer;

    private bool spawnShelves = true;


    [Header("Assets")]
    [Tooltip("The object that has the real image to display")]
    [SerializeField] Image realImage;
    [Tooltip("The shelf object to spawn")]
    [SerializeField] GameObject shelfPrefab;
    [Tooltip("Contents of each stage. Each entry should have a different type of food")]
    [SerializeField] FoodType[] aisles;
    [Tooltip("All of the shoppers that can spawn in front of butter. Should have 'MoveInDirection' scripts attached")]
    [SerializeField] GameObject[] shoppers;

    private List<GameObject> shelves;
    private List<GameObject> food;
    private int stageIndex;

    [Header("Events")]
    [SerializeField] UnityEvent onStageComplete; // Called when the player completes one stage
    [SerializeField] UnityEvent onStageGenerated; // Called whenever a new stage is made
    [SerializeField] UnityEvent onFail; // Called whenever the player clicks the wrong food or runs out of time
    [SerializeField] UnityEvent onGameWon; // Called only when the player completes all stages

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

        // Stop the timer
        timer.StopTimer();

        // If it's the last stage, have the player win. Otherwise, start the next level

        if (stageIndex >= aisles.Length)
        {
            print("YOU WIN!");
            onGameWon.Invoke();
        }
        else
        {
            onStageComplete.Invoke();
        }
    }

    // Fails the player. Called when they click the wrong food or time runs out
    public void Fail()
    {
        // Clear all the foods
        for (int i = 0; i < food.Count; i++)
        {
            Destroy(food[i]);
        }

        // Stop spawning shelves
        SetShelfSpawn(false);

        // Fail them lol
        onFail.Invoke();
    }

    // Creates a level based on the current food/level. Should only be called when a level is first started
    public void GenerateLevel()
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

            /* Spawn food on that shelf

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

        // Set the timer
        timer.SetTimer(aisles[stageIndex].length);
        timer.StartTimer();

        // Change the real food image
        realImage.sprite = aisles[stageIndex].realFoodSprite;

        onStageGenerated.Invoke();
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

                int shelfIndex = Random.Range(0, shelves[shelves.Count - 1].GetComponentsInChildren<Transform>().Length - 1);

                // Check if there are any other food objects on that shelf, and if there are then get a new shelf

                Collider2D[] otherFood = Physics2D.OverlapCircleAll(shelves[shelves.Count - 1].GetComponentsInChildren<Transform>()[shelfIndex].transform.position, 0.25f);

                while (otherFood.Length > 0)
                {
                    shelfIndex = Random.Range(0, shelves[shelves.Count - 1].GetComponentsInChildren<Transform>().Length - 1);
                    otherFood = Physics2D.OverlapCircleAll(shelves[shelves.Count - 1].GetComponentsInChildren<Transform>()[shelfIndex].transform.position, 0.25f);
                }

                // Get the transform of the shelf that the food is going to spawn on

                Transform spawnTransform = shelves[shelves.Count - 1].GetComponentsInChildren<Transform>()[shelfIndex].transform;

                // Spawn the food

                food.Add(Instantiate(foodPrefab, spawnTransform.position, foodPrefab.transform.rotation));
            }

            // Possibly spawn a shopper

            float spawnChance = Random.Range(0f, 1f);

            if (spawnChance < shopperSpawnChance)
            {
                // Spawn a shopper at the same position as the current shelf

                float yOffset = Random.Range(-minShopperDist, maxShopperDist);

                Vector3 camBot = Camera.main.ScreenToWorldPoint(new Vector3(1, 1, 10));

                Instantiate(shoppers[Random.Range(0, shoppers.Length)], new Vector3(xOffset, yOffset + camBot.y, 0), Quaternion.identity);
            }
        }
    }

    /*--------------------Utility Functions--------------------*/

    public void CheckFood(Food foodToCheck)
    {
        // Check to see if the food the player clicks is fake
        if (!foodToCheck.isFake)
        {
            // If it's not, advance the stage
            AdvanceStage();
        }
        else
        {
            // If it is fake, they lose
            Fail();
        }
    }

    // Function that toggles the automatic shelf spawning
    public void SetShelfSpawn(bool val)
    {
        spawnShelves = val;
    }
}
