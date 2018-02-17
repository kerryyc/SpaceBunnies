using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    //general variables
    public float health = 10f; //health of player
    public float speed = 10f; //player movement speed
    public float fireRate = 0.2f; //how fast player can fire bullets
    [HideInInspector] public bool canMove = true; //whether player can move

    public GameObject bulletPrefab; //bullet that the player fires
    public GameObject buttonFunction;
    
    //variables for jumping and checking if player is on the ground
    public float jumpForce = 700f;
    private bool isGrounded = false;
    public Transform groundCheck;
    public Vector2 groundRadius;
    public LayerMask whatIsGround;

    //private components
    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer spriteRend;

    //miscellaneous
    private GameObject bullet; //player bullet
    [HideInInspector] public bool facingRight = true; //whether player is facing right
    private bool canPause = true;

    //variables for fire cooldown
    private bool canFire = true;
    private float fireCoolDown;
    private float fireAnimationStop;

    //variables to enable blinking of sprite
    private float spriteBlinkingTimer = 0.0f;
    private float spriteBlinkingMiniDuration = 0.1f;
    [HideInInspector] public float spriteBlinkingTotalTimer = 0.0f;
    private float spriteBlinkingTotalDuration = 1.0f;
    [HideInInspector] public bool startBlinking = false;

    // Audio for shoot sound
    public AudioClip shootSound;

    // Audio for explosion on death
    public AudioClip explosionSound;

    // Audio for jump
    public AudioClip jumpSound;

    private AudioSource soundSource;
    private bool isDeathSoundPlayed = false;
    // randomize the volume in each shot
    private float lowRange = .5f;
    private float highRange = 1.0f;

    // Use this for initialization
    void Awake () {

        soundSource = GetComponent<AudioSource>();

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.freezeRotation = true;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }
	
	void FixedUpdate () {
        //check ground
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundRadius, 0, whatIsGround);
        if (isGrounded) {
            anim.ResetTrigger("Jump");
            anim.SetBool("Land", false);
        }
        else
            anim.SetTrigger("Jump");

        //move horizontal
        if (canMove)
            Move();

        //change between ground and air layer
        HandleLayers();
	}

    void Update() {
        //player can't move or fire if game is paused
        if (Time.timeScale == 1) {
            canPause = true;
        }

        //quit game
        if (Input.GetButtonDown("Cancel")) {
            Application.Quit();
        }

        //what happens when health is 0
        if (health <= 0) {
            //transform.parent = null; //death animation attached to platform if commented
            if (isDeathSoundPlayed == false) {
                Debug.Log("Death");
                soundSource.PlayOneShot(explosionSound, 1f); // play explosion
                isDeathSoundPlayed = true;
            }
           
            anim.SetLayerWeight(1, 0); //set animation to ground layer to play death animation
            spriteRend.enabled = true; //force enable sprite renderer if disabled by different method
            anim.Play("player_death"); //play death animation
            

            //disable all colliders
            Collider2D[] colChildren = gameObject.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in colChildren) {
                col.enabled = false;
            }
            GetComponent<Collider2D>().enabled = false;
            //disable physics so object is not affected by gravity
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

            

            Invoke("PlayerDead", 1f); //disable player when animation is done playing
        }

        //pause game
        if(canPause && Input.GetButtonDown("Pause")) {
            buttonFunction.GetComponent<ButtonScript>().PauseScene();
            canPause = false;
        }

        //jump
        if (canMove && isGrounded && Input.GetButtonDown("Jump")) {
            transform.parent = null; //if attached to a moving platform, unattach
            soundSource.PlayOneShot(jumpSound, .5f);
            rb2d.AddForce(new Vector2(0, jumpForce));
            anim.SetTrigger("Jump");
        }

        //shoot
        if (Input.GetButtonDown("Fire1") && canFire) {
            Shoot();
            anim.SetBool("Shoot", true);
            canFire = false;
            fireCoolDown = Time.time + fireRate;
            // unused
            //fireAnimationStop = Time.time + 10;
        }
        if (!canFire && Time.time > fireCoolDown) {
            canFire = true;
            anim.SetBool("Shoot", false);
        }

        //start blinking effect
        if (startBlinking)
            SpriteBlinkingEffect();
    }

    private void Move() {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        //set jump/landing animation
        if (rb2d.velocity.y < 0) {
            anim.SetBool("Land", true);
        }
            
        //set running animation
        anim.SetFloat("Speed", Mathf.Abs(moveHorizontal));
        if (moveHorizontal > 0 && !facingRight)
            Flip();
        else if (moveHorizontal < 0 && facingRight)
            Flip();

        //set movement
        rb2d.velocity = new Vector2(moveHorizontal * speed, rb2d.velocity.y);

    }

    private void Shoot() {
        // play shooting sound
        float vol = Random.Range(lowRange, highRange);
        soundSource.PlayOneShot(shootSound, vol);

        //shoot bullet depending on direction faced
        if (facingRight) {
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

    private void Flip() {
        //flips sprite depending on direction faced
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void HandleLayers() {
        //changes animation layer between ground and air for jumping
        if (!isGrounded)
            anim.SetLayerWeight(1, 1);
        else
            anim.SetLayerWeight(1, 0);
    }

    private void PlayerDead() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
