using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsController : MonoBehaviour {

	//Reference to the DataService script
	public DataService ds;

	private SceneController sc;

	[SerializeField] private string SceneChangeOnSave;

	//UI References
	private Dropdown gameTypeDD;
	private Text gameTypeValue;

	private Dropdown difficultyDD;
	private Text difficultyValue;

	private Dropdown addWordsDD;
	private Text addWordsValue;

	private Toggle ytToggle;
	private Toggle nToggle;
	private Toggle mToggle;
	private InputField rewardTimeLimit;
	private Toggle snapToggle;

	private Button saveButton;
	private Button editButton;

	// Use this for initialization
	void Start () {

		//Create new DataService object and set reference
		ds = StartupScript.ds;

		sc = GameObject.FindGameObjectWithTag("GameSettingsController").GetComponent<SceneController>(); 

		//Set references to UI objects
		gameTypeDD = GameObject.Find("GameTypeDropdown").GetComponent<Dropdown>();
		gameTypeValue = GameObject.Find("GameTypeValue").GetComponent<Text>();

		difficultyDD = GameObject.Find("DifficultyDropdown").GetComponent<Dropdown>();
		difficultyValue = GameObject.Find("DifficultyValue").GetComponent<Text>();

		addWordsDD = GameObject.Find("AddWordsDropdown").GetComponent<Dropdown>();
		addWordsValue = GameObject.Find("AddWordsValue").GetComponent<Text>();

		ytToggle = GameObject.Find("YouTubeToggle").GetComponent<Toggle>();
		ytToggle.isOn = false;
		nToggle = GameObject.Find("NetflixToggle").GetComponent<Toggle>();
		nToggle.isOn = false;
		mToggle = GameObject.Find("MinecraftToggle").GetComponent<Toggle>();
		mToggle.isOn = false;

		rewardTimeLimit = GameObject.Find("TimeLimitInputField").GetComponent<InputField>();

		snapToggle = GameObject.Find("LetterSnappingToggle").GetComponent<Toggle>();
		snapToggle.isOn = true;

		saveButton = GameObject.Find("SaveButton").GetComponent<Button>();
		editButton = GameObject.Find("WordListEditButton").GetComponent<Button>();
		

		//Call to DB to retrieve settings
		RetrieveSettings();
	}

	//################## LOADING DATA FUNCTIONS ##################//

	///<summary>Calls the DB, retrieves the Game_Settings table, sets the values of all the UI elements</summary>

	private void RetrieveSettings(){
		//Retrieve Game_Settings table data
		IEnumerable<Game_Settings> retrievedSettings = ds.GetGameSettingsTable();
		
		//Get the row containing the game settings and set each UI element
		foreach (var row in retrievedSettings){
			gameTypeValue.text = row.game_type;
			gameTypeDD.value = row.reward_id;
			difficultyValue.text = DifficultyToString(row.game_difficulty);

			//Decide how to set Word config UI based on data from DB
			switch(row.word_config){
				case "automatic":
					addWordsValue.text = "automatic";
					addWordsDD.value = 1;
					editButton.interactable = false;
					break;
				case "manual":
					addWordsValue.text = "manual";
					addWordsDD.value = 2;
					editButton.interactable = true;
					break;
				default:
					addWordsValue.text = "";
					addWordsDD.value = 0;
					editButton.interactable = false;
					break;
			}

			//Calls function to set the proper reward toggles
			LoadRewardToggles(row.reward_config);

			//Convert time limit to a string and set the UI element
			rewardTimeLimit.text = (row.reward_time_limit).ToString();

			//Decide how the Snap toggle should be set based on data from DB
			switch(row.letter_snap){
				case 0:
					//Debug.Log("Snap toggle case 0 - False entered");
					snapToggle.isOn = false;
					break;
				case 1:
					//Debug.Log("Snap toggle case 1 - true entered");
					snapToggle.isOn = true;
					break;
				default:
					//Debug.Log("Snap toggle default entered");
					snapToggle.isOn = false;
					break;
			}
		}
	}

	///<summary>Generate the proper string indicating difficulty by analyzing the stored value in the DB</summary>
	///<param name="diff">the difficulty value returned from the DB</param>
	///<returns> a string representing the difficulty</returns>

	private string DifficultyToString(int diff){
		string retVal;
		switch(diff){
			case 0: case 1:
				retVal = "Very Easy";
				difficultyDD.value = 1;
				break;
			case 2: case 3: case 4:
				retVal = "Easy";
				difficultyDD.value = 2;
				break;
			case 5: case 6:
				retVal = "Medium";
				difficultyDD.value = 3;
				break;
			case 7: case 8: case 9: case 10:
				retVal = "Hard";
				difficultyDD.value = 4;
				break;
			default:
				retVal = "Error";
				break;
		}

		return retVal;
	}
	
	///<summary>Handles setting the proper reward toggles based on the int from the DB</summary>
	///<param name="val">the integer value representing which rewards are unlocked - Youtube toggle = 2, Netflix toggle = 4, Minecraft toggle = 8</param>

	private void LoadRewardToggles(int val){
		if (val == 2 || val == 6 || val == 10 || val == 14){
			ytToggle.isOn = true;
		}
		if (val == 4 || val == 6 || val == 12 || val == 14){
			nToggle.isOn = true;
		}
		if (val == 8 || val == 10 || val == 12 || val == 14){
			mToggle.isOn = true;
		}
	}

	//################## INSERTING DATA FUNCTIONS ##################//

	///<summary>Called by Save Button to save settings to the DB</summary>

	public void SaveSettings(){

		//Decide if a word list id should be included or send 0 for null
		int wordListId = 0;
		switch(addWordsValue.text){
			case "manual":
				wordListId = 1;
				break;
			default:
				break;
		}

		//Convert reward time limit input field to an int
		int rewardLimit = Int32.Parse(rewardTimeLimit.text);

		//Decide which value to save on whether snap is toggled or not
		int snap = 0;
		if (snapToggle.isOn){snap = 1;}

		//Call to update the settings in the DB and get how many rows were updated
		int rowsUpdate = ds.UpdateGameSettings(
		gameTypeValue.text,
		DifficultyToInt(difficultyValue.text),
		addWordsValue.text,
		SaveRewardToggles(),
		gameTypeDD.value,
		wordListId,
		rewardLimit,
		snap
		);

		Debug.Log("Update called. Num of rows updated: " + rowsUpdate.ToString());
		
		//If any rows were updated, load the admin menu scene
		if (rowsUpdate > 0){
			sc.LoadSceneByName(SceneChangeOnSave);
		}
		else{
			Debug.Log("Error updating the DB");
		}
	}

	///<summary>Converts the string representing the game difficult to the proper int, or return 99 which signifies an error</summary>
	///<param name="diff">the string representing the difficulty</param>
	///<returns>an int representation of the difficulty</returns>

	private int DifficultyToInt(string diff){
		int retVal = 99;
		switch(diff){
			case "Very Easy":
				retVal = 1;
				break;
			case "Easy":
				retVal = 4;
				break;
			case "Medium":
				retVal = 6;
				break;
			case "Hard":
				retVal = 7;
				break;
			default:
				break;
		}
		return retVal;
	}

	///<summary>Calculates the value used in the DB representing which rewards are available to the user</summary>
	///<returns>the value as an int</returns>

	private int SaveRewardToggles(){
		int retVal = 0;
		if (ytToggle.isOn){
			retVal += 2;
		}
		if (nToggle.isOn){
			retVal += 4;
		}
		if (mToggle.isOn){
			retVal += 8;
		}
		return retVal;
	}


	// Update is called once per frame
	void Update () {
		//If the reward limit inputfield is empty disable save button
		if (rewardTimeLimit.text.Equals("")){
			saveButton.interactable = false;
		}
		else{
			if (!saveButton.IsInteractable()){
				saveButton.interactable = true;
			}
		}

		//If word config manual selected allow button to be used
		if (addWordsValue.text.Equals("manual")){
			editButton.interactable = true;
		}
		else{
			editButton.interactable = false;
		}
	}
}
