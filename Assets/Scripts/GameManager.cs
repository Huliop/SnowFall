using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public GameObject ia;
	public GameObject player;
	private float goalScale;

	// Use this for initialization
	void Start () {
		goalScale = 12;
	}
	
	// Update is called once per frame
	void Update () {
		if (ia.transform.localScale.x > goalScale )
			StartCoroutine(gameOver());
		else if (player.transform.localScale.x > goalScale)
			StartCoroutine(win());
	}

	public IEnumerator gameOver(){
		yield return new WaitForSeconds(0.1f);
		SceneManager.LoadScene("GameOverScene");
	}

	public IEnumerator win(){
		yield return new WaitForSeconds(0.1f);
		SceneManager.LoadScene("WinScene");
	}
}
