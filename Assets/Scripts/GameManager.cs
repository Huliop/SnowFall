using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	
	private static bool created = false;
	public bool difficult = false;
	private GameObject ia;
	private GameObject player;
	private float goalScale;

	void Awake () {
		if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
		}
		ia = GameObject.Find("IA");
		player = GameObject.Find("Player");
		
		goalScale = 12;
	}
	
	// Update is called once per frame
	void Update () {
		if (ia != null){
			if (ia.transform.localScale.x > goalScale )
				StartCoroutine(gameOver());
			else if (player.transform.localScale.x > goalScale)
				StartCoroutine(win());
		}
	}

	public bool isDifficultOn(){
		return difficult;
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
