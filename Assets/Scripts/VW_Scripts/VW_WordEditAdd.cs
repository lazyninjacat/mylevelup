using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;
using BestHTTP;

public class VW_WordEditAdd : MonoBehaviour
{
    private const string MODAL_EDIT_TEXT = "{0} edit saved!";
    private const string MODAL_ADD_TEXT = "{0} Saved!\nDo you want to add another word?";
    private const string MODAL_NO_EDITS = "No changes have been made.";
    private const string MODAL_MISSING_DATA = "Please ensure you have added an audio clip and at least one image.";

    private const string TITLE_ADD = "Add Word";
    private const string TITLE_EDIT = "Edit Word";
    private const int XY_VECTOR = 95;

    private Color32 offColor = new Color32(0, 0, 0, 0);
    private Color32 onColor = new Color32(255, 255, 0, 255);
    private MOD_WordEditing model;
    private CON_WordEditing controller;
    private int activeButtons;
    private int filledImageSlots;
    private List<Texture2D> stockList;
    private List<Texture2D> customList;
    private List<int> indicesToDelete;
    private bool imageDeleted = false;
    private string wordTags;
    private string wordTagsOriginal;



    [SerializeField] Text titleLabel;
    [SerializeField] Text wordFieldText;
    [SerializeField] Text placeHolderText;
    [SerializeField] Button micAccessButton;
    [SerializeField] Button cameraAccessButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button deletePicsButtons;
    [SerializeField] GameObject yesButton;
    [SerializeField] GameObject noButton;
    [SerializeField] GameObject okButton;
    [SerializeField] GameObject doneButton;
    [SerializeField] GameObject textModal;
    [SerializeField] GameObject wordField;
    [SerializeField] GameObject wordText;
    [SerializeField] GameObject imagesPanel;
    [SerializeField] GameObject imageToggleCopy;
    [SerializeField] GameObject stockImageToggleCopy;
    [SerializeField] GameObject galleryCameraModal;
    [SerializeField] GameObject loadingAnimPanel;
    [SerializeField] Text wordTagsText;
    [SerializeField] InputField wordTagsInputField;



    void Start()
    {
        MAS_WordEditing tempMaster = (MAS_WordEditing)COM_Director.GetMaster("MAS_WordEditing");
        model = (MOD_WordEditing)tempMaster.GetModel("MOD_WordEditing");
        controller = (CON_WordEditing)tempMaster.GetController("CON_WordEditing");
        activeButtons = 0;
        CleanUpScroll();
        EditOrAdd();
        CleanUpScroll();
        DisplayGallery();
        SetUpWordTags();
        wordTagsOriginal = wordTags;
    }

    void Update()
    {
        if ((wordField.activeSelf && wordFieldText.text != "") || placeHolderText.text != "")
        {
            cameraAccessButton.interactable = true;
            micAccessButton.interactable = true;
        }
        else if (wordField.activeSelf)
        {
            cameraAccessButton.interactable = false;
            micAccessButton.interactable = false;
        }

        wordTags = wordTagsText.text;
    }

