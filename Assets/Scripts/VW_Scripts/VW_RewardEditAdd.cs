using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EasyMobile;

public class VW_RewardEditAdd : MonoBehaviour {

    private const int PIC_WIDTH = 50;
    private const int PIC_HEIGHT = 50;

    private const string CLASSNAME = "VW_RewardEditAdd: ";

	private const string PREVIOUS_SCENE = "rewards_list";

	private const string TITLE_ADD = "Add Reward";
	private const string TITLE_EDIT = "Edit Reward";

	private const string MODAL_UPDATE_TEXT = " was updated successfully!";
    private const string MODAL_SAVE_TEXT = " was saved successfully!\nDo you want to add another reward?";
    private const string MODAL_EXISTS_TEXT = " already exists as a reward.\nTry again";
	private const string MODAL_ERROR_TEXT = "There was an error saving the reward.\nPlease try again.";

	private CON_RewardsList controller;
    private MOD_RewardsList model;

	private string rewardName = "";
    private string rewardType = "custom";
    private string rewardUrl = "";
	private int editId;

	private enum ModalType{
		EXISTS, SAVE, UPDATE, ERROR
	}

	[SerializeField] private Text titleLabel;
	[SerializeField] private GameObject inputFieldObj;
	[SerializeField] private InputField rewardNameInputField;
	[SerializeField] private GameObject rewardNameTextObj;
    [SerializeField] private Text wordFieldText;




    [SerializeField] private Button cameraButton;

	[SerializeField] private Button saveButton;
	[SerializeField] private Button cancelButton;

	[SerializeField] private GameObject textModal;
	[SerializeField] private Text modalText;
	[SerializeField] private GameObject modalDoneButton;
	[SerializeField] private GameObject modalYesButton;
	[SerializeField] private GameObject modalNoButton;
	[SerializeField] private GameObject modalOkButton;
    [SerializeField] private GameObject rewardImagePanel;
    [SerializeField] public RawImage imgPreview;




	// View Initialization
	void Start () {
		MAS_RewardsList master = (MAS_RewardsList)COM_Director.GetMaster("MAS_RewardsList");

		if (master != null){
			controller = (CON_RewardsList)master.GetController("CON_RewardsList");
		
			if (controller != null){
				if (controller.IsEditMode){
					SetupSceneEdit();
				}
				else{
					SetupSceneAdd();
				}

                LoadRewardImagePreview();



            }
		}	
	}
	

    private void LoadRewardImagePreview()
    {
        Debug.Log("Loading reward image preview");
        GameObject rewardImagePreview = rewardImagePanel.transform.Find("RewardImagePreview").gameObject;
        RawImage img = rewardImagePreview.GetComponent<RawImage>();
        Texture2D tx = new Texture2D(PIC_WIDTH, PIC_HEIGHT);
        byte[] rewardPic = FileAccessUtil.LoadRewardPic(rewardName);

        //If not a custom reward
        if (rewardPic == null)
        {
            img.texture = Resources.Load<Texture2D>("RewardPictures/" + rewardName);
        }
        //else is a custom reward
        else
        {
            if (tx.LoadImage(rewardPic))
            {
                tx.LoadImage(rewardPic);
                img.texture = tx;
            }
            else
            {
                Debug.Log(CLASSNAME + "Loading custom picture failed.");
            }
        }
    }

	void Update () {
		
		//if in Add Mode
		if (!controller.IsEditMode){
			if (rewardNameInputField.text == "")
            {
				saveButton.interactable = false;
				cameraButton.interactable = false;
			}
			else if (rewardNameInputField.text != "")
            {
                cameraButton.interactable = true;
                rewardName = rewardNameInputField.text;
			}
		}
		//Must be in edit mode then
		else
        {
			//If a change has occured in edit mode, enable the save button
			if (controller.IsThereACurrentPhoto()){
				saveButton.interactable = true;
			}
		}
	}

	private void SetupSceneEdit(){
			
			//Entering scene from: rewards_list
			
			if (!controller.IsThereACurrentPhoto())
            {
				Debug.Log(CLASSNAME + "***** Current Photo not set");
				//Send request for data to controller
				DO_Reward reward = controller.RequestRewardToEdit();
				if (reward.Equals(null)){
					Debug.LogError(CLASSNAME + "Error retrieving Reward to edit");
				}
				rewardName = reward.reward_name;
				editId = reward.id;
				titleLabel.text = TITLE_EDIT;
				saveButton.GetComponentInChildren<Text>().text = "Update";
				bool result = controller.CheckForRewardImage(rewardName);
				
				Debug.Log(CLASSNAME + "***** Does Photo Exist: " + result.ToString());
			}

			/*
			Entering scene from: reward_camera
			*/
			else{
				Debug.Log(CLASSNAME + "***** Current Photo set");
				//Original scene entry by Edit
				if (controller.KeyToEdit != ""){
					rewardName = controller.KeyToEdit;
					titleLabel.text = TITLE_EDIT;
				}
				//Original scene entry by Add New
				else{
					rewardName = controller.NewRewardName;
					titleLabel.text = TITLE_ADD;
				}
			}


			//Set UI for editing
			inputFieldObj.SetActive(false);
			rewardNameTextObj.SetActive(true);
			rewardNameTextObj.GetComponent<Text>().text = rewardName;
			Debug.Log("***** REWARD NAME: " + rewardName);
			//Disable save button
			saveButton.interactable = false;
	}

