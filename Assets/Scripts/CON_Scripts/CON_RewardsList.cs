using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This class represents the Controller for the RewardsList domain. It handles any requests to the Master,
/// and all the interactions between the MVC components.
///</summary>
public class CON_RewardsList : AB_Controller {

    private const string CLASSNAME = "CON_RewardsList: ";

    private MAS_RewardsList master;
    private MOD_RewardsList model;

    // Used for determining which mode the VW_RewardEditAdd and VW_reward_website_edit_add is in
    private bool isEditMode;
    public bool IsEditMode {
        get {return isEditMode;}
        set {isEditMode = value;}
    }

    // Used for determining whether reward to be edited is website or custom
    private bool isRewardWebsite;
    public bool IsRewardWebsite
    {
        get { return IsRewardWebsite;}
        set { IsRewardWebsite = value;}
    }

    // Used to store which Reward is to be edited
    private string keyToEdit;
    public string KeyToEdit {
        get {return keyToEdit;}
        set {keyToEdit = value;}
    }

 
    

    // Used to store a reference to the Reward name for access by multiple scenes
    private string newRewardName;
    public string NewRewardName{
        get {return newRewardName;}
        set {newRewardName = value;}
    }

    ///<summary>
    /// Class constructor that creates a reference to its domain master
    ///</summary>
    ///<param name="newMaster"> The master of the domain</param>
    public CON_RewardsList(MasterClass newMaster) : base(newMaster) {
        master = (MAS_RewardsList)newMaster;
    }

    ///<summary>
    /// Fetches the model worker from the Master passed.
    ///</summary>
    ///<param name="newMaster">The master you want to receive the workers from.</param>
    public override void GetCoworkers(MasterClass newMaster) {
        model = (MOD_RewardsList)master.GetModel("MOD_RewardsList");
    }
    
    ///<summary>
    /// This function is used to clear out the data references stored in the controller and
    /// model.
    ///</summary>
    public override void ClearData() {
        isEditMode = false;;
        isRewardWebsite = false;;
        keyToEdit = null;
        newRewardName = null;
    
        //TODO: Null appropriate model data (?) Caution as this gets called in multiple views
        model.CurrentTexture = null;
    }

    ///<summary>
    /// Makes a request to the Master to change the scene.
    ///</summary>
    ///<param name="scene">A string representing the name of the Unity scene you are requesting to change to.</param>
    public override void SceneChange(string scene) { master.RequestSceneChange(scene); }

    ///<summary>
    /// A helper function to set the isEditMode flag appropriately and then send the request to the Master to change the scene.
    ///</summary>
    ///<param name="scene">A string representing the name of the Unity scene you are requesting to change to.</param>
    ///<param name="editFlag">A boolean used to set whether the reward_edit_add scene should be in Edit or Add New mode.</param>
    public void SceneChange(string scene, bool editFlag, bool websiteFlag){
        isEditMode = editFlag;
        isRewardWebsite = websiteFlag;
        //SceneChange(scene);
    }

  

    ///<summary>
    /// Sends a request to the model to get all the Rewards from the Rewards table.
    ///</summary>
    ///<returns>Returns a Dictionary(string, DO_Reward) containing all the table data.</returns>
    public Dictionary<string, DO_Reward> GetRewardsListData(){
        return model.GetRewardsList();
    }

    ///<summary>
    /// Sends a request to the model to get a single Reward based on the Controller's keyToEdit variable.
    ///</summary>
    ///<returns>Returns a DO_Reward object.</returns>
    public DO_Reward RequestRewardToEdit(){
        return model.GetSpecificReward(keyToEdit);
    }

    public string RequestWebRewardUrl()
    {
        return model.GetRewardUrl(keyToEdit);
        
    }

