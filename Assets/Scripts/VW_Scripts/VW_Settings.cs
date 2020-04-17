using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class VW_Settings : MonoBehaviour
{    
    //Child's Name Input Field
    public InputField childnameField;
        
    //Child's Name Text
    public Text childnameText;

    //Child Name string
    private string childName;

    private DataService dataService;


    [SerializeField] Toggle onOffToggle;
    [SerializeField] Dropdown fromTimeDropdown;
    [SerializeField] Dropdown toTimeDropdown;
    [SerializeField] GameObject confirmResetModal;
    [SerializeField] GameObject pleaseWaitModal;
    [SerializeField] GameObject ResetCompleteModal;

    

    private int fromTime;
    private int toTime;

    private Dictionary<string, WordDO> wordList;
    private List<int> wordIDsForDelete;




    void Start()
    {
        dataService = StartupScript.ds;
        wordList = new Dictionary<string, WordDO>();
        wordIDsForDelete = new List<int>();


        if (wordList == null || wordList.Count == 0)
        {
            LoadWordList();
        }

        //Display ChildName Key saved to PlayerPrefs in console
        Debug.Log("Child name set to : " + PlayerPrefs.GetString("ChildNameKey").ToString());

        if ((PlayerPrefs.GetString("ChildNameKey") == "") || (PlayerPrefs.GetString("ChildNameKey") == null))
        {
            childnameText.text = "Name not set yet";
        }
        else
        {
            //Display ChildName Key saved to PlayerPrefs on screen in the ChildNameText game object in scene
            childnameText.text = (PlayerPrefs.GetString("ChildNameKey"));
        } 
   
        SetupToggles();


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

    private void Update()
    {
        if (onOffToggle.isOn)
        {
            fromTimeDropdown.interactable = true;
            toTimeDropdown.interactable = true;
        }
        else
        {
            fromTimeDropdown.interactable = false;
            toTimeDropdown.interactable = false;
        }
    }


    public void OnSubmitName()
    {
        //Set childName string to text in childnameField
        childName = childnameField.text;

        //Display Child name in console
        Debug.Log("Child name set to : " + childName);

        //Save Child Name to PlayerPrefs
        PlayerPrefs.SetString("ChildNameKey", childName);

        //Display ChildName Key saved to PlayerPrefs in scene in ChildNameText game object when user presses submitbutton and changes current Child Name
        childnameText.text = (PlayerPrefs.GetString("ChildNameKey"));

        childnameText.gameObject.GetComponent<Animation>().Play();

    }


    public void ChangeScene(string scene)
    {
        SetLockoutPrefs();
        SceneManager.LoadScene(scene);
    }


    public void SetLockoutPrefs()
    {
        fromTime = fromTimeDropdown.value;
        toTime = toTimeDropdown.value;

        if (onOffToggle.isOn)
        {
            PlayerPrefs.SetInt("lockOnOff", 1);
        }
        else
        {
            PlayerPrefs.SetInt("lockOnOff", 0);
        }
        PlayerPrefs.SetInt("LockoutFromTimeInt", fromTime);
        PlayerPrefs.SetString("LockoutFromTimeString", fromTimeDropdown.captionText.text);
        PlayerPrefs.SetInt("LockoutToTimeInt", toTime);
        PlayerPrefs.SetString("LockoutToTimeString", toTimeDropdown.captionText.text);
        PlayerPrefs.Save();

        Debug.Log("Playerprefs saved. lockOnOff = " + (PlayerPrefs.GetInt("lockOnOff")) + ", FromTime = " + (PlayerPrefs.GetInt("LockoutFromTimeInt")) + ", ToTime = " + (PlayerPrefs.GetInt("LockoutToTimeInt")));
    }

    
    private void SetupToggles()
    {
        if (PlayerPrefs.GetInt("lockOnOff") == 1)
        {
            onOffToggle.isOn = true;       
            fromTimeDropdown.value = (PlayerPrefs.GetInt("LockoutFromTimeInt"));
            toTimeDropdown.value = (PlayerPrefs.GetInt("LockoutToTimeInt"));
        }
        else
        {
            onOffToggle.isOn = false;
            fromTimeDropdown.value = (PlayerPrefs.GetInt("LockoutFromTimeInt"));
            toTimeDropdown.value = (PlayerPrefs.GetInt("LockoutToTimeInt"));
        }
    }
    
    public void OpenConfirmResetModal()
    {
        confirmResetModal.SetActive(true);
    }

    public void CloseConfirmResetModal()
    {
        confirmResetModal.SetActive(false);
    }

    public void OpenPleaseWaitModal()
    {
        pleaseWaitModal.SetActive(true);
    }

    public void ClosePleaseWaitModal()
    {
        pleaseWaitModal.SetActive(false);
    }

    public void Reset()
    {
        CloseConfirmResetModal();
        OpenPleaseWaitModal();
        // Clear playlist table and reset auto increment to 0
        dataService.DeleteAllPlaylist();
        dataService.ReseedTable("playlist", 0);

        // Delete all non stock words       
        foreach (var row in wordList)
        {
            if (row.Value.StockCustom == "custom")
            {
                Debug.Log("Adding to wordlist to delete: " + row.Value.Word_name);
                wordIDsForDelete.Add(row.Value.IdNum);
                FileAccessUtil.DeleteWordAudio(row.Value.Word_name);

                List<string> existingPicsFiles = new List<string>();
                string[] temparr = System.IO.Directory.GetFiles(Application.persistentDataPath + "/WordPictures/" + row.Value.Word_name);

                foreach (string path in temparr)
                {
                    existingPicsFiles.Add(path);
                }

                Debug.Log("existingPicsFiles array length = " + existingPicsFiles.Count);


                foreach (string path in existingPicsFiles)                    
                {
                    Debug.Log("existingPicsFiles array element recorded as: " + path);
                    System.IO.File.Delete(path);

                }
                Debug.Log("Done deleting pics for " + row.Value.Word_name);
                System.IO.Directory.Delete(Application.persistentDataPath + "/WordPictures/" + row.Value.Word_name);
                Debug.Log("Done deleting directory for " + row.Value.Word_name);
                temparr = null;
                existingPicsFiles.Clear();
            }
        }        
        foreach (int id in wordIDsForDelete)
        {
            Debug.Log(" deleting word ID: " + id);
            dataService.DeleteFromWords(id);
        }
        dataService.ReseedTable("Words", 10);
        wordList.Clear();
        wordIDsForDelete.Clear();


        // Clear existing mastery database records
        dataService.DeleteAllMastery();


        // Reset all Playerprefs
        PlayerPrefs.SetInt("isFirstKey", 0);
        PlayerPrefs.SetInt("AutoPlaylistOnOffKey", 0);

        // Reset all rewards

        ClosePleaseWaitModal();
        OpenResetCompleteModal();
    }

    private void OpenResetCompleteModal()
    {
        ResetCompleteModal.SetActive(true);
    }




}
