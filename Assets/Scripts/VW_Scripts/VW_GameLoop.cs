using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the View script for the primary Game Loop scene. Some of its
/// responsibilities are to manage static UI elements in the scene, track the
/// number of rounds, track which play list entry we are on, and calls Resume on 
/// any of the activity helper scripts that are running.
/// <seealso cref="AB_GameHelper"/>
/// </summary>
public class VW_GameLoop : MonoBehaviour {

    private const string PIN_CODE_KEY = "pinCode";
    private const int RAND_LOWER_BOUND = 1;
    private const int RAND_UPPER_BOUND = 4;

    private List<int> roundsTillRewardList;

    // Create your constants
    private const int STARTING_ITERATION = 1;
    private const int FIRST_ROUND = 1;
    private const int ZERO_INDEX = 0;

    // Create the local vars
    public int duration;
    public int playIndex;
    private float totalRoundCount;
    public float currentRound;
    private bool passLocked = false;
    private bool frontLocked = false;
    private bool infiniteLoop = false;
    public int loopIterations;
    private int maxIterations;
    private float playTimer;
    public float roundsTillReward;
    private bool newPart = true;
    public float totalRoundsThisPart;
    
    public CON_GameLoop controller;
    public AudioSource audioSource;
    public GameObject continueModal;
    public GameObject passCodeModal;
    public GameObject quitModal;

    public GameObject activeCanvas = null;
    public int activeTypeId = -1;
    
    // Timer that starts as soon as the scene is loaded
    private double gameTimer = 0.0;
    private const string CANVAS = "Canvas_";
    private const int XY_DIMENSION = 300;
    private Texture2D customTexture;

    // Get all scene game objects
    [SerializeField] GameObject invalidPinModal;
    [SerializeField] GameObject canvasContainer;
    [SerializeField] Image roundCounter;
    [SerializeField] InputField pinField;
    [SerializeField] GameObject playlistEmptyPanel;
    [SerializeField] GameObject parentalGatePanel;

    

    ////Previously playing reward video id & time in video
    //private string previousPlayingVideoId = null;
    //public string PreviousPlayingVideo {
    //    get {return previousPlayingVideoId;}
    //    set {previousPlayingVideoId = value;}
    //}
    //private DO_Video previousPlayingVideoData = null;
    //public DO_Video PreviousPlayingVideoData {
    //    get {return previousPlayingVideoData;}
    //    set {previousPlayingVideoData = value;}
    //}
    //private int timeInVideo = 0;
    //public int TimeInVideo {
    //    get {return timeInVideo;}
    //    set {timeInVideo = value;}
    //}

    // Use this for initialization
    void Start () {
    
        // Grab the master and the controller
        MAS_GameLoop tempMaster = (MAS_GameLoop)COM_Director.GetMaster("MAS_GameLoop");
        controller = (CON_GameLoop)tempMaster.GetController("CON_GameLoop");

        if (controller == null)
        {
            Debug.Log("****************************\n LOOP: controller was NULL!!!! \n ************************");
        }
        else
        {
            Debug.Log("****************************\n LOOP: controller is GOOD! \n ************************");
        }

        // Hide the empty playlist panel
        playlistEmptyPanel.SetActive(false);


        // Get the total round count for this play list
        totalRoundCount = controller.GetRoundCount();
        playIndex = ZERO_INDEX;
        loopIterations = STARTING_ITERATION;
        passLocked = controller.IsPassLocked();
        maxIterations = controller.GetIterationNumber();
        customTexture = new Texture2D(XY_DIMENSION, XY_DIMENSION);

        Debug.Log(string.Format("Pass locked is {0} and loop iterations are {1} and infinite loop is {2}", passLocked, maxIterations, infiniteLoop));

        Debug.Log("VW: Total round count is " + totalRoundCount.ToString());

        if (totalRoundCount == 0)
        {
            playlistEmptyPanel.SetActive(true);
        }
        else if (passLocked && frontLocked)
        {
            Debug.Log("****************************\n COROUTINE START \n***********************************");
            StartCoroutine(PauseThenContinue());
        }
        else
        {
            Debug.Log("****************************\n UPDATING BAR \n***********************************");
            UpdateProgressBar(0);
            Debug.Log("****************************\n BAR UPDATED! ACTIVATING CANVASSES \n***********************************");

            ActivateDeactivateCanvass(controller.GetPlayEntry(playIndex).type_id);
        }

        UpdateProgressBar(CalculateRoundsTillReward());
     

        Debug.Log("****************************\n Game Loop View STARTED! \n***********************************");


    }

