using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This is the View component for the creat new Memory Cards game scene.
/// </summary>
public class VW_MemoryCards : MonoBehaviour
{
    public GameObject availableWordsContent;
    public GameObject currentWordsContent;
    public GameObject saveSuccessModal;
    public GameObject saveErrorModal;
    public GameObject optionsPanel;

    // Get the controller
    private CON_PlayList controller;    
    private DO_MemoryCards memory = null;
    private string sceneName;
        
    private const string memoryStr = "Memory Cards";
    private const string memoryScene = "memory_cards";

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
        else if (sceneName == memoryScene)
        {
            memory = JsonUtility.FromJson<DO_MemoryCards>(controller.GetJsonByIndex(controller.GetActiveContextIndex()));
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
    /// creates a Memory Cards Game data object from it and sends it off to the model
    /// via the controller.
    /// <seealso cref="CON_PlayList.AddOrEditEntry(string, int, object)"/>
    /// <seealso cref="MOD_PlayList.AddOrEditEntryData(int, int, string, string)"/>
    /// <seealso cref="DO_MemoryCards"/>
    /// </summary>
    public void Save()
    {

        // Get the new duration value from the number of children in the current words list scroll view
        int duration = currentWordsContent.transform.childCount;

        // Create bools for the toggle options
        bool wordText = optionsPanel.transform.Find("IncludeWordTextToggle").gameObject.GetComponent<Toggle>().isOn;
        bool alts = optionsPanel.transform.Find("AltImagesToggle").gameObject.GetComponent<Toggle>().isOn;
        bool wordSound = optionsPanel.transform.Find("WordSoundToggle").gameObject.GetComponent<Toggle>().isOn;

        DO_MemoryCards tempMemory = new DO_MemoryCards(CreateWordIdList(), wordText, alts, wordSound);

        Debug.Log("************ created tempMemory data object********");

     
        if (controller.AddOrEditEntry(memoryStr, duration, tempMemory))
        {
            saveSuccessModal.SetActive(true);
        }
        else
        {
            saveErrorModal.SetActive(true);
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
    /// This method takes in a Memory Cards game data object as a param and
    /// populates both the available and current selected word lists depending
    /// on whether or not the word is part of the data object.   
    /// <seealso cref="DO_MemoryCards"/>
    /// </summary>
    private void PopulateBothViews()
    {
       if (sceneName == memoryScene)
        {
            foreach (var pair in controller.GetIdToWordsDict())
            {
                CreateAndChildToggle(pair.Value, pair.Key, memory.wordIdList.Contains(pair.Key));
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
        Debug.Log("VW_M: Creating new toggle child");
        GameObject obj = Instantiate((GameObject)Resources.Load("Word"));
        obj.name = id.ToString();

        Debug.Log("VW_M: Instantiated child successfully.");

        // Get the text from the child and set it to the word
        Text objectText = obj.GetComponentInChildren<Text>();
        objectText.text = word;
        objectText.verticalOverflow = VerticalWrapMode.Overflow;

        if (isInCurrent)
        {
            Debug.Log("VW_M: Setting new parent to available.");
            obj.transform.SetParent(currentWordsContent.transform, false);
        }
        else
        {
            Debug.Log("VW_M: Setting new parent to current.");
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
    /// Sets the options panel toggles in the scene according to the
    /// saved data in the Memory Cards Game object.
    /// <seealso cref="DO_MemoryCards"/>
    /// </summary>
    private void ToggleAllToggles()
    {
        // Set all options panel toggles
        optionsPanel.transform.Find("IncludeWordTextToggle").gameObject.GetComponent<Toggle>().isOn = memory.includeText;
        optionsPanel.transform.Find("AltImagesToggle").gameObject.GetComponent<Toggle>().isOn = memory.altImages;
        optionsPanel.transform.Find("WordSoundToggle").gameObject.GetComponent<Toggle>().isOn = memory.wordAudio;

    }
}