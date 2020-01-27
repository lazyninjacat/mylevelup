using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryHelper : AB_GameHelper {
    #region Member Variables

    // Serialized Fields
    [SerializeField] Transform puzzleField;
    [SerializeField] Transform RepeatBugPanel;
    [SerializeField] private GameObject btn;
    [SerializeField] Texture2D bgImage;
    [SerializeField] Canvas parentCanvas;
    [SerializeField] VW_GameLoop gameLoop;

    // Data Objects
    private DO_MemoryCards mc;
    private DO_PlayListObject playObj;

    // Game logic data
    public List<Texture2D> gamePuzzles = new List<Texture2D>();
    public List<Button> buttonList = new List<Button>();
    public List<string> words;
    private bool firstGuess, secondGuess;    
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;
    private int currentRound;
    private bool isSpawnSet = false;

    // Feature enabling data
    private bool wordAudio;
    private bool includeText;
    private bool altImages;

    private bool repeatBug = false;

    int deckSize = 8;



    #endregion


    private void Start()
    {
        Debug.Log("enter start");
        playObj = gameLoop.GetCurrentPlay();
        SetupFromString(playObj.json);
        Debug.Log("done start");

    }

    public override void Resume()
    {
        Debug.Log("enter Resume");
        playObj = gameLoop.GetCurrentPlay();
        SetupFromString(playObj.json);
        Debug.Log("done Resume");

    }

    public void SetupFromString(string json)
    {
        Debug.Log(json);

        mc = JsonUtility.FromJson<DO_MemoryCards>(json);

        // Set up game data  
     

        foreach (int id in mc.wordIdList)
        {
            words.Add(gameLoop.controller.GetWordById(id));
        }
        CreateButtons();        
        AddListeners();
        AddGamePuzzles();
       
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;


        // Set up extra features
        wordAudio = mc.wordAudio;
        includeText = mc.includeText;
        altImages = mc.altImages;

        


        repeatBug = true;

        Debug.Log("done setupFromString");



    }


    /// <summary>
    /// This method instatiates the memory card game objects buttons, gives them integer names from 0-8, 
    /// puts them in the puzzle field game object, and sets the background image.
    /// </summary>
    private void CreateButtons()
    {
     

        // determine the number of memory cards based on the words list count. Max size is 12.

        if (words.Count > 6) deckSize = 12;
        else if (words.Count <= 6) deckSize = ((words.Count) * 2);

        if (!repeatBug)
        {
            for (int i = 0; i < (deckSize); i++)
            {
                GameObject button = Instantiate(btn);
                button.name = "" + i;
                button.transform.SetParent(puzzleField, false);
            }

            GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
            for (int i = 0; i < objects.Length; i++)
            {
                buttonList.Add(objects[i].GetComponent<Button>());
                buttonList[i].GetComponent<RawImage>().texture = bgImage;
            }
        }
        else
        {
            for (int i = 0; i < (deckSize); i++)
            {
                GameObject button = Instantiate(btn);
                button.name = "" + i + "x";
                button.transform.SetParent(RepeatBugPanel, false);
            }
                       
            GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
            for (int i = 0; i < objects.Length; i++)
            {
                buttonList.Add(objects[i].GetComponent<Button>());
                buttonList[i].GetComponent<RawImage>().texture = bgImage;
                
            }

           

        }

    }

    

    /// <summary>
    /// This method makes the memory cards interactable to run the PickAPuzzle method onClick.
    /// </summary>
    void AddListeners() { foreach (Button btn in buttonList) btn.onClick.AddListener(() => PickAPuzzle()); }

    /// <summary>
    /// This method grabs the images from the DO_MemoryCards, and creates pairs to be applied to the memory cards.
    /// </summary>
    private void AddGamePuzzles()
    {
        int looper = buttonList.Count;

        for (int i = 0; i < looper; i++)
        {
            if (i == looper / 2)
            {
                i = 0;
            }
            
            //////////////////////////ADD CODE HERE TO ALLOW LOADING OF CUSTOM WORDS FROM DB vvvvvvvvvvvvvvvvvvvvvvv
            // Load single or multiple images to gamepuzzles depending on AltImages toggle setting
            else if (mc.altImages == true)
            {
                Texture2D tempTex;
                tempTex = Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + UnityEngine.Random.Range(1, 6));
                gamePuzzles.Add(tempTex == null ? Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + "1") : tempTex);
                //gamePuzzles.Add(Resources.Load<Texture2D>("WordPictures/" + words[index] + "/" + words[index] + "1"));
            }
            else gamePuzzles.Add(Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + "1"));         
            //////////////////////////ADD CODE HERE TO ALLOW LOADING OF CUSTOM WORDS FROM DB ^^^^^^^^^^^^^^^^^^^^^^^                            
        }

        

        return;
    }
                 
    /// <summary>
    /// This method grabs the name of the first and second guess memory cards (ints 0-8), 
    /// applies the corresponding random word image from the gamePuzzles list as specified in the DO_MemoryCards, 
    /// iterates the total countGuesses for the game module, and launches the CheckIfThePuzzlesMatch coroutine.
    /// </summary>
    public void PickAPuzzle()
    {
        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);                       
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name.Replace("1","").Replace("2","").Replace("3","").Replace("4","").Replace("5","");                       
            if (mc.wordAudio == true) gameLoop.PlayAudio(firstGuessPuzzle);            
            buttonList[firstGuessIndex].GetComponent<RawImage>().texture = gamePuzzles[firstGuessIndex];
            buttonList[firstGuessIndex].interactable = false;
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name.Replace("1","").Replace("2","").Replace("3","").Replace("4","").Replace("5","");
            if (mc.wordAudio == true) gameLoop.PlayAudio(secondGuessPuzzle);
            buttonList[secondGuessIndex].GetComponent<RawImage>().texture = gamePuzzles[secondGuessIndex];
            buttonList[secondGuessIndex].interactable = false;
            countGuesses++;
            StartCoroutine(CheckIfThePuzzlesMatch());                     
        }                
    }
    
    /// <summary>
    /// This coroutine does what it says on the tin.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckIfThePuzzlesMatch()
    {
        Debug.Log("checking if puzzles match");
        yield return new WaitForSeconds(1f);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            Debug.Log("they matched");
            yield return new WaitForSeconds(.5f);
            buttonList[firstGuessIndex].interactable = false;
            buttonList[secondGuessIndex].interactable = false;
            buttonList[firstGuessIndex].GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
            buttonList[secondGuessIndex].GetComponent<RawImage>().color = new Color(0, 0, 0, 0);

            currentRound++;
                        
            StartCoroutine(CheckIfTheGameIsFinished());
        }
        else
        {
            Debug.Log("they didn't match");

            buttonList[firstGuessIndex].GetComponent<RawImage>().texture = bgImage;
            buttonList[secondGuessIndex].GetComponent<RawImage>().texture = bgImage;
            buttonList[firstGuessIndex].interactable = true;
            buttonList[secondGuessIndex].interactable = true;
        }

        yield return new WaitForSeconds(.1f);

        firstGuess = secondGuess = false;
    }

    /// <summary>
    /// This coroutine does what it says on the tin.
    /// </summary>
    private IEnumerator CheckIfTheGameIsFinished()
    {
        Debug.Log("checking if game is finished");
        Debug.Log("decksize = " + deckSize);
        countCorrectGuesses++;
        Debug.Log("countCorrectGuesses = " + countCorrectGuesses);

        if (countCorrectGuesses == (deckSize / 4))
        {                       
            Debug.Log("Game Finished. It took you " + countGuesses + " guesses to finish the game");
            ClearItems();



            /////////////////////// CODE NEEDED HERE ::::This is where it needs to re-load the game board with images from the remaining word objects in the game set specified by the admin

            // Inform game loop view that round was completed so it can display the continue modal
            gameLoop.PlayEntryCompleted(playObj.type_id);

            // Yield and wait till continue modal is deactivated
            yield return new WaitWhile(() => gameLoop.continueModal.activeSelf);

            
        //////////////////////// CODE NEEDED HERE ^^^^^^^^^^^^^^^
        }
    }
    
    /// <summary>
    /// This method shuffles the memory cards.
    /// </summary>
    /// <param name="list"></param>
    void Shuffle(List<Texture2D> list)
    {
        for (int i=0; i < list.Count; i++)
        {
            Texture2D temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }       

    private void ClearItems()
    {
        Debug.Log("Starting ClearItems");
        //foreach (Button item in buttonList) Destroy(item);
        //foreach (Texture2D item in gamePuzzles) Destroy(item);

        buttonList.Clear();
        gamePuzzles.Clear();
        words.Clear();
        Debug.Log("Completed ClearItems");


    }

} //Memory Helper