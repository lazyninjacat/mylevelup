using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/**
 * Controls the difficulty slider in word_settings scene
 */
public class SliderController : MonoBehaviour {

	// UI game objects
	public Slider slider;
	public Text sliderSetting;

	///<summary>Locates UI game objects in scene and invokes UpdateDifficultySetting function</summary>

	public void Start() {
		slider.GetComponent<Slider> ();
		sliderSetting.GetComponent<Text> ();
		UpdateDifficultySetting ();
	}

	///<summary>Updates difficulty setting of slider based on user usage</summary>
	
	public void UpdateDifficultySetting() {
		
		if ((slider.value >= 0) && (slider.value <= 1)) {
			// Debug.Log ("Difficulty setting is: " + difficultySlider.value);
			sliderSetting.text = (slider.value + " (Very Easy)");
		} 
		else if ((slider.value >= 2) && (slider.value <= 4)) {
			sliderSetting.text = (slider.value + " (Easy)");
		}
		else if ((slider.value >= 5) && (slider.value < 7)) {
			sliderSetting.text = (slider.value + " (Medium)");
		}
		else {
			sliderSetting.text = (slider.value + " (Hard)");
		}
	}
}