    //Time.deltaTime will increase the value with 1 every second
    void Update() { gameTimer += Time.deltaTime; }


    /// <summary>
    /// Calculates the total number of rounds remaining until reaching a reward playlist entry. 
    /// This is used to properly configure the progress bar.
    /// </summary>
    private float CalculateRoundsTillReward()
    {         

        List<float> roundsTillRewardList = new List<float>();
        for (float i = currentRound ; i < totalRoundCount; i++)
        {
            DO_PlayListObject tempPlayObj = controller.GetPlayEntry(Mathf.RoundToInt(i));

            if (!(tempPlayObj.type_id == 1))
            {
                roundsTillRewardList.Add(i);
            }
            else
            {
                break;
            }
        }
        
        roundsTillReward = roundsTillRewardList.Count;
        Debug.Log("Round till reward = " + roundsTillReward);
        if (newPart == true)
        {
            totalRoundsThisPart = roundsTillReward;
            newPart = false;
        }
        Debug.Log("newPart = " + newPart + ", totalRoundsThisPart = " + totalRoundsThisPart);

        return roundsTillReward;
    }

    public void ExitToStartMenu() { controller.SceneChange("startup_menu"); }

    public void DeactivateContinueModal()
    {
        StartCoroutine(DeactivateContinueModalHelper());
        return;
    }

    public IEnumerator DeactivateContinueModalHelper()
    {
        // allow time for buttonwiggle animation to play
        yield return new WaitForSeconds(0.2f);
        continueModal.SetActive(false);
        
    }    

    public void ClosePassCodeModal() { passCodeModal.SetActive(false); }

    public void ClosePinModal() { invalidPinModal.SetActive(false); }

    /// <summary>
    /// Deactivates the quit model and reactivates the current active canvas
    /// </summary>
    public void UnpauseGame()
    {
        //if (activeCanvas != null)
        //{
        //    activeCanvas.SetActive(true);
        //}

        quitModal.SetActive(false);
    }

    /// <summary>
    /// Activates the quit model and deactivates the current active canvas
    /// </summary>
    public void PauseGame()
    {
        //if (activeCanvas != null)
        //{
        //    activeCanvas.SetActive(false);
        //}

        quitModal.SetActive(true);
    }

    /// <summary>
    /// Retrieves the Play List data object corresponding to the current index.
    /// Called by activity helpers to retrieve their play list data.
    /// </summary>
    /// <returns>Play list data object</returns>
    public DO_PlayListObject GetCurrentPlay() { return controller.GetPlayEntry(playIndex); }

    

