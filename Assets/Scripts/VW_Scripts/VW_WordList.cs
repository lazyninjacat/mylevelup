using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
//using BestHTTP;
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
    [SerializeField] GameObject filterTagsPanel;
    [SerializeField] Text averageErrorsText;
    [SerializeField] Text masteryScoreText;
    [SerializeField] Text playCountText;
    [SerializeField] GameObject DLCPanel;
    [SerializeField] GameObject DownloadProgressPanel;
    [SerializeField] Image DownloadProgressBar;
    [SerializeField] GameObject DeleteTagWordsModal;

    [SerializeField] Text DeleteTagWordsModalMessageText;


    private string tagWordsToDelete = "";

    private string tempWordName;
    private string tempWordTags;
    private bool doneWordDownloadStep = false;
    private bool doneImageDownload = false;
    private bool isDoneWordSetDataDownload;
    private string[] wordSetArray;
    private string wordSetString;
    private bool isWordSetArrayFinished;
    private static GameObject activePanel = null;
    private static RectTransform contentTransform;
    private static float yOffSet = 70;
    private string filterTags = "";

    private List<string> WordsForDeleteList = new List<string>();

    private MOD_WordEditing model;
    private CON_WordEditing controller;

    // bool to keep track for the WordsTagsToggle
    private bool isWords = true;

    // bool to keep track of scrollview state words/tags view
    private bool isTagsView;


    private List<string> wordSetsList;
    private bool isDoneDownloadWordSetsList;
    [SerializeField] GameObject DlcCopyButton;
    [SerializeField] GameObject DlcButtonContainer;




    void Start()
    {
        Debug.Log("VW: Starting the word list view!");

        MAS_WordEditing tempMaster = (MAS_WordEditing)COM_Director.GetMaster("MAS_WordEditing");

        model = (MOD_WordEditing)tempMaster.GetModel("MOD_WordEditing");

        List<string> wordSetsList = new List<string>();

        controller = (CON_WordEditing)tempMaster.GetController("CON_WordEditing");

        // TODO: Remove this when DomainShift functionality is in both the MasterClass and
        // AB_Model
        controller.PopulateInUseSet();
        //////////////////////////////////////////////////////////////////////////////////

        DisplayScrollViewWords();


        StartCoroutine(CreateDLCButtons());

        StartCoroutine(DownloadWordSetsList());


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
        isTagsView = false;
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
                //Debug.Log("No filter tags detected");
                tempPanel = GameObject.Instantiate(wordCopyPanel, contentRect.transform, false);
                tempPanel.transform.GetChild(0).GetComponent<Text>().text = TidyCase(entry.Key);

                if (entry.Value.WordTags != null)
                {
                    tempPanel.transform.GetChild(1).GetComponent<Text>().text = TidyCase(entry.Value.WordTags);
                }


                //Disable the delete button for stock words
                if (!entry.Value.StockCustom.Equals("custom"))
                {
                    tempPanel.transform.GetChild(4).GetComponent<Button>().interactable = false;
                }

                //enable the stats button
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
                Debug.Log("DELETE: Deleting a word in use!");
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
            Debug.Log("ERROR: delete word failed");
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

        isTagsView = true;

        // Clear the scroll view
        foreach (Transform child in contentRect.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject tempPanel;
        contentTransform = contentRect.GetComponent<RectTransform>();
        float entryNum = 0;

        // Create a sorted word list of the existing word database
        SortedDictionary<string, WordDO> sortedWordList = new SortedDictionary<string, WordDO>(controller.GetListCopy());

        List<string> tempScrollviewList = new List<string>();

        // Dictionary, where Key is the tag and Value is the word list
        Dictionary<string, List<string>> TagToWordListDictionary = new Dictionary<string, List<string>>();


        //Create the tags to words dictionary
        // go through the sortedWordList and check each for tags
        foreach (var entry in sortedWordList)
        {

            List<string> tempWordTagsList = new List<string>();
            List<string> tempWordsList = new List<string>();

            if (entry.Value.WordTags != null)
            {
                tempWordTagsList = entry.Value.WordTags.Split(',').ToList();
                foreach (string tag in tempWordTagsList)
                {
                    if (TagToWordListDictionary.ContainsKey(tag))
                    {
                        Debug.Log("wordTagsDictionary already contains " + tag + ". Adding word to tag dictionary key.");
                        Debug.Log("before adding, existing tag wordlist count that shouldn't be 0 actually = " + TagToWordListDictionary[tag].Count);

                        foreach (string word in TagToWordListDictionary[tag])
                        {
                            Debug.Log("before adding, includes: " + word);
                        }
                        TagToWordListDictionary[tag].Add(entry.Value.Word_name);
                        Debug.Log("Existing tag's current wordlist count = " + TagToWordListDictionary[tag].Count);
                        foreach (string word in TagToWordListDictionary[tag])
                        {
                            Debug.Log("after adding, includes: " + word);
                        }                  
                                            
                    }
                    else
                    {
                        Debug.Log("New tag entry for TagToWordListDictionary = " + tag);
                      
                        tempWordsList.Add(entry.Value.Word_name);
                        Debug.Log("New entry, tempWordsList count that should be 1 actually = " + tempWordsList.Count);
                        TagToWordListDictionary.Add(tag, tempWordsList);
                        Debug.Log("New tag's current wordlist count = " + TagToWordListDictionary[tag].Count);

                        foreach (string word in TagToWordListDictionary[tag])
                        {
                            Debug.Log("includes: " + word);
                        }

                   

                    }

                    Debug.Log("TagToWordlist entry count for " + tag + " = " + TagToWordListDictionary[tag].Count);
              
                    Debug.Log("Done with tag:" + tag + " in word:" + entry.Value.Word_name);
                }
                Debug.Log("Clearing the tempwordlist and temp wordtaglslist");
               
            }
        }
      
        foreach (var entry in TagToWordListDictionary)
        {
            //Debug.Log("tag = " + tag);
            if (!tempScrollviewList.Contains(entry.Key))
            {
                //Debug.Log(entry.Value.Word_name + " is not yet in the tempscrollviewlist");

                tempPanel = GameObject.Instantiate(tagCopyPanel, contentRect.transform, false);
                tempPanel.transform.GetChild(0).GetComponent<Text>().text = TidyCase(entry.Key);

                string tempTagWords = "";

                foreach (string word in entry.Value)
                {
                    if (tempTagWords != "")
                    {
                        tempTagWords = tempTagWords + ", " + word;
                        Debug.Log("adding " + word + " to tempwordtags string");
                    }
                    else
                    {
                        tempTagWords = word;
                        Debug.Log("adding " + word + " to temptagwords string. tempwordtage string now = " + tempTagWords);

                    }
                }
                tempPanel.transform.GetChild(1).GetComponent<Text>().text = tempTagWords;

             

                tempPanel.name = entry.Key;
                if (entry.Key == "animal")
                {
                    tempPanel.transform.GetChild(3).GetComponent<Button>().interactable = false;
                    tempPanel.transform.GetChild(0).GetComponent<Text>().text = TidyCase(entry.Key) + " - Stock (cannot delete)";

                }
                tempPanel.SetActive(true);
                entryNum += 1;
                tempScrollviewList.Add(entry.Key);
            }
        }

        tempScrollviewList.Clear();
        tempScrollviewList = null;
        sortedWordList.Clear();

        contentTransform.sizeDelta = new Vector2(contentTransform.rect.width, entryNum * yOffSet);


    }


    public void DeleteTagButton()
    {
        DeleteTagWordsModal.SetActive(true);
        DeleteTagWordsModalMessageText.text = "Are you sure you want to delete all words with the tag '" + EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(0).GetComponent<Text>().text.ToString() + "'?";
        tagWordsToDelete = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(1).GetComponent<Text>().text.ToString().Replace(" ", "");
        Debug.Log("Pressed delete button on tag: " + EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(1).GetComponent<Text>().text.ToString());
    }

    public void CloseDeleteTagWordsModal()
    {
        DeleteTagWordsModal.SetActive(false);
    }


    public void DeleteAllTagWords()
    {
        WordsForDeleteList = tagWordsToDelete.Split(',').ToList();
        foreach (string word in WordsForDeleteList)
        {
            if (word.Contains(" "))
            {
                Debug.Log("space detected in " + word + ". removing.");
                word.Replace(" ", "");
            }
       
            Debug.Log("Deleting " + word);
            controller.DeleteWord(word);
        }
        Debug.Log("Done deleting words for " + gameObject.GetComponentInParent<Transform>().name);
        tagWordsToDelete = "";
        DeleteTagWordsModal.SetActive(false);
        isWords = false;
        controller.ClearData();
        DisplayScrollViewTags();

        successModal.SetActive(true);
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
 

    public void DLCButton()
    {
        DownloadProgressPanel.SetActive(true);
        CloseDLCPanel();
        DownloadProgressBar.fillAmount = 0;
        Debug.Log("DLC button: " + EventSystem.current.currentSelectedGameObject.transform.gameObject.name);
        StartCoroutine(DownloadWordSet(EventSystem.current.currentSelectedGameObject.transform.gameObject.name));
    }

    private IEnumerator DownloadWordSetsList()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://matthewriddett.com/static/mludlc/wordsetslist.txt"))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                wordSetsList = www.downloadHandler.text.Split(',').ToList();

                foreach (string word in wordSetsList) 
                {
                    Debug.Log("wordSetArray includes: " + word);
                }

            }
        }

        isDoneDownloadWordSetsList = true;

        //HTTPRequest requestWordSetData = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/wordsetslist.txt"), OnRequestDownloadWordSetsListFinished);
        //requestWordSetData.Send();
    }



    //private void OnRequestDownloadWordSetsListFinished(HTTPRequest request, HTTPResponse response)
    //{
    //    Debug.Log("RequestWordSetData Finished! Text received: " + response.DataAsText);
    //    //create the word set array from the comma seperated list in the word set data textfile.
    //    wordSetsList = response.DataAsText.Split(',').ToList();
    //    Debug.Log("wordSetstList count = " + wordSetsList.Count);
    //    isDoneDownloadWordSetsList = true;

    //}

    private IEnumerator CreateDLCButtons()
    {
        Debug.Log("start Create dlc buttons");

        yield return new WaitUntil(() => isDoneDownloadWordSetsList);

        GameObject tempButton;
        


        foreach (string wordSet in wordSetsList)
        {
            Debug.Log("Add " + wordSet + " from wordSetsList to button container");
            tempButton = GameObject.Instantiate(DlcCopyButton, DlcButtonContainer.transform, false);
            tempButton.name = wordSet;
            tempButton.GetComponentInChildren<Text>().text = wordSet;
            tempButton.gameObject.SetActive(true);
        }

        isDoneDownloadWordSetsList = false;
    }
 

    private IEnumerator DownloadWordSet(string wordSetName)
    {
        Debug.Log("Downloading " + wordSetName);

        //get the word set data
        StartCoroutine(DownloadWordSetDataText(wordSetName));
        yield return new WaitUntil(() => isDoneWordSetDataDownload);

        //download each word in the set based on the word set data
        for (int i = 0; i < wordSetArray.Length; i++)
        {
            float tempFloat1 = ((float)i) + 1;
            float tempFloat2 = (float)wordSetArray.Length;
            DownloadProgressBar.fillAmount = tempFloat1 / tempFloat2;

            if (controller.DoesDbEntryExist(wordSetArray[i]) == false)
            {
                StartCoroutine(DownloadWord(wordSetArray[i], wordSetName));
                yield return new WaitUntil(() => doneWordDownloadStep);
            }
            else
            {
                Debug.Log(wordSetArray[i] + " already exists in the database.");
            }
        }

        //clear the wordSetArray.
        wordSetArray = null;

        //turn off the download progress panel, reset scrollview and clear data
        DownloadProgressPanel.SetActive(false);
        ResetScrollView();
        controller.ClearData();
    }

   

    private IEnumerator DownloadWordSetDataText(string wordSetName)
    {
        isDoneWordSetDataDownload = false;
        isWordSetArrayFinished = false;
        Debug.Log("Starting download word set data text coroutine for wordset: " + wordSetName.ToLower());

        string url = "https://matthewriddett.com/static/mludlc/WordSets/" + wordSetName + ".txt";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                wordSetArray =  www.downloadHandler.text.Split(',').ToArray();
                isDoneWordSetDataDownload = true;

            }
        }

        //HTTPRequest requestWordSetData = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/WordSets/" + wordSetName + ".txt"), OnRequestWordSetDataFinished);
        //requestWordSetData.Send();
        yield return new WaitUntil(() => isDoneWordSetDataDownload);
        Debug.Log("Done word set data textfile download");
        //move on to the next download step


    }

    //void OnRequestWordSetDataFinished(HTTPRequest request, HTTPResponse response)
    //{
    //    Debug.Log("RequestWordSetData Finished! Text received: " + response.DataAsText);
    //    //create the word set array from the comma seperated list in the word set data textfile.
    //    wordSetArray = response.DataAsText.Split(',').ToArray();
    //    isDoneWordSetDataDownload = true;
    //}

    private IEnumerator DownloadWord(string word, string tag)
    {
        Debug.Log("Downloading word: " + word);
        doneWordDownloadStep = false;
        doneImageDownload = false;
        tempWordName = word;
        tempWordTags = tag;
        yield return new WaitForSeconds(1);
        StartCoroutine(DownloadWordPictures(word));

        // Download this word's images
        //HTTPRequest request1 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "1.png"), OnRequestFinished1);
        //request1.Send();
        //yield return new WaitUntil(() => doneImageDownload);
        //doneImageDownload = false;
        //HTTPRequest request2 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "2.png"), OnRequestFinished1);
        //request2.Send();
        //yield return new WaitUntil(() => doneImageDownload);
        //doneImageDownload = false;
        //HTTPRequest request3 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "3.png"), OnRequestFinished1);
        //request3.Send();
        //yield return new WaitUntil(() => doneImageDownload);
        //doneImageDownload = false;
        //HTTPRequest request4 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "4.png"), OnRequestFinished1);
        //request4.Send();
        //yield return new WaitUntil(() => doneImageDownload);
        //doneImageDownload = false;
        //HTTPRequest request5 = new HTTPRequest(new System.Uri("https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "5.png"), OnRequestFinished2);
        //request5.Send();
        //yield return new WaitUntil(() => doneImageDownload);
        doneImageDownload = false;
        StartCoroutine(DownloadAudioClip(word));
    }

    //void OnRequestFinished1(HTTPRequest request, HTTPResponse response)
    //{
    //    Debug.Log("Request Finished! Text received: " + response.DataAsText);
    //    controller.SetCurrentTexture(response.DataAsTexture2D);
    //    if (controller.IsTextureSet()) {
    //        controller.AddNewTexture();
    //    }
    //    doneImageDownload = true;
    //}

    //void OnRequestFinished2(HTTPRequest request, HTTPResponse response)
    //{
    //    Debug.Log("Request Finished! Text received: " + response.DataAsText);
    //    controller.SetCurrentTexture(response.DataAsTexture2D);
    //    if (controller.IsTextureSet())
    //    {
    //        controller.AddNewTexture();
    //    }
    //    controller.SaveNewWord(tempWordName, tempWordTags);
    //    Debug.Log("Done downloading word: " + tempWordName);
    //    controller.ClearTextureList();
    //    doneImageDownload = true;


    //}

    private void ResetScrollView()
    {
        if (isTagsView)
        {
            DisplayScrollViewTags();
        }
        else
        {
            DisplayScrollViewWords();
        }
        DLCPanel.SetActive(false);
    }

          
    IEnumerator DownloadAudioClip(string file_name)
    {
        string url = "https://matthewriddett.com/static/mludlc/" + "sound/" + file_name + ".wav";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                controller.SaveDLCAudioClip(www.downloadHandler.data, file_name);
                //string savePath = string.Format("{0}/{1}.wav", Application.persistentDataPath + "/WordAudio/", file_name);
                //System.IO.File.WriteAllBytes(savePath, www.downloadHandler.data);
            }
        }

        doneWordDownloadStep = true;
        
    }

    IEnumerator DownloadWordPictures(string word)
    {
        string[] urls =
        {
        "https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "1.png",
        "https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "2.png",
        "https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "3.png",
        "https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "4.png",
        "https://matthewriddett.com/static/mludlc/pictures/" + word + "/" + word + "5.png"
        };

        foreach (string url in urls)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

             
                    controller.SetCurrentTexture(myTexture);
                    if (controller.IsTextureSet())
                    {
                        controller.AddNewTexture();
                    }
                    myTexture = null;
                }
            }
        }

        controller.SaveNewWord(tempWordName, tempWordTags);
        Debug.Log("Done downloading word: " + tempWordName);
        controller.ClearTextureList();
        doneImageDownload = true;

    }
}