    public void DLCButton()
    {       
        HTTPRequest request = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/test.png"), OnRequestFinished);
        request.Send();        
    }

    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log("Request Finished! Text received: " + response.DataAsText);
        controller.SetCurrentTexture(response.DataAsTexture2D);
        galleryCameraModal.SetActive(false);
        saveButton.interactable = true;
        CleanUpScroll();
        DisplayGallery(); 
    }

    private void SetUpWordTags()
    {
        string word = controller.GetTargetWord();
        WordDO dataObject = controller.GetWordDO(word);
        wordTags = dataObject.WordTags;
        Debug.Log("GetTargetWordTags = " + wordTags);
        wordTagsText.text = wordTags;
        Debug.Log("word tags = " + wordTagsText.text);
    }


    public void OnAddNewWordTagButton()
    {
        if (wordTagsText.text == "" || wordTagsText.text == null)
        {
            wordTagsText.text = wordTagsInputField.text;
            wordTags = wordTagsText.text;
        }
        else
        {
            wordTagsText.text = (wordTags + ", " + wordTagsInputField.text);
            wordTags = wordTagsText.text;
        }
        saveButton.interactable = true;
    }

    public void OnDeleteWordTagButton()
    {
        Debug.Log("WordTags before delete last = " + wordTags);
        if (wordTags.Contains(","))
        {
            wordTags = wordTags.Substring(0, wordTags.LastIndexOf(",") + 0);
            Debug.Log("WordTags after delete last = " + wordTags);
        }
        else
        {
            wordTags = "";
        }
       
        wordTagsText.text = wordTags;
        saveButton.interactable = true;
    }

    public void StartSave()
    {
        loadingAnimPanel.SetActive(true);
        Debug.Log("loadingAnimPanel Active = " + loadingAnimPanel.activeSelf);
        Save();
    }

    public void Save()
    {        
        string word;

        if (controller.IsEditSettings) // if the word is being edited (i.e. not new)
        {
            // Check if any changes have been made. 
            if (controller.GetCurrentClip() == null && controller.GetImages().Count == 0 && !imageDeleted && (wordTags == wordTagsOriginal))
            {
                loadingAnimPanel.SetActive(false);
                textModal.transform.GetChild(0).GetComponent<Text>().text = MODAL_NO_EDITS;   

                EnableErrorModal();
                return;
            }

            word = controller.GetTargetWord();

            if (controller.SaveWordEdits(word, wordTags) || imageDeleted)
            {
                textModal.transform.GetChild(0).GetComponent<Text>().text = string.Format(MODAL_EDIT_TEXT, TidyCase(word));
                loadingAnimPanel.SetActive(false);

                EnableEditModal();

            }
            else
            {
                Debug.Log("VW: There was an error!");
                loadingAnimPanel.SetActive(false);
                EnableErrorModal();
                textModal.transform.GetChild(0).GetComponent<Text>().text = string.Format(MODAL_MISSING_DATA);
            }
        }
        else // the word is new (i.e. not an existing word being edited)
        {
            word = controller.GetTargetWord();

            if (controller.DoesDbEntryExist(word))
            {               
                loadingAnimPanel.SetActive(false);
                textModal.transform.GetChild(0).GetComponent<Text>().text = word + " already exists\n\n(Click to try again)";
                EnableErrorModal();
            }
            else
            {
                if (controller.SaveNewWord(word, wordTags) || imageDeleted)
                {
                    //Handle UI changes to notify success and set-up popup for additional options
                    textModal.transform.GetChild(0).GetComponent<Text>().text = string.Format(MODAL_ADD_TEXT, TidyCase(word));
                    loadingAnimPanel.SetActive(false);
                    EnableAddModal();
                }
                else
                {
                    textModal.transform.GetChild(0).GetComponent<Text>().text = "Something went horribly wrong";
                    loadingAnimPanel.SetActive(false);
                    EnableErrorModal();
                    // TODO: add proper error handling
                }
            }
        }
    }

    public void ClearAndCloseModal()
    {
        controller.ClearData();
        wordFieldText.text = "";
        placeHolderText.text = "";
        //imageText.text = "No Image";
        //audioText.text = "No Audio";
        saveButton.interactable = false;
        textModal.SetActive(false);
        CleanUpScroll();
    }

    public void Cancel()
    {
        CleanUpScroll();
        controller.ClearData();
        // controller.SceneChange("words_list");
    }

    public void NavigateToRecord()
    {

        if (controller.IsEditSettings == false)
        {
            if (wordFieldText.text != "")
            {
                controller.SetTargetWord(wordFieldText.text);

            }
            else if (placeHolderText.text != "")
            {
                controller.SetTargetWord(placeHolderText.text);
            }
        }

        //Debug.Log("VW: Naving to record");
        CleanUpScroll();
        // controller.SceneChange("record_audio");
    }

    public void NavigateToCamera()
    {
        galleryCameraModal.SetActive(false);


        if (controller.IsEditSettings == false)
        {
            if (wordFieldText.text != "")
            {
                controller.SetTargetWord(wordFieldText.text);
            }
            else if (placeHolderText.text != "")
            {
                controller.SetTargetWord(placeHolderText.text);
            }
        }

        CleanUpScroll();
        controller.SceneChange("image_camera");
    }



    public void DeletePictures()
    {
        int x = 0;

        Debug.Log("******************************************\n IN DELETE \n******************************************");

       /*
       for (int x = 0; x < imagesPanel.transform.childCount; x++)
       {

       }
       */


        foreach (Transform child in imagesPanel.transform)
        {
            if ((child.gameObject.GetComponent<Toggle>().isOn))
            {
                // Call for deletion of any word pictures using the word and current integer
                //controller.DeleteWordImage(controller.IsEditSettings ? wordText.GetComponent<Text>().text : wordFieldText.text, x);
                Debug.Log("******************************************\n VWREMOVE: TOGGLE ON! THE INDICE TO DELETE IS :" + x + "\n******************************************");

                indicesToDelete.Add(x);
                Destroy(child.gameObject);

                // Decrement the customNumLoaded value to reflect the change
            }
            x += 1;

            if (x >= imagesPanel.transform.childCount)
            {
                break;
            }


        }

        if (indicesToDelete.Count > 0)
        {
            Debug.Log("******************************************\n indicesToDelete count is " + indicesToDelete.Count + " \n******************************************");
            controller.DeleteWordImages(indicesToDelete, controller.IsEditSettings ? wordText.GetComponent<Text>().text : wordFieldText.text);
            imageDeleted = true;
            saveButton.interactable = true;

        }
        saveButton.interactable = true;
    }

    private void SavePictures()
    {
        string word;

        if (controller.GetImages().Count > 0)
        {
            if (controller.IsEditSettings)
            {
                word = wordText.GetComponent<Text>().text;
            }
            else
            {
                word = wordFieldText.text;
            }

            if (controller.SaveTextures(word) <= 0)
            {
                // TODO: THROW AN ERROR
            }
        }
    }

    /// <summary>
    /// Finds the toggle in the parent gameObject and toggles it.
    /// </summary>
    /// <param name="parent"></param>
  //  public void ToggleParent(GameObject parent)
    public void ToggleImageButton(GameObject toggle)
    {
        Debug.Log("**************************\n ATTEMPTING TO TOGGLE PARENT " + toggle.name + "\n********************************");
        Debug.Log("**************************\n TOGGLE OF THE CHILD IS " + toggle.GetComponent<Toggle>().isOn.ToString() + "\n********************************");

        // Change the color of the toggle's background to mirror its on/off status
        toggle.transform.GetChild(0).GetComponent<Image>().color = toggle.GetComponent<Toggle>().isOn ? onColor : offColor;

        // Enable or disable the delete pics button
        deletePicsButtons.interactable = CheckToggles();
    }

    private void EnableEditModal()
    {
        doneButton.SetActive(true);
        textModal.SetActive(true);
    }

    private void EnableAddModal()
    {
        yesButton.SetActive(true);
        noButton.SetActive(true);
        textModal.SetActive(true);
    }

    private void EnableErrorModal()
    {
        okButton.SetActive(true);
        textModal.SetActive(true);
    }

    private bool CheckToggles()
    {
        foreach (Transform child in imagesPanel.transform)
        {
            if (child.gameObject.GetComponent<Toggle>().isOn)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Recovers a list of Texture2D objects and loops through the collection
    /// applying each texture to a cloned toggle object. If any new textures are 
    /// set they are added to the collection.
    /// </summary>
    private void DisplayGallery()
    {
        CleanUpScroll();
        // Check if there is a new stored texture and add if so.
        if (controller.IsTextureSet()) controller.AddNewTexture();

        Debug.Log("++++++++++++++++++++++++++++++++\n Past the first check in DISPLAY GALLERY \n ****************************");

        bool texturesLoaded = false;

        if (!controller.AreTexturesPresent())
        {
            if (controller.IsEditSettings)
            {
                Debug.Log("++++++++++++++++++++++++++++++++\n IS EDIT WORD IS " + wordText.GetComponent<Text>().text + " \n ****************************");
                texturesLoaded = controller.RetrievePictures(wordText.GetComponent<Text>().text);
            }
            else
            {
                if (string.Equals(placeHolderText.text, ""))
                {
                    return;
                }
                else
                {
                    Debug.Log("++++++++++++++++++++++++++++++++\n ADDING NEW WORD IS " + wordFieldText.text + " \n ****************************");
                    texturesLoaded = controller.RetrievePictures(placeHolderText.text);
                }
            }
        }

        if (texturesLoaded || controller.AreTexturesPresent())
        {

            Debug.Log("*****\n Passed the IF statement\n*****");

            GameObject tempToggle;
            RawImage tempImage;

            customList = new List<Texture2D>(controller.GetImages());

            Debug.Log("*****\n customList count is: " + customList.Count.ToString() + "\n*****");

            // Check if the customList entry count is greater than zero before trying the loop.
            if (customList.Count > 0)
            {

                Debug.Log("*****\n Passed the IF statement\n*****");

                foreach (Texture2D tex in customList)
                {
                    tempToggle = GameObject.Instantiate(imageToggleCopy, imagesPanel.transform, false);
                    tempImage = tempToggle.transform.GetChild(0).Find("RawImage").GetComponent<RawImage>();
                    //tempImage.GetComponent<RectTransform>().sizeDelta = new Vector2(XY_VECTOR, XY_VECTOR);
                    tempImage.texture = tex;
                    tempToggle.SetActive(true);
                }
            }
        }

        GameObject tempStockToggle;
        RawImage tempStockImage;

        stockList = new List<Texture2D>(controller.GetStockImages());


        if(stockList.Count > 0)
        {
            foreach (Texture2D tex in stockList)
            {
                tempStockToggle = GameObject.Instantiate(stockImageToggleCopy, imagesPanel.transform, false);
                tempStockImage = tempStockToggle.transform.GetChild(0).Find("RawImage").GetComponent<RawImage>();
                tempStockImage.texture = tex;
                tempStockToggle.SetActive(true);
            }
        }
             

    }

    public void CleanUpScroll()
    {
        foreach (Transform child in imagesPanel.transform)
        {
            Destroy(child.gameObject);
        }

    }

    private void EditOrAdd()
    {
        if (controller.IsEditSettings)
        {
            Debug.Log("VW: In Edit");

            // Enable wordText and disable word field
            wordText.SetActive(true);
            wordField.SetActive(false);

            if (!(controller.IsTextureSet() || controller.IsClipSet()))
            {
                Debug.Log("No pic or audio clip change yet");
                saveButton.interactable = false;
                CleanUpScroll();
            }

            if (controller.IsTextureSet() || controller.IsClipSet())
            {
                Debug.Log("Change made to pic and/or audio clip. Save is now allowed.");
                saveButton.interactable = true;
                CleanUpScroll();
            }

            string word = controller.GetTargetWord();


            Debug.Log("Inside edit. word is = " + word);

            Debug.Log("Inside edit. word tags are: " + wordTags);

            if (word != null || word != "")
            {
                Debug.Log("attempting to load Word_DO for Word Name");
                WordDO dataObject = controller.GetWordDO(word);
                Debug.Log("Got the Word_DO, now attempting to get the text component");
                wordText.GetComponent<Text>().text = TidyCase(word);
                titleLabel.text = TITLE_EDIT;
            }
            else
            {
                //TODO: THROW AN ERROR 
                Debug.Log("Error, word is null or blank. Word = " + word);
            }

            if (wordTags != null || wordTags != "")
            {
                Debug.Log("attempting to load Word_DO for Word Tags");
                WordDO dataObject = controller.GetWordDO(word);
                Debug.Log("Got the Word_DO, now attempting to get the text component");
                wordTagsText.text = wordTags;                
            }
            else
            {
                //TODO: THROW AN ERROR 
                Debug.Log("Error, wordtags is null or blank. Wordtags = " + wordTags);
            }

        }
        else
        {
            Debug.Log("VW: In ADD");

            titleLabel.text = TITLE_ADD;

            string word = controller.GetTargetWord();
            string wordTags = controller.GetTargetWordTags();

            // Enable word field and disable word text
            wordField.SetActive(true);
            wordText.SetActive(false);

            wordTagsText.text = wordTags;



            if (controller.GetTargetWord() == null || !controller.IsTextureSet())
            {
                saveButton.interactable = false;
                CleanUpScroll();
            }
            else
            {
                saveButton.interactable = true;
                CleanUpScroll();
            }

            if (word != null)
            {
                Debug.Log("VW: Setting text to " + controller.GetTargetWord());
                wordFieldText.text = word;
                placeHolderText.text = word;
            }
           
        }
    }

    public void OnAddPictureButton()
    {
        galleryCameraModal.SetActive(true);
    }

    public void OnGalleryButton()
    {
        galleryCameraModal.SetActive(false);
        Media.Gallery.Pick(PickFromGalleryCallback);
        Debug.Log("gallery button ///////////////////************************************");

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
        Debug.Log("pick image from gallery callback///////////////////************************************");

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
            controller.SetCurrentTexture(image);
        }

        saveButton.interactable = true;
        CleanUpScroll();
        DisplayGallery();

        Debug.Log("loadimagecallback///////////////////************************************");
    }

    public void OnCameraButton()
    {
        galleryCameraModal.SetActive(false);

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

    /// <summary>
    /// Takes in a string a returns it with the first letter upper case and the rest lower case.
    /// </summary>
    /// <param name="sourceStr"></param>
    /// <returns></returns>
    public static string TidyCase(string sourceStr)
    {
        sourceStr.Trim();
        if (!string.IsNullOrEmpty(sourceStr))
        {
            char[] allCharacters = sourceStr.ToCharArray();

            for (int i = 0; i < allCharacters.Length; i++)
            {
                char character = allCharacters[i];
                if (i == 0)
                {
                    if (char.IsLower(character))
                    {
                        allCharacters[i] = char.ToUpper(character);
                    }
                }
                else
                {
                    if (char.IsUpper(character))
                    {
                        allCharacters[i] = char.ToLower(character);
                    }
                }
            }
            return new string(allCharacters);
        }
        return sourceStr;
    }





}