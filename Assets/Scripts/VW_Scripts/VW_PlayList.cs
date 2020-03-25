﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Text;


public class VW_PlayList : MonoBehaviour
{
    // GameObjects
    [SerializeField] private GameObject addNewPanel;
    [SerializeField] private GameObject buttonCopyPanel;
    [SerializeField] private GameObject addNewContent;
    [SerializeField] private GameObject playListContent;
    [SerializeField] private GameObject confirmDeleteModal;
    [SerializeField] private GameObject playCopyPanel;
    [SerializeField] private GameObject deleteSuccessModal;
    [SerializeField] private GameObject SetRewardTimePanel;
    [SerializeField] private GameObject SetRewardsPanel;
    [SerializeField] private GameObject SetRatioPanel;
    [SerializeField] private GameObject SetWordTagsPanel;
    [SerializeField] private GameObject saveSuccessModal;
    [SerializeField] private GameObject saveErrorModal;
    [SerializeField] private GameObject autoPlaylistEnabledPanel;
    [SerializeField] private GameObject PlaylistEmptyPanel;
    [SerializeField] private GameObject confirmAutoplaylistOnModal;
    [SerializeField] private GameObject confirmAutoplaylistOffModal;
    [SerializeField] private GameObject passcodePanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private GameObject RewardsViewportContent;
    [SerializeField] private GameObject tagPanel;
    [SerializeField] private GameObject tagsViewportContent;
    [SerializeField] GameObject playlistMissingRewardModal;


    // Texts
    [SerializeField] private Text rewardTimeText;
    [SerializeField] private Text autoplaylistEnabledRewardTimeText;
    [SerializeField] private Text autoplaylistEnabledRewardsText;
    [SerializeField] private Text tagsText;
    [SerializeField] private Text ratioText;


    // Buttons
    [SerializeField] private Button addButton;
    [SerializeField] private Button autoPlaylistButton;


    // Toggles
    [SerializeField] private Toggle infiniteLoopToggle;
    [SerializeField] private Toggle passLocked;


    // Other
    [SerializeField] private InputField loopNumberField;
    [SerializeField] private Slider rewardTimeSlider;


    // Constants
    private const string SCRAM = "Word_Scramble";
    private const string REWARD = "Choose_Reward";
    private const string FLASH = "Flash_Card";
    private const string KEYB = "Keyboard_Game";
    private const string MATCH = "Matching_Game";
    private const string COUNT = "Counting_Game";
    private const string MEMORY = "Memory_Cards";
    private const int PIC_WIDTH = 75;
    private const int PIC_HEIGHT = 75;
    
    // Statics
    private static RectTransform playListTransform;
    private static RectTransform addNewTransform;
    private static float yOffSet;
    private static GameObject activePanel = null;
    private static int activePanelIndex = -1;
    
    // Ints
    private int minRatio;
    private int maxRatio;
    public int wordsPerReward = 10;
    private int wordLevel;
    private int maxParts;
    private int toggleCount = 0;
    private int tagToggleCount = 0;
    public int rewardTime = 10;
    private int filteredWordIDListCount;

    // Lists
    private List<string> words;
    private List<string> rewardsList;
    private List<int> AutoWordIds = new List<int>();
    private List<string> selectedWordTagsList = new List<string>();
    private List<int> filteredWordIntList = new List<int>();

    // Dictionaries
    private Dictionary<int, string> wordsDatabase = new Dictionary<int, string>(); // This dictionary contains the wordID int and wordName string from the words table in the database.
    private Dictionary<int, string> wordIdTagDict = new Dictionary<int, string>(); // This dictionary contains the wordID int and wordTags string from the words table in the database.
    private Dictionary<int, int> OrderedFilteredWordIdsDict = new Dictionary<int, int>(); // This dictionary is used to contain the index '0-?' ints and the wordID ints, filtered by wordtag. 
    
    // Controller and Model
    private CON_PlayList controller;

    // bool to track whether or not the playlist currently has a Choose Reward entry. If this is false then the Exit method will not allow the user to return to the admin menu.
    private bool playlistHasReward;




