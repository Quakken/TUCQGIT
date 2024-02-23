/*
Name: Alice Pocek
Date: 2/22/2024
Desc: Add this to objects that are picked up by the player
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int points = 10;

    //public AudioClip PickUpNoise;

    public GameObject SpawnOnPickUp;

    public object GameManager { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.score += points;
            AudioSource PAud = collision.gameObject.GetComponent<AudioSource>();
            if (PAud != null)
            {
                //PAud.PlayOneShot(PickUpNoise);
            }
            if (SpawnOnPickUp != null)
            {
                Instantiate(SpawnOnPickUp, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}