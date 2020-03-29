using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Tracks what order the letters are in to see if the word has been solved
public class KeyCollection : MonoBehaviour, IHasChanged
{
    // Public Variables
    public bool keyboardIsEnabled;
    public float startTime;

    // Private Variables
    private bool isAnimationLooping;
    private bool firstHasBeenPlayed; // Refers to the first letter hint being played
    private bool secondHasBeenPlayed; // Refers to the inactivity features
    private bool thirdHasBeenPlayed; // Refers to the inactivity features
    private bool fourthHasBeenPlayed; // Refers to the inactivity features
    private bool inFirst;
    private bool repeatLoop = false;
    private float elapsedTime;
    private string key;

    [SerializeField]VW_GameLoop looper;
    private KeyboardHelper helper;

    // Constant Audio Strings
    const string INCORRECT_AUDIO = "incorrect";

    // Serializable Field
    [SerializeField] Transform slots;   // Reference to the solving slots
    [SerializeField] Transform keyboardPrefab;

    public void OnEnable()
    {
        Messenger<string>.AddListener("KeyboardPressed", OnKeyPressed);
        Messenger<string>.AddListener("GameComplete", ResetGameData);
    }

    public void OnDisable()
    {
        Messenger<string>.RemoveListener("KeyboardPressed", OnKeyPressed);
        Messenger<string>.RemoveListener("GameComplete", ResetGameData);
    }

    void Start()
    {
        Debug.Log("KeyCollection Start method");

        keyboardIsEnabled = true;
        isAnimationLooping = false;
        firstHasBeenPlayed = false;
        secondHasBeenPlayed = false;
        thirdHasBeenPlayed = false;
        fourthHasBeenPlayed = false;
        inFirst = false;
        key = "";
        startTime = Time.time;        
        helper = gameObject.GetComponent<KeyboardHelper>();
        HasChanged();
        helper.gameComplete = false;
        helper.currentSlotIndex = 0;
    }

    void Update()
    {
        elapsedTime = Time.time - startTime;

        // If the game is not complete and the first display letter hint has not been played
        if (helper.gameComplete == false && firstHasBeenPlayed == false)
        {
            Debug.Log("gameComplete = " + helper.gameComplete + " . firstHasBeenPlayed = " + firstHasBeenPlayed);
            // Start the new point in time
            startTime = Time.time;
            StartCoroutine(DisplayLetterHint());
            firstHasBeenPlayed = true;
        }

        // After the first letter hint has been played, start the inactivity timer
        if (firstHasBeenPlayed == true)
        {
            StartInactivity();
        }

        // If the user is stuck on 4 errors, keep the hint enabled
        if (isAnimationLooping == true && helper.gameComplete == false)
        {
            EnableAnimationHint();
        }
    }

    #region EnableAnimationHint implementation
    // Play the HighlightLetter animation
    public void EnableAnimationHint()
    {
        //Debug.Log("KEYBOARD...Enabling animation hint");
        // Reveal location of correct next letter
        if(firstHasBeenPlayed == true)
        {
            helper.PlayKeyboardAnimation(slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower());
        }
    }
    #endregion

    #region HasChanged implementation
    // Indicates a change in the solving slots
    public void HasChanged()
    {
        Debug.Log("LC: HAS CHANGED");
        Debug.Log("LC: Keyboard Helper says " + helper.GetType().ToString());      
        Debug.Log("CURRENT SLOT INDEX: " + helper.currentSlotIndex);

        // String to store the order of the letters
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        // Loop through each letter contained in the solving section
        foreach (Transform slotTransform in slots)
        {
            // Get the letter stored in the slot
            GameObject letter = slotTransform.GetComponent<Slot>().letter;
            // If the slot contains a letter, append it to the string
            if (letter)
            {
                // Extract the letter from the full letter string Ex. Letter_A(Clone) position 7 = a
                char letterName = char.ToLower(letter.name[7]);
                // Append the letter to the builder
                builder.Append(letterName);
            }
        }

        // Finalize the string
        string wordProgress = builder.ToString();

        Debug.Log("LC: Word progress is " + wordProgress);

        // Detect if the word has been solved
        if (helper.CheckForMatch(wordProgress))
        {
            helper.gameComplete = true;
            helper.currentSlotIndex = 0;
            isAnimationLooping = false;

            Debug.Log("LC: Found a match!");

       
            firstHasBeenPlayed = false;

            helper.WordSolved();
        }
    }
    #endregion

