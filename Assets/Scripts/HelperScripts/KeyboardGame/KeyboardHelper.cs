using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardHelper : AB_GameHelper, IWordSolved
{
    // Constants
    const int NUM_LETTERS = 26;
    const string WORD_COMPLETE_AUDIO = "good_job";
    private const int pauseTime = 2;
    const float bigword = 0.9f;
    const float smallword = 1.2f;
    const float smallestword = 0.8f;


    // Public Variables
    public bool gameComplete;
    public int numKeysPressed = -1;
    public int numErrors = 0;
    public int totalErrors = 0;
    public int currentSlotIndex;
    public bool snapBack { get; set; }

    // Private Variables
    public DO_KeyboardGame keyboard;
    public DO_PlayListObject playObj;
    private KeyCollection collector;
    private List<string> words;
    private List<GameObject> itemObjects;
    private Transform endingSlotsGrid;
    private Transform startingSlotsGrid;
    private bool isKeyboardEnabled;
    private double totalSolveTimes = 0.0; // Stores the total solve time for a game (not yet implemented)
    private int currentRound;
    private string word;


    // Serializable Objects
    [SerializeField] Canvas parentCanvas;
    [SerializeField] Transform childPanel;
    [SerializeField] GameObject picture;
    [SerializeField] Transform keyboardPrefab;
    [SerializeField] VW_GameLoop looper;
    [SerializeField] GameObject keyboardObject;
    [SerializeField] GameObject provided;



    // Stores the times it took to solve each word
    List<double> wordSolveTimes = new List<double>();

    void Start()
    {
        Debug.Log("Keyboard Start method");

        //endingSlotsGrid = childPanel.Find("ToSolve");
        //startingSlotsGrid = childPanel.Find("Provided");

        //// Activate the Letter Collection and Slot script
        //gameObject.GetComponent<KeyCollection>().enabled = true;

        //// Grab the required data for the variables and generate a new round
        //Restart();

        //gameComplete = false;
    }

    void Update()
    {
        // Disable the picture in word scramble panel if quit modal is open
        picture.GetComponent<RawImage>().enabled = !looper.quitModal.activeSelf;
        //.SetActive(!gameLoop.quitModal.activeSelf);

        if (keyboard.provided == true)
        {
            startingSlotsGrid.gameObject.SetActive(!looper.quitModal.activeSelf);

        }
        else
        {
            startingSlotsGrid.gameObject.SetActive(false);

        }
        endingSlotsGrid.gameObject.SetActive(!looper.quitModal.activeSelf);
    }

    private bool IsGameOver() { return currentSlotIndex == word.Length; }

    public override void Resume()
    {
        Debug.Log("Keyboard Resume method");

        Restart();
    }

    public bool CheckForMatch(string checkWord) { return checkWord.Equals(words[currentRound - 1]); }
    //public bool CheckForMatch(string checkWord) { return checkWord.Equals("camel"); }

    #region DisableAllKeys implementation
    // Disable all the keys on the keyboard prefab
    public void DisableAllKeys()
    {
        // Loop through each key and ensure it is disabled
        for (int i = 0; i < NUM_LETTERS; i++)
        {
            var keyButton = keyboardPrefab.GetChild(i).gameObject.GetComponent<Button>();
            keyButton.interactable = false;
        }
        isKeyboardEnabled = false;
    }
    #endregion

    #region EnableAllKeys implementation
    // Enable all the keys on the keyboard prefab
    public void EnableAllKeys()
    {
        // For each key, ensure that it is enabled
        for (int i = 0; i < NUM_LETTERS; i++)
        {
            var keyButton = keyboardPrefab.GetChild(i).gameObject.GetComponent<Button>();
            keyButton.interactable = true;
        }
        isKeyboardEnabled = true;
    }
    #endregion

    #region  EnableKeyboardButton implementation
    // Enable the key that matches the next letter to be typed
    public void EnableKeyboardButton(string key)
    {
        // Loop through each key and enable the key that matches the next letter to solve
        for (int i = 0; i < NUM_LETTERS; i++)
        {
            if (keyboardPrefab.GetChild(i).name.ToString().ToLower() == key)
            {
                var keyButton = keyboardPrefab.GetChild(i).gameObject.GetComponent<Button>();
                keyButton.interactable = true;
            }
        }
    }
    #endregion

    #region GenerateRound implementation
    // Setup the game. Generate the slots and play the audio clip of the word
    public void GenerateRound()
    {
        Debug.Log("Starting GenerateRound...");

        // Grab the new word
        word = words[currentRound - 1];
        Debug.Log("Word is " + word);


        // Set the image in the picture game object and then activate it
        looper.SetImage(word, picture);
        picture.SetActive(true);
        keyboardObject.SetActive(true);

        // Set the image in the picture game object and then activate it
        //looper.SetImage(word, picture);//GetRandomTexture(word), picture);

        Debug.Log("*******************\nKEYBOARD: WORD is " + word + "\n***********************");

        //generate all slots
        for (int i = 0; i < word.Length; ++i)
        {
            //Debug.Log("For loop entered");

            //Creates a letter from the letter prefab 
            string letter = word[i].ToString().ToUpper();
            GameObject l = GameObject.Instantiate((GameObject)Resources.Load(("Letter Prefabs/Letter_" + letter)));

            //Sets the parent of the letter to its starting slot with a false flag to use the letter's own scaling
            l.transform.SetParent(startingSlotsGrid, false);
            Destroy(l.GetComponent<DragHandler>());
            l.GetComponent<CanvasGroup>().alpha = 0.3f;

            //Generates the solve slots associated with the word and in the correct order
            GameObject obj2 = GameObject.Instantiate((GameObject)Resources.Load("Slot"), endingSlotsGrid);
            obj2.name = word[i].ToString().ToUpper();
            obj2.transform.SetParent(endingSlotsGrid);
        }


        if (word.Length < 5)
        {
            startingSlotsGrid.localScale = new Vector3(0.9f, 0.9f);
            endingSlotsGrid.localScale = new Vector3(1.2f, 1.2f);
        }
        else if ((word.Length >= 5) && (word.Length < 7))
        {
            startingSlotsGrid.localScale = new Vector3(0.8f, 0.8f);
            endingSlotsGrid.localScale = new Vector3(0.9f, 0.9f);
        }
        else if ((word.Length >= 7) && (word.Length < 10)) 
        {
            startingSlotsGrid.localScale = new Vector3(0.7f, 0.7f);
            endingSlotsGrid.localScale = new Vector3(0.8f, 0.8f);
        }
        else if (word.Length == 10)
        {
            startingSlotsGrid.localScale = new Vector3(0.7f, 0.7f);
            endingSlotsGrid.localScale = new Vector3(0.7f, 0.7f);
        }

        ToggleKeyboardColor();

        ToggleProvided();

        looper.PlayAudio(word);
    }



    #region HighlightStartingSlotLetter implementation
    // Highlights the next letter in the starting slot
    public void HighlightStartingSlotLetter(int index)
    {
        Debug.Log("HighlightStartingSlotLetter index is: " + index);

        if (gameComplete)
        {
            return;
        }

        else if (currentSlotIndex < startingSlotsGrid.childCount)
        {
            
            // Change the alpha from 0.5 to 1 of the letter at the provided slot
            startingSlotsGrid.GetChild(index).GetComponent<CanvasGroup>().alpha = 1;
            startingSlotsGrid.GetChild(index).GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f);


            if (currentSlotIndex > 0)
            {
                startingSlotsGrid.GetChild(index - 1).GetComponent<CanvasGroup>().alpha = 0.2f;
                startingSlotsGrid.GetChild(index - 1).GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f);

            }


            //if (keyboard.letterName == true)
            //{
            //    PlayKeyboardAudio(index);

            //}
        }
        else
        {
            currentSlotIndex = 0;
        }
    }
    #endregion

    #region IExecuteKeyboard implementation
    // Execute the keyboard event
    private IEnumerator ExecuteKeyboard()
    {
        // Play the audio of the word and wait for it to finish
        yield return new WaitWhile(() => looper.audioSource.isPlaying);

        // Pause for three seconds
        //yield return new WaitForSeconds(pauseTime);

        currentRound++;

        Debug.Log("*******************\nKeyboard: Keyboard current round now is: " + currentRound + "\n***********************");
                     
        gameObject.SetActive(false);

        // Broadcast either the game done message or round done message
        //Messenger.Broadcast(currentRound > words.Count ? GAME : ROUND);
    }
    #endregion

    #region IGameOver implementation
    // Called when the game is complete
    public IEnumerator GameOver()
    {
        yield return new WaitWhile(() => looper.audioSource.isPlaying);

        yield return new WaitForSeconds(0.5f);
        looper.PlayAudio(WORD_COMPLETE_AUDIO);
        yield return new WaitWhile(() => looper.audioSource.isPlaying);
        //Messenger.Broadcast("GameDone");
    }
    #endregion

    #region IMakeKeyTransparent implementation
    // Briefly make the correct key transparent to aid the user
    public IEnumerator MakeKeyTransparent(int index, float seconds)
    {
        string letterName = endingSlotsGrid.GetChild(index).name.ToString().ToLower();

        // Loop through every key to find the index of the key the matches the correct letter
        for (int i = 0; i < NUM_LETTERS; i++)
        {
            // If there is a match, set the interactable component of the button to false to make it look transparent
            if (keyboardPrefab.GetChild(i).name.ToString().ToLower() == letterName)
            {
                var keyButton = keyboardPrefab.GetChild(i).gameObject.GetComponent<Button>();
                keyButton.interactable = false;// (The button may still be pressed due to the event listener)

                yield return new WaitForSeconds(seconds);

                keyButton.interactable = true;
            }
        }
    }
    #endregion

    private IEnumerator SolveWord()
    {
        // Play audio and sleep
        yield return new WaitWhile(() => looper.audioSource.isPlaying);
        yield return new WaitForSeconds(0.5f);
        looper.PlayAudio(word);
        yield return new WaitWhile(() => looper.audioSource.isPlaying);
        yield return new WaitForSeconds(0.5f);

        // IS THIS WHERE WE CAN ADD CODE TO PLAY THE LETTER NAME AUDIO IN SEQUENCE???

        currentRound++;
        picture.SetActive(false);
        ClearSlots(startingSlotsGrid);
        ClearSlots(endingSlotsGrid);
        keyboardObject.SetActive(false);

       


        //Check if it is the last round
        if (currentRound > playObj.duration)
        {

            // Report the round data to the controller
            looper.controller.ReportRoundErrorCount("Keyboard", word, totalErrors);
            totalErrors = 0;
            
            //ResetGameData();
            looper.PlayEntryCompleted(playObj.type_id);
            gameComplete = false;
        }
        else
        {
            // Report the round data to the controller
            looper.controller.ReportRoundErrorCount("Keyboard", word, totalErrors);
            totalErrors = 0;

            // Inform game loop view that round was completed so it can display the continue modal
            looper.RoundCompleted();

            // Yield and wait till continue modal is deactivated
            yield return new WaitWhile(() => looper.continueModal.activeSelf);

            gameComplete = false;
            //Messenger<string>.Broadcast("GameComplete", "Complete");

            //ResetGameData();

            GenerateRound();
        }
    }


    #region IsKeyboardEnabled implementation
    // Check if the keyboard is enabled
    public bool IsKeyboardEnabled()
    {
        return isKeyboardEnabled;
    }
    #endregion

    #region IsSolvingIndexEmpty implementation
    // Returns true if the slot at the given index is empty. False otherwise
    public bool IsSolvingIndexEmpty(int index)
    {
        if (endingSlotsGrid.GetChild(index).transform.childCount == 0)
        {
            return true;
        }
        return false;
    }
    #endregion



    /* ************ INCOMPLETE START ******************* */
    // If there are leftovers from the previous round
    //if (startingSlotsGrid.childCount > 0)
    //{
    //    int letterDifferenceCount; // The difference between the previous number of letters vs the current number
    //    int wordLetterOffset;
    //    int currentLetterCount = word.Length;
    //    int previousLetterCount = startingSlotsGrid.childCount;

    //    Debug.Log("Has child");

    //    // Current word is shorter than the number of child elements
    //    if (currentLetterCount < previousLetterCount)
    //    {
    //        // Delete extra slots
    //        letterDifferenceCount = previousLetterCount - currentLetterCount;
    //        wordLetterOffset = currentLetterCount + letterDifferenceCount;
    //        Debug.Log("Letter Difference Count is: " + letterDifferenceCount);
    //    }

    //    // Current word is the same length as the number of child elements
    //    else if (currentLetterCount == previousLetterCount)
    //    {
    //        letterDifferenceCount = 0;
    //    }

    //    // Current word is longer than the number of child elements
    //    else if (currentLetterCount > previousLetterCount)
    //    {
    //        // Add Slots

    //        letterDifferenceCount = currentLetterCount - previousLetterCount;
    //        Debug.Log("Letter Difference Count is: " + letterDifferenceCount);
    //        for(int i = 0; i < currentLetterCount; i++)
    //        {
    //            if (i < previousLetterCount) // If there is already a letter there
    //            {
    //                // Delete the letter in the slot

    //                // Load the letter into the slot

    //                //Creates a letter from the letter prefab 
    //                string letter = word[i].ToString().ToUpper();
    //                GameObject l = GameObject.Instantiate((GameObject)Resources.Load(("Letter Prefabs/Letter_" + letter)));

    //                //Sets the parent of the letter to its starting slot with a false flag to use the letter's own scaling
    //                l.transform.SetParent(startingSlotsGrid, false);
    //                Destroy(l.GetComponent<DragHandler>());
    //                l.GetComponent<CanvasGroup>().alpha = 0.5f;

    //            }
    //            else // 
    //            {
    //                // Create the slot
    //                // Load the letter into the slot

    //            }

    //        }
    //    }

    //}
    /* ************ INCOMPLETE END ******************* */

    #endregion

    #region PlayKeyboardAnimation implementation
    // Loop through each letter and find the correct letter to display the animation on
    public void PlayKeyboardAnimation(string key)
    {
        // If the key matches the next letter to be typed, play the animmation on that letter
        for (int i = 0; i < NUM_LETTERS; i++)
        {
            if (keyboardPrefab.GetChild(i).name.ToString().ToLower() == key)
            {
                keyboardPrefab.GetChild(i).gameObject.GetComponent<Animation>().Play();
            }
        }
    }
    #endregion

    #region PlayKeyboardAudio implementation
    // Play the audio of the correct key
    public void PlayKeyboardAudio(int index)
    {
        string letterName = endingSlotsGrid.GetChild(index).name.ToString().ToLower();

        // Loop through the keyboard to play the correct audio attached to the key
        for (int i = 0; i < NUM_LETTERS; i++)
        {
            if ((keyboardPrefab.GetChild(i).name.ToString().ToLower() == letterName) && (keyboard.letterName == true))
            {
                looper.PlayAudio("Letter " + letterName);
            }
        }
    }
    #endregion

    private void Restart()
    {
        endingSlotsGrid = childPanel.Find("ToSolve");
        startingSlotsGrid = childPanel.Find("Provided");

        // Activate the Letter Collection and Slot script
        gameObject.GetComponent<KeyCollection>().enabled = true;

        // Grab the play list object for this game
        playObj = looper.GetCurrentPlay();

        // De-serialize the keyboard json
        keyboard = JsonUtility.FromJson<DO_KeyboardGame>(playObj.json);

        if (words != null)
        {
            words.Clear();
        }
        else
        {
            words = new List<string>();
        }

        // Create the word List and populate it.
        PopulateWordList();

        Debug.Log("KEYBOARD: WORDS list count is " + words.Count.ToString());

        currentRound = 1;

        // Start first round
        GenerateRound();

        gameComplete = false;

    }

    // Clear the ending slots -- INCOMPLETE
    public void ClearSlots(Transform slots)
    {
        for (int i = 0; i < word.Length; i++)
        {
            Destroy(slots.GetChild(i).gameObject);
        }
    }

    public void ResetGameData()
    {
        Debug.Log("Resetting Game Data");
        startingSlotsGrid = null;
        endingSlotsGrid = null;
        currentSlotIndex = -1;
        numErrors = 0;
        gameComplete = false;
    }

    private void PopulateWordList()
    {
        foreach (int id in keyboard.wordIdList)
        {
            words.Add(looper.controller.GetWordById(id));
        }
    }

    public void WordSolved()
    {
        StartCoroutine(SolveWord());
    }

    public void ResetProvidedScales()
    {
        for (int i = 0; i < startingSlotsGrid.childCount; i++)
        {
            startingSlotsGrid.GetChild(i).GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f);
            startingSlotsGrid.GetChild(i).GetComponent<CanvasGroup>().alpha = 0.2f;
        }
    }

    public void ToggleKeyboardColor()
    {
        for (int i = 0; i < NUM_LETTERS; i++)
        {
            if (keyboard.keyboardColor == true)
            {
                keyboardPrefab.GetChild(i).gameObject.GetComponent<RawImage>().color = Color.white;
            }
            else
            {
                keyboardPrefab.GetChild(i).gameObject.GetComponent<RawImage>().color = Color.black;
            }
        }
    }

    public void ToggleProvided()
    {
        if (keyboard.provided == false)
        {
            provided.SetActive(false);
        }
    }




}