    void Start () {
        // Populate the add new scrollview
        MAS_PlayList tempMaster = (MAS_PlayList)COM_Director.GetMaster("MAS_PlayList");

        controller = (CON_PlayList)tempMaster.GetController("CON_PlayList");

        Debug.Log("VW: Starting the play list view!");

        // set the playlist has reward bool to false before populating the scroll view. If a reward is present in the playlist it will switch this bool back to true.
        playlistHasReward = false;

        SetLoopToggleAndValues();
        SetupPlayAndAddNewLists();

        if (PlayerPrefs.GetInt("AutoPlaylistOnOffKey") == 0)
        {
            autoPlaylistEnabledPanel.SetActive(false);
            SetupPlayAndAddNewLists();
        }
        else
        {
            autoPlaylistEnabledPanel.SetActive(true);
            autoplaylistEnabledRewardTimeText.text = PlayerPrefs.GetString("AutoplaylistRewardTimeKey");
            autoplaylistEnabledRewardsText.text = PlayerPrefs.GetString("AutoplaylistRewardsKey");
            tagsText.text = PlayerPrefs.GetString("CurrentAutoplaylistTags");

            addButton.interactable = false;
        }

        Dictionary<int, string> wordsDatabase = new Dictionary<int, string>(controller.GetIdToWordsDict());
        Dictionary<int, string> wordIdTagDict = new Dictionary<int, string>(controller.GetIdToWordTagsDict());
        List<string> selectedWordTagsList = new List<string>();
        Dictionary<int, int> OrderedFilteredWordIdsDict = new Dictionary<int, int>();


        if (PlayerPrefs.GetInt("AutoplaylistRewardTimeIntKey") == 0)
        {
            PlayerPrefs.SetInt("AutoplaylistRewardTimeIntKey", 1);
        }
        else
        {
            rewardTime = PlayerPrefs.GetInt("AutoplaylistRewardTimeIntKey");
            wordsPerReward = rewardTime;

        }
      

        //maxParts = ((wordsDatabase.Count / wordsPerReward));
        Debug.Log("maxParts = " + maxParts + ", wordDatabaseCount = " + wordsDatabase.Count);
        RequestData();
    }


    private void Update()
    {
        if (passLocked.isOn) { loopNumberField.interactable = true; }
        else
        {
            loopNumberField.interactable = false;
            loopNumberField.text = null;
        }                
        
        if (autoPlaylistEnabledPanel.activeSelf)
        {
            addButton.interactable = false;
            passcodePanel.SetActive(false);
        }
    }
    

    
    public void OnRewardSliderChange()
    {
        rewardTime = Mathf.RoundToInt(rewardTimeSlider.value);
        rewardTimeText.text = rewardTime.ToString();
        wordsPerReward = rewardTime;
        Debug.Log("Setting rewardTime to: " + rewardTime);
    }

    // All the open/close methods for the autoplaylist popup modals sequence.
    public void OpenSetRewardTimePanel()
    {
        SetRewardTimePanel.SetActive(true);
    }
    public void CloseSetRewardTimePanel() { SetRewardTimePanel.SetActive(false); }
    public void OpenSetRatioPanel() { SetRatioPanel.SetActive(true); }
    public void CloseSetRatioPanel() { SetRatioPanel.SetActive(false); }
    public void OpenSetRewardsPanel() { SetRewardsPanel.SetActive(true); }
    public void CloseSetRewardsPanel() { SetRewardsPanel.SetActive(false); }    
    public void OpenSetWordTagsPanel() { SetWordTagsPanel.SetActive(true); }
    public void CloseSetWordTagsPanel()
    {

        selectedWordTagsList = CreateSelectedWordTagsList();
        StringBuilder sb = new StringBuilder();
        foreach (string str in selectedWordTagsList)
        {
            int tempint = 0;
            if (selectedWordTagsList.Count == 1)
            {
                sb.Append(str);
            }
            else
            {
                if (tempint != selectedWordTagsList.Count)
                {
                    sb.Append(str + ", ");
                    tempint++;
                }
                else
                {
                    sb.Append(str);
                }
               
            }
        }
        tagsText.text = sb.ToString();
        PlayerPrefs.SetString("CurrentAutoplaylistTags", tagsText.text);
        SetWordTagsPanel.SetActive(false);

    }
    public void CloseConfirmAutoplaylistOffModal() { confirmAutoplaylistOffModal.SetActive(false); }
    public void CloseRemoveModal() { confirmDeleteModal.SetActive(false); }
    public void CloseDeleteSuccessModal() { deleteSuccessModal.SetActive(false); }
    public void CloseConfirmAutoplaylistOnModal() {
        confirmAutoplaylistOnModal.SetActive(false);
        addButton.interactable = true;
    }
    public void OpenRemoveModal()
    {
        activePanelIndex = (EventSystem.current.currentSelectedGameObject.transform.parent.GetSiblingIndex());
        activePanel = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        Debug.Log(string.Format("VW: Index of currently selected panel for removal is {0}", activePanelIndex.ToString()));
        confirmDeleteModal.SetActive(true);
    }
    public void OpenAddPanel()
    {
        addButton.interactable = false;
        addNewPanel.SetActive(true);
    }
    public void CloseAddPanel()
    {
        addButton.interactable = true;
        addNewPanel.SetActive(false);
    }
    

