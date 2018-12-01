using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] Camera mainCamera;

    //Variables
    public float speed = 20f;
    public float rotSpeed = 0.05f;
    public float snowBallSpeed = 50f;
    public GameObject prefab;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 forward = Vector3.zero;
    public float radius = 1f;
    bool isMelting = false;
 

    void Update() {
        forward = Vector3.ProjectOnPlane(mainCamera.transform.forward.normalized, Vector3.up);
        radius = transform.localScale.x;

        movePlayer(getMoveDirection(forward));
        
        if (Input.GetMouseButtonDown(0)) {
            shoot(forward);
        }

        updateSize();
        updateSpeed();
    }

    public Vector3 getDirection() {
        return moveDirection;
    }

    Vector3 getMoveDirection(Vector3 forward) {

        // On calcule la direction dans laquelle le joueur veut aller en fonction de la où il regarde
        Vector3 forwardOrthoPLanXZ = Vector3.Cross(Vector3.up, forward);
        moveDirection = (forward.normalized * Input.GetAxisRaw("Vertical") + forwardOrthoPLanXZ.normalized * Input.GetAxisRaw("Horizontal")).normalized;
        return moveDirection;
    }

    void movePlayer(Vector3 moveDirection) {

        // On calcule l'axe autour de laquelle la boule va rouler
        Vector3 rotationAxis = Vector3.Cross(moveDirection, Vector3.up);

        // On met à jour la position et la rotation de la boule
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        transform.RotateAround(transform.position, rotationAxis, -Mathf.Sin(moveDirection.magnitude*rotSpeed*2*Mathf.PI)*Mathf.Rad2Deg);
    }

    void shoot(Vector3 forward) {

        // On instancie la boule de neige
        GameObject clone = Instantiate(prefab, transform.position + forward.normalized * (radius/2 + prefab.transform.localScale.x/2) , Quaternion.identity);

        clone.GetComponent<Rigidbody>().AddForce(forward.normalized * snowBallSpeed);
        Destroy(clone, 10f);
        transform.localScale -= Vector3.one * 0.1f;
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
            movePlayer(-moveDirection);
        }
    }

    void OnCollisionStay(Collision collision) {
        if (collision.collider.tag == "Wall") {
            movePlayer(-moveDirection);
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.collider.tag == "Meteor")
            isMelting = false;
    }
 }

