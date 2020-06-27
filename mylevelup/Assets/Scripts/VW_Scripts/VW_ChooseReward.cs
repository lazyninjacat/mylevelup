using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class VW_ChooseReward : MonoBehaviour {

	private const string CLASSNAME = "VW_ChooseReward: ";
	private const string PREVIOUS_SCENE = "play_list";
	private const string TYPESTR = "Choose_Reward";
	private const int PIC_WIDTH = 75;
	private const int PIC_HEIGHT = 75;

	private CON_PlayList controller;
    private DO_ChooseReward chooseRewardObj;

    [SerializeField] private GameObject viewportContent;
	[SerializeField] private GameObject rewardPanel;
	[SerializeField] private InputField durationInput;
	[SerializeField] private Button saveButton;
	[SerializeField] private GameObject successModal;
	[SerializeField] private GameObject errorModal;

	private int toggleCount = 0;


	// View Initialization
	void Start () {
		Debug.Log(CLASSNAME + "creating view");
		MAS_PlayList master = (MAS_PlayList)COM_Director.GetMaster("MAS_PlayList");

		if (master != null){
			controller = (CON_PlayList)master.GetController("CON_PlayList");            
		
			if (controller != null){
				//Start setting up scene
				//Request Rewards from controller
				RequestData();
			}

            if (controller.CheckIfNewEntry())
            {
                Debug.Log("New Entry");
            }
            else
            {
                chooseRewardObj = JsonUtility.FromJson<DO_ChooseReward>(controller.GetJsonByIndex(controller.GetActiveContextIndex()));
                ToggleAllToggles();
            }

        }	
		
	}
	
	void Update () {
		
		if ((durationInput.text != "") && (toggleCount > 0)){
			saveButton.interactable = true;
		}
		else{
			saveButton.interactable = false;
		}
	}


	public void OnToggleChange(Toggle change){
		if (change.isOn){
			toggleCount++;
		}
		else{
			toggleCount--;
		}
	}

	private void RequestData(){
		Debug.Log(CLASSNAME + "Sending data request to the controller...");
		Dictionary<int, string> dict = controller.RequestRewards();

		//Debug.Log(CLASSNAME + "Available Rewards: ");
		foreach (var reward in dict){
			//Create a panel
			GameObject panel = GameObject.Instantiate(rewardPanel, viewportContent.transform, false);
			panel.name = reward.Key.ToString();
			GameObject label = panel.transform.Find("Label").gameObject;
			label.GetComponent<Text>().text = reward.Value;
			label.name = reward.Value.ToString();

			Toggle t = panel.GetComponent<Toggle>();
			t.onValueChanged.AddListener(delegate {
				OnToggleChange(t);
			});
	
			panel.SetActive(true);

			GameObject panelImage = panel.transform.Find("Image").gameObject;
			RawImage img = panelImage.GetComponent<RawImage>();
			Texture2D tx = new Texture2D(PIC_WIDTH,PIC_HEIGHT);
			byte[] rewardPic = FileAccessUtil.LoadRewardPic(reward.Value);
			//If not a custom reward
			if (rewardPic == null){
				img.texture = Resources.Load<Texture2D>("RewardPictures/" + reward.Value);
			}
			//else is a custom reward
			else{
				if (tx.LoadImage(rewardPic)){
					img.texture = tx;
				}
				else{
					Debug.Log(CLASSNAME + "Loading custom picture failed.");
				}
			}
		}
	}

    /// <summary>
    /// Creates and returns a list of reward id integers.
    /// </summary>
    /// <returns>A list of ints</returns>
    private List<int> CreateRewardIdsList()
    {
        List<int> list = new List<int>();

        foreach (Transform child in viewportContent.transform)
        {
            if (child.GetComponent<Toggle>().isOn)
            {
                list.Add(Int16.Parse(child.name));
            }
        }

        return list;
    }

    public void SaveRewardToPlayList(){

       
        // Get the duration from the timer input field
        int duration = Convert.ToInt32(durationInput.text);
		Debug.Log("Current Duration: " + duration.ToString());

        // Save this choose reward to the playlist through the controller
        Debug.Log(CLASSNAME + "Sending request to Controller to save reward to playlist...");

        DO_ChooseReward tempReward = new DO_ChooseReward(CreateRewardIdsList(), duration);

        // Success feedback
        if (controller.AddOrEditEntry(TYPESTR, duration, tempReward))
        {
            successModal.SetActive(true);
        }
        else
        {
            errorModal.SetActive(true);
            // TODO: Log an error
        }

    }

	//Cancel Button / Success Modal
	public void ReturnToPreviousScene(){
		controller.ClearData();
		//controller.SceneChange(PREVIOUS_SCENE);
	}

	//Error modal ok button
	public void CloseErrorModal(){
		Debug.Log(CLASSNAME + "Error modal Ok button clicked!");
	}

    public void ToggleAllToggles()
    {
        durationInput.text = (chooseRewardObj.duration.ToString());

        Dictionary<int, string> dict = controller.RequestRewards();

        foreach (var reward in dict)
        {
            if (chooseRewardObj.rewardIdsList.Contains(reward.Key))
            {
                viewportContent.transform.Find(reward.Key.ToString()).gameObject.GetComponent<Toggle>().isOn = true;
            }
        }




    }

}