    public void TurnOffAutoPlaylist()
    {
        PlayerPrefs.SetInt("AutoPlaylistOnOffKey", 0);
        autoPlaylistEnabledPanel.SetActive(false);
        confirmAutoplaylistOffModal.SetActive(false);
        ClearPlaylist();
        addButton.interactable = true;
        passcodePanel.SetActive(true);
        controller.ClearData();
    }


    public void ResetAutoplaylistButton()
    {
        confirmAutoplaylistOnModal.SetActive(true);
    }
    

    public void OnAutoPlaylistButton()
    {
        addButton.interactable = false;
        autoPlaylistButton.interactable = false;

        if ((PlayerPrefs.GetInt("AutoPlaylistOnOffKey") == 0))
        {
            confirmAutoplaylistOnModal.SetActive(true);
        }
        else
        {
            confirmAutoplaylistOffModal.SetActive(true);
        }
    }


    public void BeginCreateAutoplaylist()
    {
        StartCoroutine(BeginCreateAutoplaylistLaunchHelper());
        loadingPanel.SetActive(true);
    }
    private IEnumerator BeginCreateAutoplaylistLaunchHelper()
    {
        yield return new WaitForSeconds(2);
        CreateAutoPlaylist();
    }


    /// <summary>
    /// This method executes the auto playlist creation. 
    /// It clears the playlist, create the new entries, and displays the new playlist in the scrollview.
    /// </summary>
    public void CreateAutoPlaylist()
    {
        Debug.Log("Begin CreateAutoPlaylist Method");
        addButton.interactable = false;
        passcodePanel.SetActive(false);

        PlayerPrefs.SetInt("AutoPlaylistOnOffKey", 1);

        autoPlaylistEnabledPanel.SetActive(true);

        // First clear all existing playlist entries
        ClearPlaylist();

        //List<int> tempFilteredWordIds = CreateFilteredWordIdList();
        //Dictionary<int, int> OrderedFilteredWordIdsDict = new Dictionary<int, int>();
        //int tempOrderInt = 0;

        //foreach (int id in tempFilteredWordIds)
        //{
        //    OrderedFilteredWordIdsDict.Add(tempOrderInt, id);
        //    Debug.Log("adding to OrderedWordIdDict, order = " + tempOrderInt + ", wordID = " + id);
        //    tempOrderInt++;
        //}
        //Debug.Log("Done creating orderedwordiddict. count = " + OrderedFilteredWordIdsDict.Count);

        // Then create the list of word IDs filtered by the tags selected by the user in the SetWordTags panel
        //CreateFilteredWordIdList();


        PlaylistEmptyPanel.SetActive(false);

        // Then we create each of the playlist entries one by one, up to the wordsPerReward.
        Debug.Log("Begin Create Autoplaylist method");

        List<int> tempFilteredWordIds = CreateFilteredWordIdList();
        filteredWordIDListCount = tempFilteredWordIds.Count;


        if (wordsPerReward > tempFilteredWordIds.Count)
        {
            maxParts = 0;
        }
        else
        {
            maxParts = ((tempFilteredWordIds.Count / wordsPerReward));

        }
        Debug.Log("tempFilteredWordIds count = " + tempFilteredWordIds.Count);
        Debug.Log("wordsPerReward = " + wordsPerReward);
        Debug.Log("maxparts = " + maxParts);

      

        for (int i = 0; i <= maxParts; i++)
        {          
            CreateAutoPlaylistEntries(i);
        }

        Debug.Log("Done Create Autoplaylist method");

        loadingPanel.SetActive(false);

        //Display the newly created playlist in the scrollview
        //SetupPlayAndAddNewLists();
    }


