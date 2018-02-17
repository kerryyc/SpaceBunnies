using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorTriggerScript : MonoBehaviour {

    // Use this for initialization
    public GameObject EndPoint;
    public AudioClip unlockSound;
    private AudioSource soundSource;
    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"){
            GetComponent<SpriteRenderer>().enabled = false;
            soundSource.PlayOneShot(unlockSound, 1f);

            string imagePath = "Sprites/DoorOpen";
            EndPoint.GetComponent<EndLevel1>().isDoorOpen = true;
            Debug.Log("Collision with trigger is made");
            EndPoint.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(imagePath);
            Invoke("destroyTrigger", 1.2f);
        }
       
    }
    private void destroyTrigger()
    {
        // destroy once player has made contact with the trigger
        Destroy(this.gameObject);

    }
}
