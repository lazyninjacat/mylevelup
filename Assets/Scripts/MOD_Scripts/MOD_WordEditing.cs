using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using UnityEditor;
using System;

/// <summary>
/// The model for the Word Editing domain.
/// </summary>
public class MOD_WordEditing : AB_Model
{
    private const int SAVED_PIC_XY = 95;

    // Create the regex pattern to isolate <filename><intsuffix>.png strings
    private const string PNG_PATTERN = @"([\w|W]*.png)";

    private Dictionary<string, WordDO> wordList;
    private DataService dataService;
    private int lastPlayEntryId;
    private List<Texture2D> textureList;
    private List<Texture2D> stockTextureList;
    private List<string> wordImageNames;
    private List<string> stockCheckerList;
    private List<string> toDelete;
    private List<Texture2D> imagesForDelete;


    // Create getters and setters
    public string WordName { get; set; }
    public string WordTags { get; set; }
    public AudioClip CurrentClip { get; set; }
    public Texture2D CurrentTexture { get; set; }
    public Texture2D CurrentStockTexture { get; set; }
    public HashSet<int> inUseWordIds;

    public MOD_WordEditing(MasterClass newMaster) : base(newMaster)
    {
        dataService = StartupScript.ds;
        wordList = new Dictionary<string, WordDO>();
        textureList = new List<Texture2D>();
        stockTextureList = new List<Texture2D>();
        wordImageNames = new List<string>();
        lastPlayEntryId = dataService.GetLastPlayEntryId();

        if (wordList == null || wordList.Count == 0)
        {
            LoadWordList();
        }
    }

    public override void GetCoworkers(MasterClass master) {}
    public void DeleteWordImage(string word) { FileAccessUtil.DeleteWordPic(word);  }
    //public bool ImageDeleteCleanup(string word) { return FileAccessUtil.WordImageDirRepair(word); }

    /// <summary>
    /// Deletes a word entry from the data base and removes it from the word list dictionary. 
    /// Additionally, it deletes any .wav or .png files associated with the word too.
    /// Returns true if the entry is successfully deleted from the data base.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>bool</returns>
    public bool RemoveWordListEntry(string key)
    {
        bool success = false;

        if (dataService.DeleteFromWords(wordList[key].IdNum) > 0)
        {
            success = !success;
            wordList.Remove(key);
            FileAccessUtil.DeleteWordAudio(key);
            List<string> existingPicsFiles = new List<string>();
            string[] temparr = Directory.GetFiles(Application.persistentDataPath + "/WordPictures/" + key);

            foreach (string path in temparr)
            {
                existingPicsFiles.Add(path);
            }

            Debug.Log("existingPicsFiles array length = " + existingPicsFiles.Count);

            foreach (string path in existingPicsFiles)
            {
                Debug.Log("existingPicsFiles array element recorded as: " + path);
                File.Delete(path);
            }

            Debug.Log("Done deleting pics for " + key);
            Directory.Delete(Application.persistentDataPath + "/WordPictures/" + key);
            Debug.Log("Done deleting directory for " + key);
            temparr = null;
            existingPicsFiles.Clear();
        }
        else
        {
            Debug.Log("Error trying to delete " + key); 
        }

        return success;
    }

    public float GetWordMastery(string wordName)
    {
        return dataService.GetWordMastery(wordName);
    }

    public int GetWordPlayCount(string wordName)
    {
        return dataService.GetWordPlayCount(wordName);
    }

