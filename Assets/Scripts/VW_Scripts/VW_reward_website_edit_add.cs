using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EasyMobile;

public class VW_reward_website_edit_add : MonoBehaviour
{

    private const int PIC_WIDTH = 50;
    private const int PIC_HEIGHT = 50;

    private const string CLASSNAME = "VW_reward_website_edit_add: ";

    private const string PREVIOUS_SCENE = "rewards_list";

    private const string TITLE_ADD = "Add Reward";
    private const string TITLE_EDIT = "Edit Reward";

    private const string MODAL_UPDATE_TEXT = " was updated successfully!";
    private const string MODAL_SAVE_TEXT = " was saved successfully!\nDo you want to add another website reward?";
    private const string MODAL_EXISTS_TEXT = " already exists as a website reward.\nTry again";
    private const string MODAL_ERROR_TEXT = "There was an error saving the website reward.\nPlease try again.";

    private CON_RewardsList controller;
    private MOD_RewardsList model;

    private string rewardName = "";
    private string rewardType = "website";
    private string rewardUrl;
    private int editId;


    public UniWebView webView;

    private bool urlSubmitted = false;

    private bool isEdit = false;

    private bool editPic = false;
    private bool editUrl = false;

    private string copyUrl;

    private enum ModalType
    {
        EXISTS, SAVE, UPDATE, ERROR
    }

    [SerializeField] private Text titleLabel;
    [SerializeField] private GameObject inputFieldObj;
    [SerializeField] private InputField rewardNameInputField;
    [SerializeField] private GameObject rewardNameTextObj;
    [SerializeField] private Text wordFieldText;

    [SerializeField] private Button cameraButton;
    [SerializeField] private Button galleryButton;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private GameObject textModal;
    [SerializeField] private Text modalText;
    [SerializeField] private GameObject modalDoneButton;
    [SerializeField] private GameObject modalYesButton;
    [SerializeField] private GameObject modalNoButton;
    [SerializeField] private GameObject modalOkButton;
    [SerializeField] public RawImage imgPreview;

    [SerializeField] private GameObject urlInputFieldObj;
    [SerializeField] private InputField urlInputField;
    [SerializeField] private Text currentUrl;
    [SerializeField] private Text rewardUrlText;
    [SerializeField] private GameObject copyCurrentWebsiteUrlButton;
    [SerializeField] private GameObject exitWithoutUrlButton;
    [SerializeField] private GameObject submitManualUrlPanel;
    [SerializeField] private GameObject screenshotButton;
    [SerializeField] Text copyUrlButtonText;
    [SerializeField] Text screenshotButtonText;
    [SerializeField] RawImage screenshotButtonThumnail;

    


    [SerializeField] RectTransform WebviewRectTrans;

    // View Initialization
    void Start()
    {
        MAS_RewardsList master = (MAS_RewardsList)COM_Director.GetMaster("MAS_RewardsList");

        if (master != null)
        {
            controller = (CON_RewardsList)master.GetController("CON_RewardsList");

            if (controller != null)
            {
                if (controller.IsEditMode)
                {
                    SetupSceneEdit();
                }
                else
                {
                    SetupSceneAdd();
                }

                LoadRewardImagePreview();

            }
        }

        StartCoroutine(StartWaitHelper());


    }

    private IEnumerator StartWaitHelper()
    {
        yield return new WaitForSeconds(0.3f);
        exitWithoutUrlButton.gameObject.SetActive(false);
        copyCurrentWebsiteUrlButton.gameObject.SetActive(false);
    }


    private void LoadRewardImagePreview()
    {
        Debug.Log("Loading reward image preview");
        Texture2D tx = new Texture2D(PIC_WIDTH, PIC_HEIGHT);
        byte[] rewardPic = FileAccessUtil.LoadRewardPic(rewardName);

        //If not a custom reward
        if (rewardPic == null)
        {
            imgPreview.texture = Resources.Load<Texture2D>("RewardPictures/" + rewardName);
        }
        //else is a custom reward
        else
        {
            if (tx.LoadImage(rewardPic))
            {
                tx.LoadImage(rewardPic);
                imgPreview.texture = tx;
            }
            else
            {
                Debug.Log(CLASSNAME + "Loading custom picture failed.");
            }
        }
    }

