using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed = 10f;
    public float fallSpeed = 10f;
    public bool canMove = true;
    public float health = 10f;
    public float fireRate = 0.2f;

    public GameObject bulletPrefab;

    public float jumpForce = 700f;
    private bool isGrounded = false;
    public Transform groundCheck;
    public Vector2 groundRadius;
    public LayerMask whatIsGround;

    private Rigidbody2D rb2d;
    private Animator anim;
    private bool facingRight = true;
    private bool canFire = true;
    private float fireCoolDown;
    private float fireAnimationStop;

	// Use this for initialization
	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.freezeRotation = true;
        anim = GetComponent<Animator>();
    }
	

	// Update is called once per frame
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

        HandleLayers();
	}

    void Update() {
        //what happens when health is 0
        if (health <= 0)
            Destroy(this.gameObject);

        //jump
        if (canMove && isGrounded && Input.GetButtonDown("Jump")) {
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
    }

    private void Move() {
        float newSpeed;
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

        newSpeed = moveHorizontal * speed;
        rb2d.velocity = new Vector2(newSpeed, rb2d.velocity.y);

    }

    private void Shoot() {
        if (facingRight) {
            GameObject bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x + 0.8f, transform.position.y, 0), transform.rotation);
            //bullet.GetComponent<Projectile>().changeDirection();
        }
        else {
            GameObject bullet = (GameObject)Instantiate(bulletPrefab, new Vector3(transform.position.x - 0.8f, transform.position.y, 0), transform.rotation);
            bullet.GetComponent<Projectile>().changeDirection();
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
        if (!isGrounded)
            anim.SetLayerWeight(1, 1);
        else
            anim.SetLayerWeight(1, 0);
    }
}