    ///<summary>
    /// Sends a request to the model to check for an image file with the name passed.
    ///</summary>
    ///<param name="fileName">A string representing the image file name</param>
    ///<returns> Returns a bool signalling if it was found or not.</returns>
    public bool CheckForRewardImage(string fileName){
        return model.DoesRewardPhotoExist(fileName);
    }

    ///<summary>
    /// Sends a request to the model to check if a Reward exists in the Rewards database table.
    ///</summary>
    ///<param name="rewardName">A string representing the reward to search for.</param>
    ///<returns>Returns a bool signalling if it was found or not.</returns>
    public bool CheckForExistingReward(string rewardName){
        return model.DoesRewardAlreadyExist(rewardName);
    }


    ///<summary>
    /// Sends a request to the model to check if a Reward is a website type in the Rewards database table.
    ///</summary>
    ///<param name="rewardName">A string representing the reward to search for.</param>
    ///<returns>Returns a bool signalling if it is a website reward or not.</returns>
    public bool CheckIsWebsiteReward(string rewardName)
    {
        return model.IsRewardWebsite(rewardName);
    }

    ///<summary>
    /// Sets the photo passed to the model's CurrentTexture.
    ///</summary>
    ///<param name="photo">A Texture2D object representing the photo passed.</param>
    public void SetCurrentTexture(Texture2D photo){
        model.CurrentTexture = photo;
    }

    ///<summary>
    /// Clears the model's CurrentTexture by setting it to null.
    ///</summary>
    public void ClearCurrentTexture(){
        model.CurrentTexture = null;
    }

    ///<summary>
    /// Checks if the model's CurrentTexture is null or if it contains a photo.
    ///</summary>
    ///<returns>Returns a bool signalling whether a current photo is set or not.</returns>
    public bool IsThereACurrentPhoto(){
        if (model.CurrentTexture == null) {
            return false;
        }
        else{
            return true;
        }
    }

    ///<summary>
    /// Sends a request to the model to save the photo with the file name passed.
    ///</summary>
    ///<param name="fileName">A string representing the file name you want to save the photo under.</param>
    ///<returns>Returns a bool signalling whether it was successful or not.</returns>
    public bool RequestToSavePhoto(string fileName){
        return model.SavePhoto(model.CurrentTexture, fileName);
    }



    ///<summary>
    /// Sends a request to the model to save the URL with the URL passed.
    ///</summary>
    ///<param name="url">A string representing the URL you want to save to the reward.</param>
    ///<returns>Returns a bool signalling whether it was successful or not.</returns>
    public bool RequestToSaveUrl(string rewardName, string url)
    {
        return model.SaveUrl(rewardName, url);
    }


    ///<summary>
    /// Sends a request to the model to delete the photo saved under the file name passed.
    ///</summary>
    ///<param name="fileName">A string representing the file name of the photo you want to delete.</param>
    public void RequestToDeletePhoto(string fileName){
        model.DeletePhoto(fileName);
    }

    ///<summary>
    /// Sends a request to the model to save a Reward based on the name, type, and url passed to the database.
    ///</summary>
    ///<param name="rewardName">A string representing the name of the Reward you wish to save to the DB.</param>
    ///<param name="rewardType">A string representing the name of the Reward you wish to save to the DB.</param>
    ///<param name="rewardUrl">A string representing the name of the Reward you wish to save to the DB.</param>
    ///<returns>Returns a bool signalling whether it was successful saving to the DB or not.</returns>
    public bool RequestToSaveReward(string rewardName, string rewardType, string rewardUrl){
        return model.SaveReward(rewardName, rewardType, rewardUrl);
    }

    ///<summary>
    /// Sends a request to the model to delete the Reward from the DB with the name passed.
    ///</summary>
    ///<param name="rewardName">A string representing the name of the Reward you wish to delete.</param>
    ///<returns>Returns a bool signalling whether deletion from the DB was successful or not.</returns>
    public bool RequestToDeleteReward(string rewardName){
        return model.DeleteReward(rewardName);
    }
}
