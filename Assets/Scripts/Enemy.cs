using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float health = 10f;
    public float detectDistance = 5f;
    public float attackRate = 3f;
    public bool facingLeft = false;
    public bool attackPlayerWithBullet = true;
    public GameObject bulletPrefab;

    private Rigidbody2D rb2d;
    private Animator anim;
    private Collider2D thisColl;
    private GameObject player;
    private GameObject bullet;

    //values for timing attacks
    private bool canAttack = true;
    private float attackCoolDown = 3;

    //values for timing player continuous damage when colliding with enemy
    private bool canDamage = true;
    private float damageCoolDown = 0.5f;

    // Use this for initialization
    void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        this.anim = this.GetComponent<Animator>();
        thisColl = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
    void Update() {
        //flip enemy depending on face direction
        //no movement for now -> simple one time flip
        if (facingLeft)
            Flip();

        if (health <= 0) {
            //trigger death animation, disable physics, then destroy
            anim.SetTrigger("Dead");
            thisColl.enabled = false;
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            Destroy(this.gameObject, 2);
        }
        else {
            //attack player with bullet or melee
            if (attackPlayerWithBullet)
                AttackPlayerBullet();
            else
                AttackPlayerMelee();
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

    void OnCollisionStay2D (Collision2D coll) {
        if (coll.gameObject.tag == "Player") {
            if (canDamage) {
                Debug.Log("Stayed with Player");
                canDamage = false;
                damageCoolDown = Time.time + 0.5f;
            }
            else if (!canDamage && Time.time > damageCoolDown) {
                canDamage = true;
                Debug.Log("Damaged Player");
                player.GetComponent<PlayerController>().health -= 1;
            }
        }
    }

    private void Flip() {
        facingLeft = !facingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void AttackPlayerBullet() {
        if (canAttack && Vector2.Distance(transform.position, player.transform.position) <= detectDistance) {
            anim.Play("bunny_attack");
            canAttack = false;
            attackCoolDown = Time.time + attackRate;
            Invoke("CreateBullet", 0.3f);
        }
        if(!canAttack && Time.time > attackCoolDown) {
            canAttack = true;
        }
    }

    private void AttackPlayerMelee() {
        if (canAttack && Vector2.Distance(transform.position, player.transform.position) <= detectDistance) {
            anim.Play("bunny_attack");
            canAttack = false;
            attackCoolDown = Time.time + attackRate;
        }
        if (!canAttack && Time.time > attackCoolDown) {
            canAttack = true;
        }
    }

    private void CreateBullet() {
        if (facingLeft) {
            bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x + 0.8f, transform.position.y, 0), transform.rotation);
        }
        else {
            bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x - 0.8f, transform.position.y, 0), transform.rotation);
            bullet.GetComponent<Projectile>().changeDirection();
        }
    }
}
