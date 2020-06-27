using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VW_RewardsList : MonoBehaviour {


    private const int PIC_WIDTH = 50;
    private const int PIC_HEIGHT = 50;

    private const string CLASSNAME = "VW_RewardsList: ";

	private const string PREVIOUS_SCENE = "admin_menu";
	private const string EDIT_ADD_SCENE = "reward_edit_add";
    private const string ADD_WEBSITE_SCENE = "reward_website_edit_add";

	private const string MODAL_SUCCESS_TEXT = " was successfully deleted !";
	private const string MODAL_ERROR_TEXT = "There was an error deleting the reward.\n Please try again";

	/* 
	Number of static rewards in the game that you dont want to be editable/deleteable.
	These rewards will show up first in the DB table, so this const
	is used to check the id of the reward against this const var.
	If the id <= numOfStaticRewards, it is a static reward.
	*/
	private const int NUM_OF_STATIC_REWARDS = 5;

	private CON_RewardsList controller;


	private int defaultEntryCount = 5;
	private int currentEntries = 0;
	private float entrySize = 0.0f;

	[SerializeField] private GameObject viewportContent;
	[SerializeField] private RectTransform vpContentBoxRect;
	[SerializeField] private Button addNewButton;
	[SerializeField] private Button cancelButton;

	//Instantiable panel used for populating the rewards into View Port content
	[SerializeField] private GameObject itemPanel;

	[SerializeField] private GameObject confirmModal;
	[SerializeField] private GameObject resultModal;
	[SerializeField] private Text resultModalText;
    [SerializeField] GameObject rewardTypeModal;


    private string keyToDelete;
	private GameObject objToDelete;




    // View Initialization
    void Start () {
		Debug.Log(CLASSNAME + "Starting up view");
		MAS_RewardsList master = (MAS_RewardsList)COM_Director.GetMaster("MAS_RewardsList");
		VerticalLayoutGroup verticalLayout = viewportContent.transform.GetComponent<VerticalLayoutGroup>();
		// entrySize = g.cellSize.y + g.spacing.y;
				
		if (master != null){
			controller = (CON_RewardsList)master.GetController("CON_RewardsList");
			if (controller != null){
				Debug.Log(CLASSNAME + "***************** CONTROLLER NOT NULL");
				Dictionary<string, DO_Reward> data = controller.GetRewardsListData();

				foreach (var entry in data){
					AddRewardItem(entry.Value.id, entry.Value.reward_name);	
				}
				AlterContentBoxSizing(currentEntries);
			}
		}	
	}

	private void AddRewardItem(int id, string name){
		GameObject panel = GameObject.Instantiate(itemPanel, viewportContent.transform, false);
		panel.name = name;
		panel.transform.Find("Text").GetComponent<Text>().text = name;

        GameObject panelImage = panel.transform.Find("RewardIcon").gameObject;
        RawImage img = panelImage.GetComponent<RawImage>();
        Texture2D tx = new Texture2D(PIC_WIDTH, PIC_HEIGHT);
        byte[] rewardPic = FileAccessUtil.LoadRewardPic(name);

        //If custom reward with custom pic
        if (rewardPic != null)
        {
            if (tx.LoadImage(rewardPic))
            {
                img.texture = tx;
            }
            else
            {
                Debug.Log(CLASSNAME + "Loading custom picture failed.");
            }

        }
        //else is a stock reward
        else
        {
            img.texture = Resources.Load<Texture2D>("RewardPictures/" + name);
        }

        if (id <= NUM_OF_STATIC_REWARDS){
			//panel.transform.Find("Edit").GetComponent<Button>().interactable = false;
			panel.transform.Find("Delete").GetComponent<Button>().interactable = false;
		}
		panel.SetActive(true);
		currentEntries++;
	}

	private void AlterContentBoxSizing(int entries){
		int e = entries;
		if (e < 0){
			Debug.LogError(CLASSNAME + " AlterContentSizing: The value passed must be >= 0. Using default value.");
			e = defaultEntryCount;
		}
		float newHeight = entrySize * e;
		vpContentBoxRect.sizeDelta = new Vector2(vpContentBoxRect.sizeDelta.x, newHeight);
	}

	public void AddEditRequest(){
        rewardTypeModal.SetActive(false);
        if (EventSystem.current.currentSelectedGameObject.name.Equals("Edit")){
			GameObject obj = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
			controller.KeyToEdit = obj.name;
			Debug.Log(CLASSNAME + " **** Edit requested - set entry to edit as " + controller.KeyToEdit);

            if (controller.CheckIsWebsiteReward(obj.name))
            {
                controller.SceneChange(ADD_WEBSITE_SCENE, true, true);
            }
            else
            {
                controller.SceneChange(EDIT_ADD_SCENE, true, false);
            }

        }
		else{
			controller.KeyToEdit = "";
			Debug.Log(CLASSNAME + " **** Add Requested - KeyToEdit set to empty string");
			controller.SceneChange(EDIT_ADD_SCENE, false, false);
		}
	}

	public void DeleteConfirmationOpen(){
		objToDelete = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
		keyToDelete = objToDelete.name;
		confirmModal.SetActive(true);
		addNewButton.interactable = false;
		cancelButton.interactable = false;
	}

	public void DeleteReward(){
		Debug.Log(CLASSNAME + "Delete confirmed - initiating Delete request");
		//Hide delete confirm popup
		confirmModal.SetActive(false);
		// Request Controller to initiate delete and return if successful
		if(controller.RequestToDeleteReward(keyToDelete)){
			controller.RequestToDeletePhoto(keyToDelete);
			//was successful
			resultModalText.text = keyToDelete + MODAL_SUCCESS_TEXT;
			resultModal.SetActive(true);
			Destroy(objToDelete);
			currentEntries--;
			AlterContentBoxSizing(currentEntries);
		}
		else{
			//did not delete / error
			resultModalText.text = MODAL_ERROR_TEXT;
			resultModal.SetActive(true);
		}
	}

	public void CloseModal(){
		confirmModal.SetActive(false);
		resultModal.SetActive(false);
		objToDelete = null;
		keyToDelete = null;
		addNewButton.interactable = true;
		cancelButton.interactable = true;
	}

	public void ReturnToPreviousScene(){
		controller.ClearData();
		controller.SceneChange(PREVIOUS_SCENE);
	}
	
	


    //////////////////////////////// NEW CODE
    ///

    public void OnAddNewButton()
    {
        rewardTypeModal.SetActive(true);
    }

    public void OnWebsiteButton()
    {
        rewardTypeModal.SetActive(false);
        controller.SceneChange(ADD_WEBSITE_SCENE);
    }


}
