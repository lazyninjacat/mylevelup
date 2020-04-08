using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The controller for the Word Editing domain that services multiple views.
/// </summary>
public class CON_WordEditing : AB_Controller
{
    public MOD_WordEditing model;
    private MAS_WordEditing master;

    public CON_WordEditing(MasterClass newMaster) : base(newMaster)
    {
        master = (MAS_WordEditing)newMaster;
    }

    public bool IsEditSettings { get; set; }

    public override void GetCoworkers(MasterClass master)
    {
        model = (MOD_WordEditing)master.GetModel("MOD_WordEditing");
    }

    public override void SceneChange(string scene) { master.RequestSceneChange(scene); }

    public void SceneChange(string scene, bool isEditScene)
    {
        Debug.Log("CON: in scenechange method setting isEditScene to : " + isEditScene);
        IsEditSettings = isEditScene;
        //SceneChange(scene);
    }

    public override void ClearData()
    {
        model.WordName = null;
        model.CurrentClip = null;
        model.CurrentTexture = null;
        model.CurrentStockTexture = null;
        model.ClearTextureList();
        model.ClearStockTextureList();
    }

    public void DeleteWordImages(List<int> indiceSet, string word) { model.DeleteWordImages(indiceSet, word); }
    //public bool ImageDeleteCleanup(string word) { return model.ImageDeleteCleanup(word); }

    /// <summary>
    /// Saves any changes made to a word entry.
    /// <seealso cref="MOD_WordEditing.EditDbEntry(string)"/>
    /// </summary>
    /// <param name="word"></param>
    /// <returns>bool</returns>
    public bool SaveWordEdits(string word, string wordTags) { return model.EditDbEntry(word, wordTags); }

    /// <summary>
    /// Saves a new word entry to the data base. 
    /// <seealso cref="MOD_WordEditing.CreateNewDbEntry(string)"/>
    /// </summary>
    /// <param name="word"></param>
    /// <returns>bool</returns>
    public bool SaveNewWord(string word, string wordTags) { return model.CreateNewDbEntry(word, wordTags); }

    /// <summary>
    /// Get the currently set texture2D value.
    /// </summary>
    /// <returns>Texture2D</returns>
    public Texture2D GetCurrentTexture() { return model.CurrentTexture; }

    /// <summary>
    /// Returns true if the current texture is set.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsTextureSet() { return (model.CurrentTexture != null); }

    /// <summary>
    /// Sets the current texture to the provided texture2D value.
    /// </summary>
    /// <param name="texture"></param>
    public void SetCurrentTexture(Texture2D texture) { model.CurrentTexture = texture; }

    /// <summary>
    /// Returns the currently set AudioClip.
    /// </summary>
    /// <returns>AudioClip</returns>
    public AudioClip GetCurrentClip() { return model.CurrentClip; }

    /// <summary>
    /// Returns true if the current clip value is set.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsClipSet() { return (model.CurrentClip != null); }

    /// <summary>
    /// Sets current clip to the provided AudioClip value.
    /// </summary>
    /// <param name="clip"></param>
    public void SetCurrentClip(AudioClip clip) { model.CurrentClip = clip; }

    /// <summary>
    /// Saves a downloaded audio file.
    /// </summary>
    /// <param name="audioFile"></param>
    /// <param name="wordName"></param>
    public void SaveDLCAudioClip(byte[] audioFile, string wordName) { model.SaveDLCAudio(audioFile, wordName);  }

    /// <summary>
    /// Sets the target word to the provided value.
    /// </summary>
    /// <param name="word"></param>
    public void SetTargetWord(string word) { model.WordName = word; }

    /// <summary>
    /// Sets the target word tags to the provided value.
    /// </summary>
    /// <param name="wordTags"></param>
    public void SetTargetWordTags(string wordTags) { model.WordTags = wordTags; }

    /// <summary>
    /// Returns the target word as a string.
    /// </summary>
    /// <returns>string</returns>
    public string GetTargetWord() { return model.WordName; }

    /// <summary>
    /// Returns the target word's word tags as a string.
    /// </summary>
    /// <returns>string</returns>
    public string GetTargetWordTags() { return model.WordTags; }

  

    /// <summary>
    /// Returns a Word data object based on the provided key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>Word data object</returns>
    public WordDO GetWordDO(string key) { return model.GetListValue(key); }

    /// <summary>
    /// Deletes a word. Returns true if successful.
    /// <seealso cref="MOD_WordEditing.RemoveWordListEntry(string)"/>
    /// </summary>
    /// <param name="word"></param>
    /// <returns>bool</returns>
    public bool DeleteWord(string word) { return model.RemoveWordListEntry(word); }

    /// <summary>
    /// Returns a copy of the word list dictionary from the model.
    /// </summary>
    /// <returns>Dictionary</returns>
    public Dictionary<string, WordDO> GetListCopy() { return model.GetListCopy(); }

    /// <summary>
    /// Returns true if the word list dictionary has the entry.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>bool</returns>
    public bool DoesDbEntryExist(string key) { return model.CheckForDbEntry(key); }

    /// <summary>
    /// Takes the word as a string and returns true if that word is in use.
    /// </summary>
    /// <param name="word"></param>
    /// <returns>bool</returns>
    public bool WordInUse(string word)
    {
        WordDO temp = model.GetListValue(word);
        return model.inUseWordIds.Contains(temp.IdNum);
    }

    /// <summary>
    /// Takes an int and returns true if the word belonging to that id is in use.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    public bool WordInUse(int id) { return model.inUseWordIds.Contains(id); }

    public void AddNewTexture() { model.AddNewTexture(); }

    public void DeleteInvalidPlayEntries() { model.DeleteInvalidPlayEntries(); }

    public void PopulateInUseSet() { model.PopulateInUseSet(); }

    public void ClearTextureList() { model.ClearTextureList(); }

    public bool RetrievePictures(string word) { return model.RetrieveSavedPics(word); }

    public int SaveTextures(string word) { return model.SaveTextures(word); }

    public List<Texture2D> GetImages() { return model.GetImageList(); }

    public List<Texture2D> GetStockImages() { return model.GetStockImageList(); }

    public bool AreTexturesPresent() { return model.AreTexturesPresent(); } 
}