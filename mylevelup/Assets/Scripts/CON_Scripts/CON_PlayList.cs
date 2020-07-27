using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Constant;

public class CON_PlayList : MonoBehaviour
{
    private MOD_PlayList model;
    private MAS_PlayList master;
    private VW_PlayList view;

    // member variables from the view
    private int wordLevel;
    private int maxParts;
    public int toggleCount = 0;
    public int tagToggleCount = 0;
    public int gameToggleCount = 0;
    private int filteredWordIDListCount;

    private int wordsPerReward = 10;
    public int rewardTime = 10;
    public int m_toggleCount;


    public static RectTransform playListTransform;
    private static RectTransform addNewTransform;
    private static float yOffSet;

    // Lists
    private List<string> words;
    private List<string> rewardsList;
    private List<int> AutoWordIds = new List<int>();
    private List<string> selectedWordTagsList = new List<string>();
    private List<int> filteredWordIntList = new List<int>();
    private List<string> selectedGamesList = new List<string>();

    public Dictionary<int, int> m_OrderedFilteredWordIdsDict = new Dictionary<int, int>();

    public CON_PlayList(MasterClass newMaster)// : base(newMaster)
    {
        master = (MAS_PlayList)newMaster;
        if (PlayerPrefs.GetInt("AutoplaylistRewardTimeIntKey") == 0)
        {
            PlayerPrefs.SetInt("AutoplaylistRewardTimeIntKey", 1);
        }
        else
        {
            rewardTime = PlayerPrefs.GetInt("AutoplaylistRewardTimeIntKey");
            wordsPerReward = rewardTime;
        }
    }
    private string GetTypeTextString(GameObject obj) { return obj.transform.parent.Find("TypeText").gameObject.GetComponent<Text>().text.ToLower().Replace(" ", "_"); }
    private string GetAddNewTypeTextString(GameObject obj) { return obj.transform.Find("TypeText").gameObject.GetComponent<Text>().text.ToLower().Replace(" ", "_"); }

    public void ClearData()
    {
        model.ResetSceneData();
    }

    public void GetCoworkers(MasterClass newMaster)
    {
        model = (MOD_PlayList)master.GetModel("MOD_PlayList");
    }

    public void SceneChange(string scene)
    {
        master.RequestSceneChange(scene);
    }

    public Dictionary<int, string> GetIdToWordsDict()
    {
        return model.wordDict;
    }

    public List<string> GetWordTagsList()
    {
        return model.wordTagsList;
    }

    public Dictionary<int, string> GetIdToWordTagsDict()
    {   
        return model.tagsDict;
    } 

    /// <summary>
    /// Returns the total number of words currently in the words table of the database.
    /// </summary>
    /// <returns></returns>
    public int GetTotalWordsCount()
    {
        return model.GetTotalWords();
    }

    public string CreateActivityListString(int index, int typeId)
    {       
        StringBuilder builder = new StringBuilder();

        int max;

        switch (typeId)
        {
            case 0:
                DO_WordScramble scramble = JsonUtility.FromJson<DO_WordScramble>(model.GetJsonData(index));

                max = scramble.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[scramble.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[scramble.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 1:
                DO_ChooseReward reward = JsonUtility.FromJson<DO_ChooseReward>(model.GetJsonData(index));
                
                max = reward.rewardIdsList.Count;      

                break;

            case 2:
                DO_FlashCard flash = JsonUtility.FromJson<DO_FlashCard>(model.GetJsonData(index));
                
                max = flash.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {                      
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[flash.wordIdList[idx]]));
                    }
                    else
                    {                       
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[flash.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 3:
                DO_CountingGame counting = JsonUtility.FromJson<DO_CountingGame>(model.GetJsonData(index));
                
                max = counting.wordIds.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[counting.wordIds[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[counting.wordIds[idx]])), ", ");
                    }
                }

                break;

            case 4:
                DO_KeyboardGame keyboard = JsonUtility.FromJson<DO_KeyboardGame>(model.GetJsonData(index));

                max = keyboard.wordIdList.Count;
                
                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[keyboard.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[keyboard.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 5:
                DO_MemoryCards memory = JsonUtility.FromJson<DO_MemoryCards>(model.GetJsonData(index));
                
                max = memory.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[memory.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[memory.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 6:
                DO_MatchingGame matching = JsonUtility.FromJson<DO_MatchingGame>(model.GetJsonData(index));
                
                max = matching.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[matching.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[matching.wordIdList[idx]])), ", ");
                    }
                }

                break;

            default:
                // TODO: log an error
                return null;
        }       

