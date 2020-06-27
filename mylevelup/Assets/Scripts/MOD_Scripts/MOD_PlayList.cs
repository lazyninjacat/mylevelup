using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// Model for the Play List domain. In charge of querying the DB, holding and mutating data, and providing 
/// that data to views that need it.
/// </summary>
public class MOD_PlayList : AB_Model
{
    private DataService dataService;
    private CON_PlayList controller;
    private List<DO_PlayListObject> playListEntries;
    private HashSet<DO_PlayListObject> entryCheckSet;
    private HashSet<int> wordIdCheckSet;
    public Dictionary<int, string> wordDict;
    public Dictionary<int, string> rewardDict;
    public Dictionary<int, string> tagsDict;
    public List<string> wordTagsList;
    public int loopIterations { get; set; }
    public bool infiniteLoops { get; set; }
    public bool passLocked { get; set; }
    public bool creatingNewEntry { get; set; }
    public int activeEntryIndex { get; set; }

    // IMPORTANT: when creating a new activity type make sure to move it into the enum at the end and insert new cases throughout codebase where necessary
    enum TypeIds { Word_Scramble, Choose_Reward, Flash_Card, Counting_Game, Keyboard_Game, Memory_Cards, Matching_Game, Multiple_Choice, Question_and_Answer, Face_Scramble, Letter_Trace, Sorting, Face_Match, Random };

    public MOD_PlayList(MasterClass newMaster) : base(newMaster)
    {
        dataService = StartupScript.ds;

        playListEntries = new List<DO_PlayListObject>();

        entryCheckSet = new HashSet<DO_PlayListObject>();

        wordIdCheckSet = new HashSet<int>();

        RetrieveAndSort();

        PopulateWordList();

        PopulateWordTagList();

        PopulateTagsDictionary();

        GetGameLoopSettings(0);
    }

    public override void GetCoworkers(MasterClass master)
    {
        controller = (CON_PlayList)master.GetController("CON_PlayList");
    }

    public List<DO_PlayListObject> GetPlayListObjects() { return playListEntries; }

    public int GetDurationValue(int idx) { return playListEntries[idx].duration; }

    public string GetJsonData(int idx) { return playListEntries[idx].json; }

    public string GetCustomJson(int idx) { return playListEntries[idx].custom_json; }

    public string[] GetTypeStrings() { return Enum.GetNames(typeof(TypeIds)); }

    public IEnumerable<Words> GetWordsTable() { return dataService.GetWordsTable(); }

    public int GetTypeId(string typeStr) { return (int)Enum.Parse(typeof(TypeIds), typeStr.Replace(" ", "_")); }

    public string GetTypeString(int typeId) { return Enum.GetName(typeof(TypeIds), typeId); }

    /// <summary>
    /// Resets variables related to the scene.
    /// </summary>
    public void ResetSceneData()
    {
        activeEntryIndex = -1;

        creatingNewEntry = false;
    }

    /// <summary>
    /// This method either creates a new entry or edits an existing one. It creates
    /// a play list data object, updates the DB, and then inserts the new data
    /// object into the play list entries list. Returns true if saved successfully to
    /// the DB.
    /// </summary>
    /// <param name="typeId"></param>
    /// <param name="duration"></param>
    /// <param name="json"></param>
    /// <param name="customJson"></param>
    /// <returns>bool</returns>
    public bool AddOrEditEntryData(int typeId, int duration, string json, string customJson = null)
    {
        DO_PlayListObject temp;

        // Check if we are creating a new entry
        if (creatingNewEntry)
        {
            temp = new DO_PlayListObject(playListEntries.Count, duration, typeId, json);

            if (dataService.AddNewPlayListRow(temp) < 1)
            {
                return false;
            }

            IEnumerable<DO_PlayListObject> tempCollection = dataService.GetLastInsertedRowId(temp.order_id);

            int newRowId = -1;

            foreach (DO_PlayListObject obj in tempCollection)
            {
                newRowId = obj.id;
            }

            temp.id = newRowId;

            playListEntries.Add(temp);
        }
        else
        {
            temp = new DO_PlayListObject(playListEntries[activeEntryIndex].id, playListEntries[activeEntryIndex].order_id, duration, typeId, json);

            if (dataService.EditPlayListRow(temp) < 1)
            {
                return false;
            }

            playListEntries[activeEntryIndex] = temp;
        }

        return true;
    }

