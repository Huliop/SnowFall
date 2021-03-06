﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] Camera mainCamera;

    //Variables
    public float speed = 20f;
    public float rotSpeed = 0.05f;
    public float snowBallSpeed = 250f;
    public GameObject prefab;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 forward = Vector3.zero;
    public float radius = 1f;
    bool isMelting = false;
    private bool stun = false;
    private float timeStampStun;
    bool touchingXWall = false;
    bool touchingZWall = false;
 
    void Update() {
        // la taille du joueur augmente en continue donc son rayon est mis à jour à chaque frame
        radius = transform.localScale.x;
        if (!stun){
            forward = Vector3.ProjectOnPlane(mainCamera.transform.forward.normalized, Vector3.up);

            movePlayer(getMoveDirection(forward));
            
            if (Input.GetMouseButtonDown(0)) {
                shoot(forward);
            }
        }
        else {
            if (timeStampStun < Time.time)
                stun = false;
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
        Vector3 newScale = transform.localScale - Vector3.one * 0.1f;
        if (newScale.x > 0)
            transform.localScale = newScale;
    }

    // fonction qui réduit la taille de la boule si elle se trouve dans le rayon d'un météor
    void updateSize() {
        if (isMelting) {
            Vector3 newScale = transform.localScale - Vector3.one * 0.04f;
            if (newScale.x > 0)
                transform.localScale = newScale;
        }
    }

    // la vistesse de déplacement dépend de la taille
    void updateSpeed() {
        speed = 20f - transform.localScale.x;
    }

    // on détecte les collisions
    void OnTriggerEnter(Collider col){
        if (col.tag == "Meteor"){
            isMelting = true;
        }
        // pour ne pas traverser les murs
        if (touchingXWall && touchingZWall) {
            movePlayer(-moveDirection);
        }
        else if (col.tag == "XWall") {
            touchingXWall = true;
            if (touchingXWall) {
                Vector3 newDirection = moveDirection;
                newDirection.x *= -1f;
                movePlayer(newDirection);
            }
        }

        else if (col.tag == "ZWall") {
            touchingZWall = true;
            if (touchingZWall) {
                Vector3 newDirection = moveDirection;
                newDirection.z *= -1f;
                movePlayer(newDirection);
            }
        }
    }

    void OnTriggerStay(Collider col) {
        if (touchingXWall && touchingZWall) {
            movePlayer(-moveDirection);
        }
        else if (col.tag == "XWall") {
            if (touchingXWall) {
                Vector3 newDirection = moveDirection;
                newDirection.x *= -1f;
                movePlayer(newDirection);
            }
        }

        else if (col.tag == "ZWall") {
            if (touchingZWall) {
                Vector3 newDirection = moveDirection;
                newDirection.z *= -1f;
                movePlayer(newDirection);
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

    public bool isStun(){
        return stun;
    }

    public void meltOff(){
        isMelting = false;
    }

    public bool melt(){
        return isMelting;
    }
 }

