﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

	public Button play;
	public Button credit;
	public Button menu;
	public Button exit;

	// Use this for initialization
	void Start () {
		if (gameObject.tag == "Credit"){
			menu.onClick.AddListener(menuOnClick);
			
		}
		else if (gameObject.tag == "EndGame"){
			play.onClick.AddListener(playOnClick);
			menu.onClick.AddListener(menuOnClick);
		}
		else {
			play.onClick.AddListener(playOnClick);
			credit.onClick.AddListener(creditOnClick);
			exit.onClick.AddListener(exitOnClick);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void playOnClick(){
		changeScene("Map");
	}

	void creditOnClick(){
		changeScene("Credit");
	}

	void menuOnClick(){
		changeScene("Menu");
	}

	void exitOnClick(){
		Application.Quit();
	}

	void changeScene(string scene) {
        SceneManager.LoadScene(scene);

    }
}
