using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class RewardHelper : AB_GameHelper
{
    // Serialized fields   
    [SerializeField] VW_GameLoop gameLoop;
    [SerializeField] GameObject reward1Obj;
    [SerializeField] GameObject reward2Obj;
    [SerializeField] GameObject reward3Obj;
    [SerializeField] GameObject reward4Obj;
    [SerializeField] GameObject reward5Obj;
    [SerializeField] GameObject reward6Obj;
    [SerializeField] GameObject customRewardPanel1;
    [SerializeField] GameObject customRewardPanel2;
    [SerializeField] GameObject customRewardPanel3;
    [SerializeField] GameObject customRewardPanel4;
    [SerializeField] GameObject customRewardPanel5;
    [SerializeField] GameObject customRewardPanel6;
    [SerializeField] Text customRewardNameText1;
    [SerializeField] Text customRewardNameText2;
    [SerializeField] Text customRewardNameText3;
    [SerializeField] Text customRewardNameText4;
    [SerializeField] Text customRewardNameText5;
    [SerializeField] Text customRewardNameText6;
    [SerializeField] RawImage customRewardNameRawImage1;
    [SerializeField] RawImage customRewardNameRawImage2;
    [SerializeField] RawImage customRewardNameRawImage3;
    [SerializeField] RawImage customRewardNameRawImage4;
    [SerializeField] RawImage customRewardNameRawImage5;
    [SerializeField] RawImage customRewardNameRawImage6;
    [SerializeField] RectTransform webviewRectReference;

    [SerializeField] Text reward1Text;
    [SerializeField] Text reward2Text;
    [SerializeField] Text reward3Text;
    [SerializeField] Text reward4Text;
    [SerializeField] Text reward5Text;
    [SerializeField] Text reward6Text;

    [SerializeField] GameObject exitButton;



    // Data objects
    private DO_PlayListObject playlistObj;
    private DO_ChooseReward chooseRewardObject;

    // Dataservice
    private DataService ds;

    // Logic data
    private const int TYPE = 1;
    private int duration;
    private bool countdownActive = false;
    private int rewardCount;
    private bool isWebReward = false;

    // Lists
    private List<int> rewardIdList;
    private List<string> alreadyLoadedImageList;

    // Public Gameobjects
    public GameObject continueModal; 
    public GameObject RoundCounterProgressBar;
    public Image TimeRemainingBar;
    public GameObject timeBarObj;
    private GameObject currentCustomRewardPanel;

    // UniWebView
    public UniWebView webView;

    //WebReward Data Reporting
    private DateTime startTime;





    void Start()
    {
        Debug.Log("RH Start");
    }

 

    public override void Resume()
    {
        Debug.Log("RH Resume");
        Restart();
    }

    void Restart()
    {
        // Grab the play list object for this game
        playlistObj = gameLoop.GetCurrentPlay();

        // Grab the choose reward object
        chooseRewardObject = JsonUtility.FromJson<DO_ChooseReward>(playlistObj.json);

        // Start the dataservice
        ds = StartupScript.ds;

        //create the already loaded lists
        alreadyLoadedImageList = new List<string>();

        // Creat the reward buttons
        CreateRewardButtons();

        //Create the webview
        CreateWebview();

        // Clear the already loaded list
        alreadyLoadedImageList.Clear();

        // deactivate the round counter bar so the countdown bar will be visable
        RoundCounterProgressBar.SetActive(false);
    }

   private void CreateRewardButtons()
    {
        reward1Obj.SetActive(false);
        reward2Obj.SetActive(false);
        reward3Obj.SetActive(false);
        reward4Obj.SetActive(false);
        reward5Obj.SetActive(false);
        reward6Obj.SetActive(false);

        customRewardPanel1.SetActive(false);
        customRewardPanel2.SetActive(false);
        customRewardPanel3.SetActive(false);
        customRewardPanel4.SetActive(false);
        customRewardPanel5.SetActive(false);
        customRewardPanel6.SetActive(false);

        rewardCount = chooseRewardObject.rewardIdsList.Count;

        if (rewardCount == 0)
        {
            Debug.Log("Reward count is 0");
        }
        else if (rewardCount == 1)
        {
            SetRewardButtonImage(reward1Obj, customRewardPanel1, customRewardNameRawImage1, customRewardNameText1, reward1Text);
        }
        else if (rewardCount == 2)
        {
            SetRewardButtonImage(reward1Obj, customRewardPanel1, customRewardNameRawImage1, customRewardNameText1, reward1Text);
            SetRewardButtonImage(reward2Obj, customRewardPanel2, customRewardNameRawImage2, customRewardNameText2, reward2Text);

        }
        else if (rewardCount == 3)
        {   
            SetRewardButtonImage(reward1Obj, customRewardPanel1, customRewardNameRawImage1, customRewardNameText1, reward1Text);
            SetRewardButtonImage(reward2Obj, customRewardPanel2, customRewardNameRawImage2, customRewardNameText2, reward2Text);
            SetRewardButtonImage(reward3Obj, customRewardPanel3, customRewardNameRawImage3, customRewardNameText3, reward3Text);
        }
        else if (rewardCount == 4)
        {
            SetRewardButtonImage(reward1Obj, customRewardPanel1, customRewardNameRawImage1, customRewardNameText1, reward1Text);
            SetRewardButtonImage(reward2Obj, customRewardPanel2, customRewardNameRawImage2, customRewardNameText2, reward2Text);
            SetRewardButtonImage(reward3Obj, customRewardPanel3, customRewardNameRawImage3, customRewardNameText3, reward3Text);
            SetRewardButtonImage(reward4Obj, customRewardPanel4, customRewardNameRawImage4, customRewardNameText4, reward4Text);
        }
        else if (rewardCount == 5)
        {  
            SetRewardButtonImage(reward1Obj, customRewardPanel1, customRewardNameRawImage1, customRewardNameText1, reward1Text);
            SetRewardButtonImage(reward2Obj, customRewardPanel2, customRewardNameRawImage2, customRewardNameText2, reward2Text);
            SetRewardButtonImage(reward3Obj, customRewardPanel3, customRewardNameRawImage3, customRewardNameText3, reward3Text);
            SetRewardButtonImage(reward4Obj, customRewardPanel4, customRewardNameRawImage4, customRewardNameText4, reward4Text);
            SetRewardButtonImage(reward5Obj, customRewardPanel5, customRewardNameRawImage5, customRewardNameText5, reward5Text);
        }
        else if (rewardCount == 6)
        {    
            SetRewardButtonImage(reward1Obj, customRewardPanel1, customRewardNameRawImage1, customRewardNameText1, reward1Text);
            SetRewardButtonImage(reward2Obj, customRewardPanel2, customRewardNameRawImage2, customRewardNameText2, reward2Text);
            SetRewardButtonImage(reward3Obj, customRewardPanel3, customRewardNameRawImage3, customRewardNameText3, reward3Text);
            SetRewardButtonImage(reward4Obj, customRewardPanel4, customRewardNameRawImage4, customRewardNameText4, reward4Text);
            SetRewardButtonImage(reward5Obj, customRewardPanel5, customRewardNameRawImage5, customRewardNameText5, reward5Text);
            SetRewardButtonImage(reward6Obj, customRewardPanel6, customRewardNameRawImage6, customRewardNameText6, reward6Text);
        }
    }
    

    private void SetRewardButtonImage(GameObject rb, GameObject rp, RawImage ri, Text rt, Text label)
    {
        rb.SetActive(true);
        
        var rewards = ds.GetRewardsTable();

        Debug.Log("SetRewardButtonImage method starting, with rb = " + rb.name + ", rp = " + rp.name + ", ri = " + ri.name + ", rt = " + rt.name);

        RawImage img = rb.GetComponent<RawImage>();
        RawImage customPanelImg = ri.GetComponent<RawImage>();
        Text customPanelText = rt.GetComponent<Text>();
        GameObject customPanel = rp;

        foreach (var row in rewards)
        {
            if (chooseRewardObject.rewardIdsList.Contains(row.reward_id) && !alreadyLoadedImageList.Contains(row.reward_name))
            {
                Debug.Log("Reward Loading. name = " + row.reward_name + ", type = " + row.reward_type + ", url = " + row.reward_url);

                customPanel.name = row.reward_name;
                label.text = row.reward_name;
            
                Texture2D tx = new Texture2D(75, 75);
                byte[] rewardPic = FileAccessUtil.LoadRewardPic(row.reward_name);              

                // If it is stock reward pic:
                if (rewardPic == null)
                {
                    Debug.Log("It's a stock pic");
                    img.texture = Resources.Load<Texture2D>("RewardPictures/" + row.reward_name);
                    alreadyLoadedImageList.Add(row.reward_name);

                    if (row.reward_url != "" || row.reward_url != null)
                    {
                        Debug.Log("Setting button name to reward url");
                        rb.name = row.reward_url;
                    }

                    return;
                }
                // Else it is a custom reward pic:
                else
                {
                    tx.LoadImage(rewardPic);
                    img.texture = tx;
                    customPanelImg.texture = tx;
                    customPanelText.text = rp.name;

                    if (row.reward_type == "website")
                    {
                        rb.name = row.reward_url;
                    }
                    else 
                    {
                        rb.name = row.reward_name;
                        rp.name = row.reward_name;
                        rt.text = row.reward_name;
                        Debug.Log("itsa custom");
                    }
                    
                    alreadyLoadedImageList.Add(row.reward_name);
                   
                    return;
                }                
            }            
        }
    }
    
    
    private void CreateWebview()
    {
        // Initialize UniWebView and set options
        var webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
        UniWebView.SetAllowInlinePlay(true);
        startTime = DateTime.Now;

        webView.OnPageStarted += (webView, url) => {            
            print("Loading started for url: " + url);
            gameLoop.controller.ReportWebRewardData(url, startTime, DateTime.Now);
            startTime = DateTime.Now;
        };


    }


    public void BeginBrowserRewardOne()
    {
        Debug.Log("reward1Obj.name = " + reward1Obj.name);

        exitButton.SetActive(true);

        // Check to see if countdown timer has already started, and if not then start countdown timer
        if (countdownActive == false)
        {
            countdownActive = true;
            StartCoroutine(BeginBrowserCount());
        }

        if (reward1Obj.name.Contains("http")) 
        {
            // Create WebView and load reward Url
            webView.ReferenceRectTransform = webviewRectReference;
            webView.Load(reward1Obj.name);
            webView.Show(true, UniWebViewTransitionEdge.Top, 2);
            isWebReward = true;
        }
        else if (reward1Obj.name == "" || reward1Obj.name == null)
        {
            Debug.Log(reward1Obj.name + " is Not a website reward. Launching custom reward panel");
            customRewardPanel1.SetActive(true);
            currentCustomRewardPanel = customRewardPanel1;
            isWebReward = false;
            customRewardNameText1.text = customRewardPanel1.name;
        }            
    }


    public void BeginBrowserRewardTwo()
    {
        Debug.Log("reward2Obj.name = " + reward2Obj.name);

        exitButton.SetActive(true);


        // Check to see if countdown timer has already started, and if not then start countdown timer
        if (countdownActive == false)
        {
            countdownActive = true;
            StartCoroutine(BeginBrowserCount());
        }

        if (reward2Obj.name.Contains("http"))
        {
            // Create WebView and load reward Url
            webView.ReferenceRectTransform = webviewRectReference;
            webView.Load(reward2Obj.name);
            webView.Show(true, UniWebViewTransitionEdge.Top, 2);
            isWebReward = true;
        }
        else if (reward2Obj.name == "" || reward2Obj.name == null)
        {
            Debug.Log(reward2Obj.name + " is Not a website reward. Launching custom reward panel");
            customRewardPanel2.SetActive(true);
            currentCustomRewardPanel = customRewardPanel2;
            isWebReward = false;
            customRewardNameText2.text = customRewardPanel2.name;
        }
    }


    public void BeginBrowserRewardThree()
    {
        Debug.Log("reward3Obj.name = " + reward3Obj.name);

        exitButton.SetActive(true);

        // Check to see if countdown timer has already started, and if not then start countdown timer
        if (countdownActive == false)
        {
            countdownActive = true;
            StartCoroutine(BeginBrowserCount());
        }

        if (reward3Obj.name.Contains("http"))
        {
            // Create WebView and load reward Url
            webView.ReferenceRectTransform = webviewRectReference;
            webView.Load(reward3Obj.name);
            webView.Show(true, UniWebViewTransitionEdge.Top, 2);
            isWebReward = true;
        }
        else if (reward3Obj.name == "" || reward3Obj.name == null)
        {
            Debug.Log(reward3Obj.name + " is Not a website reward. Launching custom reward panel");
            customRewardPanel3.SetActive(true);
            currentCustomRewardPanel = customRewardPanel3;
            isWebReward = false;
            customRewardNameText3.text = customRewardPanel3.name;
        }
    }


    public void BeginBrowserRewardFour()
    {
        exitButton.SetActive(true);


        // Check to see if countdown timer has already started, and if not then start countdown timer
        if (countdownActive == false)
        {
            countdownActive = true;
            StartCoroutine(BeginBrowserCount());
        }

        if (reward4Obj.name.Contains("http"))
        {
            // Create WebView and load reward Url
            webView.ReferenceRectTransform = webviewRectReference;
            webView.Load(reward4Obj.name);
            webView.Show(true, UniWebViewTransitionEdge.Top, 2);
            isWebReward = true;
        }
        else if (reward4Obj.name == "" || reward4Obj.name == null)
        {
            Debug.Log(reward4Obj.name + " is Not a website reward. Launching custom reward panel");
            customRewardPanel4.SetActive(true);
            currentCustomRewardPanel = customRewardPanel4;
            isWebReward = false;
            customRewardNameText4.text = customRewardPanel4.name;
        }
    }


    public void BeginBrowserRewardFive()
    {

        exitButton.SetActive(true);


        // Check to see if countdown timer has already started, and if not then start countdown timer
        if (countdownActive == false)
        {
            countdownActive = true;
            StartCoroutine(BeginBrowserCount());
        }

        if (reward5Obj.name.Contains("http"))
        {
            // Create WebView and load reward Url
            webView.ReferenceRectTransform = webviewRectReference;
            webView.Load(reward5Obj.name);
            webView.Show(true, UniWebViewTransitionEdge.Top, 2);
            isWebReward = true;
        }
        else if (reward5Obj.name == "" || reward5Obj.name == null)
        {
            Debug.Log(reward5Obj.name + " is Not a website reward. Launching custom reward panel");
            customRewardPanel5.SetActive(true);
            currentCustomRewardPanel = customRewardPanel5;
            isWebReward = false;
            customRewardNameText5.text = customRewardPanel5.name;
        }
    }


    public void BeginBrowserRewardSix()
    {
        exitButton.SetActive(true);

        // Check to see if countdown timer has already started, and if not then start countdown timer
        if (countdownActive == false)
        {
            countdownActive = true;
            StartCoroutine(BeginBrowserCount());
        }

        if (reward6Obj.name.Contains("http"))
        {
            // Create WebView and load reward Url
            webView.ReferenceRectTransform = webviewRectReference;
            webView.Load(reward6Obj.name);
            webView.Show(true, UniWebViewTransitionEdge.Top, 2);
            isWebReward = true;
        }
        else if (reward6Obj.name == "" || reward6Obj.name == null)
        {
            Debug.Log(reward6Obj.name + " is Not a website reward. Launching custom reward panel");
            customRewardPanel6.SetActive(true);
            currentCustomRewardPanel = customRewardPanel6;
            isWebReward = false;
            customRewardNameText6.text = customRewardPanel6.name;
        }        
    }


    private void UpdateTimeRemainingBar(float timeRemaining)
    {
        // Time remaining countdown bar. Begins full and decreases as the countdown progresses.
        float percentToFill = ((playlistObj.duration * 60) - timeRemaining) / (playlistObj.duration * 60);
        TimeRemainingBar.fillAmount = percentToFill;
    }    


    private IEnumerator BeginBrowserCount()
    {
        // for (int x = 0; x < ((playlistObj.duration * 60)); x++)
        for (int x = 0; x < 3; x++) // testing shortcut. Reduces the reward timer to 10 seconds.

        {
            yield return new WaitForSeconds(1);
            // Debug.Log(x);

            UpdateTimeRemainingBar(x);
            
            // Countdown audio
            if (x == ((playlistObj.duration * 60) - 32)) { gameLoop.PlayAudio("30"); }
            if (x == ((playlistObj.duration * 60) - 27)) { gameLoop.PlayAudio("25"); }
            if (x == ((playlistObj.duration * 60) - 22)) { gameLoop.PlayAudio("20"); }
            if (x == ((playlistObj.duration * 60) - 17)) { gameLoop.PlayAudio("15"); }
            if (x == ((playlistObj.duration * 60) - 12)) { gameLoop.PlayAudio("10"); }
            if (x == ((playlistObj.duration * 60) - 11)) { gameLoop.PlayAudio("9"); }
            if (x == ((playlistObj.duration * 60) - 10)) { gameLoop.PlayAudio("8"); }
            if (x == ((playlistObj.duration * 60) - 9)) { gameLoop.PlayAudio("7"); }
            if (x == ((playlistObj.duration * 60) - 8)) { gameLoop.PlayAudio("6"); }
            if (x == ((playlistObj.duration * 60) - 7)) { gameLoop.PlayAudio("5"); }
            if (x == ((playlistObj.duration * 60) - 6)) { gameLoop.PlayAudio("4"); }
            if (x == ((playlistObj.duration * 60) - 5)) { gameLoop.PlayAudio("3"); }
            if (x == ((playlistObj.duration * 60) - 4)) { gameLoop.PlayAudio("2"); }
            if (x == ((playlistObj.duration * 60) - 3)) { gameLoop.PlayAudio("1"); }


            


        }
        StartCoroutine(StopBrowserReward());
    }     


    private IEnumerator StopBrowserReward()
    {
        if (isWebReward = true)
        {            
            // Trigger fade out webview animation
            webView.Hide(true, UniWebViewTransitionEdge.Top, 3);
            yield return new WaitForSeconds(3);
        }
        else
        {
            // Hide any open custom reward panel and set the variable to null
            currentCustomRewardPanel.SetActive(false);
            
        }
              
        // Destroy the webview and release from memory
        Destroy(webView);
        webView = null;

        // Null any current CustomRewardPanel
        currentCustomRewardPanel = null;

        // Reset the countdown bool
        countdownActive = false;

        //Activate continue prompt modal
        continueModal.SetActive(true);

        // Inform Game Loop of completion
        gameLoop.PlayEntryCompleted(TYPE);
    }


    public void OnExitButton()
    {
        if (isWebReward == true)
        {
            // Hide the webview, trigger fade out animation slide to top edge of screen
            webView.Hide(true, UniWebViewTransitionEdge.Top, 1);
        }
        else
        {
            currentCustomRewardPanel.SetActive(false);
        }

        exitButton.SetActive(false);


    }



}
