using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] Camera mainCamera;

    //Variables
    public float speed = 6.0F;
    public float rotSpeed = 0.05f;
    public float snowBallSpeed = 50f;
    public GameObject prefab;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 forward = Vector3.zero;
    public float radius = 1f;
 

    void Update() {
        forward = Vector3.ProjectOnPlane(mainCamera.transform.forward.normalized, Vector3.up);
        radius = transform.localScale.x;

        movePlayer(getMouvement(forward));
        
        if (Input.GetMouseButtonDown(0)) {
            shoot(forward);
        }
     
    }

    public Vector3 getDirection(){
        return Vector3.Normalize(moveDirection);
    }


    Vector3 getMouvement(Vector3 forward) {
        Vector3 forwardOrthoPLanXZ = Vector3.Cross(Vector3.up, forward);
        return (forward.normalized * Input.GetAxisRaw("Vertical") + forwardOrthoPLanXZ.normalized * Input.GetAxisRaw("Horizontal")).normalized;
    }
    public void movePlayer(Vector3 moveDirection) {

        // On calcule l'axe de rotation
        Vector3 rotationAxis = Vector3.Cross(moveDirection, Vector3.up);

        // On met à jour la position et la rotation de la boule
        transform.Translate(moveDirection, Space.World);
        transform.RotateAround(transform.position, rotationAxis, -Mathf.Sin(moveDirection.magnitude*rotSpeed*2*Mathf.PI)*Mathf.Rad2Deg);
    }

    void shoot(Vector3 forward) {

        // On instancie la boule de neige
        GameObject clone = Instantiate(prefab, transform.position + forward.normalized * (radius/2 + prefab.transform.localScale.x/2) , Quaternion.identity);

        clone.GetComponent<Rigidbody>().AddForce(forward.normalized * snowBallSpeed);
        Destroy(clone, 10.0f);
        transform.localScale -= Vector3.one * 0.1f;
    }
 }

