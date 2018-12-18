using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

	public Button play;
	public Button credit;
	public Button menu;
	public Button exit;
	public GameObject main;
	public GameObject credits;

	// Use this for initialization
	void Start () {
		if (gameObject.tag == "EndGame"){
			play.onClick.AddListener(playOnClick);
			menu.onClick.AddListener(menuOnClick);
		}
		else {
			play.onClick.AddListener(playOnClick);
			credit.onClick.AddListener(creditOnClick);
			exit.onClick.AddListener(exitOnClick);
		}
	}

	// les différentes actions à effectuer selon le bouton cliqué  

	void playOnClick(){
		changeScene("Map");
	}

	void creditOnClick(){
		main.SetActive(false);
		credits.SetActive(true);
		menu.onClick.AddListener(menuOnClick);
	}

	void menuOnClick(){
		if (gameObject.tag == "EndGame")
			changeScene("Menu");
		else {
			main.SetActive(true);
			credits.SetActive(false);
		} 
	}

	void exitOnClick(){
		Application.Quit();
	}

	void changeScene(string scene) {
        SceneManager.LoadScene(scene);

    }
}
