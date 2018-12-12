using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : MonoBehaviour {

	public float speed = 20f;
	public float rotSpeed = 0.05f;
    public float snowBallSpeed = 250f;
	public float wanderDistance = 10f;
	public float wanderRadius = 2f;
	public float wanderJitter = 3f;
    public GameObject prefab;
	public GameObject player;
    public float radius = 1f;
	Vector3 moveDirection = Vector3.zero;
	Vector3 wanderTarget;
	bool isMelting = false;
    private bool stun = false;
    private float timeStampStun;
	private float timeStampShoot;
	bool touchingXWall;
	bool touchingZWall;

	// Use this for initialization
	void Start () {
		updateWanderTarget();

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
		updateWanderTarget();

		Vector3 posTarget = wanderTarget + transform.position + moveDirection*wanderDistance;

		moveDirection = (posTarget - transform.position).normalized;
	}

	void updateWanderTarget() {
		float randX = Random.Range(-1f,1f);
		float randZ = Random.Range(-1f,1f);
		wanderTarget += new Vector3(randX * wanderJitter, 0, randZ * wanderJitter);
		wanderTarget = wanderTarget.normalized * wanderRadius;
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

	void updateSize() {
        if (isMelting) {
			Vector3 newScale = transform.localScale - Vector3.one * 0.04f;
            if (newScale.x > 0)
				transform.localScale = newScale;
        }
    }
	void updateSpeed() {
        speed = 20f - transform.localScale.x;
	}

	void OnTriggerEnter(Collider col){
        if (col.tag == "Meteor"){
            isMelting = true;
        }
        
        if (touchingXWall && touchingZWall) {
            moveAI(-moveDirection);
        }
        else if (col.tag == "XWall") {
            touchingXWall = true;
            if (touchingXWall) {
                Vector3 newDirection = moveDirection;
                newDirection.x *= -1f;
                moveAI(newDirection);
            }
        }

        else if (col.tag == "ZWall") {
            touchingZWall = true;
            if (touchingZWall) {
                Vector3 newDirection = moveDirection;
                newDirection.z *= -1f;
                moveAI(newDirection);
            }
        }
    }

    void OnTriggerStay(Collider col) {
        if (touchingXWall && touchingZWall) {
            moveAI(-moveDirection);
        }
        else if (col.tag == "XWall") {
            if (touchingXWall) {
                Vector3 newDirection = moveDirection;
                newDirection.x *= -1f;
                moveAI(newDirection);
            }
        }

        else if (col.tag == "ZWall") {
            if (touchingZWall) {
                Vector3 newDirection = moveDirection;
                newDirection.z *= -1f;
                moveAI(newDirection);
            }
        }
    }

    void OnTriggerExit(Collider col){
        if (col.tag == "Meteor"){
            isMelting = false;
        }
        if (col.tag == "XWall")
            touchingXWall = false;
        if (col.tag == "ZWall")
            touchingZWall = false;
    }

	void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Meteor")
            isMelting = true;
		if (collision.collider.tag == "snowBall") {
            stun = true;
            timeStampStun = Time.time + 2;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.collider.tag == "Meteor")
            isMelting = false;
    }
}
