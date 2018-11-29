using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallBehaviour : MonoBehaviour {

	public float ballStrength = 40f;

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.tag == "Player") {

			// On calcule la direction dans laquelle l'objet touché doit aller
			Vector3 contactPoint = collision.contacts[0].point;
			Vector3 objectHitPosition = collision.gameObject.transform.position;
			Vector3 knockBackDir = objectHitPosition - contactPoint;
			Vector3 knockBackDirNormalized = Vector3.ProjectOnPlane(knockBackDir, Vector3.up).normalized;

			// On le bouge
			//Vector3.MoveTowards(objectHitPosition, new Vector3(100, objectHitPosition.y, 100), 100f);
			collision.gameObject.transform.Translate(knockBackDirNormalized * ballStrength);

			// On détruit la boule de neige
			Destroy(gameObject);
		}
    }
}
