using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour {

	public Button menu;

	// Use this for initialization
	void Start () {
		menu.onClick.AddListener(menuOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void menuOnClick(){
		StartCoroutine(changeScene("Menu"));
	}

	IEnumerator changeScene(string scene) {
		yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(scene);

    }
}
