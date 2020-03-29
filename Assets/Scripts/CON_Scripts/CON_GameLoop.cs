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


 

    public void ReportRoundErrorCount(string gameType, string word, int totalErrors)
    {
        model.ReportRoundErrors(gameType, word, totalErrors);
        
    }

    public void ReportWebRewardData(string url, DateTime startTime, DateTime endTime)
    {
        model.ReportWebRewardData(url, startTime, endTime);
    }


}
