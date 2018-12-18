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
    private GameObject gameManager;
    private GameManager gameManagerScript;
    private GameObject terrainObject;
    private TerrainBehaviour terrainScript;
    private bool difficult;
    private Vector3 wayPoint;
    private float distanceTolerance; 

	// Use this for initialization
	void Start () {
		updateWanderTarget();
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();
        difficult = gameManagerScript.isDifficultOn();

        terrainObject = GameObject.Find("Terrain");
        terrainScript = terrainObject.GetComponent<TerrainBehaviour>();

		timeStampShoot = Time.time + 5;
        distanceTolerance = transform.localScale.x + 4;
        wayPoint = new Vector3(85,0,85);
	}
	
	// Update is called once per frame
	void Update () {
        // comme la boule grossit, son rayon est mis à jour à chaque frame
		radius = transform.localScale.x;
        distanceTolerance = radius + 4;

		if (!stun){
            // on calcule la direction de l'IA 
			updateMoveDirection();
            // puis on la recalcule au contact d'un obstacle pour l'éviter
            avoidObstacles(radius);
            // on peut ensuite la faire avancer
			moveAI(moveDirection);

            // logique floue du tir
            float dif = radius - player.transform.localScale.x;
            // l'IA ne tire que si sa taille est supérieure à celle du joueur (- la perte en tirant)
            if (dif > 0.1f){
                if (timeStampShoot < Time.time){
                    // la probabilité varie selon la distance entre les deux boules
                    float distance = Vector3.Distance(player.transform.position, transform.position);
                    int r = Random.Range(0,101);
                    float res = Mathf.Clamp(r - distance + dif*5, 0, 100);
                    if (res > 40){
                        // on tire vers l'emplacement actuel du joueur
                        Vector3 direction = player.transform.position - transform.position;
                        shoot(direction);
                        timeStampShoot = Time.time + 3;
                    }
                }
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
        if (!difficult){
            // toujours le cas ici
		    updateWanderTarget();

            Vector3 posTarget = wanderTarget + transform.position + moveDirection*wanderDistance;

            moveDirection = (posTarget - transform.position).normalized;
        }
        else {
            if (Vector3.Distance(transform.position, wayPoint) < distanceTolerance){
                updateWayPoint();
            }
            moveDirection = (wayPoint - transform.position).normalized;
        }
        moveDirection.y = 0;
	}

    // fonction qui met à jour la position du target point dans le wandering
	void updateWanderTarget() {
		float randX = Random.Range(-1f,1f);
		float randZ = Random.Range(-1f,1f);
		wanderTarget += new Vector3(randX * wanderJitter, 0, randZ * wanderJitter);
		wanderTarget = wanderTarget.normalized * wanderRadius;
	}

    // fonciton qui met à jour la position du way point
    // inutilisée ici
    void updateWayPoint(){
        wayPoint = terrainScript.getMaxHeightPos();
        wayPoint.y = transform.position.y;
    }

    // fonction qui modifie la direction de l'IA pour éviter les météors et les murs
    void avoidObstacles(float radius){
        // on trace un rayon de chaque côté de la boule, dans la direction où elle avance
        RaycastHit hit1, hit2;
        Vector3 normale = Vector3.Cross(moveDirection, Vector3.up).normalized;
        Physics.Raycast(new Vector3(transform.position.x + normale.x * radius/2, 0, transform.position.z + normale.z * radius/2), moveDirection, out hit1, radius/2 + 5);
        Physics.Raycast(new Vector3(transform.position.x - normale.x * radius/2, 0, transform.position.z - normale.z * radius/2), moveDirection, out hit2, radius/2 + 5);
        
        float d1 = (hit1.distance == 0 ? 100 : hit1.distance);
        float d2 = (hit2.distance == 0 ? 100 : hit2.distance);
        // on décale la direction selon la normale de l'impact le plus proche
        if (d2 < d1){
            Vector3 hitNormal = hit2.normal;
            hitNormal.y = 0.0f; //Don't want to move in Y-Space  
            moveDirection = (moveDirection + hitNormal + normale).normalized;
        }
        else if ( d1 < d2){
            Vector3 hitNormal = hit1.normal;
            hitNormal.y = 0.0f; //Don't want to move in Y-Space  
            moveDirection = (moveDirection + hitNormal - normale).normalized;
        }
    }

	void shoot(Vector3 forward) {
        if (!difficult){
            // On instancie la boule de neige
            GameObject clone = Instantiate(prefab, transform.position + forward.normalized * (radius/2 + prefab.transform.localScale.x/2) , Quaternion.identity);
            clone.GetComponent<Rigidbody>().AddForce(forward.normalized * snowBallSpeed);
            // la boule est détruire au bout de 10 secondes
            Destroy(clone, 10f);
            Vector3 newScale = transform.localScale - Vector3.one * 0.1f;
            if (newScale.x > 0)
                transform.localScale = newScale;
            }
    }

	public Vector3 getDirection() {
        return moveDirection;
    }

    public bool isStun(){
        return stun;
    }

    // fonction qui réduit la taille de la boule si celle-ci se trouve dans le rayon d'un météor
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

    // détection des collisions
	void OnTriggerEnter(Collider col){
        if (col.tag == "Meteor"){
            isMelting = true;
        }
        
        // pour éviter de traverser les murs
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

    public bool melt(){
        return isMelting;
    }

    public void meltOff(){
        isMelting = false;
    }
}
