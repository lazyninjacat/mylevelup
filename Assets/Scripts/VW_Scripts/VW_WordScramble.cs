using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


/// <summary>
/// This is the View component for the Word Scramble scene.
/// </summary>
public class VW_WordScramble : MonoBehaviour
{
    public GameObject availableWordsContent;
    public GameObject currentWordsContent;
    public GameObject saveSuccessModal;
    public GameObject saveErrorModal;
    public GameObject optionsPanel;

    // Get the controller
    private CON_PlayList controller;
    private DO_WordScramble scramble = null;
    private string sceneName;

    private const string scrambleStr = "Word Scramble";
    private const string scrambleScene = "word_scramble";

    private List<Transform> availableWordList = new List<Transform>();
    private List<int> wordIdsToSave = new List<int>();

    void Start () {

        sceneName = SceneManager.GetActiveScene().name;

        // Populate the add new scrollview
        MAS_PlayList tempMaster = (MAS_PlayList)COM_Director.GetMaster("MAS_PlayList");

        controller = (CON_PlayList)tempMaster.GetController("CON_PlayList");

        if (controller.CheckIfNewEntry())
        {
            PopulateAvailableViewOnly();
        }
        else
        {
            scramble = JsonUtility.FromJson<DO_WordScramble>(controller.GetJsonByIndex(controller.GetActiveContextIndex()));
            ToggleAllToggles();
            PopulateBothViews();
        }
     
    }

    public void CloseErrorModal() { saveErrorModal.SetActive(false); }

    /// <summary>
    /// This method handles clearing scene tranistion data and changing the scene.
    /// </summary>
    public void ExitAndClear()
    {
        controller.ClearData();
        ClearLists();

        // controller.SceneChange("play_list");
    }


    /// <summary>
    /// This method handles moving toggle button entries from the available 
    /// list to the current selected list.
    /// </summary>
    public void MoveToCurrentView()
    {
        HashSet<Transform> wordsToMove = new HashSet<Transform>();

        //For each child game object (the words)
        foreach (Transform word in availableWordsContent.transform)
        {
            if (word.gameObject.GetComponent<Toggle>().isOn)
            {
                wordsToMove.Add(word);
            }
        }

        foreach (Transform word in wordsToMove)
        {
            word.SetParent(currentWordsContent.transform, false);
            word.gameObject.GetComponent<Toggle>().isOn = false;
        }
    }


    /// <summary>
    /// This method handles moving toggle button entries from the current 
    /// list to the available selected list.
    /// </summary>
    public void MoveToAvailableView()
    {
        HashSet<Transform> wordsToMove = new HashSet<Transform>();

        foreach (Transform child in availableWordsContent.transform)
        {
            availableWordList.Add(child);
        }

        //For each child game object (the words)
        foreach (Transform word in currentWordsContent.transform)
        {
            if (word.gameObject.GetComponent<Toggle>().isOn)
            {
                availableWordList.Add(word);
                // wordsToMove.Add(word);
            }
        }

     
        SortAvailableWords();


    }

    /// <summary>
    /// This method takes the list of words from the current selected word list,
    /// creates a Word Scramble data object from it and sends it off to the model
    /// via the controller.
    /// <seealso cref="CON_PlayList.AddOrEditEntry(string, int, object)"/>
    /// <seealso cref="MOD_PlayList.AddOrEditEntryData(int, int, string, string)"/>
    /// <seealso cref="DO_WordScramble"/>
    /// </summary>
    public void Save()
    {

        // Create the word ids list for the playlist dataobject
        List<int> wordIdsToSave = new List<int>();

        foreach (var pair in controller.GetIdToWordsDict())
        {
            foreach (Transform child in currentWordsContent.transform)
            {
                if (child.name == pair.Value)
                {
                    Debug.Log("found word in currentWordsContentList to add to wordsToSave list.");
                    wordIdsToSave.Add(pair.Key);
                }

            }

        }
        Debug.Log("wordIdsToSave list count = " + wordIdsToSave.Count);

        // Get the new duration value from the number of children in the current words list scroll view
        int duration = currentWordsContent.transform.childCount;

        // Create bools for the toggle options
        bool snapOn = optionsPanel.transform.Find("LetterSnapToggle").gameObject.GetComponent<Toggle>().isOn;

        DO_WordScramble tempScramble = new DO_WordScramble(wordIdsToSave, snapOn);

        if (controller.AddOrEditEntry(scrambleStr, duration, tempScramble))
        {
            saveSuccessModal.SetActive(true);
        }
        else
        {
            saveErrorModal.SetActive(true);
            // TODO: Log an error
        }

    }

