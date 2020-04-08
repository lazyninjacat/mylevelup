using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using BestHTTP;
using UnityEngine.Networking;

public class VW_WordList : MonoBehaviour
{
    


    private const string MODAL_DEFAULT_TEXT = "Are you sure you want to delete the word {0}?";
    private const string IN_USE_DELETE_TEXT = "The word {0} is currently being used. Removing it will delete some play lists as well. Continue?";

    [SerializeField] GameObject wordCopyPanel;
    [SerializeField] GameObject tagCopyPanel;
    [SerializeField] GameObject contentRect;
    [SerializeField] GameObject deleteModal;
    [SerializeField] GameObject successModal;
    [SerializeField] GameObject viewDataModal;
    [SerializeField] InputField filterTagsInputField;
    [SerializeField] Text filterTagsText;
    [SerializeField] private GameObject filterTagsPanel;
    [SerializeField] Text averageErrorsText;
    [SerializeField] Text masteryScoreText;
    [SerializeField] Text playCountText;
    [SerializeField] GameObject DLCPanel;


    private static GameObject activePanel = null;
    private static RectTransform contentTransform;
    private static float yOffSet = 70;
    private string filterTags = "";

 
    private MOD_WordEditing model;
    private CON_WordEditing controller;

    // bool to keep track for the WordsTagsToggle
    private bool isWords = true;


    // TODO: Finish stats so we can reactivate the button
    public Button statsButton;
    //***************

    // Use this for initialization
    void Start()
    {
        MAS_WordEditing tempMaster = (MAS_WordEditing)COM_Director.GetMaster("MAS_WordEditing");

        model = (MOD_WordEditing)tempMaster.GetModel("MOD_WordEditing");
        controller = (CON_WordEditing)tempMaster.GetController("CON_WordEditing");

        // TODO: Remove this when DomainShift functionality is in both the MasterClass and
        // AB_Model
        controller.PopulateInUseSet();
        //////////////////////////////////////////////////////////////////////////////////

        Debug.Log("VW: Starting the word list view!");


        DisplayScrollViewWords();

    }


    public void OpenFilterTagsPanel()
    {
        filterTagsPanel.SetActive(true);
    }
    public void CloseFilterTagsPanel()
    {
        filterTagsPanel.SetActive(false);

    }

    private void DisplayScrollViewWords()
    {
        Debug.Log("Begin DisplayScrollViewWords method");
        foreach (Transform child in contentRect.transform)
        {
            Destroy(child.gameObject);
        }


        GameObject tempPanel;
        contentTransform = contentRect.GetComponent<RectTransform>();
        float entryNum = 0;
        
        // Create a sorted word list to use when spawning word panels
        SortedDictionary<string, WordDO> sortedWordList = new SortedDictionary<string, WordDO>(controller.GetListCopy());

        List<string> tempScrollviewList = new List<string>();



        foreach (var entry in sortedWordList)
        {

            List<string> wordTagsList = new List<string>();
            List<string> filterTagsList = new List<string>();

            if (entry.Value.WordTags != null)
            {
                wordTagsList = entry.Value.WordTags.Split(',').ToList();
                filterTagsList = filterTags.Split(',').ToList();
            }
           



            if (filterTags == "" || filterTags == null)
            {
                Debug.Log("No filter tags detected");
                tempPanel = GameObject.Instantiate(wordCopyPanel, contentRect.transform, false);
                tempPanel.transform.GetChild(0).GetComponent<Text>().text = TidyCase(entry.Key);

                if (entry.Value.WordTags != null)
                {
                    tempPanel.transform.GetChild(1).GetComponent<Text>().text = TidyCase(entry.Value.WordTags);
                }



                if (!entry.Value.StockCustom.Equals("custom"))
                {
                    tempPanel.transform.GetChild(4).GetComponent<Button>().interactable = false;
                }

                tempPanel.transform.GetChild(3).GetComponent<Button>().interactable = true;

                tempPanel.name = entry.Key;
                tempPanel.SetActive(true);
                entryNum += 1;
            }
            else
            {
                Debug.Log("Filter tags detected");

                foreach (string tag in filterTagsList)
                {
                    //Debug.Log("tag = " + tag);
                    if (!tempScrollviewList.Contains(entry.Value.Word_name))
                    {
                        //Debug.Log(entry.Value.Word_name + " is not yet in the tempscrollviewlist");

                        if ((entry.Value.WordTags.Contains(tag)) || (entry.Value.WordTags.Contains(" " + tag)))
                        {
                            Debug.Log("wordtaglist contains = " + tag + " for word " + entry.Value.Word_name);
                            tempPanel = GameObject.Instantiate(wordCopyPanel, contentRect.transform, false);
                            tempPanel.transform.GetChild(0).GetComponent<Text>().text = TidyCase(entry.Key);

                            if (entry.Value.WordTags != null)
                            {
                                tempPanel.transform.GetChild(1).GetComponent<Text>().text = TidyCase(entry.Value.WordTags);
                            }

                            if (!entry.Value.StockCustom.Equals("custom"))
                            {

                                tempPanel.transform.GetChild(4).GetComponent<Button>().interactable = false;
                            }

                            tempPanel.transform.GetChild(3).GetComponent<Button>().interactable = true;

                            tempPanel.name = entry.Key;
                            tempPanel.SetActive(true);
                            entryNum += 1;
                            tempScrollviewList.Add(entry.Value.Word_name);
                        }
                    }
                }
            }
        }

        tempScrollviewList.Clear();
        tempScrollviewList = null;
        sortedWordList.Clear();

        contentTransform.sizeDelta = new Vector2(contentTransform.rect.width, entryNum * yOffSet);

    }