    #region IAdaptDifficulty implementation
    // Decrease the difficulty based on the number of errors
    public IEnumerator AdaptDifficulty(int numErrors, GameObject letter)
    {
        // Disable Keyboard
        //keyboardIsEnabled = false;

        // Destroy the incorrect letter
        yield return new WaitForSeconds(0.2f);
        Destroy(letter);

        // If 3 or fewer errors, start the WaitForHint Coroutine based on the number of errors
        if (numErrors < 4)
        {
            StartCoroutine(WaitForHint(numErrors));
        }

        // If the number of user errors are 4 or higher, disable the keyboard until the correct letter is pressed 
        if (numErrors >= 4)
        {
            // Loop the animation and disable all keys except the correct one
            isAnimationLooping = true;
            keyboardIsEnabled = false;
            helper.DisableAllKeys();
            helper.EnableKeyboardButton(slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower());

            yield return new WaitUntil(() => slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower() == key.ToString().ToLower());


            // After the user solves the correct letter, enable the keyboard and disable the hint loop
            helper.EnableAllKeys();
            keyboardIsEnabled = true;
            isAnimationLooping = false;

            // Get the correct letter
            OnKeyPressed(key);

        }

        // Re-enable the keyboard
        keyboardIsEnabled = true;
    }
    #endregion

    #region IDisplayLetterHint implementation
    // Wait 2 seconds then highlight the letter in the provided slot and play the audio clip
    public IEnumerator DisplayLetterHint()
    {
        inFirst = true;
        Debug.Log("DISPLAYING LETTER HINT... ");
        Debug.Log("game complete is " + helper.gameComplete );

        // Disable the keyboard
        keyboardIsEnabled = false;
        helper.DisableAllKeys();

        yield return new WaitForSeconds(1);

        // Highlight the next letter to solve
        helper.HighlightStartingSlotLetter(helper.currentSlotIndex);

        // Re-enable the keyboard
        helper.EnableAllKeys();
        keyboardIsEnabled = true;
        firstHasBeenPlayed = true;
        inFirst = false;
    }
    #endregion

    #region IWaitForHint implementation
    // Display hint based on number of errors(seconds)
    public IEnumerator WaitForHint(int seconds)
    {
        yield return new WaitForSeconds(5 - seconds);

        // Loop the animation, disable the keyboard, then wait to re-enable
        isAnimationLooping = true;
        keyboardIsEnabled = false;
        //helper.DisableAllKeys();
        helper.EnableKeyboardButton(slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower());

        yield return new WaitForSeconds(seconds/2);

        helper.EnableAllKeys();
        isAnimationLooping = false;
        keyboardIsEnabled = true;
    }
    #endregion

    #region IWaitUntilCorrectLetterPressed implementation
    // Waits for correct key to be pressed
    public IEnumerator WaitUntilCorrectLetterPressed()
    {
        // Reset the key value to ensure duplicate letters do not trigger this instantly
        key = "";

        // Wait until the current slot letter matches the letter pressed
        yield return new WaitUntil(() => slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower() == key.ToString().ToLower());
        helper.EnableAllKeys();
        keyboardIsEnabled = true;
        OnKeyPressed(key);
    }
    #endregion