    /// <summary>
    /// Populates the available view with the word list. Used for
    /// adding new entries.
    /// </summary>
    private void PopulateAvailableViewOnly()
    {
        int idx = 0;

        foreach (var pair in controller.GetIdToWordsDict())
        {
            CreateAndChildToggle(pair.Value, pair.Key, false);
            idx++;
        }

        SortAvailableWords();

    }

    /// <summary>
    /// This method takes in a Word Scramble data object as a param and
    /// populates both the available and current selected word lists depending
    /// on whether or not the word is part of the data object.
    /// <seealso cref="DO_WordScramble"/>
    /// </summary>
    private void PopulateBothViews()
    {
        foreach (var pair in controller.GetIdToWordsDict())
        {
            CreateAndChildToggle(pair.Value, pair.Key, scramble.wordIdList.Contains(pair.Key));
        }

        SortAvailableWords();


    }

    /// <summary>
    /// Thist method retrieves a prefab toggable word object sets the text of the
    /// toggle and then childs them to their proper list.
    /// </summary>
    /// <param name="word"></param>
    /// <param name="id"></param>
    /// <param name="isInCurrent"></param>
    private void CreateAndChildToggle(string word, int id, bool isInCurrent)
    {
        Debug.Log("VW_WS: Creating new toggle child");
        GameObject obj = Instantiate((GameObject)Resources.Load("Word"));
        obj.name = id.ToString();

        Debug.Log("VW_WS: Instantiated child successfully.");

        // Get the text from the child and set it to the word
        Text objectText = obj.GetComponentInChildren<Text>();
        objectText.text = TidyCase(word);
        objectText.verticalOverflow = VerticalWrapMode.Overflow;

        if (isInCurrent)
        {
            Debug.Log("VW_WS: Setting new parent to available.");
            obj.transform.SetParent(currentWordsContent.transform, false);
            obj.name = word;

        }
        else
        {
            Debug.Log("VW_WS: Setting new parent to current.");
            obj.name = word;
            availableWordList.Add(obj.transform);
        }
    }

    /// <summary>
    /// Creates and returns a list of word id integers.
    /// </summary>
    /// <returns>A list of ints</returns>
    private List<int> CreateWordIdList()
    {
        List<int> list = new List<int>();

        foreach (Transform child in currentWordsContent.transform)
        {
            list.Add(Int16.Parse(child.name));
        }

        return list;
    }

    /// <summary>
    /// Sets the audio and difficulty toggles in the scene according to the
    /// saved data in the Word Scramble object.
    /// <seealso cref="DO_WordScramble"/>
    /// </summary>
    private void ToggleAllToggles()
    {
        // Set all options panel toggles
        optionsPanel.transform.Find("LetterSnapToggle").gameObject.GetComponent<Toggle>().isOn = scramble.letterSnap;
        optionsPanel.transform.Find("ShadowLettersToggle").gameObject.GetComponent<Toggle>().isOn = scramble.shadowLetters;
        optionsPanel.transform.Find("HintsToggle").gameObject.GetComponent<Toggle>().isOn = scramble.hints;

        // Set all audio panel toggles
        optionsPanel.transform.Find("WordSoundToggle").gameObject.GetComponent<Toggle>().isOn = scramble.wordSound;
        optionsPanel.transform.Find("LetterNameToggle").gameObject.GetComponent<Toggle>().isOn = scramble.letterName;
        optionsPanel.transform.Find("LetterSoundToggle").gameObject.GetComponent<Toggle>().isOn = scramble.letterSound;      
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

    /// <summary>
    /// Sorts the words in the Available scrollview
    /// </summary>
    private void SortAvailableWords()
    {
        if (availableWordsContent.transform.childCount > 0)
        {
            availableWordsContent.transform.DetachChildren();
        }
        else
        {
            Debug.Log("availablewordsContent has no words yet");
        }
        availableWordList = availableWordList.OrderBy(x => x.name).ToList();
        foreach (Transform child in availableWordList)
        {
            Debug.Log(child.name);
            child.transform.SetParent(availableWordsContent.transform, false);
            child.gameObject.GetComponent<Toggle>().isOn = false;
        }

        foreach (Transform child in availableWordsContent.transform)
        {
            child.transform.localScale = new Vector3(1, 1, 1);
        }

        availableWordList.Clear();

    }



    /// <summary>
    /// A method to clear all current lists. Clears availableWordList and currentWordsContentList.
    /// </summary>
    public void ClearLists()
    {
        Debug.Log("Clear Lists");
        availableWordList.Clear();
        wordIdsToSave.Clear();

    }




}