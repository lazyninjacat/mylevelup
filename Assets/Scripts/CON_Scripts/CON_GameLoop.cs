using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CON_GameLoop : AB_Controller
{
    private MAS_GameLoop master;
    private MOD_GameLoop model;

    public CON_GameLoop(MasterClass newMaster) : base(newMaster) { master = (MAS_GameLoop)newMaster; }
    public override void ClearData() {}
    public override void GetCoworkers(MasterClass newMaster) { model = (MOD_GameLoop)master.GetModel("MOD_GameLoop"); }
    public override void SceneChange(string scene) { master.RequestSceneChange(scene); }

    // TODO: REPLACE THIS BULLSHIT
    public bool PinGood(int pin) { return model.CheckPin(pin) == 1 ? true : false; }
    public bool PinsExist() { return model.AdminsRowNumber() > 0 ? true : false; }
    /////////////////////////////////////////////////////////////////////

    public byte[] LoadRandomWordPic(string word) { return model.LoadRandomWordPic(word); }

    /// <summary>
    /// Returns the play list data object corresponding to the index value.
    /// <seealso cref="MOD_GameLoop"/>
    /// </summary>
    /// <param name="idx"></param>
    /// <returns>Play List data object</returns>
    public DO_PlayListObject GetPlayEntry(int idx) { return model.GetPlayEntryById(idx); }

    /// <summary>
    /// Returns the total round count from the model.
    /// <seealso cref="MOD_GameLoop"/>
    /// </summary>
    /// <returns>float</returns>
    public float GetRoundCount() { return model.GetTotalRoundCount(); }

    /// <summary>
    /// Returns a word corresponding to the int id from the model.
    /// <seealso cref="MOD_GameLoop"/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns>string</returns>
    public string GetWordById(int id) { return model.GetWordById(id); }

    /// <summary>
    /// Returns the bool result from the model.
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <returns>bool</returns>
    public bool PlayListCompleted(int currentIndex) { return model.EndOfPlay(currentIndex); }

    /// <summary>
    /// Returns the number of iterations from the model.
    /// </summary>
    /// <returns>int</returns>
    public int GetIterationNumber() { return model.GetIterationCount(); }

    /// <summary>
    /// Returns true if the pass lock is on in the config settings.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsPassLocked() { return model.IsPassLocked(); }

    ///<summary>
    /// Calls the model to return the contents of the VideoHistory table
    ///</summary>
    ///<returns>A sorted Dictionary of a video id with video data</returns>
    public Dictionary<string, DO_Video> GetVideoHistory(){return model.GetVideoHistory();}

    ///<summary>
    /// Calls the model to return the number of rows passed starting from the row passed from the VideoHistory table
    ///</summary>
    ///<param name="numOfRows">An int representing the number of rows you want returned</param>
    ///<param name="pageStart">An int representing the row you wish to start the query on. If this value is greater than the number of rows in the table, it will returning nothing.</param>
    ///<returns>A Dictionary containing the video id's and video data for the rows returned</returns>
    public Dictionary<string, DO_Video> RequestPageOfVideoHistory(int numOfRows, int pageStart){
        return model.GetPageOfHistory(numOfRows, pageStart);
    }

    ///<summary>
    /// Makes a call to the model to get the count of rows in VideoHistory
    ///</summary>
    ///<returns>An int representing the number of rows in the table</returns>
    public int RequestVideoHistoryCount(){
        return model.GetVideoHistoryCount();
    }

    ///<summary>
    /// Calls the model to save a video to the VideoHistory table
    ///</summary>
    ///<returns>An int representing how many rows were updated/inserted</returns>
    public int SaveVideoToVideoHistory(string vid, DO_Video vData){
        int result = model.SaveVideoToVideoHistory(vid, vData);
        return result;
    }

    public void ReportRoundErrorCount(string gameType, string word, int totalErrors)
    {
        model.ReportRoundErrors(gameType, word, totalErrors);
        
    }

    public void ReportWebRewardData(string url, DateTime startTime, DateTime endTime)
    {
        model.ReportWebRewardData(url, startTime, endTime);
    }


}
