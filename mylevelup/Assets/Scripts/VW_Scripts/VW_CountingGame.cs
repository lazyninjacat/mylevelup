using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This is the View component for the Counting Game scene.
/// </summary>
public class VW_CountingGame : MonoBehaviour
{
    public GameObject availableWordsContent;
    public GameObject currentWordsContent;
    public GameObject saveSuccessModal;
    public GameObject saveErrorModal;
    public GameObject optionsPanel;
    public InputField collectNumField;
    public InputField itemMaxField;

    // Get the controller
    private CON_PlayList controller;
    private DO_CountingGame counting = null;
    private string sceneName;

    private const string countingStr = "Counting Game";
    private const string countingScene = "counting_game";

    private const int duration = 1;

    void Start()
    {

        sceneName = SceneManager.GetActiveScene().name;

        // Populate the add new scrollview
        MAS_PlayList tempMaster = (MAS_PlayList)COM_Director.GetMaster("MAS_PlayList");

        controller = (CON_PlayList)tempMaster.GetController("CON_PlayList");

        if (controller.CheckIfNewEntry())
        {
            PopulateAvailableViewOnly();
        }
        else if (sceneName == countingScene)
        {
            counting = JsonUtility.FromJson<DO_CountingGame>(controller.GetJsonByIndex(controller.GetActiveContextIndex()));
            ToggleAllToggles();
            SetAllInputValues();
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
        controller.SceneChange("play_list");
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

        //For each child game object (the words)
        foreach (Transform word in currentWordsContent.transform)
        {
            if (word.gameObject.GetComponent<Toggle>().isOn)
            {
                wordsToMove.Add(word);
            }
        }

        foreach (Transform word in wordsToMove)
        {
            word.SetParent(availableWordsContent.transform, false);
            word.gameObject.GetComponent<Toggle>().isOn = false;
        }
    }

    /// <summary>
    /// This method takes the list of words from the current selected word list,
    /// creates a Counting Game data object from it and sends it off to the model
    /// via the controller.
    /// <seealso cref="CON_PlayList.AddOrEditEntry(string, int, object)"/>
    /// <seealso cref="MOD_PlayList.AddOrEditEntryData(int, int, string, string)"/>
    /// <seealso cref="DO_CountingGame"/>
    /// </summary>
    public void Save()
    {
        // Set the duration value to 1.
        int duration = 1;
               
        if (sceneName == countingScene)
        {
            // Create bools for the toggle options
            bool hints = optionsPanel.transform.Find("HintsToggle").gameObject.GetComponent<Toggle>().isOn;
            bool countAudio = optionsPanel.transform.Find("CountAudioToggle").gameObject.GetComponent<Toggle>().isOn;
            bool defaultImg = optionsPanel.transform.Find("DefaultImageToggle").gameObject.GetComponent<Toggle>().isOn;
            bool startAudio = false;

            // Create the collect and Item Max numbers
            int collectNum = Convert.ToInt32(collectNumField.text);
            int itemMax = Convert.ToInt32(itemMaxField.text);

            DO_CountingGame tempCounting = new DO_CountingGame(CreateWordIdList(), collectNum, itemMax, hints, countAudio, startAudio, defaultImg);

            if (controller.AddOrEditEntry(countingStr, duration, tempCounting))
            {
                saveSuccessModal.SetActive(true);
            }
            else
            {
                saveErrorModal.SetActive(true);
                // TODO: Log an error
            }
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
    }

    /// <summary>
    /// This method takes in a Counting Game data object as a param and
    /// populates both the available and current selected word lists depending
    /// on whether or not the word is part of the data object.
    /// <seealso cref="DO_CountingGame"/>
    /// </summary>
    private void PopulateBothViews()
    {
        if (sceneName == countingScene)
        {
            foreach (var pair in controller.GetIdToWordsDict())
            {
                CreateAndChildToggle(pair.Value, pair.Key, counting.wordIds.Contains(pair.Key));
            }
        }
       
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
        Debug.Log("VW_CG: Creating new toggle child");
        GameObject obj = Instantiate((GameObject)Resources.Load("Word"));
        obj.name = id.ToString();

        Debug.Log("VW_CG: Instantiated child successfully.");

        // Get the text from the child and set it to the word
        Text objectText = obj.GetComponentInChildren<Text>();
        objectText.text = word;
        objectText.verticalOverflow = VerticalWrapMode.Overflow;

        if (isInCurrent)
        {
            Debug.Log("VW_CG: Setting new parent to available.");
            obj.transform.SetParent(currentWordsContent.transform, false);
        }
        else
        {
            Debug.Log("VW_CG: Setting new parent to current.");
            obj.transform.SetParent(availableWordsContent.transform, false);
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
    /// Sets the option toggles in the scene according to the
    /// saved data in the Counting Game object.
    /// <seealso cref="DO_CountingGame"/>
    /// </summary>
    private void ToggleAllToggles()
    {
        // Set all option panel toggles
        optionsPanel.transform.Find("DefaultImageToggle").gameObject.GetComponent<Toggle>().isOn = counting.startAudio;
        optionsPanel.transform.Find("CountAudioToggle").gameObject.GetComponent<Toggle>().isOn = counting.countAudio;
        optionsPanel.transform.Find("HintsToggle").gameObject.GetComponent<Toggle>().isOn = counting.hints;

    }

    /// <summary>
    /// Sets the input fields in the scene according to the
    /// saved data in the Counting Game object.
    /// <seealso cref="DO_CountingGame"/>
    /// </summary>
    private void SetAllInputValues()
    {
        collectNumField.text = Convert.ToString(counting.collectNum);
        itemMaxField.text = Convert.ToString(counting.itemMax);
    }
}