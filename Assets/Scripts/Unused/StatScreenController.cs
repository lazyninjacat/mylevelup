using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatScreenController : MonoBehaviour {

	//Reference to DataService
	private DataService ds;

	//Reference to UI User drop down
	private Dropdown userDD;
	//Associated List of user id's
	private List<int> userId;

	//References to content containers of the scroll views
	private GameObject gameHistoryContainer;
	private GameObject rewardHistoryContainer;

	// Use this for initialization
	void Start () {

		//Create reference to DataService
		ds = StartupScript.ds;

		//Create references to UI elements
		userDD = GameObject.Find("AvailableUsersDropDown").GetComponent<Dropdown>();
		gameHistoryContainer = GameObject.FindGameObjectWithTag("GHistory");
		rewardHistoryContainer = GameObject.FindGameObjectWithTag("RHistory");

		//Create User ID list and fill index 0 to match dropdown list
		userId = new List<int>();
		userId.Add(0);

		//Query DB and pull all users from the DB
		IEnumerable<Users> users = ds.GetUsersTable();
		//Popuplate the dropdown with all the users and the userId list with all the user id's
		foreach (var user in users){
            Dropdown.OptionData od = new Dropdown.OptionData();
			od.text = user.user_name;
			userDD.options.Add(od);
			userId.Add(user.user_id);
		}
	}
	
	///<summary> Retrieves Game History data from the DB and populates the GameHistory UI container with the results</summary>

	public void PopulateUserGameHistory(){
		int uid = userId[userDD.value];
		IEnumerable<Game_History> results = ds.GetUserGameHistory(uid);
		foreach (var row in results){
			if (row.game_history_id > 0){
				GameObject obj = Instantiate((GameObject)Resources.Load("StatRow"), gameHistoryContainer.transform);
				Text t = obj.GetComponent<Text>();
				string spacer = "          ";
				t.text += row.game_history_id.ToString() + spacer +
				row.rounds_completed.ToString() + spacer +
				row.words_solved + spacer;

				//Turn solve time in seconds to proper minutes:seconds
				decimal d = (decimal) row.solve_time;
				d /= 60m;
				d = Math.Floor(d);
				int m = (int) d;
				int s = row.solve_time % 60;

				t.text += d.ToString() + ":" + s.ToString() + spacer + row.num_tile_moves.ToString();
			}
		}
	}

	///<summary> Retrieves Reward History data from the DB and populates the RewardHistory UI container with the results</summary>
	
	public void PopulateUserRewardHistory(){
		int uid = userId[userDD.value];
		IEnumerable<Reward_Stats> results = ds.GetUserRewardHistory(uid);
		foreach (var row in results){
			if (row.reward_stats_id > 0){
				GameObject obj = Instantiate((GameObject)Resources.Load("StatRow"), rewardHistoryContainer.transform);
				Text t = obj.GetComponent<Text>();
				string spacer = "          ";
				t.text += row.reward_stats_id.ToString() + spacer +
				row.reward_type + spacer +
				row.video_title + spacer +
				row.keywords + spacer +
				row.video_id;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