	private void SetupSceneAdd(){
		//Setup UI for Add New
		titleLabel.text = TITLE_ADD;
		rewardNameTextObj.SetActive(false);
		inputFieldObj.SetActive(true);
		//Disable camera button
		cameraButton.interactable = false;
		//Disable save button
		saveButton.interactable = false;
	}

	public void ReturnToPreviousScene(){
		controller.ClearData();
		controller.SceneChange(PREVIOUS_SCENE);
	}
    
	public void RequestToSaveReward(){
		Debug.Log(CLASSNAME + "***** Request to save reward sent");
		//If reward exists, you are editing a current reward
		if (controller.CheckForExistingReward(rewardName.ToLower())){
			//Resave the reward picture
			if (controller.RequestToSavePhoto(rewardName)){
				DisplayModal(ModalType.UPDATE, rewardName + MODAL_UPDATE_TEXT);
			}
			else{
				DisplayModal(ModalType.ERROR, MODAL_ERROR_TEXT);
			}
		}
		//else this is a new reward 
		else{
            rewardName = wordFieldText.text;

            //If request to save to DB was successful
            if (controller.RequestToSaveReward(rewardName, rewardType, rewardUrl)){
				//If request to save photo was successful
				if (controller.RequestToSavePhoto(rewardName)){
					DisplayModal(ModalType.SAVE, rewardName + MODAL_SAVE_TEXT);
				}
				else{
					DisplayModal(ModalType.ERROR, MODAL_ERROR_TEXT);
				}
			}
			else{
				DisplayModal(ModalType.ERROR, MODAL_ERROR_TEXT);
			}
		}
	}

	private void DisplayModal(ModalType type, string message){
		switch (type){
			case ModalType.EXISTS:
				modalOkButton.SetActive(true);
				break;
			case ModalType.SAVE:
				modalYesButton.SetActive(true);
				modalNoButton.SetActive(true);
				break;
			case ModalType.UPDATE:
				modalDoneButton.SetActive(true);
				break;
			case ModalType.ERROR:
				modalOkButton.SetActive(true);
				break;
			default:
				Debug.LogError(CLASSNAME + "Error displaying modal");
				break;
		}
		modalText.text = message;
		textModal.SetActive(true);
		saveButton.interactable = false;
		cancelButton.interactable = false;
	}

	public void CloseAndResetModal(){
		modalDoneButton.SetActive(false);
		modalOkButton.SetActive(false);
		modalNoButton.SetActive(false);
		modalYesButton.SetActive(false);
		modalText.text = "";
		textModal.SetActive(false);
		cancelButton.interactable = true;
	}

	public void AddAnotherReward(){
		CloseAndResetModal();
		controller.ClearCurrentTexture();
		controller.IsEditMode = false;
        controller.IsRewardWebsite = false;
		SetupSceneAdd();
	}



    //////////////////////////**************** New Camera Gallery Code Below
    ///


    public void OnGalleryButton()
    {
        Media.Gallery.Pick(PickFromGalleryCallback);
        Debug.Log("gallery button");

    }

    private void PickFromGalleryCallback(string error, MediaResult[] results)
    {
        if (!string.IsNullOrEmpty(error))
        {
            Debug.Log("Error picking from gallery");
            // TODO: display notice to user
        }
        else
        {
            // Loop through all the results (should be only one).
            foreach (MediaResult result in results)
            {
                Media.Gallery.LoadImage(result, LoadImageCallback);
            }
        }
        Debug.Log("pick image from gallery callback");

    }
    
    public void OnCameraButton()
    {

        EasyMobile.CameraType cameraType = EasyMobile.CameraType.Front;
        Media.Camera.TakePicture(cameraType, TakePictureCallback);
    }

    private void TakePictureCallback(string error, MediaResult result)
    {
        if (!string.IsNullOrEmpty(error))
        {
            // TODO: show the error to them.
            Debug.Log("Error on take picture with native camera app");
        }
        else
        {
            Media.Gallery.LoadImage(result, LoadImageCallback);
        }
    }

    private void LoadImageCallback(string error, Texture2D image)
    {
        if (!string.IsNullOrEmpty(error))
        {
            // TODO: There was an error, show it to users. 
            Debug.Log("Error on load image callback");
        }
        else
        {
            imgPreview.texture = image;
            controller.SetCurrentTexture(image);
        }

        saveButton.interactable = true;

        Debug.Log("load image callback///////////////////************************************");
    }

}
