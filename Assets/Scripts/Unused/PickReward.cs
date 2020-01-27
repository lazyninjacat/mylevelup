using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickReward : MonoBehaviour {

	private DataService ds;
	
	private Button reward1;
    private Button reward2;
    private Button reward3;
    private Button reward4;
    private Button reward5;
    private Button reward6;


	// Use this for initialization
	void Start () {
		reward1 = GameObject.Find("Reward1").GetComponent<Button>();
		reward2 = GameObject.Find("Reward2").GetComponent<Button>();
		reward3 = GameObject.Find("Reward3").GetComponent<Button>();
        reward4 = GameObject.Find("Reward4").GetComponent<Button>();
        reward5 = GameObject.Find("Reward5").GetComponent<Button>();
        reward6 = GameObject.Find("Reward6").GetComponent<Button>();

        reward1.interactable = false;
        reward2.interactable = false;
        reward3.interactable = false;
        reward4.interactable = false;
        reward5.interactable = false;
        reward6.interactable = false;


        //DB Call to find out what rewards are selectable
        ds = StartupScript.ds;

		int rewardConfig = 0;
		var settings = ds.GetGameSettingsTable();
		foreach(var row in settings){
			rewardConfig = row.reward_config;
		}
		
		if (rewardConfig == 2 || rewardConfig == 6 || rewardConfig == 10 || rewardConfig == 14){
			reward1.interactable = true;
		}
		if (rewardConfig == 4 || rewardConfig == 6 || rewardConfig == 12 || rewardConfig == 14){
			reward2.interactable = true;
		}
		if (rewardConfig == 8 || rewardConfig == 10 || rewardConfig == 12 || rewardConfig == 14){
			reward3.interactable = true;
		}


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