    private Dictionary<int, int> CreateOrderedWordIDList()
    {
        List<int> tempFilteredWordIds = CreateFilteredWordIdList();

        Dictionary<int, int> OrderedFilteredWordIdsDict = new Dictionary<int, int>();
        int tempOrderInt = 0;

        foreach (int id in tempFilteredWordIds)
        {
            OrderedFilteredWordIdsDict.Add(tempOrderInt, id);
            //Debug.Log("adding to OrderedWordIdDict, order = " + tempOrderInt + ", wordID = " + id);
            tempOrderInt++;
        }
        Debug.Log("Done creating orderedwordiddict. count = " + OrderedFilteredWordIdsDict.Count);
        return OrderedFilteredWordIdsDict;
    }


    /// <summary>
    /// This method creates a filtered word list consisting of the word ID ints from the existing word database 
    /// entries that contain one or more of the word tags specified by the user in the SetWordTags panel.
    /// </summary>
    private List<int> CreateFilteredWordIdList()
    {
        Debug.Log("Begin CreateFilteredWordIdList method");

        List<int> filteredWordIntList = new List<int>();
        // List<string> selectedWordTagsList = CreateSelectedWordTagsList();
        Dictionary<int, string> wordIdTagDict = new Dictionary<int, string>(controller.GetIdToWordTagsDict());
        Dictionary<int, string> wordsDatabase = new Dictionary<int, string>(controller.GetIdToWordsDict());
        Debug.Log("selctedwordtagslist count = " + selectedWordTagsList.Count());


        foreach (var entry in wordIdTagDict)
        {
            List<string> tempTagList = entry.Value.Split(',').ToList();

            foreach (string tg in tempTagList)
            {
                //Debug.Log("tg = " + tg);

                if (!(filteredWordIntList.Contains(entry.Key)))
                {
                    //Debug.Log("word id " + entry.Key + " is not in the filteredwordlist yet");
                    if ((selectedWordTagsList.Contains(tg)) || (selectedWordTagsList.Contains(" " + entry.Value)))
                    {
                        filteredWordIntList.Add(entry.Key);
                        //Debug.Log("adding wordID " + entry.Key + " to filteredWordIdList from tg = " + tg); 
                    }
                }
                
            }
          

        }

        return filteredWordIntList;

    }

    private void CreateAutoPlaylistEntries(int part)
    {
        Debug.Log("Beginning CreateAutoPlaylistEntries, part = " + part);
        // Create a choose reward playlist entry, with pre-set reward time (rewardTime) and types (rewardInts)
        List<int> rewardInts = new List<int>();
        rewardInts.Add(1);
        DO_ChooseReward tempChooseReward = new DO_ChooseReward(CreateRewardIdsList(), rewardTime);

        Dictionary<int, int> OrderedFilteredWordIdsDict = CreateOrderedWordIDList();
        //Debug.Log("OrderedFilteredWordIdsDict count = " + OrderedFilteredWordIdsDict.Count);

        Debug.Log("Words per reward = " + wordsPerReward);



        if (part == maxParts)
        {
            int tempint = 0;
            for (int i = ((wordsPerReward * part) + 1); i <= filteredWordIDListCount; i++)
            {
                foreach (var entry in OrderedFilteredWordIdsDict)
                {
                    if (entry.Key == i)
                    {
                        CreateWordLists(entry.Value);
                    }
                }
                tempint++;
            }
            int remainderInt = wordsPerReward - tempint;
            Debug.Log("remainderInt = " + remainderInt);
            for (int i = 1; i <= remainderInt; i++)
            {
                foreach (var entry in OrderedFilteredWordIdsDict)
                {
                    if (entry.Key == i)
                    {
                        CreateWordLists(entry.Value);
                    }
                }
                tempint++;
            }

        }
        else if (part == 0 && part != maxParts)
        {
            for (int i = 1; i <= wordsPerReward; i++)
            {
                foreach (var entry in OrderedFilteredWordIdsDict)
                {
                    //Debug.Log("entry.Key = " + entry.Key + ", forloop i = " + i);
                    if (entry.Key == i)
                    {
                        CreateWordLists(entry.Value);
                    }
                }
            }
        }
        
        else
        {
            for (int i = ((wordsPerReward * part) + 1); i <= (wordsPerReward * (part + 1)); i++)
            {
                foreach (var entry in OrderedFilteredWordIdsDict)
                {
                    if (entry.Key == i)
                    {
                        CreateWordLists(entry.Value);
                    }
                }
            }
        }

        //Debug.Log("About to add a reward playlist entry. rewardTime = " + rewardTime + ", rewards =" );
        //foreach (int rewardInt in tempChooseReward.rewardIdsList)
        //{
        //    Debug.Log("rewardInt " + rewardInt);

        //}

        controller.CreatingNew();
        controller.AddOrEditEntry("Choose Reward", rewardTime, tempChooseReward);
    }


