using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrambleHelper : AB_GameHelper, IWordSolved
{
    private Transform endingSlotsGrid;
    private Transform startingSlotsGrid;
    private DO_WordScramble scramble;
    private DO_PlayListObject playObj;
    private List<string> words;
    private int currentRound;
    private string word;

    // Error counting
    public int numErrors;
    public int totalErrors;

    private const int pauseTime = 2;


    [SerializeField] VW_GameLoop gameLoop;
    [SerializeField] Transform childPanel;
    [SerializeField] GameObject picture;

    public bool snapBack { get; set; }

    // Stores the times it took to solve each word
    List<double> wordSolveTimes = new List<double>();
    // Stores the total solve time for a game
    double totalSolveTimes = 0.0;
    // Store the indiviual and total number of tile moves 
    public int numTileMoves = -1;
    private int numTileMovesTotal;

    void Start()
    { 
        endingSlotsGrid = childPanel.Find("ToSolve");
        startingSlotsGrid = childPanel.Find("Provided");
        
        // Grab the required data for the variables and generate a new round
        Restart();

        snapBack = true;

        // Activate the Letter Collection and Slot script
        gameObject.GetComponent<LetterCollection>().enabled = true;
    }

    public override void Resume()
    {
        // TODO: Adjust for stats collection?
        Restart();
    }

    void Update()
    {
        // Disable the picture in word scramble panel if quit modal is open
        picture.GetComponent<RawImage>().enabled = !gameLoop.quitModal.activeSelf;
            //.SetActive(!gameLoop.quitModal.activeSelf);
        startingSlotsGrid.gameObject.SetActive(!gameLoop.quitModal.activeSelf);
        endingSlotsGrid.gameObject.SetActive(!gameLoop.quitModal.activeSelf);
    }

    public bool CheckForMatch(string checkWord) { return checkWord.Equals(words[currentRound - 1]); }

    public void WordSolved()
    {
        StartCoroutine(SolveWord());
    }

    ///<summary>Generates the board for a round of the Scramble Game</summary>
    private void GenerateRound()
    {
        word = words[currentRound - 1];
        string scrambledWord = gameLoop.ScrambleWord(word);

        Debug.Log("*******************************************\nHELPER: WORD IS: " + word + "\n********************************************");

        // Set the image in the picture game object and then activate it
        gameLoop.SetImage(word, picture);
        picture.SetActive(true);

        //generate all slots
        for (int i = 0; i < word.Length; ++i)
        {
            //Debug.Log("For loop entered");

            //Creates a starting slot GameObject from the slot prefab
            GameObject obj = GameObject.Instantiate((GameObject)Resources.Load("Slot"), startingSlotsGrid);

            string letter = scrambledWord[i].ToString().ToUpper();
            //Names the object the same as the letter
            obj.name = letter;
            //Sets the parent of the slot to the letterSlots object
            obj.transform.SetParent(startingSlotsGrid);
            //Creates a letter from the letter prefab 
            GameObject l = GameObject.Instantiate((GameObject)Resources.Load(("Letter Prefabs/Letter_" + letter)));
            //Sets the parent of the letter to its starting slot with a false flag to use the letter's own scaling
            l.transform.SetParent(obj.transform, false);


            //Generates the solve slots associated with the word and in the correct order
            GameObject obj2 = GameObject.Instantiate((GameObject)Resources.Load("Slot"), endingSlotsGrid);
            obj2.name = word[i].ToString().ToUpper();
            obj2.transform.SetParent(endingSlotsGrid);
        }

        gameLoop.PlayAudio(word);
    }

    private IEnumerator SolveWord()
    {
        // Pause for two seconds
        yield return new WaitForSeconds(pauseTime);

        // Play audio and sleep
        gameLoop.PlayAudio(word);
        yield return new WaitWhile(() => gameLoop.audioSource.isPlaying);

        // Pause for two seconds
        yield return new WaitForSeconds(pauseTime);

        // IS THIS WHERE WE CAN ADD CODE TO PLAY THE LETTER NAME AUDIO IN SEQUENCE???

        gameLoop.controller.ReportRoundErrorCount("WordScramble", word, numErrors);
        numErrors = 0;


        currentRound++;

        

        
        picture.SetActive(false);
        ClearSlots(startingSlotsGrid);
        ClearSlots(endingSlotsGrid);

        //Check if it is the last round
        if (currentRound > playObj.duration)
        {            
            gameLoop.PlayEntryCompleted(playObj.type_id);
        }
        else
        {
           
            // Inform game loop view that round was completed so it can display the continue modal
            gameLoop.RoundCompleted();

            // Yield and wait till continue modal is deactivated
            yield return new WaitWhile(() => gameLoop.continueModal.activeSelf);

            GenerateRound();
        }
    }

    private void Restart()
    {
        // Grab the play list object for this game
        playObj = gameLoop.GetCurrentPlay();

        // De-serialize the word scramble json
        scramble = JsonUtility.FromJson<DO_WordScramble>(playObj.json);

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

        Debug.Log("SCRAM: WORDS list count is " + words.Count.ToString());

        currentRound = 1;

        // Start first round
        GenerateRound();
    }

    ///<summary> Clears the child transforms from the transform given</summary>
    ///<param name="t">the transform to clear the children from</param>
    private void ClearSlots(Transform t)
    {
        foreach (Transform child in t)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void PopulateWordList()
    {
        foreach (int id in scramble.wordIdList)
        {
            words.Add(gameLoop.controller.GetWordById(id));
        }
    }
}