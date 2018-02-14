using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float health = 10f; //health of enemy
    public float detectDistance = 5f; //detection of player distance
    public float attackRate = 3f; //how fast enemy can fire
    public bool facingLeft = false; //which direction enemy is facing
    public bool attackPlayerWithBullet = true; //whether to attack with melee or bullet
    public GameObject bulletPrefab; //enemy bullet

    //general components
    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer spriteRend; 
    private Collider2D thisColl; //used to disable collider when dead
    private Collision2D currentColl; //TODO: use to fix constant damage to player

    //other objects to be called
    private GameObject player;
    private GameObject bullet;

    //platform information
    private Vector3 platformSize;

    //values for timing attacks
    private bool canAttack = true;
    private float attackCoolDown = 3f; //how often enemy can attack

    //values for timing player continuous damage when colliding with enemy
    private bool canDamage = true; 
    private float damageCoolDown = 0.5f;

    //variables to enable blinking of sprite
    private float spriteBlinkingTimer = 0.0f;
    private float spriteBlinkingMiniDuration = 0.1f;
    private float spriteBlinkingTotalTimer = 0.0f;
    private float spriteBlinkingTotalDuration = 0.4f;
    [HideInInspector] public bool startBlinking = false;

    // Use this for initialization
    void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        this.anim = this.GetComponent<Animator>();
        thisColl = GetComponent<Collider2D>();
        spriteRend = GetComponent<SpriteRenderer>();
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
            //check blinking effect
            if (startBlinking)
                SpriteBlinkingEffect();

            //attack player with bullet or melee
            if (attackPlayerWithBullet)
                AttackPlayerBullet();
            else
                AttackPlayerMelee();
        }
    }
	
	void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == "Projectile") {
            //when hit with projectile, take damage
            startBlinking = true;
            spriteBlinkingTotalTimer = 0f;
            --health;
        }
	}

    void OnCollisionEnter2D (Collision2D coll) {
        if (coll.gameObject.tag == "Platform") {
            //when initially colliding with platform, get all information about the platform
            platformSize = coll.gameObject.GetComponent<Renderer>().bounds.size;
            Debug.Log(platformSize);
        }
        if (coll.collider.GetType() == typeof(BoxCollider2D) && coll.gameObject.tag == "Player") {
            //if player collides, then player takes damage
            player.GetComponent<PlayerController>().health -= 1;
            player.GetComponent<PlayerController>().startBlinking = true;
        }
    }

    //void OnCollisionStay2D (Collision2D coll) {
    //    if (coll.gameObject.tag == "Player") {
    //        if (canDamage) {
    //            Debug.Log("Stayed with Player");
    //            canDamage = false;
    //            damageCoolDown = Time.time + 0.5f;
    //        }
    //        if (!canDamage && Time.time > damageCoolDown) {
    //            canDamage = true;
    //            Debug.Log("Damaged Player");
    //            player.GetComponent<PlayerController>().health -= 1;
    //            player.GetComponent<PlayerController>().startBlinking = true;
    //        }
    //    }
    //}

    private void Flip() {
        //flip sprite depending on direction faced
        facingLeft = !facingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void AttackPlayerBullet() {
        //starts attack animation and creates a bullet
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
        //starts attack animation, but does not create a bullet
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
        //creates enemy bullet
        if (facingLeft) {
            bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x + 0.8f, transform.position.y, 0), transform.rotation);
        }
        else {
            bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x - 0.8f, transform.position.y, 0), transform.rotation);
            bullet.GetComponent<Projectile>().changeDirection();
        }
    }

    private void SpriteBlinkingEffect() {
        //turns on and off sprite renderer to create blinking effect
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration) {
            startBlinking = false;
            spriteBlinkingTotalTimer = 0.0f;
            spriteRend.enabled = true;
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration) {
            spriteBlinkingTimer = 0.0f;
            if (spriteRend.enabled)
                spriteRend.enabled = false;
            else
                spriteRend.enabled = true;
        }
    }
}