    /// <summary>
    /// Clears the playlist database table of all entries, and clear the scrollview.
    /// </summary>
    public void ClearPlaylist()
    {
        controller.DeleteAllPlaylist();

        foreach (Transform child in playListContent.transform)
        {
            Destroy(child.gameObject);
        }
        //playListTransform.sizeDelta = new Vector2(playListTransform.rect.width, playListTransform.rect.height - (yOffSet));
        ResetActiveVariables();
        SetupPlayAndAddNewLists();
        OrderedFilteredWordIdsDict.Clear();
        Debug.Log("Playlist cleared");
        autoPlaylistButton.interactable = true;
        
    }

    /// <summary>
    /// This method creates all of the playlist entries programatically for the autoplaylist feature. 
    /// First flashcard, then keyboard, then wordscramble.
    /// </summary>
    /// <param name="wordInt"></param>
    private void CreateWordLists(int wordInt)
    {
        Debug.Log("Creating word list and adding playlist entries for word: " + wordInt);
        List<int> AutoWordIds = new List<int>();
        AutoWordIds.Add(wordInt);
        List<int> AutoWordIdsMatchingGame = new List<int>();
        AutoWordIdsMatchingGame.Add(wordInt);
        AutoWordIdsMatchingGame.Add(wordInt + 1);
        AutoWordIdsMatchingGame.Add(wordInt + 2);



        controller.CreatingNew();


        DO_FlashCard tempFlash = new DO_FlashCard(AutoWordIds);
        DO_KeyboardGame tempKeyboard = new DO_KeyboardGame(AutoWordIds, true, true, false, true, true);
        DO_WordScramble tempScramble = new DO_WordScramble(AutoWordIds);
        DO_KeyboardGame tempKeyboardTwo = new DO_KeyboardGame(AutoWordIds, true, true, false, true, false);
        DO_MatchingGame matchingGame = new DO_MatchingGame(AutoWordIdsMatchingGame, false, true);


        Debug.Log("******************************************** AutoWordIds:");
        foreach (int wordID in AutoWordIds) { Debug.Log(wordID); }

        controller.CreatingNew();
        controller.AddOrEditEntry("Flash Card", AutoWordIds.Count, tempFlash);

        controller.CreatingNew();
        controller.AddOrEditEntry("Keyboard Game", AutoWordIds.Count, tempKeyboard);

        controller.CreatingNew();
        controller.AddOrEditEntry("Word Scramble", AutoWordIds.Count, tempScramble);

        controller.CreatingNew();
        controller.AddOrEditEntry("Keyboard Game", AutoWordIds.Count, tempKeyboard);

        controller.CreatingNew();
        controller.AddOrEditEntry("Keyboard Game", AutoWordIds.Count, tempKeyboardTwo);

        controller.CreatingNew();
        controller.AddOrEditEntry("Matching Game", AutoWordIds.Count, matchingGame);

        controller.CreatingNew();
        controller.AddOrEditEntry("Keyboard Game", AutoWordIds.Count, tempKeyboard);

        controller.CreatingNew();
        controller.AddOrEditEntry("Keyboard Game", AutoWordIds.Count, tempKeyboardTwo);

        AutoWordIds.Clear();
        tempFlash = null;
        tempKeyboard = null;
        tempScramble = null;
        tempKeyboardTwo = null;

    }