        return builder.ToString();
         
            //case 7:
            //    break;
            //case 8:
            //    break;
            //case 10:
            //    break;
            //case 11:
    }

    public bool AddOrEditEntry(string typeStr, int duration, object dataObject)
    {
        string json = "";

        int typeId = model.GetTypeId(typeStr);

        switch (typeId)
        {
            case 0:
                json = JsonUtility.ToJson((DO_WordScramble)dataObject);
                break;

            case 1:                
                json = JsonUtility.ToJson((DO_ChooseReward)dataObject);
                break;

            case 2:
                json = JsonUtility.ToJson((DO_FlashCard)dataObject);               
                break;

            case 3:
                json = JsonUtility.ToJson((DO_CountingGame)dataObject);
                break;

            case 4:
                json = JsonUtility.ToJson((DO_KeyboardGame)dataObject);
                break;

            case 5:
                json = JsonUtility.ToJson((DO_MemoryCards)dataObject);
                break;
           
            case 6:
                json = JsonUtility.ToJson((DO_MatchingGame)dataObject);
                break;

            default:
                // TODO: log an error
                return false;

        }

        return model.AddOrEditEntryData(typeId, duration, json);
              
            //case 7:
            //    break;
            //case 8:
            //    break;        
            //case 10:
            //    break;
            //case 11:
            //    break;         
    }

    //########################### Choose Reward Functionality ####################

    public Dictionary<int, string> RequestRewards()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();

        foreach (var row in model.GetRewardsTable())
        {
            dict.Add(row.reward_id, row.reward_name);
        }

        return dict;
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

    // Return a copy of the play list
    public int GetDurationByIndex(int idx) { return model.GetDurationValue(idx); }

    public string GetJsonByIndex(int idx) { return model.GetJsonData(idx); }

    public string[] GetTypeStrings() { return model.GetTypeStrings(); }

    public string GetTypeString(int typeId) { return model.GetTypeString(typeId); }

    public List<DO_PlayListObject> GetPlayList() { return model.GetPlayListObjects(); }

    public bool CheckIfNewEntry() { return model.creatingNewEntry; }

    public void CreatingNew() { model.creatingNewEntry = true; }

    public void EditingEntry() { model.creatingNewEntry = false; }

    public void SetActiveEntryIndex(int currentIdx) { model.activeEntryIndex = currentIdx; }

    public int GetActiveContextIndex() { return model.activeEntryIndex; }

    public bool SwapListEntries(int idxA, int idxB) { return model.SwapEntryData(idxA, idxB); }

    public bool RemoveEntryData(int idx) { return model.RemoveEntryData(idx); }

    public bool DeleteAllPlaylist() { return model.DeletePlaylist(); }

    public bool CheckIfInfiniteLoop() { return model.infiniteLoops; }

    public int GetLoopNumber() { return model.loopIterations; }

    public bool CheckIfPassLocked() { return model.passLocked; }

    public bool SetLoopData(int playId, bool infinite, int iterations, bool passLocked) { return model.SetLoopData(playId, infinite, iterations, passLocked); }

    public void VerifyData() { model.VerifyData(); }








    // *********** METHODS FROM THE VIEW **********//


    /// <summary>
    /// This method executes the auto playlist creation. 
    /// It clears the playlist, creates the new entries, and displays the new playlist in the scrollview.
    /// </summary>


    public void TurnOffAutoPlaylist()
    {
        PlayerPrefs.SetInt("AutoPlaylistOnOffKey", 0);
        view.DisableAutoPlaylistPanels();
        ClearPlaylist();
        ClearData();
    }

    public void CreateAutoPlaylist()
    {
        PlayerPrefs.SetInt("AutoPlaylistOnOffKey", 1);
        view.EnableAutoPlaylistPanels();
        // Clear all existing playlist entries
        ClearPlaylist();

        view.PlaylistEmptyPanel.SetActive(false);

        List<int> tempFilteredWordIds = CreateFilteredWordIdList();
        filteredWordIDListCount = tempFilteredWordIds.Count;

        // Calculate the max parts of the playlist
        maxParts = (wordsPerReward > tempFilteredWordIds.Count) ? 0 : (tempFilteredWordIds.Count / wordsPerReward);

        // Create each of the playlist entries one by one, up to the wordsPerReward, then repeat up to maxParts.
        for (int i = 0; i <= maxParts; i++)
        {
            CreateAutoPlaylistEntries(i);
        }

        view.SetLoadingPanelActive(false);
    }


    private Dictionary<int, int> CreateOrderedWordIDList()
    {
        List<int> tempFilteredWordIds = CreateFilteredWordIdList();
        Dictionary<int, int> OrderedFilteredWordIdsDict = new Dictionary<int, int>();
        int tempOrderInt = 1;

        foreach (int id in tempFilteredWordIds)
        {
            Debug.Log("adding " + id + " to Ordered Filtered Word IDs Dict with tempOrderInt:" + tempOrderInt);
            OrderedFilteredWordIdsDict.Add(tempOrderInt, id);
            tempOrderInt++;
        }

        return OrderedFilteredWordIdsDict;
    }

    /// <summary>
    /// This method creates a filtered word list consisting of the word IDs (int) from the existing word database 
    /// entries that contain one or more of the word tags specified by the user in the SetWordTags panel.
    /// </summary>
    private List<int> CreateFilteredWordIdList()
    {
        List<int> filteredWordIntList = new List<int>();
        Dictionary<int, string> wordIdTagDict = new Dictionary<int, string>(GetIdToWordTagsDict());
        Dictionary<int, string> wordsDatabase = new Dictionary<int, string>(GetIdToWordsDict());

        foreach (var entry in wordIdTagDict)
        {
            List<string> tempTagList = (entry.Value.Split(',')).ToList();

            foreach (string tg in tempTagList)
            {
                if (!(filteredWordIntList.Contains(entry.Key)))
                {
                    if ((selectedWordTagsList.Contains(tg)) || (selectedWordTagsList.Contains(" " + entry.Value)))
                    {
                        filteredWordIntList.Add(entry.Key);
                        Debug.Log("Adding " + entry.Key + " to filteredWordIntList");
                    }
                }
            }
        }

        return filteredWordIntList;
    }

    private void CreateAutoPlaylistEntries(int part)
    {
        // Create a choose reward playlist entry, with pre-set reward time (rewardTime) and types (rewardInts)
        List<int> rewardInts = new List<int>();
        rewardInts.Add(1);
        DO_ChooseReward tempChooseReward = new DO_ChooseReward(CreateRewardIdsList(), rewardTime);
        Dictionary<int, int> OrderedFilteredWordIdsDict = CreateOrderedWordIDList();


        if (part == maxParts)
        {
            int tempint = 0;


            for (int i = ((wordsPerReward * part)); i <= filteredWordIDListCount; i++)
            {
                foreach (var entry in OrderedFilteredWordIdsDict)
                {
                    if (entry.Key == i)
                    {
                        Debug.Log("sending " + i + " to CreateWordLists");
                        CreateWordLists(entry.Value);
                    }
                }

                tempint++;
            }

            int remainderInt = wordsPerReward - tempint;

            for (int i = 1; i <= remainderInt; i++)
            {
                foreach (var entry in OrderedFilteredWordIdsDict)
                {
                    if (entry.Key == i)
                    {
                        Debug.Log("sending " + i + " to CreateWordLists");

                        CreateWordLists(entry.Value);
                    }
                }

                tempint++;
            }
        }
        else if (part == 0 && part != maxParts)
        {

            //////////////////////////////////////////////////////// TESTING NEW CODE HERE
            for (int i = 1; i <= (wordsPerReward); i++) //TEST CODE
            {
                foreach (var entry in OrderedFilteredWordIdsDict)
                {
                    if (entry.Key == i)
                    {
                        Debug.Log("sending " + i + " to CreateWordLists");

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
                        Debug.Log("sending " + i + " to CreateWordLists");

                        CreateWordLists(entry.Value);
                    }
                }
            }
        }

        CreatingNew();
        AddOrEditEntry("Choose Reward", rewardTime, tempChooseReward);
    }



    /// <summary>
    /// This method creates all of the playlist entries programatically for the autoplaylist feature.  
    /// </summary>
    /// <param name="wordInt"></param>
    private void CreateWordLists(int wordInt)
    {

        //Create the single word word list for the flashcard, wordscramble, and keyboard games
        List<int> AutoWordIds = new List<int>();
        AutoWordIds.Clear();
        Debug.Log("adding " + wordInt + " to AutoWordIDs list");
        AutoWordIds.Add(wordInt);

        // Create AutoWordIds for the matching game, with the current word and two additional random words.
        List<int> AutoWordIdsMatchingGame = new List<int>();
        AutoWordIdsMatchingGame.Clear();
        AutoWordIdsMatchingGame.Add(wordInt);
        AutoWordIdsMatchingGame.Add(UnityEngine.Random.Range(1, GetTotalWordsCount()));

        if (AutoWordIdsMatchingGame[1] == AutoWordIdsMatchingGame[0])
        {
            AutoWordIdsMatchingGame[1] = UnityEngine.Random.Range(1, GetTotalWordsCount());
        }

        AutoWordIdsMatchingGame.Add(UnityEngine.Random.Range(1, GetTotalWordsCount()));

        if (AutoWordIdsMatchingGame[2] == AutoWordIdsMatchingGame[1] || AutoWordIdsMatchingGame[2] == AutoWordIdsMatchingGame[0])
        {
            AutoWordIdsMatchingGame[2] = UnityEngine.Random.Range(1, GetTotalWordsCount());
        }

        Debug.Log("Matching Game autoplaylist words set to:");
        foreach (int id in AutoWordIdsMatchingGame)
        {
            Debug.Log(id);
        }

        // Create AutoWordIds for the memory game, with the current word and one additional random word.
        List<int> AutoWordIdsMemoryGame = new List<int>();
        AutoWordIdsMemoryGame.Clear();
        AutoWordIdsMemoryGame.Add(wordInt);
        AutoWordIdsMemoryGame.Add(UnityEngine.Random.Range(1, GetTotalWordsCount()));

        CreatingNew();
        Debug.Log("games list = ");
        foreach (string game in selectedGamesList)
        {
            Debug.Log(game);

        }

        AutoPlaylistCreateEntry("flash", 0, AutoWordIds);
        AutoPlaylistCreateEntry("keyboard", 0, AutoWordIds);
        AutoPlaylistCreateEntry("keyboard", 1, AutoWordIds);
        AutoPlaylistCreateEntry("matching", 0, AutoWordIdsMatchingGame);
        AutoPlaylistCreateEntry("keyboard", 0, AutoWordIds);
        AutoPlaylistCreateEntry("keyboard", 1, AutoWordIds);
        AutoPlaylistCreateEntry("scramble", 0, AutoWordIds);
        AutoPlaylistCreateEntry("memory", 0, AutoWordIdsMemoryGame);
        AutoPlaylistCreateEntry("counting", 0, AutoWordIdsMatchingGame);
        AutoPlaylistCreateEntry("matching", 0, AutoWordIdsMatchingGame);

        AutoWordIds.Clear();
        AutoWordIdsMatchingGame.Clear();
        AutoWordIdsMemoryGame.Clear();

    }

    private void AutoPlaylistCreateEntry(string gameType, int gameConfig, List<int> wordIDsList)
    {
        if (gameType == "scramble" && selectedGamesList.Contains("Word_Scramble"))
        {
            DO_WordScramble tempScramble = new DO_WordScramble(wordIDsList);
            CreatingNew();
            AddOrEditEntry("Word Scramble", 1, tempScramble);
        }
        if (gameType == "flash" && selectedGamesList.Contains("Flash_Card"))
        {
            DO_FlashCard tempFlash = new DO_FlashCard(wordIDsList);
            CreatingNew();
            AddOrEditEntry("Flash Card", 1, tempFlash);
        }
        if (gameType == "keyboard" && selectedGamesList.Contains("Keyboard_Game"))
        {
            if (gameConfig == 0)
            {
                DO_KeyboardGame tempKeyboard = new DO_KeyboardGame(wordIDsList, true, true, false, true, true);
                CreatingNew();
                AddOrEditEntry("Keyboard Game", 1, tempKeyboard);
            }
            else if (gameConfig == 1)
            {
                DO_KeyboardGame tempKeyboardTwo = new DO_KeyboardGame(wordIDsList, true, true, false, true, false);
                CreatingNew();
                AddOrEditEntry("Keyboard Game", 1, tempKeyboardTwo);
            }
        }
        if (gameType == "counting" && selectedGamesList.Contains("Couting_Game"))
        {
            DO_CountingGame countingGame = new DO_CountingGame(wordIDsList, 3, 10, true, true, true, false);
            CreatingNew();
            AddOrEditEntry("Counting Game", wordIDsList.Count, countingGame);
        }
        if (gameType == "matching" && selectedGamesList.Contains("Matching_Game"))
        {
            DO_MatchingGame matchingGame = new DO_MatchingGame(wordIDsList, false, true);
            CreatingNew();
            AddOrEditEntry("Matching Game", 1, matchingGame);
        }
        if (gameType == "memory" && selectedGamesList.Contains("Memory_Cards"))
        {
            DO_MemoryCards tempMemory = new DO_MemoryCards(wordIDsList, true, false, true);
            CreatingNew();
            AddOrEditEntry("Memory Cards", 1, tempMemory);
        }
    }

    public void AddEntry()
    {
        // Set new entry creation flag to true
        CreatingNew();
        EventSystem.current.currentSelectedGameObject.GetComponent<Animation>().Play("ButtonWiggleAnim");
        string nextScene = GetAddNewTypeTextString(EventSystem.current.currentSelectedGameObject);
        StartCoroutine(AddEntryHelper(nextScene));
    }

    private IEnumerator AddEntryHelper(string nextScene)
    {
        yield return new WaitForSeconds(1);
        SceneChange(nextScene);
    }

    public void EditEntry()
    {
        GameObject currentObj = EventSystem.current.currentSelectedGameObject;
        EditingEntry();
        SetActiveEntryIndex(currentObj.transform.parent.GetSiblingIndex());
        SceneChange(GetTypeTextString(currentObj));
    }

    public void RemoveEntry()
    {

        if (RemoveEntryData(ACTIVE_PANEL_INDEX))
        {
            SetDeleteObjects();
            view.ResetActivePanel();
        }
        //else if ()
        //{
        //    // TODO: log an error regarding DB query failure
        //}
        //else
        //{
        //    //TODO: log an error about active panel index
        //}
    }

    public void MoveEntryUp()
    {
        Transform panelParent = EventSystem.current.currentSelectedGameObject.transform.parent;
        panelParent.gameObject.GetComponent<Animation>().Play();
        int currentIndex = panelParent.GetSiblingIndex();

        if (currentIndex > (0))
        {
            if (SwapListEntries(currentIndex, currentIndex - 1))
            {
                panelParent.SetSiblingIndex(currentIndex - 1);
            }
            else
            {
                // TODO: Log an error
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
            if (SwapListEntries(currentIndex, currentIndex + 1))
            {
                panelParent.SetSiblingIndex(currentIndex + 1);
            }
            else
            {
                // TODO: log a db query error
            }
        }
    }


    /// <summary>
    /// Clears the playlist database table of all entries, and clear the scrollview.
    /// </summary>
    public void ClearPlaylist()
    {
        DeleteAllPlaylist();

        foreach (Transform child in view.PlayListContent.transform)
        {
            Destroy(child.gameObject);
        }

        view.ResetActivePanel();
        SetupPlayAndAddNewLists();
        m_OrderedFilteredWordIdsDict.Clear();
        view.AutoPlaylistButton.interactable = true;
    }

    public void RequestData()
    {
        Dictionary<int, string> rewardDict = RequestRewards();
        List<string> listTags = GetWordTagsList();
        Dictionary<int, string> tagsDict = GetIdToWordTagsDict();

        foreach (var reward in rewardDict)
        {
            //Create a panel
            GameObject panel = GameObject.Instantiate(view.RewardPanel, view.RewardsViewportContent.transform, false);
            panel.name = reward.Key.ToString();
            GameObject label = panel.transform.Find("Label").gameObject;
            label.GetComponent<Text>().text = reward.Value;
            label.name = reward.Value.ToString();
            Toggle t = panel.GetComponent<Toggle>();

            t.onValueChanged.AddListener(delegate
            {
                view.OnToggleChange(t);
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
                    // TODO: log an error
                    Debug.Log("VW_Playlist Loading custom picture failed.");
                }
            }
        }

        foreach (string entry in listTags)
        {
            //Create a panel
            GameObject tPanel = GameObject.Instantiate(view.TagCopyPanel, view.TagsViewportContent.transform, false);
            tPanel.name = entry;
            GameObject tagLabel = tPanel.transform.Find("Label").gameObject;
            tagLabel.GetComponent<Text>().text = entry;
            tagLabel.name = entry;
            Toggle toggleTag = tPanel.GetComponent<Toggle>();

            toggleTag.onValueChanged.AddListener(delegate
            {
                view.OnTagToggleChange(toggleTag);
            });

            tPanel.SetActive(true);
        }

        foreach (string entry in GetTypeStrings())
        {
            // TODO: Remove negative ref to memory cards once it has been debugged.
            if (entry != "Choose_Reward" && entry != "Memory_Cards")
            {
                //Create a panel
                GameObject sgPanel = GameObject.Instantiate(view.GameCopyPanel, view.SetGamesContent.transform, false);
                sgPanel.name = entry;
                GameObject gameLabel = sgPanel.transform.Find("Label").gameObject;
                gameLabel.GetComponent<Text>().text = entry;
                gameLabel.name = entry;
                Toggle toggleGame = sgPanel.GetComponent<Toggle>();

                toggleGame.onValueChanged.AddListener(delegate
                {
                    view.OnGameToggleChange(toggleGame);
                });

                sgPanel.SetActive(true);
            }
        }
    }

    private List<string> CreateSelectedWordTagsList()
    {
        List<string> tagStrings = new List<string>();

        foreach (Transform child in view.TagsViewportContent.transform)
        {

            if (view.AutoSetTagsToggle.isOn)
            {
                tagStrings.Add(child.name);
            }
            else
            {
                if (child.GetComponent<Toggle>().isOn)
                {
                    tagStrings.Add(child.name);
                }
            }

            if (child.GetComponent<Toggle>().isOn)
            {
                tagStrings.Add(child.name);
            }
        }

        return tagStrings;
    }

    private List<string> CreateSelectedGamesList()
    {
        List<string> gamesStrings = new List<string>();

        foreach (Transform child in view.SetGamesContent.transform)
        {

            if (view.AutoSetGameToggle.isOn)
            {
                gamesStrings.Add(child.name);
            }
            else
            {
                Debug.Log("found " + child.name + " in setGamesContent.transform");
                if (child.GetComponent<Toggle>().isOn)
                {
                    gamesStrings.Add(child.name);
                }
            }
        }
        Debug.Log("Creating selectedGamesList. gamesStrings count = " + gamesStrings.Count);
        return gamesStrings;
    }

    /// <summary>
    /// Creates and returns a list of reward id integers.
    /// </summary>
    /// <returns>A list of ints</returns>
    private List<int> CreateRewardIdsList()
    {
        Dictionary<int, string> dict = RequestRewards();

        List<int> list = new List<int>();

        List<string> rewardStrings = new List<string>();

        foreach (Transform child in view.RewardsViewportContent.transform)
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

        string combindedString = string.Join(", ", rewardStrings.ToArray());
        view.AutoPlaylistEnabledRewardTimeText.text = rewardTime.ToString();
        PlayerPrefs.SetString("AutoplaylistRewardTimeKey", rewardTime.ToString());
        PlayerPrefs.SetInt("AutoplaylistRewardTimeIntKey", rewardTime);
        view.AutoPlaylistEnabledRewardsText.text = combindedString;
        PlayerPrefs.SetString("AutoplaylistRewardsKey", combindedString);
        return list;
    }


    public void CloseSetGamesPanel()
    {
        selectedGamesList = CreateSelectedGamesList();
        StringBuilder sb = new StringBuilder();

        foreach (string str in selectedGamesList)
        {
            int tempint = 0;

            if (selectedGamesList.Count == 1)
            {
                sb.Append(str);
            }
            else
            {
                if (tempint != selectedGamesList.Count)
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

        view.GamesText.text = sb.Replace("_", " ").ToString();
        PlayerPrefs.SetString("CurrentAutoplaylistGames", view.GamesText.text);
        view.ToggleSetGamesPanel(false);
    }

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

        view.TagsText.text = sb.ToString();
        PlayerPrefs.SetString("CurrentAutoplaylistTags", view.TagsText.text);
        view.SetWordTagsPanel.SetActive(false);

    }

    public void SetupPlayAndAddNewLists()
    {
        VerifyData();

        // Populate the play list scrollview and size it properly
        VerticalLayoutGroup verticalLayout = view.PlayListContent.GetComponent<VerticalLayoutGroup>();
        GameObject tempPanel;
        playListTransform = view.PlayListContent.GetComponent<RectTransform>();
        float entryNum = 0;
        yOffSet = verticalLayout.preferredHeight + verticalLayout.spacing;
        List<DO_PlayListObject> playList = GetPlayList();

        if (playList.Count == 0) { view.PlaylistEmptyPanel.SetActive(true); }
        else { view.PlaylistEmptyPanel.SetActive(false); }

        for (int idx = 0; idx < playList.Count; idx++)
        {
            // Instantiate the new panel and set its text accordingly
            tempPanel = GameObject.Instantiate(view.PlayCopyPanel, view.PlayListContent.transform, false);

            tempPanel.transform.Find("TypeText").GetComponent<Text>().text = GetTypeString(playList[idx].type_id).Replace("_", " ");

            if (GetTypeString(playList[idx].type_id) == "Choose_Reward")
            {
                view.playlistHasReward = true;
            }

            tempPanel.transform.Find("DurationText").GetComponent<Text>().text = CreateActivityListString(idx, playList[idx].type_id);

            // Activate the panel and add one to the entry count
            tempPanel.SetActive(true);

            entryNum += 1;
        }

        // Resize the content box on the Y
        playListTransform.sizeDelta = new Vector2(playListTransform.rect.width, entryNum * yOffSet);

        // Clear values and populate the add new list scrollview and size it properly
        verticalLayout = view.AddNewContent.GetComponent<VerticalLayoutGroup>();

        addNewTransform = view.AddNewContent.GetComponent<RectTransform>();
        entryNum = 0;
        yOffSet = verticalLayout.preferredHeight + verticalLayout.spacing;

        foreach (string typeStr in GetTypeStrings())
        {
            // Instantiate the new panel and set its text accordingly
            tempPanel = GameObject.Instantiate(view.ButtonCopyPanel, view.AddNewContent.transform, false);

            tempPanel.transform.Find("TypeText").GetComponent<Text>().text = typeStr.Replace("_", " ");

            if (!CheckType(typeStr))
            {
                tempPanel.transform.gameObject.GetComponent<Button>().interactable = false;
            }

            // Activate the panel and add one to the entry count
            tempPanel.SetActive(true);

            entryNum += 1;
        }

        // Resize the content box on the Y
        addNewTransform.sizeDelta = new Vector2(addNewTransform.rect.width, entryNum * yOffSet);
    }

    public void SetDeleteObjects()
    {
        view.SetDeleteObjectsView();
        playListTransform.sizeDelta = new Vector2(playListTransform.rect.width, playListTransform.rect.height - (yOffSet));
    }
    private bool CheckType(string type)
    {
        switch (type)
        {
            case Constant.SCRAM:

            case Constant.REWARD:

            case Constant.FLASH:

            case Constant.KEYB:

            case Constant.MATCH:

            case Constant.COUNT:

            case Constant.MEMORY:
                return true;

            default:
                return false;
        }
    }
}