    public bool DeletePlaylist()
    {

        if (dataService.DeleteAllPlaylist() > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes an entry from the play list entry list and deletes the corresponding entry from the data base.
    /// Returns false if deletion from the data base is not successful.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns>bool</returns>
    public bool RemoveEntryData(int idx)
    {
        bool pass = false;

        if (dataService.DeleteFromPlayList(playListEntries[idx].id) > 0)
        {
            if (FixOrderIds(idx))
            {             
                // TODO: catch out of bounds exception
                playListEntries.RemoveAt(idx);
                pass = true;
            }
        }

        if (!pass)
        {
            // TODO: Toss an error
            return false;
        }

        return true;
    }         

    /// <summary>
    /// Sets the config data for the game loop. Takes in a number of ints and bools.
    /// Returns true if the data is saved successfully to the data base.
    /// </summary>
    /// <param name="playId"></param>
    /// <param name="infinite"></param>
    /// <param name="iterations"></param>
    /// <param name="locked"></param>
    /// <returns>bool</returns>
    public bool SetLoopData(int playId, bool infinite, int iterations, bool locked)
    {
        int rowNum = 0;
        loopIterations = iterations;
        passLocked = locked;

        rowNum = dataService.UpdateConfingData(playId, infinite == true ? 1 : 0, iterations, locked == true ? 1 : 0);

        if (rowNum > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Swaps the order id values of play list entries in the database and the playlist entry list.
    /// Returns true if the changes took hold in the database.
    /// </summary>
    /// <param name="idxA"></param>
    /// <param name="idxB"></param>
    /// <returns>bool</returns>
    public bool SwapEntryData(int idxA, int idxB)
    {
        if (dataService.ChangeOrderIdValue(playListEntries[idxA].id, idxB) > 0 && dataService.ChangeOrderIdValue(playListEntries[idxB].id, idxA) > 0)
        {
            var temp = playListEntries[idxA];

            playListEntries[idxA] = playListEntries[idxB];

            playListEntries[idxB] = temp;
        }
        else
        {
            //TODO: inform user of db query error
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if any changes have been made to the data structs in use by this model.
    /// </summary>
    public void VerifyData()
    {
        int tempCheck;

        // Verify Play List first
        foreach (DO_PlayListObject entry in playListEntries)
        {
            tempCheck = dataService.DoesPlayEntryExist(entry.id);

            // Check if play list entry is NOT present in database
            if (tempCheck <= 0)
            {               
                entryCheckSet.Add(entry);
            }
            else
            {
                Debug.Log("VERIFY: tempCheck CLEARED!");
            }
        }

        // Check if any of the play list objects were flagged as dead
        if (entryCheckSet.Count > 0)
        {
            // Remove all dead play list objects from the play entry list
            foreach (DO_PlayListObject entry in entryCheckSet)
            {               
                playListEntries.Remove(entry);
            }

            // Clear the entry check set for later re-use
            entryCheckSet.Clear();
        }

        int lastId = -1;

        lastId = dataService.GetLastIdInWords();

        // Check if a new word id has been added to the word dict
        if (lastId == -1)
        {
            Debug.Log("VERIFY: LAST ID WAS -1!");
            // TODO: Log an ERROR
        }
        else if (!wordDict.ContainsKey(lastId))
        {          
            // Re-populate the wordDict if a new id is found
            PopulateWordList();

            PopulateWordTagList();

            PopulateTagsDictionary();
        }
        else
        {
            // Verify word list if no new id is found to check for removed words
            foreach (var entry in wordDict)
            {
                tempCheck = dataService.DoesWordIdExist(entry.Key);

                // Check if word is NOT present in data base
                if (tempCheck <= 0)
                {
                    wordIdCheckSet.Add(entry.Key);
                }
            }

            // Check if any of the word ids were flagged as dead
            if (wordIdCheckSet.Count > 0)
            {
                // Remove all dead ids from the word id/word dictionary
                foreach (int id in wordIdCheckSet)
                {
                    wordDict.Remove(id);
                }

                // Clear the word id check set for later use
                wordIdCheckSet.Clear();
            }
        }
    }

    public int GetTotalWords()
    {
        List<int> wordsTotalList = new List<int>();

        foreach (var row in GetWordsTable())
        {
            wordsTotalList.Add(row.word_id);
        }

        return wordsTotalList.Count;
    }

    /// <summary>
    /// Creates and populates the word id/word Dictionary.
    /// </summary>
    private void PopulateWordList()
    {
        if (wordDict == null)
        {
            wordDict = new Dictionary<int, string>();
        }
        else if (wordDict.Count > 0)
        {
            wordDict.Clear();
        }

        GetWordsTable();

        foreach (var row in GetWordsTable())
        {
            wordDict.Add(row.word_id, row.word_name);
        }
    }

    /// <summary>
    /// Creates and populates the word tag list.
    /// </summary>
    private void PopulateWordTagList()
    {
        if (wordTagsList == null)
        {
            wordTagsList = new List<string>();
        }
        else if (wordTagsList.Count > 0)
        {
            wordTagsList.Clear();
        }

        GetWordsTable();

        List<string> tempWordTagsList = new List<string>();


        foreach (var row in GetWordsTable())
        {
            tempWordTagsList = row.word_tags.Split(',').ToList();

            foreach (string tag in tempWordTagsList)
            {
                if (!(wordTagsList.Contains(tag) || wordTagsList.Contains(" " + tag)))
                {
                    wordTagsList.Add(tag);
                }
            }

            tempWordTagsList.Clear();

            tempWordTagsList = null;            
        }     
    }

    private void PopulateTagsDictionary()
    {
        if (tagsDict == null)
        {
            tagsDict = new Dictionary<int, string>();
        }
        else if (tagsDict.Count > 0)
        {
            tagsDict.Clear();
        }

        GetWordsTable();

        foreach (var row in GetWordsTable())
        {
            Debug.Log("Adding " + row.word_name + " to tagsDict");
            tagsDict.Add(row.word_id, row.word_tags);
        }
    }

    private void PopulateRewardList()
    {
        if (rewardDict == null)
        {
            rewardDict = new Dictionary<int, string>();
        }
        else if (rewardDict.Count > 0)
        {
            rewardDict.Clear();
        }

        foreach (var row in GetRewardsTable())
        {
            rewardDict.Add(row.reward_id, row.reward_name);
        }
    }

    /// <summary>
    /// Retrieves the play list setting data from the database. 
    /// </summary>
    /// <param name="playListId"></param>
    private void GetGameLoopSettings(int playListId)
    {
        foreach (var row in dataService.GetConfigByPlayListId(playListId))
        {
            loopIterations = row.iteration_number;

            infiniteLoops = row.infinite_loop == 0 ? false : true;

            passLocked = row.pass_locked == 0 ? false : true;
        }
    }

    /// <summary>
    /// This function is called each time an entry is deleted from the playlist. 
    /// Using the order ids for each entry it shuffles them into their proper order.
    /// </summary>
    private bool FixOrderIds(int idx)
    {
        int startIdx = idx + 1;

        bool pass = true;

        int failIdx = 0;

        for (; startIdx < playListEntries.Count; startIdx++)
        {           
            // Change the order ID of the target playlist entry by -1
            if (dataService.ChangeOrderIdValue(playListEntries[startIdx].id, playListEntries[startIdx].order_id - 1) > 0)
            {
                playListEntries[startIdx].order_id = playListEntries[startIdx].order_id - 1;
            }
            else
            {
                //TODO: Log error

                // Change the failIdx value to match the index value of the failed entry
                failIdx = startIdx;
                
                // Flip the pass flag to false 
                pass = false;
                
                // Exit the loop early
                break;
            }
        }

        // If any one call to the data service failed then restore all playlist entry's order_id value to previous values up to the failIdx entry
        if (!pass)
        {
            for (int x = idx + 1; x < failIdx; x++)
            {
                playListEntries[startIdx].order_id = playListEntries[startIdx].order_id + 1;
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Retrieves the play list data from the data base and sorts it according to the order id.
    /// </summary>
    private void RetrieveAndSort()
    {
        DO_PlayListObject temp;

        foreach (var row in dataService.GetPlayList())
        {
            temp = new DO_PlayListObject(
                row.id,
                row.order_id,
                row.duration,
                row.type_id,
                row.json,
                row.custom_json);

            playListEntries.Add(temp);
        }

        playListEntries.Sort();
    }

    public IEnumerable<Rewards> GetRewardsTable() { return dataService.GetRewardsTable(); }
}