    /// <summary>
    /// Plays the audio clip corresponding to the string parameter.
    /// </summary>
    /// <param name="word"></param>
    public void PlayAudio(string word)
    {
        AudioClip tempClip = FileAccessUtil.LoadWordAudio(word + ".wav");

        if (tempClip != null)
        {
            audioSource.clip = tempClip;
            audioSource.Play();
        }
        else
        {
            tempClip = Resources.Load<AudioClip>("Sound/" + word);

          
               if (tempClip != null)
                    {
                        audioSource.clip = tempClip;
                        audioSource.Play();
                    }
                    else
                    {
                        //Do nothing
                    }

                audioSource.clip = tempClip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// This function takes the name of an image (usually the word itself), a RawImage Game Object, 
    /// and some optional XY dimensions. The imageName is used to recover a byte array and load 
    /// a png from it for display. Otherwise, it will attempt to load from assets. The optional
    /// dimensions are for the resizing of the image. A manual imgNum must be provided to fetch 
    /// a specific image from resources.
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="obj"></param>
    /// <param name="imgNum"></param>
    /// <param name="xDelta"></param>
    /// <param name="yDelta"></param>
    public void SetManualImage(string imageName, GameObject obj, int imgNum, int xDelta = XY_DIMENSION, int yDelta = XY_DIMENSION) {
        Debug.Log("*******************************************\nIMG: IMAGE NAME IS " + imageName + imgNum);
        int xVect = xDelta, yVect = yDelta;

        //bool success = true;
        byte[] bytes = null;

        Debug.Log("*******************************************\nIMG: RANDO WAS 1 grabbing from assets \n********************************************");

        // Grab a stock texture
        Texture2D tempTex = Resources.Load<Texture2D>("WordPictures/" + imageName + "/" + imageName + imgNum);

        // If the stock texture is not null use it else grab a random user image
        if (tempTex != null) {
            Debug.Log("*******************************************\nIMG: ASSET RESOURCE WAS GOOD! appyling \n********************************************");
            obj.GetComponent<RawImage>().texture = tempTex;
        } else {
            Debug.Log("*******************************************\nIMG: COULD NOT LOAD FROM assets! Grabbing random image  \n********************************************");
            SetRandomImage(bytes, obj, imageName);
        }

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(xVect, yVect);
    }

    /// <summary>
    /// This function takes the name of an image (usually the word itself), a RawImage Game Object, 
    /// and some optional XY dimensions. The imageName is used to recover a byte array and load 
    /// a png from it for display. Otherwise, it will attempt to load from assets. The optional
    /// dimensions are for the resizing of the image.
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="obj"></param>
    /// <param name="xDelta"></param>
    /// <param name="yDelta"></param>
    public void SetImage(string imageName, GameObject obj/*, int xDelta = XY_DIMENSION, int yDelta = XY_DIMENSION*/)
    {
        Debug.Log("*******************************************\nIMG: IMAGE NAME IS " + imageName);
        //int xVect = xDelta, yVect = yDelta;
        System.Random rand = new System.Random();

        //TEMP CODE
        bool isRandom = true;
        //REMOVE LATER

        //bool success = true;
        byte[] bytes = null;

        // Check if load random option is checked
        if (isRandom)
        {
            // Choose randomly whether stock image will be used or not. Weight it towards non stock
            if (rand.Next(RAND_LOWER_BOUND, RAND_UPPER_BOUND) == 1)
            {
                Debug.Log("*******************************************\nIMG: RANDO WAS 1 grabbing from assets \n********************************************");

                // Grab a stock texture
                Texture2D tempTex = Resources.Load<Texture2D>("WordPictures/" + imageName + "/" + imageName + UnityEngine.Random.Range(1,6));

                // If the stock texture is not null use it else grab a random user image
                if (tempTex != null)
                {
                    Debug.Log("*******************************************\nIMG: ASSET RESOURCE WAS GOOD! appyling \n********************************************");
                    obj.GetComponent<RawImage>().texture = tempTex;
                }
                else
                {
                    Debug.Log("*******************************************\nIMG: COULD NOT LOAD FROM assets! Grabbing random image  \n********************************************");
                    SetRandomImage(bytes, obj, imageName);
                }
            }
            else
            {
                // Grab a random user image
                SetRandomImage(bytes, obj, imageName);
            }
        }
        else
        {
            if (activeTypeId == 1)
            {
                Debug.Log("*******************************************\nIMG: ACTIVE TYPE WAS REWARD \n********************************************");
                bytes = FileAccessUtil.LoadRewardPic(imageName);
            }
            else
            {
                bytes = FileAccessUtil.LoadWordPic(imageName);
            }

            if (bytes != null)
            {
                Debug.Log("*******************************************\nIMG: bYTES ARE GOOD! \n********************************************");

                if (customTexture.LoadImage(bytes))
                {
                    obj.GetComponent<RawImage>().texture = customTexture;
                }
                else
                {
                    // TODO: Add some sort of error. Blah.
                    return;
                }
            }
            else
            {
                Debug.Log("*******************************************\nIMG: BYTES WERE NULL \n********************************************");
                obj.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("WordPictures/" + imageName + "/" + imageName + UnityEngine.Random.Range(1, 6));
                //customTexture = Resources.Load<Texture2D>("WordPictures/" + imageName);
            }
        }

        //obj.GetComponent<RawImage>().texture = customTexture;
        RectTransform rt = obj.GetComponent<RectTransform>();
        //rt.sizeDelta = new Vector2(xVect, yVect);
    }



    /// <summary>
    /// This function is called by an activity helper when it finishes a round, but
    /// not all the total rounds in a play list.
    /// </summary>
    public void RoundCompleted()
    {
        
        Debug.Log("round completed");
        currentRound++;
        UpdateProgressBar(CalculateRoundsTillReward());


        
        if ((currentRound <= totalRoundCount) && (activeTypeId !=5))
        {
            continueModal.SetActive(true);
        }
    }

    /// <summary>
    /// This function is called by activity helpers once they are finished all 
    /// the rounds in a play list. Upon completion of all rounds in all the play
    /// list entries it will increment the number of iterations if appilcable or 
    /// block the user with the pass lock modal.
    /// <seealso cref="CON_GameLoop.PlayListCompleted(int)"/>
    /// </summary>
    /// <param name="completedTypeId"></param>
    public void PlayEntryCompleted(int completedTypeId)
    {

        Debug.Log("playlist entry completed");
        currentRound++;
        playIndex++;
        roundCounter.gameObject.SetActive(true);
        UpdateProgressBar(CalculateRoundsTillReward());

        // Check if we have completed the playlist
        if (controller.PlayListCompleted(playIndex))
        {
            Debug.Log("Playlist completed");
            ResetRoundListCount();
           
            if (passLocked && (loopIterations >= maxIterations) && controller.PinsExist())
            {
              loopIterations = STARTING_ITERATION;
              passCodeModal.SetActive(true);
              parentalGatePanel.SetActive(true);
            
            }
            else
            {
                // Increment loop iteration
                loopIterations++;
            }
            continueModal.SetActive(true);
        }
        else
        {
            continueModal.SetActive(true);
        }

        int nextTypeId = controller.GetPlayEntry(playIndex).type_id;
        if (nextTypeId == 1)
        {
            newPart = true;
            roundCounter.fillAmount = 1;

        }
    

        ActivateDeactivateCanvass(nextTypeId);
    }

    /// <summary>
    /// Takes the pin from the pin input field and verifies that it is the right pin before
    /// closing the pass code modal
    /// </summary>
    public void VerifyPin()
    {
        int pin = short.Parse(pinField.text);

        // If pin matches then deactivate the pass code modal and continue; else, block.
        if (controller.PinGood(pin))
        {
            passCodeModal.SetActive(false);
        }
        else
        {
            invalidPinModal.SetActive(true);
        }

        pinField.text = "";
    }



    /// <summary>
    /// Returns true if the script attached to the canvas is still active.
    /// </summary>
    /// <param name="canvas"></param>
    /// <returns>bool</returns>
    private bool IsScriptActive(GameObject canvas) { return canvas.GetComponent<AB_GameHelper>().enabled; }

    /// <summary>
    /// Resets all the vars that track round and player progress.
    /// </summary>
    private void ResetRoundListCount()
    {

        Debug.Log("Reseting round list count");
        playIndex = ZERO_INDEX;
        UpdateProgressBar(0);
        currentRound = FIRST_ROUND;
    }

    /// <summary>
    /// This function is called at each play list entry transition and upon
    /// entering the Game Loop scene.
    /// <seealso cref="SetActiveCanvas(int)"/>
    /// </summary>
    /// <param name="nextTypeId"></param>
    private void ActivateDeactivateCanvass(int nextTypeId)
    {
        // Check if current canvass is null and if next canvas
        if (activeCanvas == null)
        {
            Debug.Log("No active canvas is currently set. Active canvas is Null. Now setting to nextTypeId: " + nextTypeId);

            SetActiveCanvas(nextTypeId);
        }
        else
        {
            Debug.Log("Active canvas is not null. Currntly set to : " + activeCanvas.name);
            Debug.Log("Now setting " + activeCanvas.name + " to SetActive(false)");

            activeCanvas.SetActive(false);
            StartCoroutine(PauseBeforeCanvasActivation(nextTypeId));
        }
    }

    /// <summary>
    /// Sets the active canvas and activates it. Additionally, it checks if the 
    /// helper script attached to the canvas object is active and if so will call
    /// resume. 
    /// </summary>
    /// <param name="nextTypeId"></param>
    private void SetActiveCanvas(int nextTypeId)
    {

        Debug.Log("Setting active canvas to : " + nextTypeId);
        activeCanvas = canvasContainer.transform.Find(CANVAS + nextTypeId).gameObject;
        activeTypeId = nextTypeId;
        Debug.Log("Active type ID is now set to : " + activeTypeId);


        if (IsScriptActive(activeCanvas))
        {
            Debug.Log(activeCanvas + " script is active. Launching it's 'Resume' method now.");

            activeCanvas.GetComponent<AB_GameHelper>().Resume();
        }
        else
        {
            Debug.Log(activeCanvas + " script is not active. Enabling now.");

            activeCanvas.GetComponent<AB_GameHelper>().enabled = true;
        }

        activeCanvas.SetActive(true);

        Debug.Log(activeCanvas + " is now SetActive.");

    }

    ///<summary> 
    /// Adjusts the visual round counter based on how rounds have been completed
    ///</summary>
    private void UpdateProgressBar(float roundCount)
    {
        //generate round counter
        float percentToFill = ((totalRoundsThisPart - roundCount) / totalRoundsThisPart);
        roundCounter.fillAmount = percentToFill;
        Debug.Log("roundCount = " + roundCount + ", totalRoundsThisPart = " + totalRoundsThisPart);
    }


    /// <summary>
    /// Pauses gameloop while the passcode modal is open
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator PauseThenContinue()
    {
        // Activate the pass code modal and wait till user deactivates it
        passCodeModal.SetActive(true);
        parentalGatePanel.SetActive(true);
        yield return new WaitWhile(() => passCodeModal.activeSelf);

        UpdateProgressBar(currentRound);
        ActivateDeactivateCanvass(controller.GetPlayEntry(playIndex).type_id);
    }

    /// <summary>
    /// Pauses code execution when transitioning between rounds or play list
    /// iterations.
    /// <seealso cref="SetActiveCanvas(int)"/>
    /// </summary>
    /// <param name="nextTypeId"></param>
    /// <returns>IEnumerator</returns>
    private IEnumerator PauseBeforeCanvasActivation(int nextTypeId)
    {
        Debug.Log("Inside PauseBeforeCanvasActivation coroutine. Passed with nextTypeId: " + nextTypeId);
        yield return new WaitWhile(() => continueModal.activeSelf);
        yield return new WaitWhile(() => passCodeModal.activeSelf);

        SetActiveCanvas(nextTypeId);

    }

    /// <summary>
    /// Grabs a byte array and if it is not null creates an image to use.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="obj"></param>
    /// <param name="imageName"></param>
    private void SetRandomImage(byte[] bytes, GameObject obj, string imageName)
    {
        bytes = controller.LoadRandomWordPic(imageName.ToLower());

        if (bytes != null)
        {
            Debug.Log("*******************************************\nIMG: bYTES ARE GOOD! \n********************************************");

            if (customTexture.LoadImage(bytes))
            {
                Debug.Log("*******************************************\nIMG: LOADED IMAGE!!!!\n********************************************");
                obj.GetComponent<RawImage>().texture = customTexture;
            }
            else
            {
                Debug.Log("*******************************************\nIMG: COULD NOT LOAD IMAGE SEARCHING FOR DEFAULT \n********************************************");

                obj.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("WordPictures/" + imageName + "/" + imageName + UnityEngine.Random.Range(1,6));
            }
        }
        else
        {
            Debug.Log("*******************************************\nIMG: COULD NOT LOAD IMAGE SEARCHING FOR DEFAULT \n********************************************");

            obj.GetComponent<RawImage>().texture = Resources.Load<Texture2D>("WordPictures/" + imageName + "/" + imageName + UnityEngine.Random.Range(1, 6));
        }
    }
}