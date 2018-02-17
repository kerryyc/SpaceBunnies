using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float health = 10f; //health of enemy
    public float xDistance = 5f; //detection of x player distance
    public float yDistance = 3f; //detection of y player distance
    public float attackRate = 3f; //how fast enemy can fire
    public float speed = 4f; //movement speed

    public bool facingLeft = false; //which direction enemy is facing
    public bool attackPlayerWithBullet = true; //whether to attack with melee or bullet
    public GameObject bulletPrefab; //enemy bullet

    public bool canMove = false; //whether enemy can move
    private bool toggleMove;
    public bool toggleAttackWhileMove = false;
    public float offset = 0f; //offset of enemy when flipping sprite if applicable
    
    //general components
    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer spriteRend; 
    private Collider2D thisColl; //used to disable collider when dead
    private Collision2D currentColl; //TODO: use to fix constant damage to player

    //other objects to be called
    private GameObject player;
    private GameObject bullet;

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

    // Audio for shoot sound
    public AudioClip shootSound;
    public AudioClip explosionSound;

    private AudioSource soundSource;

    // randomize the volume in each shot
    private float lowRange = 1f;
    private float highRange = 1.5f;
    private bool isDeathSoundPlayed = false;
    // Use this for initialization
    void Awake () {
        // get audio source
        soundSource = GetComponent<AudioSource>();

        rb2d = GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        thisColl = GetComponent<Collider2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (facingLeft)
            Flip();
        else
            facingLeft = !facingLeft;
        toggleMove = canMove;
    }
	
    void FixedUpdate() {
        //set movement
        if (canMove) {
            anim.SetBool("Move", true);
            rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
        }
        else {
            anim.SetBool("Move", false);
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
    }

    void Update() {
        if (health <= 0) {
            if (isDeathSoundPlayed == false)
            {
                soundSource.PlayOneShot(explosionSound, 1f); // play explosion
                isDeathSoundPlayed = true;

            }
            //trigger death animation, disable physics, then destroy
            spriteRend.enabled = true;

            anim.Play("bunny_explosion");
            thisColl.enabled = false;
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
           
            Destroy(this.gameObject, 2);
        }
        else {
            //check blinking effect
            if (startBlinking)
                SpriteBlinkingEffect();

            //attack player if within detect distance
            Vector3 detectDistance = transform.position - player.transform.position;
            if (Mathf.Abs(detectDistance.x) <= xDistance && Mathf.Abs(detectDistance.y) <= yDistance) {
                //if not facing player, flip
                if (Mathf.Abs(detectDistance.x) > 0.4 && ((detectDistance.x < 0 && !facingLeft) || (detectDistance.x > 0 && facingLeft)))
                    Flip();

                //if can't attack while moving, disable movement
                if (!toggleAttackWhileMove) {
                    rb2d.velocity = new Vector2(0, rb2d.velocity.y);
                    canMove = false;
                }

                //attack player with bullet or with melee depending on toggle
                if (attackPlayerWithBullet)
                    AttackPlayerBullet();
                else
                    AttackPlayerMelee();

                
            }
            else {
                //allow movement if enabled
                canMove = toggleMove;
            }

        }
    }
	
	void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == "Projectile") {
            //when hit with projectile, take damage
            startBlinking = true;
            spriteBlinkingTotalTimer = 0f;
            --health;
        }
        else if (other.gameObject.tag == "PlatformTrigger")
            Flip();
	}

    void OnCollisionEnter2D (Collision2D coll) {
        if (coll.collider.GetType() == typeof(BoxCollider2D) && coll.gameObject.tag == "Player") {
            //if player collides, then player takes damage
            player.GetComponent<PlayerController>().health -= 1;
            player.GetComponent<PlayerController>().startBlinking = true;
        }
    }

    void OnCollisionStay2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player") {
            if (canDamage) {
                Debug.Log("Stayed with Player");
                canDamage = false;
                damageCoolDown = Time.time + 0.5f;
            }
            if (!canDamage && Time.time > damageCoolDown) {
                canDamage = true;
                //Debug.Log("Damaged Player");
                player.GetComponent<PlayerController>().health -= 1;
                player.GetComponent<PlayerController>().startBlinking = true;
            }
        }
    }

    private void Flip() {
        //flip sprite depending on direction faced
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        speed *= -1;

        //change offset of enemy
        if (facingLeft)
            transform.position = new Vector2(transform.position.x - offset, transform.position.y);
        else
            transform.position = new Vector2(transform.position.x + offset, transform.position.y);
        facingLeft = !facingLeft;
    }

    private void AttackPlayerBullet() {
        //starts attack animation and creates a bullet
        
        if (canAttack) {
            // play shooting sound
            float vol = Random.Range(lowRange, highRange);
            soundSource.PlayOneShot(shootSound, vol);

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
        if (canAttack) {
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