    public void AddEntry()
    {
        Debug.Log("VW_PL: Trying to add entry of type " + GetAddNewTypeTextString(EventSystem.current.currentSelectedGameObject));

        // Set new entry creation flag to true
        controller.CreatingNew();

        EventSystem.current.currentSelectedGameObject.GetComponent<Animation>().Play("ButtonWiggleAnim");

        string nextScene =  GetAddNewTypeTextString(EventSystem.current.currentSelectedGameObject);
        StartCoroutine(AddEntryHelper(nextScene));

    }

    private IEnumerator AddEntryHelper(string nextScene)
    {
        yield return new WaitForSeconds(1);
        controller.SceneChange(nextScene);

    }

    public void EditEntry()
    {
        GameObject currentObj = EventSystem.current.currentSelectedGameObject;
        Debug.Log("VW_PL: Trying to EDIT entry of type " + GetTypeTextString(currentObj));

        controller.EditingEntry();
        controller.SetActiveEntryIndex(currentObj.transform.parent.GetSiblingIndex());

        Debug.Log("VW_PL: Set edit entry flag to true and set active index to " + currentObj.transform.parent.GetSiblingIndex());
        controller.SceneChange(GetTypeTextString(currentObj));
    }

    public void RemoveEntry()
    {
        Debug.Log("VW_PL: Attempting to remove entry at index " + activePanelIndex.ToString());
        if (activePanelIndex > -1)
        {
            Debug.Log("VW_PL: Active panel index was greater than one. Proceeding to removal.");
            if (controller.RemoveEntryData(activePanelIndex))
            {
                Debug.Log("VW_PL: Controller returned true on remove data entry");
                confirmDeleteModal.SetActive(false);
                deleteSuccessModal.SetActive(true);

                // Destroy the panel belonging to the word entry
                Destroy(activePanel);
                playListTransform.sizeDelta = new Vector2(playListTransform.rect.width, playListTransform.rect.height - (yOffSet));
                ResetActiveVariables();
            }
            else
            {
                // TODO: toss an error regarding DB query failure
            }
        }
        else
        {
            //TODO: toss an error about active panel index
        }
    }

    public void MoveEntryUp()
    {
        Transform panelParent = EventSystem.current.currentSelectedGameObject.transform.parent;
        panelParent.gameObject.GetComponent<Animation>().Play();
        int currentIndex = panelParent.GetSiblingIndex();

        if (currentIndex > (0))
        {
            if (controller.SwapListEntries(currentIndex, currentIndex - 1))
            {
                panelParent.SetSiblingIndex(currentIndex - 1);
            }
            else
            {
                Debug.Log("Error in MoveEntryUp method");
            }
        } 
    }

    public void MoveEntryDown()
    {
        Transform panelParent = EventSystem.current.currentSelectedGameObject.transform.parent;
        panelParent.gameObject.GetComponent<Animation>().Play();

        int currentIndex = panelParent.GetSiblingIndex();

        if ((currentIndex - 1) < panelParent.parent.childCount)
        {
            if (controller.SwapListEntries(currentIndex, currentIndex + 1))
            {
                panelParent.SetSiblingIndex(currentIndex + 1);
            }
            else
            {
                // TODO: toss a db query error
            }
        } 
    }

    

    public void ToggleLoopOptions() { loopNumberField.interactable = passLocked.isOn;  }
    
    public void ClosePlaylistMissingRewardModal()
    {
        playlistMissingRewardModal.SetActive(false);
    }

    public void Exit()
    {
        bool success = false;
        success = ((controller.SetLoopData(0, infiniteLoopToggle.isOn, loopNumberField.text == "" ? 1 : Int16.Parse(loopNumberField.text), passLocked.isOn)) && (playlistHasReward || ((PlayerPrefs.GetInt("AutoPlaylistOnOffKey")) == 1))) ;

        if (!success)
        {
            playlistMissingRewardModal.SetActive(true);
            return;
        }

        controller.SceneChange("admin_menu");
    }
    
    private void ResetActiveVariables()
    {
        activePanel = null;
        activePanelIndex = -1;
    }

