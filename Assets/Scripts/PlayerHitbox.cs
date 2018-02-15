using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Projectile") {
            PlayerController player = GetComponentInParent<PlayerController>(); //get PlayerController
            player.startBlinking = true; //start blinking effect
            player.health -= 1; //decrement health
            player.spriteBlinkingTotalTimer = 0f; //reset blinking timer
        }
    }
}
