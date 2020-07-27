using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Text;
using static Constant;
//using UnityEngine.UIElements;

public class VW_PlayList : MonoBehaviour
{
    // GameObjects
    [SerializeField] private GameObject addNewPanel;
    [SerializeField] private GameObject buttonCopyPanel;

    public GameObject ButtonCopyPanel
    {
        get
        {
            return buttonCopyPanel;
        }

        set
        {
            buttonCopyPanel = value;
        }
    }
        
    
    [SerializeField] private GameObject addNewContent;

    public GameObject AddNewContent
    {
        get
        {
            return addNewContent;
        }

        set
        {
            addNewContent = value;
        }
    }
    [SerializeField] private GameObject playListContent;

    public GameObject PlayListContent
    {
        get
        {
            return playListContent;
        }

        set
        {
            playListContent = value;
        }
    }
    [SerializeField] private GameObject confirmDeleteModal;
    [SerializeField] private GameObject playCopyPanel;

    public GameObject PlayCopyPanel
    {
        get
        {
            return playCopyPanel;
        }

        set
        {
            playCopyPanel = value;
        }
    }

    [SerializeField] private GameObject deleteSuccessModal;
    [SerializeField] private GameObject setRewardTimePanel;
    [SerializeField] private GameObject setRewardsPanel;
    [SerializeField] private GameObject setRatioPanel;
    [SerializeField] private GameObject setWordTagsPanel;

    public GameObject SetWordTagsPanel
    {
        get
        {
            return setWordTagsPanel;
        }

        set
        {
            setWordTagsPanel = value;
        }
    }

    [SerializeField] private GameObject setGamesPanel;
    [SerializeField] private GameObject saveSuccessModal;
    [SerializeField] private GameObject saveErrorModal;
    [SerializeField] private GameObject autoPlaylistEnabledPanel;
    [SerializeField] private GameObject playlistEmptyPanel;

    public GameObject PlaylistEmptyPanel
    {
        get
        {
            return playlistEmptyPanel;
        }

        set
        {
            playlistEmptyPanel = value;
        }
    }

    [SerializeField] private GameObject confirmAutoplaylistOnModal;
    [SerializeField] private GameObject confirmAutoplaylistOffModal;
    [SerializeField] private GameObject passcodePanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject rewardPanel;

    public GameObject RewardPanel
    {
        get
        {
            return rewardPanel;
        }

        set
        {
            rewardPanel = value;
        }
    }

    [SerializeField] private GameObject rewardsViewportContent;
    public GameObject RewardsViewportContent
    {
        get
        {
            return rewardsViewportContent;
        }

        set
        {
            rewardsViewportContent = value;
        }
    }

    [SerializeField] private GameObject tagCopyPanel;
    public GameObject TagCopyPanel
    {
        get
        {
            return tagCopyPanel;
        }

        set
        {
            tagCopyPanel = value;
        }
    }

    [SerializeField] private GameObject gameCopyPanel;
    public GameObject GameCopyPanel
    {
        get
        {
            return gameCopyPanel;
        }

        set
        {
            gameCopyPanel = value;
        }
    }

    [SerializeField] private GameObject tagsViewportContent;
    public GameObject TagsViewportContent
    {
        get
        {
            return tagsViewportContent;
        }

        set
        {
            tagsViewportContent = value;
        }
    }

    [SerializeField] private GameObject setGamesContent;
    public GameObject SetGamesContent
    {
        get
        {
            return setGamesContent;
        }

        set
        {
            setGamesContent = value;
        }
    }

    [SerializeField] private GameObject playlistMissingRewardModal;
    [SerializeField] private GameObject GamesViewportContent;

    // SetGamePanel and tagsPanelAuto/Manual toggle
    [SerializeField] private Toggle autoSetGameToggle;
    public Toggle AutoSetGameToggle
    {
        get
        {
            return autoSetGameToggle;
        }

        set
        {
            autoSetGameToggle = value;
        }
    }

    [SerializeField] private Toggle autoSetTagsToggle;
    public Toggle AutoSetTagsToggle
    {
        get
        {
            return autoSetTagsToggle;
        }

        set
        {
            autoSetTagsToggle = value;
        }
    }

    // Texts
    [SerializeField] private Text rewardTimeText;
    [SerializeField] private Text autoPlaylistEnabledRewardTimeText;

    public Text AutoPlaylistEnabledRewardTimeText
    {
        get
        {
            return autoPlaylistEnabledRewardTimeText;
        }
        set
        {
            autoPlaylistEnabledRewardTimeText.text = value.ToString();
        }
    }

