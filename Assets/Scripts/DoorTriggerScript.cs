using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorTriggerScript : MonoBehaviour {

    // Use this for initialization
    public GameObject EndPoint;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"){
            string imagePath = "Sprites/DoorOpen";
            EndPoint.GetComponent<EndLevel1>().isDoorOpen = true;
            Debug.Log("Collision with trigger is made");
            EndPoint.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(imagePath);

            // destroy once player has made contact with the trigger
            Destroy(this.gameObject);
        }
       
    }
}
