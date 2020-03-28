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
    /// The constructor for the game loop model.
    /// </summary>
    /// <param name="newMaster"></param>
    public MOD_GameLoop(MasterClass newMaster) : base(newMaster)
    {
        dataService = StartupScript.ds;
        GrabListData();
        entryMax = playListData.Count;
        Debug.Log("MOD: Play list data count is " + entryMax.ToString());
        GetGameLoopSettings(0);
        TotalRoundCount();
    }

    public override void GetCoworkers(MasterClass master) { }

    // TODO: REPLACE THIS BULLSHIT
    public int CheckPin(int pin) { return dataService.DoesPinExist(pin); }
    public int AdminsRowNumber() { return dataService.NumberOfPins(); }
    /////////////////////////////////////////////////////////////////////

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
        Debug.Log("Current Entry Value = " + currentEntryValue);
        Debug.Log("Entry max = " + entryMax);
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

    // Incomplete methods
    public AudioClip GetClipByName(string word) { return null; }
    public Texture GetTextureByName(string word) { return null; }
    // Incomplete methods

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
        Debug.Log("******************\n MOD: In TotalRoundCount \n**********************");

        foreach (DO_PlayListObject entry in playListData)
        {
            Debug.Log("******************\n MOD: Entry has type id of: " + entry.type_id.ToString() + "\n**********************");

            if (entry.type_id != REWARD_ID)
            {
                Debug.Log("******************\n MOD: Entry Id was not REWARD_ID \n**********************");
                totalRoundCount = totalRoundCount + entry.duration;
                Debug.Log("******************\n MOD: Total round count is now: " + totalRoundCount.ToString() + "\n**********************");
            }
        }

        Debug.Log("******************\n MOD: Final round count is: " + totalRoundCount.ToString() + "\n**********************");
    }

    ///<summary>
    /// Gets a count of the rows in the VideoHistoryTable
    ///</summary>
    public int GetVideoHistoryCount(){
        return dataService.GetVideoHistoryCount();
    }

    ///<summary>
    /// Retrieves the data from the VideoHistory table, creates and sorts a Dictionary based on
    /// the watch count of the videos.
    ///</summary>
    ///<returns> A Dictionary with video id as a key and the details and stats of that video in a DO_Video object as the value</returns>
    public Dictionary<string, DO_Video> GetVideoHistory(){
        Dictionary<string, DO_Video> videos = new Dictionary<string, DO_Video>();
        DO_Video video;
        IEnumerable<VideoHistory> data = dataService.GetVideoHistories();
        foreach (var row in data){
            //Debug.Log(row.video_data);
            video = JsonUtility.FromJson<DO_Video>(row.video_data);
            //Debug.Log(video);
            videos.Add(row.videoId, video);
        }

        var sortedByWatchCount = videos.OrderByDescending(pair => pair.Value.watchCount).ToDictionary(pair => pair.Key, pair => pair.Value);
                           
        return sortedByWatchCount;
    }

    ///<summary>
    /// Saves a video id and it's associated data to the DB.true If the video exists as an entry in the DB it will perform an update.
    /// If It doesn;t exist in the DB, it will perform an insert.
    ///</summary>
    ///<param name="vid">A string representing the Youtube video id of the video</param>
    ///<param name="vData">A DO_Video object containing the data for the video</param>
    ///<returns>An int signalling the result of the transaction.true Returns 1 of successful, 0 if unsuccessful.</returns>
    public int SaveVideoToVideoHistory(string vid, DO_Video vData){
        int result = dataService.CheckIfVideoHistoryExists(vid);
        string videoData = JsonUtility.ToJson(vData, false);
        int dbOperationResult = 0;
        if (result > 0){//exists
            Debug.Log("Video Exists....performing an UPDATE");
            dbOperationResult = dataService.SaveToVideoHistory(vid, videoData, true);
        }
        else{
            Debug.Log("Video doesn't exist.....performing an INSERT");
            dbOperationResult = dataService.SaveToVideoHistory(vid, videoData, false);
        }
        return dbOperationResult;
    }

    ///<summary>
    /// Queries the DB for a set of rows from the VideoHistory table based on the parameters passed.
    ///</summary>
    ///<param name="numOfRows">The number of rows you want returned</param>
    ///<param name="pageStart">The row you wish the query to start on</param>
    ///<returns>A Dictionary (string, DO_Video) containing the result of the query.</returns>
    public Dictionary<string, DO_Video> GetPageOfHistory(int numOfRows, int pageStart ){
        Dictionary<string, DO_Video> videos = new Dictionary<string, DO_Video>();
        DO_Video video;
        IEnumerable<VideoHistory> data = dataService.GetPageOfVideoHistory(numOfRows, pageStart);
        foreach (var row in data){
            //Debug.Log(row.video_data);
            video = JsonUtility.FromJson<DO_Video>(row.video_data);
            //Debug.Log(video);
            videos.Add(row.videoId, video);
        }

        return videos;
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