    void Update()
    {
        //if in Add Mode
        if (!controller.IsEditMode)
        {
            if (rewardNameInputField.text == "")
            {
                saveButton.interactable = false;
                cameraButton.interactable = false;
                galleryButton.interactable = false;
                
            }
            else if (rewardNameInputField.text != "")
            {
                cameraButton.interactable = true;
                galleryButton.interactable = true;
                if (editUrl == true)
                {
                    saveButton.interactable = true;
                }
            }
        }
        //Must be in edit mode then
        else
        {
            //If a change has occured in edit mode, enable the save button
            if (controller.IsThereACurrentPhoto())
            {
                saveButton.interactable = true;
            }
            else if (urlSubmitted && (currentUrl.text != ""))
            {
                saveButton.interactable = true;
            }
        }
    }

    private void SetupSceneEdit()
    {
        /*
        Entering scene from: rewards_list
        */

        //Send request for data to controller
        DO_Reward reward = controller.RequestRewardToEdit();
        rewardName = reward.reward_name;
        rewardUrl = controller.RequestWebRewardUrl();
        Debug.Log("reward name: " + reward.reward_name + ", reward type: " + reward.reward_type + ", reward url: " + reward.reward_url);
       
        editId = reward.id;
        titleLabel.text = TITLE_EDIT;
        saveButton.GetComponentInChildren<Text>().text = "Update";

        if (!controller.IsThereACurrentPhoto())
        {
            Debug.Log(CLASSNAME + "***** Current Photo not set");
            
            if (reward.Equals(null))
            {
                Debug.LogError(CLASSNAME + "Error retrieving Reward to edit");
            }
                    
            bool result = controller.CheckForRewardImage(rewardName);           
            Debug.Log(CLASSNAME + "***** Does Photo Exist: " + result.ToString());
        }

        

        //Set up UI for editing
        inputFieldObj.SetActive(false);
        rewardNameTextObj.SetActive(true);
        rewardNameTextObj.GetComponent<Text>().text = rewardName;
        Debug.Log("***** REWARD NAME: " + rewardName);


        currentUrl.text = rewardUrl;

        Debug.Log("////////// REWARD URL: " + rewardUrl);

        //Disable save button
        saveButton.interactable = false;
    }

    private void SetupSceneAdd()
    {
        //Setup UI for Add New
        titleLabel.text = TITLE_ADD;
        rewardNameTextObj.SetActive(false);
        inputFieldObj.SetActive(true);
        //Disable camera button
        cameraButton.interactable = false;
        //Disable save button
        saveButton.interactable = false;
    }

    public void ReturnToPreviousScene()
    {
        controller.ClearData();
        editPic = false;
        editUrl = false;
        //controller.SceneChange(PREVIOUS_SCENE);
    }

   
    public void RequestToSaveReward()
    {
        

        Debug.Log(CLASSNAME + "***** Request to save reward sent, reward name: " + rewardName);

        //If reward exists, you are editing a current reward
        if (controller.CheckForExistingReward(rewardName))
        {
            Debug.Log("edit pic = " + editPic + ", edit url = " + editUrl);
            //Resave the reward picture
            if ((editPic == true) && (editUrl == false))
            {
                Debug.Log("attempting to edit reward pic");
                controller.RequestToSavePhoto(rewardName);
                DisplayModal(ModalType.UPDATE, rewardName + MODAL_UPDATE_TEXT);
            }
            else if (editPic == true && (editUrl = true))
            {
                Debug.Log("attempting to edit reward pic and reward url");

                // controller.RequestToDeleteReward(rewardName);
                // controller.RequestToSaveReward(rewardName, rewardType, rewardUrl);
                controller.RequestToDeletePhoto(rewardName);
                controller.RequestToSavePhoto(rewardName);
                controller.RequestToSaveUrl(rewardName, rewardUrl);
                DisplayModal(ModalType.UPDATE, rewardName + MODAL_UPDATE_TEXT);
            }
            else if ((editPic ==  false) && (editUrl == true))
            {
                Debug.Log("attempting to edit reward url");

                controller.RequestToSaveUrl(rewardName, rewardUrl);
                DisplayModal(ModalType.UPDATE, rewardName + MODAL_UPDATE_TEXT);
            }
            else
            {
                Debug.Log("something went wring when attempting to edit");

                DisplayModal(ModalType.ERROR, MODAL_ERROR_TEXT);
            }
        }
        //else this is a new reward 
        else
        {
            rewardName = wordFieldText.text;

            //If request to save to DB was successful
            if (controller.RequestToSaveReward(rewardName, rewardType, rewardUrl))
            {
                
                if (controller.IsThereACurrentPhoto())
                {
                    //If request to save photo was successful
                    if (controller.RequestToSavePhoto(rewardName))
                    {
                        DisplayModal(ModalType.SAVE, rewardName + MODAL_SAVE_TEXT);
                    }
                    else
                    {
                        DisplayModal(ModalType.ERROR, MODAL_ERROR_TEXT);
                    }
                }
                else
                {
                    DisplayModal(ModalType.SAVE, rewardName + MODAL_SAVE_TEXT);
                }

            }
            else
            {
                DisplayModal(ModalType.ERROR, MODAL_ERROR_TEXT);
            }
        }

        editUrl = false;
        editPic = false;

    }

