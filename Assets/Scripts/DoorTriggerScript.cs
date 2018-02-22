using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorTriggerScript : MonoBehaviour {

    // Use this for initialization
    public GameObject EndPoint;
    public AudioClip unlockSound;
    private AudioSource soundSource;
    private GameObject player;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"){
            GetComponent<SpriteRenderer>().enabled = false;
            soundSource.PlayOneShot(unlockSound, 1f);
            player.GetComponent<PlayerController>().hitDoor = true;

            string imagePath = "Sprites/DoorOpen";
            EndPoint.GetComponent<EndLevel1>().isDoorOpen = true;
            EndPoint.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(imagePath);
            Invoke("destroyTrigger", 1.2f);
        }
       
    }
    private void destroyTrigger()
    {
        // disable once player has made contact with the trigger
        this.gameObject.SetActive(false);

    }
}
