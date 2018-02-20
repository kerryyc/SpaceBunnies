using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour {

    private Rigidbody2D rb2d;
    private float time;

    public float speed = 1f;
    public float cooldown = 10f;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start() {
        cooldown = cooldown + Time.time;
    }

    void FixedUpdate() {
        if (Time.time > cooldown)
            rb2d.velocity = new Vector2(0, speed);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if(coll.gameObject.tag == "Player") {
            coll.gameObject.GetComponent<PlayerController>().health = 0;
        }
        if (coll.gameObject.tag == "Enemy") {
            coll.gameObject.GetComponent<Enemy>().health = 0;
        }
    }
}