    #region OnKeyPressed implementation
    // Executes based on the "KeyboardPressed" event from KeyPressHandler
    public void OnKeyPressed(string key)
    {
        // Start the new point in time
        startTime = Time.time;
        this.key = key; // Reference to the key pressed
        //firstHasBeenPlayed = true;

        Debug.Log("Slots child is: " + slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower());
        Debug.Log("Current index is: " + helper.currentSlotIndex);

        // If the keyboard is enabled and the slot at the current index is empty
        if (helper.IsKeyboardEnabled() && helper.IsSolvingIndexEmpty(helper.currentSlotIndex))
        {
            // If the letter to solve at the current slot index matches the letter typed
            if (slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower() == key.ToString().ToLower())
            {
                isAnimationLooping = false;
           
                // Create a copy of the letter, place it in the slot, and increase the slot index. Then call HasChanged() to update the string
                GameObject l = GameObject.Instantiate((GameObject)Resources.Load(("Letter Prefabs/Letter_" + key.ToString().ToUpper())));
                l.transform.SetParent(slots.GetChild(helper.currentSlotIndex), false);
                l.gameObject.GetComponent<Animation>().Play();

                if (helper.keyboard.letterName == true)
                {
                    helper.PlayKeyboardAudio(helper.currentSlotIndex);
                }
                                
                // Remove the drag handler component
                Destroy(l.GetComponent<DragHandler>());

                // Increase the slot index, set the current number of errors to 0, and call HasChanged()
                if (helper.currentSlotIndex < slots.childCount-1)
                {
                    helper.currentSlotIndex++;
                    helper.numErrors = 0;
                    Debug.Log("Starting DisplayLetterHint...1");
                    //StartCoroutine(DisplayLetterHint());
                }
                else // The word has been solved
                {
                    helper.ResetProvidedScales();
                    helper.currentSlotIndex = 0;
                    helper.gameComplete = true;
                    firstHasBeenPlayed = false;
                    StartCoroutine(helper.GameOver());
                }
                HasChanged();
                helper.HighlightStartingSlotLetter(helper.currentSlotIndex);

            }
            // The letter pressed is incorrect
            else
            {
                if (helper.IsKeyboardEnabled())
                {
                    // Increase the number of errors
                    helper.numErrors++;
                    helper.totalErrors++;

                    // Display Letter in slot
                    GameObject l = GameObject.Instantiate((GameObject)Resources.Load(("Letter Prefabs/Letter_" + key.ToString().ToUpper())));
                    l.transform.SetParent(slots.GetChild(helper.currentSlotIndex), false);

                    // Remove the drag handler component
                    Destroy(l.GetComponent<DragHandler>());

                    // Adapt the difficulty
                    StartCoroutine(AdaptDifficulty(helper.numErrors, l));
                }
            }
        }
    }
    #endregion

    void ResetGameData(string s)
    {
        Debug.Log("Resetting game data...");
        helper.gameComplete = false;
        firstHasBeenPlayed = false;
        secondHasBeenPlayed = false;
        thirdHasBeenPlayed = false;
        fourthHasBeenPlayed = false;
        while (firstHasBeenPlayed == false)
        {
            startTime = Time.time;
        }
        HasChanged();
    }

    #region StartInactivity implementation
    // Starts the inactivity features
    public void StartInactivity()
    {    
        // Play brief hint and audio clip to make the correct key transparent
        if (elapsedTime >= 4.0f && firstHasBeenPlayed == true)
        {
            if (secondHasBeenPlayed == false )
            {
                //helper.PlayKeyboardAudio(helper.currentSlotIndex);
                StartCoroutine(helper.MakeKeyTransparent(helper.currentSlotIndex, 0.5f)); // Current slot and time 

                // Reset the timer
                startTime = Time.time;
                secondHasBeenPlayed = true;
            }
        }

        // Play slightly longer hint and audio clip
        if (elapsedTime >= 6.0f && secondHasBeenPlayed == true)
        {
            if (thirdHasBeenPlayed == false )
            {
                //helper.PlayKeyboardAudio(helper.currentSlotIndex);
                StartCoroutine(helper.MakeKeyTransparent(helper.currentSlotIndex, 1.5f)); // Current slot and time 

                // Reset the timer
                startTime = Time.time;
                thirdHasBeenPlayed = true;
            }
        }

        // Disable keyboard, play hint animation, and set the played value to true to only play once.
        if (elapsedTime >= 8.0f && thirdHasBeenPlayed == true)
        {
            if (fourthHasBeenPlayed == false)
            {
                // Loop the animation, disable the keyboard except the correct key, and wait for user to press the correct key
                isAnimationLooping = true;
                keyboardIsEnabled = false;
                helper.DisableAllKeys();
                helper.PlayKeyboardAudio(helper.currentSlotIndex);
                helper.EnableKeyboardButton(slots.GetChild(helper.currentSlotIndex).name.ToString().ToLower());

                StartCoroutine(WaitUntilCorrectLetterPressed());

                // Reset the timer
                startTime = Time.time;
                fourthHasBeenPlayed = true;
            }
        }
    }
    #endregion

}