    private void DisplayModal(ModalType type, string message)
    {
        switch (type)
        {
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

    public void CloseAndResetModal()
    {
        modalDoneButton.SetActive(false);
        modalOkButton.SetActive(false);
        modalNoButton.SetActive(false);
        modalYesButton.SetActive(false);
        modalText.text = "";
        textModal.SetActive(false);
        cancelButton.interactable = true;
    }

    public void AddAnotherReward()
    {
        CloseAndResetModal();
        controller.ClearCurrentTexture();
        controller.IsEditMode = false;
        controller.IsRewardWebsite = true;
        SetupSceneAdd();
    }

    public void OnScreenshotButton()
    {
        StartCoroutine(RecordFrame());
    }

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
        editPic = true;

        Debug.Log("load image callback complete");
    }

    public void OnSubmitUrlButton()
    {
        rewardUrl = rewardUrlText.text;
        currentUrl.text = rewardUrl;
        urlSubmitted = true;

        if (wordFieldText.text != ""/* && imgPreview != null*/)
        {
            saveButton.interactable = true;
        }
        editUrl = true;
        Debug.Log("reward Url set to: " + rewardUrl);
        submitManualUrlPanel.SetActive(false);
    }

    private void CreateWebview()
    {
        // Initialize UniWebView and set options
        var webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
        UniWebView.SetAllowInlinePlay(true);
    }

    public void OnWebsiteSearchButton()
    {
        // Create WebView and load BrowserUrlKey
        copyCurrentWebsiteUrlButton.SetActive(true);
        exitWithoutUrlButton.SetActive(true);
        //screenshotButton.SetActive(true);
        CreateWebview();
        webView.ReferenceRectTransform = WebviewRectTrans;
        webView.Load("https://www.google.com");
        webView.Show(true, UniWebViewTransitionEdge.Top);
    }

    public void OnExitButton()
    {        
        webView.Hide(true, UniWebViewTransitionEdge.Top);
        Destroy(webView);
        webView = null;
        copyUrlButtonText.text = "Copy Website URL";
        screenshotButtonText.text = "Take Screenshot";
        screenshotButtonThumnail.texture = null;
        exitWithoutUrlButton.SetActive(false);
        copyCurrentWebsiteUrlButton.SetActive(false);
        //screenshotButton.SetActive(false);
    }

    public void OnCopyWebsiteUrlButton()
    {    
        copyUrl = webView.Url;
        rewardUrlText.text = copyUrl;
        currentUrl.text = copyUrl;
        rewardUrl = copyUrl;
        copyUrlButtonText.text = "URL Saved!";
         
        editUrl = true;

        if (rewardName != ""/* && imgPreview != null*/)
        {
            saveButton.interactable = true;
        }


    }


    private IEnumerator RecordFrame()
    {
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.UpdateFrame();
        webView.Show();
        yield return new WaitForSeconds(1);
        yield return new WaitForEndOfFrame();
        
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        // do something with texture
        imgPreview.texture = texture;
        //screenshotButtonThumnail.texture = texture;
        
        controller.SetCurrentTexture(texture);
        screenshotButtonText.text = "Screenshot taken!";

        // cleanup
        //Object.Destroy(texture);

    }

    public void OnSubmitManualUrlButton()
    {
        submitManualUrlPanel.SetActive(true);
    }

    public void CancelCloseManualUrlPanel()
    {
        submitManualUrlPanel.SetActive(false);
    }

}