    public void OnAddNewfilterTagButton()
    {

        if (filterTagsText.text == "" || filterTagsText.text == null)
        {
            filterTagsText.text = filterTagsInputField.text;
            filterTags = filterTagsText.text;
        }
        else
        {
            filterTagsText.text = (filterTags + ", " + filterTagsInputField.text);
            filterTags = filterTagsText.text;
        }
        
        DisplayScrollViewWords();
    }

    public void OnDeletefilterTagButton()
    {


        if (filterTags.Contains(","))
        {
            filterTags = filterTags.Substring(0, filterTags.LastIndexOf(",") + 0);
        }
        else
        {
            filterTags = "";
        }

        filterTagsText.text = filterTags;
        DisplayScrollViewWords();

    }



    // TODO: Modify DataService to allow for editing of words in DB
    public void EditWord()
    {
        // Grab the Game Object of the panel that is parent to the edit button that was touched
        //CONTROLLER 
        //sc.LoadSceneByName("word_edit");
        controller.SetTargetWord(EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name);
        controller.SceneChange("word_edit_add", true);
    }

    public void DeleteWord()
    {
        string word = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name;
        bool inUse = controller.WordInUse(word);

        if (controller.DeleteWord(word))
        {
            if (inUse)
            {
                Debug.Log("*************************************\n DELETE: Deleting a word in use! \n ********************************");
                controller.DeleteInvalidPlayEntries();
            }

            successModal.SetActive(true);
            deleteModal.SetActive(false);

            // Destroy the panel belonging to the word entry
            Destroy(activePanel);
            //activePanel.SetActive(false);
            contentTransform.sizeDelta = new Vector2(contentTransform.rect.width, contentTransform.rect.height - (yOffSet));
        }
        else
        {
            deleteModal.SetActive(false);
            //TODO: THROW AN ERROR
        }
    }

    public void OpenDeleteModal()
    {
        // Replace the modal {WORD} place holder with the word being deleted and display the modal.
        string word = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name;
        deleteModal.gameObject.name = word;
        activePanel = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;

        if (controller.WordInUse(word))
        {
            deleteModal.transform.GetChild(0).GetComponent<Text>().text = string.Format(IN_USE_DELETE_TEXT, word);
        }
        else
        {
            deleteModal.transform.GetChild(0).GetComponent<Text>().text = string.Format(MODAL_DEFAULT_TEXT, word);
        }

        deleteModal.SetActive(true);
    }

    public void CancelAndClose()
    {
        controller.ClearData();
        controller.SceneChange("admin_menu");
    }

    public void OpenViewDataModal()
    {
        viewDataModal.SetActive(true);
        string word = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name;
        float tempAverage = (controller.model.GetWordMastery(word));        
        averageErrorsText.text = tempAverage.ToString();
        Debug.Log("average errors for " + word + " is " + tempAverage.ToString());
        masteryScoreText.text = (1-(tempAverage / (word.Length * 6))).ToString();
        playCountText.text = (controller.model.GetWordPlayCount(word)).ToString();
    }

    public void CloseViewDataModal()
    {
        averageErrorsText.text = "No Data";
        playCountText.text = "0";
        masteryScoreText.text = "No Data";
        viewDataModal.SetActive(false);

    }

