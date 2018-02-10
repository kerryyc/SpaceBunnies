using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour {

    public Sprite heatlhSprite;

    private GameObject player;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update() {
        float health = player.GetComponent<PlayerController>().health;
        string imagePath = "Sprites/Healthbar/healthbar" + (int)health;
        GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
    }
}
