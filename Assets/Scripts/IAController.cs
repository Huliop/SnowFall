using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : MonoBehaviour {

	public float speed = 20f;
	public float rotSpeed = 0.05f;
    public float snowBallSpeed = 250f;
    public GameObject prefab;
	public GameObject player;
    public float radius = 1f;
	private Vector3 forward = Vector3.zero;
	private Vector3 moveDirection = Vector3.zero;
	bool isMelting = false;
    private bool stun = false;
    private float timeStampStun;
	private float timeStampShoot;

	// Use this for initialization
	void Start () {
		updateForward();
		timeStampShoot = Time.time + 5;
	}
	
	// Update is called once per frame
	void Update () {
		radius = transform.localScale.x;
		if (!stun){
			updateMoveDirection();
			moveAI(moveDirection);
			if (timeStampShoot < Time.time){
				Vector3 direction = player.transform.position - transform.position;
				shoot(direction);
				timeStampShoot = Time.time + 5;
			}
		}

		else {
            if (timeStampStun < Time.time)
                stun = false;
        }

		updateSize();
		updateSpeed();
	}

	void moveAI(Vector3 moveDirection) {
		Vector3 rotationAxis = Vector3.Cross(moveDirection, Vector3.up);

        // On met à jour la position et la rotation de la boule
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        transform.RotateAround(transform.position, rotationAxis, -Mathf.Sin(moveDirection.magnitude*rotSpeed*2*Mathf.PI)*Mathf.Rad2Deg);
	}

	void updateMoveDirection() {
		int proba = Random.Range(0,101);

		if (proba<80) {
			moveDirection = forward;
		}
		else {
			updateForward();
			moveDirection = forward;
		}

	}

	void shoot(Vector3 forward) {

        // On instancie la boule de neige
        GameObject clone = Instantiate(prefab, transform.position + forward.normalized * (radius/2 + prefab.transform.localScale.x/2) , Quaternion.identity);

        clone.GetComponent<Rigidbody>().AddForce(forward.normalized * snowBallSpeed);
        Destroy(clone, 10f);
        transform.localScale -= Vector3.one * 0.1f;
    }

	public Vector3 getDirection() {
        return moveDirection;
    }

	void updateForward() {
		float moveHorizontal = Random.Range(-1f, 1f);
		float moveVertical = Random.Range(-1f, 1f);

		forward = new Vector3(moveHorizontal, 0, moveVertical).normalized;
	}

	void updateSize() {
        if (isMelting) {
            transform.localScale -= Vector3.one * 0.04f;
        }
    }
	void updateSpeed() {
        speed = 20f - transform.localScale.x;
	}

	void OnTriggerEnter(Collider col){
        if (col.tag == "Meteor"){
            isMelting = true;
        }
    }

    void OnTriggerExit(Collider col){
        if (col.tag == "Meteor"){
            isMelting = false;
        }
    }

	void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Meteor")
            isMelting = true;
        if (collision.collider.tag == "Wall") {
            moveAI(-moveDirection);
        }
		if (collision.collider.tag == "snowBall") {
            stun = true;
            timeStampStun = Time.time + 2;
        }
    }

    void OnCollisionStay(Collision collision) {
        if (collision.collider.tag == "Wall") {
            moveAI(-moveDirection);
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.collider.tag == "Meteor")
            isMelting = false;
    }
}