    [SerializeField] private Text autoPlaylistEnabledRewardsText;
    public Text AutoPlaylistEnabledRewardsText
    {
        get
        {
            return autoPlaylistEnabledRewardsText;
        }

        set
        {
            autoPlaylistEnabledRewardsText.text = value.ToString();
        }
    }

    [SerializeField] private Text tagsText;
    public Text TagsText
    {
        get
        {
            return tagsText;
        }

        set
        {
            tagsText.text = value.ToString();
        }
    }

    [SerializeField] private Text gamesText;
    public Text GamesText
    {
        get
        {
            return gamesText;
        }

        set
        {
            gamesText.text = value.ToString();
        }
    }

    [SerializeField] private Text ratioText;
    [SerializeField] private Text maxRatioSliderText;
    [SerializeField] private Text minRatioSliderText;

    // Buttons
    [SerializeField] private Button addButton;
    public Button AddButton
    {
        get
        {
            return addButton;
        }
    }

    [SerializeField] private Button autoPlaylistButton;
    public Button AutoPlaylistButton
    {
        get
        {
            return autoPlaylistButton;
        }
    }

    // Toggles
    [SerializeField] private Toggle infiniteLoopToggle;
    [SerializeField] private Toggle passLocked;

    // Other
    [SerializeField] private InputField loopNumberField;
    [SerializeField] private Slider rewardTimeSlider;
    [SerializeField] private Slider minRatioSlider;
    [SerializeField] private Slider maxRatioSlider;



    // Tutorial
    [SerializeField] GameObject tutorialPanel1;
    [SerializeField] GameObject tutorialAddNewPanel;
    [SerializeField] GameObject tutorialAutoplaylist;

    [SerializeField] GameObject tutorialButtonIconOn;
    [SerializeField] GameObject tutorialButtonIconOff;

    // Statics
    private static GameObject activePanel = null;
    private static int ACTIVE_PANEL_INDEX = -1;

    // Ints
    private int wordsPerReward = 10;
    //private float minRatio;
    //private float maxRatio;


    private List<GameObject> tutorialObjects;

    // Dictionaries
    private Dictionary<int, string> wordsDatabase = new Dictionary<int, string>(); // This dictionary contains the wordID int and wordName string from the words table in the database.
    private Dictionary<int, string> wordIdTagDict = new Dictionary<int, string>(); // This dictionary contains the wordID int and wordTags string from the words table in the database.
    
    // TODO: Replace this dictionary with a simple array or list and just use the index instead of the dictionary key.
    private Dictionary<int, int> OrderedFilteredWordIdsDict = new Dictionary<int, int>(); // This dictionary is used to contain the index '0-?' ints and the wordID ints, filtered by wordtag. 
    
    // Controller and Model
    private CON_PlayList controller;

    // bool to track whether or not the playlist currently has a Choose Reward entry. If this is false then the Exit method will not allow the user to return to the admin menu.
    public bool playlistHasReward;


    void Start () {
        // Populate the add new scrollview
        MAS_PlayList tempMaster = (MAS_PlayList)COM_Director.GetMaster("MAS_PlayList");
        controller = (CON_PlayList)tempMaster.GetController("CON_PlayList");
        
        // set the playlist has reward bool to false before populating the scroll view. If a reward is present in the playlist it will switch this bool back to true.
        playlistHasReward = false;

        SetLoopToggleAndValues();

        tutorialObjects = GameObject.FindGameObjectsWithTag("tutorial").ToList();

        foreach (GameObject obj in tutorialObjects)
        {
            obj.SetActive(false);
        }

        if (PlayerPrefs.GetInt("AutoPlaylistOnOffKey") == 0)
        {
            autoPlaylistEnabledPanel.SetActive(false);
            controller.SetupPlayAndAddNewLists();
        }
        else
        {
            autoPlaylistEnabledPanel.SetActive(true);
            autoPlaylistEnabledRewardTimeText.text = PlayerPrefs.GetString("AutoplaylistRewardTimeKey");
            autoPlaylistEnabledRewardsText.text = PlayerPrefs.GetString("AutoplaylistRewardsKey");
            tagsText.text = PlayerPrefs.GetString("CurrentAutoplaylistTags");
            gamesText.text = PlayerPrefs.GetString("CurrentAutoplaylistGames");
            addButton.interactable = false;
        }

        Dictionary<int, string> wordsDatabase = new Dictionary<int, string>(controller.GetIdToWordsDict());
        Dictionary<int, string> wordIdTagDict = new Dictionary<int, string>(controller.GetIdToWordTagsDict());
        List<string> selectedWordTagsList = new List<string>();
        Dictionary<int, int> OrderedFilteredWordIdsDict = new Dictionary<int, int>();

        if (PlayerPrefs.GetInt("AutoplaylistRewardTimeIntKey") == 0)
        {
            PlayerPrefs.SetInt("AutoplaylistRewardTimeIntKey", 1);
        }
        else
        {
            controller.rewardTime = PlayerPrefs.GetInt("AutoplaylistRewardTimeIntKey");
            wordsPerReward = controller.rewardTime;
        }      

        controller.RequestData();

        if (PlayerPrefs.GetInt("isTutorial") == 1)
        {
            tutorialPanel1.SetActive(true);
            tutorialButtonIconOff.SetActive(true);
            tutorialButtonIconOn.SetActive(false);
        }
        else
        {
            tutorialButtonIconOff.SetActive(false);
            tutorialButtonIconOn.SetActive(true);
        }

    }

