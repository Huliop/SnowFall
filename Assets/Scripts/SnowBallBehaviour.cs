using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallBehaviour : MonoBehaviour {

	public float ballStrength = 40f;

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.tag == "Player") {
			GameObject player = collision.gameObject;
			// On calcule la direction dans laquelle l'objet touché doit aller
			Vector3 contactPoint = collision.contacts[0].point;
			Vector3 objectHitPosition = player.transform.position;
			Vector3 knockBackDir = objectHitPosition - contactPoint;
			Vector3 knockBackDirNormalized = Vector3.ProjectOnPlane(knockBackDir, Vector3.up).normalized;

			// On le bouge
			//Vector3.MoveTowards(objectHitPosition, new Vector3(100, objectHitPosition.y, 100), 100f);
			//collision.gameObject.transform.Translate(knockBackDirNormalized * ballStrength);
			Vector3 newPos = player.transform.position + knockBackDirNormalized * 5;
			newPos.x = Mathf.Clamp(newPos.x, 0 + player.transform.localScale.x, 100 - player.transform.localScale.x);
			newPos.z = Mathf.Clamp(newPos.z, 0 + player.transform.localScale.x, 100 - player.transform.localScale.x);
			player.GetComponent<Rigidbody>().MovePosition(newPos);

			// On détruit la boule de neige
			Destroy(gameObject);
		}
		if (collision.collider.tag == "Terrain") {
			// On détruit la boule de neige
			Destroy(gameObject);
		}
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) {
			// On détruit la boule de neige
			Destroy(gameObject);
		}
    }
}
