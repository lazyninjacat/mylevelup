using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This is the model for the Game Loop domain that drives the primary play experience of the 
/// child user. 
/// </summary>
/// <remark>
/// Domain: Game Loop
/// Data: Play list data, word list data, reward data, config data, video history data
/// </remark>
public class MOD_GameLoop : AB_Model
{
    private DataService dataService;
    private List<DO_PlayListObject> playListData;
    private Dictionary<int, string> wordList;
    private int entryMax;
    private int durationMax { get; set; }
    private int loopIterations;
    private bool infiniteLoop;
    private bool passLocked;
    public float totalRoundCount = 0;
    private const int REWARD_ID = 1;

    /// <summary>
    /// This is the constructor for the game loop model.
    /// </summary>
    /// <param name="newMaster"></param>
    public MOD_GameLoop(MasterClass newMaster) : base(newMaster)
    {
        dataService = StartupScript.ds;

        GrabListData();

        entryMax = playListData.Count;

        GetGameLoopSettings(0);

        TotalRoundCount();
    }

    public override void GetCoworkers(MasterClass master) { }

    /// <summary>
    /// Takes a file name (a word) and recovers randomly selected byte array belonging to that word.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>byte array</returns>
    public byte[] LoadRandomWordPic(string fileName) { return FileAccessUtil.LoadRandomWordPic(fileName); }

    /// <summary>
    /// Retrieves a word string corresponding to the id parameter.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>string</returns>
    public string GetWordById(int id) { return wordList[id]; }
    
    /// <summary>
    /// Returns a data object that corresponds to the order id.
    /// <seealso cref="DO_PlayListObject"/>
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns>Play List data object</returns>
    public DO_PlayListObject GetPlayEntryById(int orderId) { return playListData[orderId]; }

    /// <summary>
    /// Returns true if the passed value is >= to the max number of play list entries.
    /// </summary>
    /// <param name="currentEntryValue"></param>
    /// <returns>boolean</returns>    
    public bool EndOfPlay(int currentEntryValue)
    {
        return currentEntryValue >= entryMax;
    }

    /// <summary>
    /// Returns the sum of all the play list entry duration values.
    /// </summary>
    /// <returns>float</returns>
    public float GetTotalRoundCount() { return totalRoundCount; }

    /// <summary>
    /// Returns the number of iterations that were set in the config settings. 
    /// </summary>
    /// <returns>int</returns>
    public int GetIterationCount() { return loopIterations; }

    /// <summary>
    /// Returns true if the pass locked option is true.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsPassLocked() { return passLocked; }

    /// <summary>
    /// Grabs all play list and word + word id data from the data service 
    /// and populates the list and dictionary.
    /// </summary>
    private void GrabListData()
    {
        // Instantiate the play list, populate it and sort
        playListData = new List<DO_PlayListObject>();

        playListData = dataService.GetPlayList().ToList();

        playListData.Sort();

        // Instantiate words list and populate it
        wordList = new Dictionary<int, string>(); 

        foreach (var pair in dataService.GetWordsAndIdsOnly())
        {
            wordList.Add(pair.word_id, pair.word_name);
        }
    }

    /// <summary>
    /// Sets the play list configuration settings based on play list id. 
    /// </summary>
    /// <param name="playListId"></param>
    private void GetGameLoopSettings(int playListId)
    {
        foreach (var row in dataService.GetConfigByPlayListId(playListId))
        {
            loopIterations = row.iteration_number;

            infiniteLoop = row.infinite_loop == 0 ? false : true;

            passLocked = row.pass_locked == 0 ? false : true;
        }
    }

    /// <summary>
    /// Calculates the sum of all the duration values for each play list entry
    /// with the exception of reward entries. 
    /// </summary>
    private void TotalRoundCount()
    {
        foreach (DO_PlayListObject entry in playListData)
        {
            if (entry.type_id != REWARD_ID)
            {
                totalRoundCount = totalRoundCount + entry.duration;
            }
        }
    }    

    public void ReportRoundErrors(string gameType, string word, int totalErrors)
    {
        dataService.RecordRoundData(gameType, word, totalErrors);
    }

    public void ReportWebRewardData(string url, DateTime startTime, DateTime endTime)
    {
        dataService.RecordWebRewardData(url, startTime, endTime);
    }

}