    private void Update()
    {
        if (passLocked.isOn) { loopNumberField.interactable = true; }
        else
        {
            loopNumberField.interactable = false;
            loopNumberField.text = null;
        }                
        
        if (autoPlaylistEnabledPanel.activeSelf)
        {
            addButton.interactable = false;
            passcodePanel.SetActive(false);
        }
        if (PlayerPrefs.GetInt("isTutorial") == 0)
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
        }
    }   
    
    public void ToggleTutorialButton()
    {
        if (PlayerPrefs.GetInt("isTutorial") == 0)
        {
            PlayerPrefs.SetInt("isTutorial", 1);
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
            tutorialPanel1.SetActive(true);
            tutorialButtonIconOff.SetActive(true);
            tutorialButtonIconOn.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetInt("isTutorial", 0);
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
            tutorialButtonIconOff.SetActive(false);
            tutorialButtonIconOn.SetActive(true);
        }    
    }

    public void OnRewardSliderChange()
    {
        controller.rewardTime = Mathf.RoundToInt(rewardTimeSlider.value);
        rewardTimeText.text = controller.rewardTime.ToString();

        //TODO: remove this when minmax ratio is implemented
        wordsPerReward = controller.rewardTime;
    }

    //public void OnMinLearningRewardRatioSliderChange()
    //{
    //    minRatio = Mathf.RoundToInt(minRatioSlider.value);
    //    minRatioSliderText.text = "Minimum " + minRatioSlider.value + " / " + (100 - minRatioSlider.value);
    //}

    //public void OnMaxLearningRewardRatioSliderChange()
    //{
    //    maxRatio = Mathf.RoundToInt(maxRatioSlider.value);
    //    minRatioSliderText.text = "Maximum " + maxRatioSlider.value + " / " + (100 - maxRatioSlider.value);

    //}

    // All the open/close methods for the autoplaylist popup modals sequence.
    public void OpenSetRewardTimePanel() { setRewardTimePanel.SetActive(true); }

    public void CloseSetRewardTimePanel() { setRewardTimePanel.SetActive(false); }

    public void OpenSetRatioPanel() { setRatioPanel.SetActive(true); }

    public void CloseSetRatioPanel() { setRatioPanel.SetActive(false); }

    public void OpenSetRewardsPanel() { setRewardsPanel.SetActive(true); }

    public void CloseSetRewardsPanel() { setRewardsPanel.SetActive(false); }    

    public void OpenSetWordTagsPanel() { setWordTagsPanel.SetActive(true); }

    public void ToggleSetGamesPanel(bool active) { setGamesPanel.SetActive(active); }

    public void CloseConfirmAutoplaylistOffModal() { confirmAutoplaylistOffModal.SetActive(false); }

    public void CloseRemoveModal() { confirmDeleteModal.SetActive(false); }

    public void CloseDeleteSuccessModal() { deleteSuccessModal.SetActive(false); }

    public void CloseConfirmAutoplaylistOnModal()
    {
        confirmAutoplaylistOnModal.SetActive(false);
        addButton.interactable = true;
    }

    public void OpenAddPanel()
    {
        addButton.interactable = false;
        addNewPanel.SetActive(true);

        if (PlayerPrefs.GetInt("isTutorial") == 1)
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);             
            }
            tutorialAddNewPanel.SetActive(true);
        }
        else
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    public void CloseAddPanel()
    {
        addButton.interactable = true;
        addNewPanel.SetActive(false);

        if (PlayerPrefs.GetInt("isTutorial") == 1)
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
            tutorialPanel1.SetActive(true);
        }
        else
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
        }
    }
    
    public void ResetAutoplaylistButton()
    {
        confirmAutoplaylistOnModal.SetActive(true);
    }
    
    public void OnAutoPlaylistButton()
    {
        addButton.interactable = false;
        autoPlaylistButton.interactable = false;

        if (PlayerPrefs.GetInt("isTutorial") == 1)
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
            tutorialAutoplaylist.SetActive(true);
        }
        else
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
        }


        if ((PlayerPrefs.GetInt("AutoPlaylistOnOffKey") == 0))
        {
            confirmAutoplaylistOnModal.SetActive(true);
            if (PlayerPrefs.GetInt("isTutorial") == 1)
            {
                foreach (GameObject obj in tutorialObjects)
                {
                    obj.SetActive(false);
                }
                tutorialAutoplaylist.SetActive(true);
            }
            else
            {
                foreach (GameObject obj in tutorialObjects)
                {
                    obj.SetActive(false);
                }
            }
        }
        else
        {
            confirmAutoplaylistOffModal.SetActive(true);
            if (PlayerPrefs.GetInt("isTutorial") == 1)
            {
                foreach (GameObject obj in tutorialObjects)
                {
                    obj.SetActive(false);
                }
                tutorialPanel1.SetActive(true);
            }
            else
            {
                foreach (GameObject obj in tutorialObjects)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
    
    public void BeginCreateAutoplaylist()
    {
        StartCoroutine(BeginCreateAutoplaylistLaunchHelper());

        loadingPanel.SetActive(true);
    }
    private IEnumerator BeginCreateAutoplaylistLaunchHelper()
    {
        yield return new WaitForSeconds(2);
        controller.CreateAutoPlaylist();
    }

    public void DisableAutoPlaylistPanels()
    {
        autoPlaylistEnabledPanel.SetActive(false);
        confirmAutoplaylistOffModal.SetActive(false);
        addButton.interactable = true;
        passcodePanel.SetActive(true);
    }

    public void EnableAutoPlaylistPanels()
    {
        autoPlaylistEnabledPanel.SetActive(true);
        addButton.interactable = false;
        passcodePanel.SetActive(false);
    }

    public void ToggleLoopOptions() { loopNumberField.interactable = passLocked.isOn;  }
    
    public void ClosePlaylistMissingRewardModal()
    {
        playlistMissingRewardModal.SetActive(false);
    }

    public void Exit()
    {
        bool success = false;
        success = ((controller.SetLoopData(0, infiniteLoopToggle.isOn, loopNumberField.text == "" ? 1 : Int16.Parse(loopNumberField.text), passLocked.isOn)) && (playlistHasReward || ((PlayerPrefs.GetInt("AutoPlaylistOnOffKey")) == 1))) ;

        if (!success)
        {
            playlistMissingRewardModal.SetActive(true);
            return;
        }

        controller.SceneChange("admin_menu");
    }
    
    public void ResetActivePanel()
    {
        activePanel = null;
    }

    public void SetTutorialElements(bool setActive)
    {
        if (setActive)
        {
        tutorialPanel1.SetActive(true);
        tutorialButtonIconOff.SetActive(true);
        tutorialButtonIconOn.SetActive(false);
        }

        else
        {
            tutorialButtonIconOff.SetActive(false);
            tutorialButtonIconOn.SetActive(true);
        }
    }    

    private void SetLoopToggleAndValues()
    {
        bool check = controller.CheckIfPassLocked();
        passLocked.isOn = check;
        loopNumberField.interactable = check;
        loopNumberField.text = controller.GetLoopNumber().ToString();
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
    
    /////////////////////////////// Choose Reward Stuff vvvvvvvvvvvvvvvvvvvvv

    

    public void OnTagToggleChange(Toggle tagToggleChange)
    {
        if (tagToggleChange.isOn)
        {
            controller.m_toggleCount++;
        }
        else
        {
            controller.m_toggleCount--;
        }
    }

    public void OnGameToggleChange(Toggle gameToggleChange)
    {
        if (gameToggleChange.isOn)
        {
            controller.gameToggleCount++;
        }
        else
        {
            controller.gameToggleCount--;
        }
    }

    public void OnToggleChange(Toggle change)
    {
        if (change.isOn)
        {
            controller.toggleCount++;
        }
        else
        {
            controller.toggleCount--;
        }
    }

    public void SetLoadingPanelActive(bool active)
    {
        loadingPanel.SetActive(active);
    }

    public void SetDeleteObjectsView()
    {
        confirmDeleteModal.SetActive(false);
        deleteSuccessModal.SetActive(true);

        Destroy(activePanel);
    }    
}