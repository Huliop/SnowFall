using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : MonoBehaviour {

	public float speed = 20f;
	public float rotSpeed = 0.05f;
	private Vector3 forward = Vector3.zero;
	private Vector3 moveDirection = Vector3.zero;
	bool isMelting = false;

	// Use this for initialization
	void Start () {
		updateForward();
	}
	
	// Update is called once per frame
	void Update () {

		updateMoveDirection();
		moveAI(moveDirection);

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
            transform.localScale -= new Vector3(1,1,1) * 0.02f;
        }
    }
	void updateSpeed() {
        speed = 20f - transform.localScale.x;
	}

	void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Meteor")
            isMelting = true;
        if (collision.collider.tag == "Wall") {
            moveAI(-moveDirection);
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