    public void AddWord() { controller.SceneChange("word_edit_add", false); }
    public void CloseDeleteModal() { deleteModal.SetActive(false); }
    public void CloseSuccessModal() { successModal.SetActive(false); }




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


    public void WordsTagsToggle()
    {
        if (isWords)
        {
            DisplayScrollViewTags();
            isWords = false;
        }
        else
        {
            DisplayScrollViewWords();
            isWords = true;
        }
    }


    private void DisplayScrollViewTags()
    {
        Debug.Log("Begin DisplayScrollViewTags method");

        // Clear the scroll view
        foreach (Transform child in contentRect.transform)
        {
            Destroy(child.gameObject);
        }



    }

    public void OpenDLCPanel()
    {
        DLCPanel.SetActive(true);
    }
    public void CloseDLCPanel()
    {
        DLCPanel.SetActive(false);
    }



    //////////////////////////////////////////
    //////////////////////////////////////////
    // DLC 

    private string tempWordName;
    private string tempWordTags;
    private bool doneWordDownloadStep = false;
    private bool doneImageDownload = false;

    public void VehiclesDLC()
    {
        string[] vehicles = { "vehicles", "airplane", "bus", "truck", "car", "scooter", "boat", "bike", "van", "train" };
        StartCoroutine(DownloadWordSet(vehicles));       
    }

    private IEnumerator DownloadWordSet(string[] dlcArray)
    {
        Debug.Log("Downloading " + dlcArray[0]);

        for (int i = 0; i < dlcArray.Length - 1; i++)
        {
            StartCoroutine(DownloadWord(dlcArray[i+1], dlcArray[0]));
            yield return new WaitUntil(() => doneWordDownloadStep);
        }

        ResetScrollView();
    }

    private IEnumerator DownloadWord(string word, string tags)
    {
        Debug.Log("Downloading word: " + word);
        doneWordDownloadStep = false;
        doneImageDownload = false;
        tempWordName = word;
        tempWordTags = tags;

        HTTPRequest request1 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/" + word + "/" + word + "1.png"), OnRequestFinished1);
        request1.Send();
        yield return new WaitUntil(() => doneImageDownload);
        doneImageDownload = false;
        HTTPRequest request2 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/" + word + "/" + word + "2.png"), OnRequestFinished1);
        request2.Send();
        yield return new WaitUntil(() => doneImageDownload);
        doneImageDownload = false;
        HTTPRequest request3 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/" + word + "/" + word + "3.png"), OnRequestFinished1);
        request3.Send();
        yield return new WaitUntil(() => doneImageDownload);
        doneImageDownload = false;
        HTTPRequest request4 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/" + word + "/" + word + "4.png"), OnRequestFinished1);
        request4.Send();
        yield return new WaitUntil(() => doneImageDownload);
        doneImageDownload = false;
        HTTPRequest request5 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/" + word + "/" + word + "5.png"), OnRequestFinished2);
        request5.Send();
        yield return new WaitUntil(() => doneImageDownload);
        doneImageDownload = false;
        StartCoroutine(DownloadAudioClip(word));
    }

    void OnRequestFinished1(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log("Request Finished! Text received: " + response.DataAsText);
        controller.SetCurrentTexture(response.DataAsTexture2D);
        if (controller.IsTextureSet()) {
            controller.AddNewTexture();
        }
        doneImageDownload = true;
    }

    void OnRequestFinished2(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log("Request Finished! Text received: " + response.DataAsText);
        controller.SetCurrentTexture(response.DataAsTexture2D);
        if (controller.IsTextureSet())
        {
            controller.AddNewTexture();
        }
        controller.SaveNewWord(tempWordName, tempWordTags);
        Debug.Log("Done downloading word: " + tempWordName);
        controller.ClearTextureList();
        doneImageDownload = true;


    }

    private void ResetScrollView()
    {
        DisplayScrollViewWords();
        DLCPanel.SetActive(false);
    }

          
    IEnumerator DownloadAudioClip(string file_name)
    {
        string url = "https://matthewriddett.com/static/mludlc/" + file_name + "/" + file_name + ".wav";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string savePath = string.Format("{0}/{1}.wav", Application.persistentDataPath + "/WordAudio/", file_name);
                System.IO.File.WriteAllBytes(savePath, www.downloadHandler.data);
            }
        }

        doneWordDownloadStep = true;
        
    }
}



