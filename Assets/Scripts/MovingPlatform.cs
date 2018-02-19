using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float speed;
    public bool movingLeft = false;

    void Awake() {
        if (!movingLeft)
            speed *= -1;
    }

    void Update() {
        transform.Translate(speed * Vector2.left * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Collision with stop trigger");
        if (other.gameObject.tag == "PlatformTrigger") {
            speed *= -1;
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {
        coll.transform.parent = this.transform;
    }
    private void OnCollisionStay2D(Collision2D coll)
    {
        coll.transform.parent = this.transform;
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        // detach the player from the platform
        coll.transform.parent = null;
    }
}