    private void SetupPlayAndAddNewLists()
    {
        Debug.Log("VW_PlayList: Setting up!");

        controller.VerifyData();
        
        // Populate the play list scrollview and size it properly
        VerticalLayoutGroup verticalLayout = playListContent.GetComponent<VerticalLayoutGroup>();

        GameObject tempPanel;
        playListTransform = playListContent.GetComponent<RectTransform>();
        float entryNum = 0;
        yOffSet = verticalLayout.preferredHeight + verticalLayout.spacing;
        
        List<DO_PlayListObject> playList = controller.GetPlayList();
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ \n VW: COUNT OF PLAY LIST IS " + playList.Count.ToString() + "\n &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

        if (playList.Count == 0) { PlaylistEmptyPanel.SetActive(true); }
        else { PlaylistEmptyPanel.SetActive(false); }

        Debug.Log("VW_PlayList: ENTERING FOR for PANELS!");

        for (int idx = 0; idx < playList.Count; idx++)
        {
            Debug.Log("VW_PlayList: INDEX IS " + idx.ToString());

            // Instantiate the new panel and set its text accordingly
            tempPanel = GameObject.Instantiate(playCopyPanel, playListContent.transform, false);
            tempPanel.transform.Find("TypeText").GetComponent<Text>().text = controller.GetTypeString(playList[idx].type_id).Replace("_", " ");

            if (controller.GetTypeString(playList[idx].type_id) == "Choose_Reward")
            {
                playlistHasReward = true;
            }

            tempPanel.transform.Find("DurationText").GetComponent<Text>().text = controller.CreateActivityListString(idx, playList[idx].type_id);

            // Activate the panel and add one to the entry count
            tempPanel.SetActive(true);
            entryNum += 1;

            

        }

        // Resize the content box on the Y
        playListTransform.sizeDelta = new Vector2(playListTransform.rect.width, entryNum * yOffSet);

        Debug.Log("VW_PlayList: FINISHED PLAY LIST SETUP!");

        // Clear values and populate the add new list scrollview and size it properly
        verticalLayout = addNewContent.GetComponent<VerticalLayoutGroup>();

        addNewTransform = addNewContent.GetComponent<RectTransform>();
        entryNum = 0;
        yOffSet = verticalLayout.preferredHeight + verticalLayout.spacing;

        foreach (string typeStr in controller.GetTypeStrings())
        {
            // Instantiate the new panel and set its text accordingly
            tempPanel = GameObject.Instantiate(buttonCopyPanel, addNewContent.transform, false);
            tempPanel.transform.Find("TypeText").GetComponent<Text>().text = typeStr.Replace("_", " ");




            if (!CheckType(typeStr))
            {
                tempPanel.transform.gameObject.GetComponent<Button>().interactable = false;
            }

            /**
            if (entryNum >= 3)
            {
                tempPanel.transform.Find("AddButton").gameObject.GetComponent<Button>().interactable = false;
            }
            **/

            // Activate the panel and add one to the entry count
            tempPanel.SetActive(true);
            entryNum += 1;
        }
        
        Debug.Log("VW_PlayList: DONE SETUP!");
        // Resize the content box on the Y
        addNewTransform.sizeDelta = new Vector2(addNewTransform.rect.width, entryNum * yOffSet);
    }

    private void SetLoopToggleAndValues()
    {
        bool check = controller.CheckIfPassLocked();
        passLocked.isOn = check;
        loopNumberField.interactable = check;
        loopNumberField.text = controller.GetLoopNumber().ToString();
    }

    private string GetTypeTextString(GameObject obj) { return obj.transform.parent.Find("TypeText").gameObject.GetComponent<Text>().text.ToLower().Replace(" ", "_"); }
    private string GetAddNewTypeTextString(GameObject obj) { return obj.transform.Find("TypeText").gameObject.GetComponent<Text>().text.ToLower().Replace(" ", "_"); }

