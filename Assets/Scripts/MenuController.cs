using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public Button play;
	public Button credit;
	public Button exit;

	// Use this for initialization
	void Start () {
		play.onClick.AddListener(playOnClick);
		credit.onClick.AddListener(creditOnClick);
		exit.onClick.AddListener(exitOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void playOnClick(){
		StartCoroutine(changeScene("Map"));
	}

	void creditOnClick(){
		StartCoroutine(changeScene("Credit"));
	}

	void exitOnClick(){
		Application.Quit();
	}

	IEnumerator changeScene(string scene) {
		yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(scene);

    }
}
