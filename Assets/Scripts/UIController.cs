using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	[SerializeField] GameManager gameManager;
	[SerializeField] GameObject player;
	[SerializeField] GameObject ai;
	[SerializeField] Image sizePlayer;
	[SerializeField] Image sizeAI;
	[SerializeField] Text playerText;
	[SerializeField] Text aiText;
	float currentPlayerSize;
	float currentAISize;
	float goalScale;
	
	void Start () {
		goalScale = gameManager.getGoalScale();
	} 

	void Update () {
		updateData();
		updateSizePlayer();
		updateSizeAI();
		updateText();
	}

	void updateData() {
		currentPlayerSize = player.transform.localScale.x;
		currentAISize = ai.transform.localScale.x;
	}

	void updateSizePlayer() {
		float ratio = currentPlayerSize / goalScale;
        sizePlayer.rectTransform.localScale = new Vector3(ratio, 1, 1);
	}

	void updateSizeAI() {
		float ratio = currentAISize / goalScale;
        sizeAI.rectTransform.localScale = new Vector3(ratio, 1, 1);
	}

	void updateText() {
		playerText.text = (currentPlayerSize/goalScale * 100).ToString("00.0");
		aiText.text = (currentAISize/goalScale * 100).ToString("00.0");
	}
}