    private bool CheckType(string type)
    {
            switch (type)
            {
                case SCRAM:
                case REWARD:
                case FLASH:
                case KEYB:
                case MATCH:
                //case COUNT:
                //case MEMORY:
                    return true;
                default:
                    return false;
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


    /////////////////////////////// Choose Reward Stuff vvvvvvvvvvvvvvvvvvvvv
    ///

    private void RequestData()
    {
        Debug.Log("VW_Playlist Sending data request to the controller...");
        Dictionary<int, string> rewardDict = controller.RequestRewards();
        List<string> listTags = controller.GetWordTagsList();
        Dictionary<int, string> tagsDict = controller.GetIdToWordTagsDict();


        foreach (var reward in rewardDict)
        {
            //Create a panel
            GameObject panel = GameObject.Instantiate(rewardPanel, RewardsViewportContent.transform, false);
            panel.name = reward.Key.ToString();
            GameObject label = panel.transform.Find("Label").gameObject;
            label.GetComponent<Text>().text = reward.Value;
            label.name = reward.Value.ToString();


            Toggle t = panel.GetComponent<Toggle>();
            t.onValueChanged.AddListener(delegate
            {
                OnToggleChange(t);
            });

            panel.SetActive(true);

            GameObject panelImage = panel.transform.Find("Image").gameObject;
            RawImage img = panelImage.GetComponent<RawImage>();
            Texture2D tx = new Texture2D(PIC_WIDTH, PIC_HEIGHT);
            byte[] rewardPic = FileAccessUtil.LoadRewardPic(reward.Value);
            //If not a custom reward
            if (rewardPic == null)
            {
                img.texture = Resources.Load<Texture2D>("RewardPictures/" + reward.Value);
            }
            //else is a custom reward
            else
            {
                if (tx.LoadImage(rewardPic))
                {
                    img.texture = tx;
                }
                else
                {
                    Debug.Log("VW_Playlist Loading custom picture failed.");
                }
            }

            
        }

        foreach (string entry in listTags)
        {
            //Create a panel
            GameObject tPanel = GameObject.Instantiate(tagPanel, tagsViewportContent.transform, false);
            tPanel.name = entry;
            GameObject tagLabel = tPanel.transform.Find("Label").gameObject;
            tagLabel.GetComponent<Text>().text = entry;
            tagLabel.name = entry;

            Toggle toggleTag = tPanel.GetComponent<Toggle>();
            toggleTag.onValueChanged.AddListener(delegate
            {
                OnToggleTagChange(toggleTag);
            });

            tPanel.SetActive(true);

        }

                     
    }

    public void OnToggleTagChange(Toggle tagToggleChange)
    {
        if (tagToggleChange.isOn)
        {
            tagToggleCount++;
        }
        else
        {
            tagToggleCount--;
        }
    }

    public void OnToggleChange(Toggle change)
    {
        if (change.isOn)
        {
            toggleCount++;
        }
        else
        {
            toggleCount--;
        }
    }

    private List<string> CreateSelectedWordTagsList()
    {
        //Dictionary<int, string> dict = controller.GetIdToWordTagsDict();
        List<string> tagStrings = new List<string>();

        foreach (Transform child in tagsViewportContent.transform)
        {
            
            if (child.GetComponent<Toggle>().isOn)
            {
                //Debug.Log(child.name + " tag toggle is on");
                tagStrings.Add(child.name);
            }
            //else
            //{
            //    Debug.Log(child.name + " tag is off");
            //}
        }

        Debug.Log("*************************************************************Done creating tagStrings list. Count and list items follow: \n Count = " + tagStrings.Count);
        foreach (string str in tagStrings)
        {
            Debug.Log(str);
        }
        

        return tagStrings;
    }


    /// <summary>
    /// Creates and returns a list of reward id integers.
    /// </summary>
    /// <returns>A list of ints</returns>
    private List<int> CreateRewardIdsList()
    {
        Dictionary<int, string> dict = controller.RequestRewards();
        List<int> list = new List<int>();
        List<string> rewardStrings = new List<string>();

        foreach (Transform child in RewardsViewportContent.transform)
        {
            if (child.GetComponent<Toggle>().isOn)
            {
                list.Add(Int16.Parse(child.name));
            }
        }

        foreach (var reward in dict)
        {
            int temptint = reward.Key;
            if (list.Contains(temptint))
            {
                rewardStrings.Add(reward.Value);

            }
        }

        string combindedString = string.Join(",", rewardStrings.ToArray());


        autoplaylistEnabledRewardTimeText.text = rewardTime.ToString();
        PlayerPrefs.SetString("AutoplaylistRewardTimeKey", rewardTime.ToString());
        PlayerPrefs.SetInt("AutoplaylistRewardTimeIntKey", rewardTime);
        autoplaylistEnabledRewardsText.text = combindedString;
        PlayerPrefs.SetString("AutoplaylistRewardsKey", combindedString);




        return list;
    }


}