    /// <summary>
    /// Updates the word entry data in the data base and the word list dictionary as well.
    /// Additionally, it also saves any changes made to the audio or picture files.
    /// Returns true if the entry in the data base was updated successfully.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public bool EditDbEntry(string word, string wordTags)
    {
        WordDO dataObject = wordList[word];
        bool audioSuccess = true, photoSuccess = true;

        // Check if the audio file path and photo file paths are active
        if (CurrentClip != null)
        {
            dataObject.WordSound = word + ".wav";

            //TODO: Fix false positive when overwriting existing files
            if (!SaveAudioClip())
            {
                //TODO: Add some error handling
                audioSuccess = false;
            }
        }
     

        if (CurrentTexture != null)
        {
            dataObject.WordImage = word + ".png";

            //TODO: Fix false positive when overwriting existing files
            if (SaveTextures(word) <= 0)
            {
                //TODO: add error message
                photoSuccess = false;
            }
        }

        // Adjust all the data object values to the new values
        dataObject.WordTags = wordTags;


        if (audioSuccess && photoSuccess)
        {
            if (dataService.EditWordEntry(dataObject) == 1)
            {
                wordList[word] = dataObject;
                return true;
            }
            else
            {
                //TODO: error handling
            }
        }
        else
        {
            Debug.Log("EDITDB: AUDIO OR PHOTO SUCCESS NOT SUCCESS");
        }

        return false;
    }



