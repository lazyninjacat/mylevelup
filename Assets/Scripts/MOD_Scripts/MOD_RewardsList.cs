using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

///<summary>
/// This class represents the Model for the RewardsList domain. It handles receiving requests from the Controller,
/// sending and receiving data to the DataService class, sending result signals back to the Controller, and sending data to the Controller.
///</summary>
public class MOD_RewardsList : AB_Model {

    private const string CLASSNAME = "MOD_RewardsList: ";

    private DataService dataService;

    private Dictionary<string, DO_Reward> rewardList;

    private Texture2D currentTexture;
    public Texture2D CurrentTexture{
        get{return currentTexture;}
        set{currentTexture = value;}
    }

    ///<summary>
    /// Constructor that creates a model worker, a reference to the DataService class, and initiates the CurrentTexture.
    ///</summary>
    ///<param name="newMaster">The master that this worker belongs to</param>
    public MOD_RewardsList(MasterClass newMaster) : base(newMaster) {
        Debug.Log(CLASSNAME + "Creating new model");
        dataService = StartupScript.ds;
        currentTexture = null;
    }

    ///<summary>
    /// Fetches the controller worker from the Master passed.
    ///</summary>
    ///<param name="newMaster">The master you want to receive the workers from.</param>
    ///<remarks>Currently this has not been implemented as this model does not need a reference to it's controller.</remarks>
    public override void GetCoworkers(MasterClass master) {
        Debug.Log(CLASSNAME + "TODO: implement GetCoworkers if needed.");
    }

    ///<summary>
    /// Retrieves the Rewards table data from the DataService.
    ///</summary>
    ///<returns>Returns a Dictionary(string, DO_Reward) containing the table's data.</returns>
    public Dictionary<string, DO_Reward> GetRewardsList(){
        Debug.Log(CLASSNAME + "Retrieving all rewards");
        Dictionary<string, DO_Reward> rewardList = new Dictionary<string, DO_Reward>();
        IEnumerable<Rewards> rewards = dataService.GetRewardsTable();
        DO_Reward rewardEntry;

        foreach (var row in rewards){
            //Debug.Log(CLASSNAME + "******* Reward: " + row.reward_name);
            rewardEntry = new DO_Reward(
                row.reward_id,
                row.reward_name,
                row.reward_type,
                row.reward_url

            );

            rewardList.Add(row.reward_name, rewardEntry);
        }
        return rewardList;
    }

    ///<summary>
    /// Retrieves a single reward from the Rewards table via the DataService.
    ///</summary>
    ///<param name="rewardName">A string representing the Reward you are requesting</param>
    ///<returns>Returns a DO_Reward object containing the data of that reward.</returns>
    public DO_Reward GetSpecificReward(string rewardName){
        IEnumerable<Rewards> reward = dataService.SearchForReward(rewardName);
        DO_Reward result = null;
        foreach (var row in reward){
            result = new DO_Reward(row.reward_id, row.reward_name, row.reward_type, row.reward_url);
        }
        return result;
    }

  

    ///<summary>
    /// Checks to see if the Reward exists in the Rewards table via the DataService.
    ///</summary>
    ///<param name="rewardName">A string representing the Reward you wish to check for.</param>
    ///<returns>Returns a bool signalling whether the reward exists or not.</returns>
    public bool DoesRewardAlreadyExist(string rewardName)
    {
        IEnumerable<Rewards> result = dataService.SearchForReward(rewardName);
        int count = 0;
        foreach (var row in result){
            count++;
        }
        if (count > 0){
            return true;
        }
        return false;
    }

    public bool IsRewardWebsite(string rewardName)
    {
        IEnumerable<Rewards> result = dataService.FindRewardType(rewardName);
        int count = 0;
        foreach (var row in result)
        {
            if(row.reward_type == "website")
            {
                count++;
            }
            Debug.Log("In IsRewardWebsite, Reward Type is: " + row.reward_type);

        }
        if (count > 0)
        {
            return true;
        }
        return false;
    }

    public string GetRewardUrl(string rewardName)
    {
        IEnumerable<Rewards> result = dataService.FindRewardUrl(rewardName);
        foreach (var row in result)
        {
            if (row.reward_url != null)
            {
                return row.reward_url;
            }
            else
            {
                Debug.Log("No Url is currently set for " + row.reward_name);
                return "";
            }
        }
        return "";
    }

    ///<summary>
    /// Calls on the FileAccessUtil to check whether a photo exists with the file name passed.
    ///</summary>
    ///<param name="fileName">A string representing the file name you wish to check for.</param>
    ///<returns>Returns a bool signalling whether the photo exists or not.</returns>
    public bool DoesRewardPhotoExist(string fileName){
        return FileAccessUtil.DoesRewardPicExist(fileName);
    }

   
    ///<summary>
    /// Attempts to save a new reward to the Rewards table with the reward name passed.
    ///</summary>
    ///<param name="rewardName">A string representing the reward name you wish to save.</param>
    ///<returns>Returns a bool signalling whether saving the Reward was successful or not.</returns>
    public bool SaveReward(string rewardName, string reward_type, string reward_url){
        int result = dataService.AddReward(rewardName, reward_type, reward_url);
        if (result > 0){
            return true;
        }
        return false;
    }

    ///<summary>
    /// Calls on the FileAccessUtil to save a photo with the file name and Texture2D object passed.
    ///</summary>
    ///<param name="photo">A Texture2D object representing the photo you are wishing to save.</param>
    ///<param name="fileName">A string representing the file name you wish to save the photo under.</param>
    ///<returns>Returns a bool signalling whether the photo was successfully saved or not.</returns>
    public bool SavePhoto(Texture2D photo, string fileName){
        return FileAccessUtil.SaveRewardPic(photo, fileName);
    }

    ///<summary>
    /// Calls on the DataService to pdate a reward in the Rewards table and save a URL with the reward name and URL string passed.
    ///</summary>
    ///<param name="rewardName">A string representing the reward you are wishing to save.</param>
    ///<param name="url">A string representing the url you wish to save.</param>
    ///<returns>Returns a bool signalling whether the url was successfully saved or not.</returns>
    public bool SaveUrl(string rewardName, string url)
    {
        int result = dataService.InsertUrlIntoRewards(rewardName, url);
        if (result > 0)
        {
            return true;
        }
        return false;
    }


    ///<summary>
    /// Attempts to delete a reward from the Rewards table with the reward name passed.
    ///</summary>
    ///<param name="rewardName">A string representing the reward you are wishing to delete.</param>
    ///<returns>Returns a bool signalling whether deletion of the reward was successful or not.</returns>
    public bool DeleteReward(string rewardName){
        int result = dataService.DeleteReward(rewardName);
        if (result > 0){
            return true;
        }
        return false;
    }

    ///<summary>
    /// Calls on the FileAccessUtil to delete a reward picture with the file name passed.
    ///</summary>
    ///<param name="fileName">A string representing the file name of the photo you wish to delete.</param>
    public void DeletePhoto(string fileName){
        FileAccessUtil.DeleteRewardPic(fileName);
    }
}
