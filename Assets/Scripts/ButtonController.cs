using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

	public Button replay;
	public Button menu;

	// Use this for initialization
	void Start () {
		replay.onClick.AddListener(replayOnClick);
		menu.onClick.AddListener(menuOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void replayOnClick(){
		StartCoroutine(changeScene("Map"));
	}

	void menuOnClick(){
		StartCoroutine(changeScene("Menu"));
	}

	IEnumerator changeScene(string scene) {
		yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(scene);

    }
}