    public bool SaveDLCAudio(byte[] audioFile, string wordName)
    {
        bool audioSuccess = false;

        audioSuccess = FileAccessUtil.SaveDLCWordAudio(audioFile, wordName);

        if (audioSuccess)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new word list entry and saves it to the database and the word list dictionary.
    /// Additionally, it saves any new audio or picture files.
    /// Returns true if saving data to the data base is successful.
    /// </summary>
    /// <param name="word"></param>
    /// <returns>bool</returns>
    public bool CreateNewDbEntry(string word, string wordTags)
    {
        string img;
        string sound;
        string tags;
        int rowsInserted;
        bool audioSuccess = true;
        bool photoSuccess = true;

        // Check if the audio file path and photo file paths are active
        if (CurrentClip != null)
        {
            //TODO: Fix false positive when overwriting existing files
            // Maybe a check between the files date metadata and the current time?
            audioSuccess = FileAccessUtil.SaveWordAudio(CurrentClip, word);

            if (!audioSuccess)
            {
                //TODO: add error checking
            }
        }

        if (CurrentTexture != null)
        {
            //TODO: Fix false positive when overwriting existing files
            // Maybe a check between the files date metadata and the current time?

            /*
            photoSuccess = FileAccessUtil.SaveWordPic(CurrentTexture, word);

            if (!photoSuccess)
            {
                //TODO: add error checking
            }
            */

            SaveTextures(word);
        }

        //Save word to DB
        string category = "custom";
        //int diff = (int)Math.Round(difficultySlider.value);

        //Preset Image and sound entries - This will need to be changed when adding features for an image/audio path
        img = word + ".png";
        sound = word + ".wav";
        tags = wordTags;

        if (audioSuccess && photoSuccess)
        {
            //Call to DB to insert the word
            rowsInserted = dataService.CreateWord(word, category, tags, sound, img);

            if (rowsInserted == 1)
            {
                // Get the newly created id of the word entry from the data base
                IEnumerable<Words> rows = dataService.GetLastById();
                int id = 0;
                string check = "";

                foreach (var row in rows)
                {
                    id = row.word_id;
                    check = row.word_name;
                }

                if (check.Equals(word))
                {
                    // Create a new word data object and add a new entry into the static word/WordDO dictionary
                    WordDO dataObject = new WordDO(id, word, category, tags, sound, img);
                    wordList[word] = dataObject;
                    return true;
                }
                else
                {
                    //TODO: THROW AN ERROR
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks all play list entries if they are still valid and delete those that are not.
    /// </summary>
    public void DeleteInvalidPlayEntries()
    {
        Debug.Log("MOD: Trying to validate!");


        foreach (DO_PlayListObject entry in dataService.GetPlayList())
        {
            Debug.Log("MOD: Trying to validate type: " + entry.type_id.ToString());

            if (!entry.ValidateChild(dataService) && entry.type_id != 1)
            {
                Debug.Log("MOD: NON VALID ENTRY FOUND DELETING!");

                dataService.DeleteFromPlayList(entry.id);
            }
        }
    }

    /// <summary>
    /// Creates a hash set of word ids that are currently being used by one or more 
    /// play lists. 
    /// </summary>
    public void PopulateInUseSet()
    {
        // Check if the inUse Hash set is initialized yet
        if (inUseWordIds == null)
        {
            inUseWordIds = new HashSet<int>();
        }
        else
        {
            inUseWordIds.Clear();
        }

        foreach (var entry in dataService.GetPlayList())
        {
            switch (entry.type_id)
            {
                case 0:
                    DO_WordScramble scramble = JsonUtility.FromJson<DO_WordScramble>(entry.json);
                    inUseWordIds.UnionWith(scramble.wordIdList);
                    break;
                case 2:
                    DO_FlashCard flash = JsonUtility.FromJson<DO_FlashCard>(entry.json);
                    inUseWordIds.UnionWith(flash.wordIdList);
                    break;
                case 3:
                    DO_CountingGame counting = JsonUtility.FromJson<DO_CountingGame>(entry.json);
                    inUseWordIds.UnionWith(counting.wordIds);
                    break;
                case 4:
                    DO_KeyboardGame keyboard = JsonUtility.FromJson<DO_KeyboardGame>(entry.json);
                    inUseWordIds.UnionWith(keyboard.wordIdList);
                    break;
                case 5:
                    DO_MemoryCards memory = JsonUtility.FromJson<DO_MemoryCards>(entry.json);
                    inUseWordIds.UnionWith(memory.wordIdList);
                    break;
                case 6:
                    DO_MatchingGame matching = JsonUtility.FromJson<DO_MatchingGame>(entry.json);
                    inUseWordIds.UnionWith(matching.wordIdList);
                    break;
                default:
                    break;
            }
            /*       
        
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
         */
        }
    }

    public void DeleteWordImages(List<int> deleteIndices, string word)
    {
        foreach (int idx in deleteIndices)
        {
            Debug.Log("Index number is " + idx);

            if (idx < wordImageNames.Capacity && wordImageNames[idx] != null)
            {
                // Add the file name to the delete names hash set for deletion
                toDelete.Add(wordImageNames[idx]);

                Debug.Log("*****\n wordImageNames entry is: " + wordImageNames[idx] + "");

                // Grab a ref to the texture to be deleted
                imagesForDelete.Add(textureList[idx]);
            }

            Debug.Log("removed from TEXTURELIST");
        }
        if (toDelete.Count > 0)
        {
            for (int x = 0; x < toDelete.Count; x++)
            {
                wordImageNames.Remove(toDelete[x]);
                textureList.Remove(imagesForDelete[x]);
            }

            Debug.Log("to DELETE WAS NOT COUNT ZERO");
            FileAccessUtil.DeleteWordPictures(toDelete, word);

            //Clear the the temporary delete lists
            toDelete.Clear();
            imagesForDelete.Clear();
        }
    }





    /// <summary>
    /// Takes a word string and retrieves all of the png images associated with it.
    /// </summary>
    /// <param name="word"></param>/// 
    /// <returns>bool</returns>
    public bool RetrieveSavedPics(string word)
    {
        Debug.Log("MOD: trying to retirieve pictures for word " + word);
        int x = 1;

        List<byte[]> tempList = FileAccessUtil.RetrieveWordPics(word);

        /****************************************************************/
        // THIS IS USED TO DISPLAY MULTIPLE STOCK IMAGES
        // Grab any stock textures from assets        

        if (stockTextureList.Count == 0)
        {
            Texture2D stockChecker = Resources.Load<Texture2D>("WordPictures/" + word + "/" + word + "1");

            if (stockChecker != null) 
            {
                Debug.Log("Stock images exist for: " + word);
                Texture2D stockTexture1 = Resources.Load<Texture2D>("WordPictures/" + word + "/" + word + "1");
                Texture2D stockTexture2 = Resources.Load<Texture2D>("WordPictures/" + word + "/" + word + "2");
                Texture2D stockTexture3 = Resources.Load<Texture2D>("WordPictures/" + word + "/" + word + "3");
                Texture2D stockTexture4 = Resources.Load<Texture2D>("WordPictures/" + word + "/" + word + "4");
                Texture2D stockTexture5 = Resources.Load<Texture2D>("WordPictures/" + word + "/" + word + "5");

                stockTextureList.Add(stockTexture1);
                stockTextureList.Add(stockTexture2);
                stockTextureList.Add(stockTexture3);
                stockTextureList.Add(stockTexture4);
                stockTextureList.Add(stockTexture5);                
            }
            else
            {
                Debug.Log("no stock images exist for " + word);
            }
            
        }


        if (tempList != null && tempList.Count > 0)
        {
            foreach (byte[] bytes in tempList)
            {
                if (bytes == null || bytes.Length <= 0)
                {
                    Debug.Log("MOD: BYTES ARE NO GOOOOOOD");
                }

                Debug.Log("MOD: IN FOR EACH x IS " + x.ToString());

                Texture2D tempTex = new Texture2D(SAVED_PIC_XY, SAVED_PIC_XY);
                tempTex.LoadImage(bytes);
                textureList.Add(tempTex);
                x += 1;
            }
        }
        else
        {
            return false;
        }

        string[] isolatedName;

        foreach (string fileName in FileAccessUtil.GetWordImagePaths(word))
        {
            isolatedName = Regex.Split(fileName, PNG_PATTERN);
            Debug.Log("REGEX: isolated name is " + isolatedName[1]);
            if (!wordImageNames.Contains(isolatedName[1]))
            {
                wordImageNames.Add(isolatedName[1]);
            }
        }

        foreach (string fileName in wordImageNames)
        {
            Debug.Log("Next word image name is " + fileName);
        }



        Debug.Log("MOD: TEX LIST COUNT IS " + textureList.Count.ToString());
        return true;
    }


    public bool AreTexturesPresent() { return textureList.Count > 0 ? true : false; }
    public void AddNewTexture() { if (!textureList.Contains(CurrentTexture)) textureList.Add(CurrentTexture); }
    public List<Texture2D> GetImageList() { return textureList; }
    public List<Texture2D> GetStockImageList() { return stockTextureList; }
    public void ClearTextureList() { textureList.Clear(); }
    public void ClearStockTextureList() { stockTextureList.Clear(); }
    public bool SaveAudioClip() { return FileAccessUtil.SaveWordAudio(CurrentClip, WordName); }
    public WordDO GetListValue(string key) { return wordList[key]; }
    public bool CheckForDbEntry(string word) { return dataService.CheckWordExistance(word); }
    public Dictionary<string, WordDO> GetListCopy() { return new Dictionary<string, WordDO>(wordList); }
    private void AddEditValues(string key, WordDO value) { wordList[key] = value; }
    private bool SaveTexture() { return FileAccessUtil.SaveWordPic(CurrentTexture, WordName); }

    public int SaveTextures(string word)
    {
        int x = 0;

       
        string[] isolatedName;

        foreach (string fileName in FileAccessUtil.SaveWordPics(textureList, word))
        {
            if (!wordImageNames.Contains(fileName)) {
                isolatedName = Regex.Split(fileName, PNG_PATTERN);
                Debug.Log("Saving textures, isolated file name is " + isolatedName[1]);
                wordImageNames.Add(isolatedName[1]);
                x += 1;
            }
        }

        return x;
    }


    /// <summary>
    /// Loads the word list from the data base into the class word list. 
    /// </summary>
    private void LoadWordList()
    {
        WordDO tempObject;
        IEnumerable<Words> words = dataService.GetWordsTable();

        foreach (var row in words)
        {
            tempObject = new WordDO(
                row.word_id,
                row.word_name,
                row.stock_custom,                
                row.word_tags,
                row.word_sound,
                row.word_image
                );

            wordList.Add(row.word_name, tempObject);
        }
    }
}