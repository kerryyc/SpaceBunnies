using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float health = 10f;
    public GameObject player;
    public bool facingRight = false;

    private Rigidbody2D rb2d;
    private Animator anim;
    private Collider2D thisColl;

	// Use this for initialization
	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        thisColl = GetComponent<Collider2D>();
        //flip enemy depending on face direction
        //no movement for now -> simple one time flip
        if (facingRight)
            Flip();
	}
	
    void Update() {
        if (health <= 0) {
            anim.SetTrigger("Dead");
            thisColl.enabled = false;
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            Destroy(this.gameObject, 2);
        }
    }
	
	void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == "Projectile") {
            --health;
        }
        else if (other.gameObject.tag == "Platform") {
            rb2d.velocity = new Vector2(0, 0);
        }
	}

    void OnCollisionEnter2D (Collision2D coll) {
        if (coll.collider.GetType() == typeof(BoxCollider2D) && coll.gameObject.tag == "Player") {
            Debug.Log("Collided with Player");
            player.GetComponent<PlayerController>().health -= 1;
        }
    }

    private void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
