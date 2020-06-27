using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemoryHelper : AB_GameHelper {
    #region Member Variables

    // Serialized Fields
    [SerializeField] Transform puzzleField;
    [SerializeField] Texture2D bgImage;
    [SerializeField] Canvas parentCanvas;
    [SerializeField] VW_GameLoop gameLoop;
    [SerializeField] GameObject card1;
    [SerializeField] GameObject card2;
    [SerializeField] GameObject card3;
    [SerializeField] GameObject card4;
    [SerializeField] GameObject card5;
    [SerializeField] GameObject card6;
    [SerializeField] GameObject card7;
    [SerializeField] GameObject card8;
    [SerializeField] GameObject card9;
    [SerializeField] GameObject card10;
    [SerializeField] GameObject card11;
    [SerializeField] GameObject card12;





    // Data Objects
    private DO_MemoryCards mc;
    private DO_PlayListObject playObj;

    // Game logic data
    public List<Texture2D> gamePuzzles = new List<Texture2D>();
    public List<string> words;
    public bool firstGuess, secondGuess;
    public int countGuesses;
    public int countCorrectGuesses;
    public int gameGuesses;
    public int firstGuessIndex, secondGuessIndex;
    public string firstGuessPuzzle, secondGuessPuzzle;
    public int currentRound;

    // Feature enabling data
    private bool wordAudio;
    private bool includeText;
    private bool altImages;


    int deckSize = 8;


    public Button[] btns;

    #endregion


    public override void Resume()
    {
        Debug.Log("Start Resume");
        playObj = gameLoop.GetCurrentPlay();
        SetupFromString(playObj.json);
        Debug.Log("End Resume");

    }

    public void SetupFromString(string json)
    {
        Debug.Log("Start SetupFromString");
      

        Debug.Log(json);

        mc = JsonUtility.FromJson<DO_MemoryCards>(json);

        // Set up game data  
     

        foreach (int id in mc.wordIdList)
        {
            words.Add(gameLoop.controller.GetWordById(id));
        }


        //CreateButtons();  
        SetupCards();
        //AddListeners();
        AddGamePuzzles();
       
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;


        // Set up extra features
        wordAudio = mc.wordAudio;
        includeText = mc.includeText;
        altImages = mc.altImages;

        

        Debug.Log("End setupFromString");



    }

    private void TurnOffAllCards()
    {
        card1.SetActive(false);
        card2.SetActive(false);
        card3.SetActive(false);
        card4.SetActive(false);
        card5.SetActive(false);
        card6.SetActive(false);
        card7.SetActive(false);
        card8.SetActive(false);
        card9.SetActive(false);
        card10.SetActive(false);
        card11.SetActive(false);
        card12.SetActive(false);
    }

    private void SetupCards()
    {
        //determine the number of memory cards based on the words list count. Max size is 12.
        if (words.Count > 6) deckSize = 12;
        else deckSize = ((words.Count) * 2);

        if (deckSize == 12)
        {
            card1.SetActive(true);
            card2.SetActive(true);
            card3.SetActive(true);
            card4.SetActive(true);
            card5.SetActive(true);
            card6.SetActive(true);
            card7.SetActive(true);
            card8.SetActive(true);
            card9.SetActive(true);
            card10.SetActive(true);
            card11.SetActive(true);
            card12.SetActive(true);

        }
        else if (deckSize == 10)
        {
            card1.SetActive(true);
            card2.SetActive(true);
            card3.SetActive(true);
            card4.SetActive(true);
            card5.SetActive(true);
            card6.SetActive(true);
            card7.SetActive(true);
            card8.SetActive(true);
            card9.SetActive(true);
            card10.SetActive(true);
        }
        else if (deckSize == 8)
        {
            card1.SetActive(true);
            card2.SetActive(true);
            card3.SetActive(true);
            card4.SetActive(true);
            card5.SetActive(true);
            card6.SetActive(true);
            card7.SetActive(true);
            card8.SetActive(true);
        }
        else if (deckSize == 6)
        {
            card1.SetActive(true);
            card2.SetActive(true);
            card3.SetActive(true);
            card4.SetActive(true);
            card5.SetActive(true);
            card6.SetActive(true);
        }
        else if (deckSize == 4)
        {
            card1.SetActive(true);
            card2.SetActive(true);
            card3.SetActive(true);
            card4.SetActive(true);
        }
        else if (deckSize == 2)
        {
            card1.SetActive(true);
            card2.SetActive(true);
        }

        btns = puzzleField.GetComponentsInChildren<Button>();
        Debug.Log("btns array length = " + btns.Length);

     
        
    }


    ///// <summary>
    ///// This method instatiates the memory card game objects buttons, gives them integer names from 0-8, 
    ///// puts them in the puzzle field game object, and sets the background image.
    ///// </summary>
    //private void CreateButtons()
    //{
    //    Debug.Log("Start CreateButtons");

    //    // determine the number of memory cards based on the words list count. Max size is 12.

    //    //if (words.Count > 6) deckSize = 12;
    //    //else if (words.Count <= 6) deckSize = ((words.Count) * 2);

    //    deckSize = 4; // Used for testing 

    //    for (int i = 0; i < (deckSize); i++)
    //    {
    //        GameObject button = Instantiate(btn);
    //        button.name = "" + i;
    //        button.transform.SetParent(puzzleField, false);
    //        Debug.Log("Creating button " + button.name);
    //        buttonGameobjectList.Add(button);
    //        button.GetComponent<RawImage>().texture = bgImage;

    //    }


    //    Debug.Log("End CreateButtons");
    //    Debug.Log("button list length =  " + buttonList.Count);
    //    Debug.Log("button gameobject list length =  " + buttonGameobjectList.Count);



    //}




    ///// <summary>
    ///// This method makes the memory cards interactable to run the PickAPuzzle method onClick.
    ///// </summary>
    //void AddListeners()
    //{
    //    Debug.Log("Start AddListeners");
    //    foreach (Button btn in buttonList) btn.onClick.AddListener(() => PickAPuzzle());
    //    Debug.Log("End Add listeners");


    //}

    /// <summary>
    /// This method grabs the images from the DO_MemoryCards, and creates pairs to be applied to the memory cards.
    /// </summary>
    private void AddGamePuzzles()
    {
        Debug.Log("Start AddGamePuzzles");

        int looper = btns.Length / 2;

        for (int i = 0; i < looper; i++)
        {
            //if (i == looper / 2)
            //{
            //    i = 0;
            //}

            //////////////////////////ADD CODE HERE TO ALLOW LOADING OF CUSTOM WORDS FROM DB vvvvvvvvvvvvvvvvvvvvvvv
            // Load single or multiple images to gamepuzzles depending on AltImages toggle setting
            /*else */
            if (mc.altImages == true)
            {
                Texture2D tempTex;
                tempTex = Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + UnityEngine.Random.Range(1, 6));
                gamePuzzles.Add(tempTex == null ? Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + "1") : tempTex);
                //gamePuzzles.Add(Resources.Load<Texture2D>("WordPictures/" + words[index] + "/" + words[index] + "1"));
            }
            else
            {
                gamePuzzles.Add(Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + "1"));

            }

            //////////////////////////ADD CODE HERE TO ALLOW LOADING OF CUSTOM WORDS FROM DB ^^^^^^^^^^^^^^^^^^^^^^^                            
        }


        for (int i = 0; i < looper; i++)
        {

            if (mc.altImages == true)
            {
                Texture2D tempTex;
                tempTex = Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + UnityEngine.Random.Range(1, 6));
                gamePuzzles.Add(tempTex == null ? Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + "1") : tempTex);
                //gamePuzzles.Add(Resources.Load<Texture2D>("WordPictures/" + words[index] + "/" + words[index] + "1"));
            }
            else
            {
                gamePuzzles.Add(Resources.Load<Texture2D>("WordPictures/" + words[i] + "/" + words[i] + "1"));

            }
        }
        Debug.Log("End AddGamePuzzles");

        return;
    }
                 
    /// <summary>
    /// This method grabs the name of the first and second guess memory cards (ints 0-8), 
    /// applies the corresponding random word image from the gamePuzzles list as specified in the DO_MemoryCards, 
    /// iterates the total countGuesses for the game module, and launches the CheckIfThePuzzlesMatch coroutine.
    /// </summary>
    public void PickAPuzzle()
    {
        Debug.Log("Start PickAPuzzle");
        if (!firstGuess)
        {
            Debug.Log("starting first guess");

            firstGuess = true;
            firstGuessIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name);

            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name.Replace("1","").Replace("2","").Replace("3","").Replace("4","").Replace("5","");                       
            if (mc.wordAudio == true) gameLoop.PlayAudio(firstGuessPuzzle);            
            btns[firstGuessIndex].GetComponent<RawImage>().texture = gamePuzzles[firstGuessIndex];
            btns[firstGuessIndex].interactable = false;
        }
        else if (!secondGuess)
        {
            Debug.Log("Starting second guess");
            secondGuess = true;
            secondGuessIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name.Replace("1","").Replace("2","").Replace("3","").Replace("4","").Replace("5","");
            if (mc.wordAudio == true) gameLoop.PlayAudio(secondGuessPuzzle);
            btns[secondGuessIndex].GetComponent<RawImage>().texture = gamePuzzles[secondGuessIndex];
            btns[secondGuessIndex].interactable = false;
            countGuesses++;
            StartCoroutine(CheckIfThePuzzlesMatch());                     
        }
        else
        {
            Debug.Log("Wierd bug");
        }
        Debug.Log("End PickAPuzzle");

    }

    /// <summary>
    /// This coroutine does what it says on the tin.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckIfThePuzzlesMatch()
    {
        Debug.Log("Start Checking if puzzles match");
        yield return new WaitForSeconds(1f);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            Debug.Log("they matched");
            yield return new WaitForSeconds(.5f);
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;
            btns[firstGuessIndex].GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].GetComponent<RawImage>().color = new Color(0, 0, 0, 0);

                        
            StartCoroutine(CheckIfTheGameIsFinished());
        }
        else
        {
            Debug.Log("they didn't match");

            btns[firstGuessIndex].GetComponent<RawImage>().texture = bgImage;
            btns[secondGuessIndex].GetComponent<RawImage>().texture = bgImage;
            btns[firstGuessIndex].interactable = true;
            btns[secondGuessIndex].interactable = true;
        }

        yield return new WaitForSeconds(.2f);

        firstGuess = false;
        secondGuess = false;
        Debug.Log("End Checking if puzzles match");
        

    }

    /// <summary>
    /// This coroutine does what it says on the tin.
    /// </summary>
    private IEnumerator CheckIfTheGameIsFinished()
    {
        Debug.Log("Start checking if game is finished");
        Debug.Log("decksize = " + deckSize);
        countCorrectGuesses++;
        Debug.Log("countCorrectGuesses = " + countCorrectGuesses);

        if (countCorrectGuesses == (deckSize / 2))
        {
            currentRound++;

            Debug.Log("Game Finished. It took you " + countGuesses + " guesses to finish the game");
            ClearItems();



            /////////////////////// CODE NEEDED HERE ::::This is where it needs to re-load the game board with images from the remaining word objects in the game set specified by the admin

            // Inform game loop view that round was completed so it can display the continue modal
            gameLoop.PlayEntryCompleted(playObj.type_id);

            // Yield and wait till continue modal is deactivated
            yield return new WaitWhile(() => gameLoop.continueModal.activeSelf);

            
        //////////////////////// CODE NEEDED HERE ^^^^^^^^^^^^^^^
        }
        Debug.Log("End checking if game is finished");

    }

    /// <summary>
    /// This method shuffles the memory cards.
    /// </summary>
    /// <param name="list"></param>
    void Shuffle(List<Texture2D> list)
    {
        Debug.Log("Start Shuffle");

        for (int i=0; i < list.Count; i++)
        {
            Texture2D temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        Debug.Log("End Shuffle");

    }

    private void ClearRawImages()
    {
        foreach (Button btn in btns)
        {
            btn.gameObject.GetComponent<RawImage>().texture = bgImage;
        }        

    }

    private void ResetColors()
    {
        foreach (Button btn in btns)
        {
            btn.gameObject.GetComponent<RawImage>().color = new Color(1,1,1,1);
        }    
    }

    private void ResetInteractable()
    {
       foreach (Button btn in btns)
        {
            btn.interactable = true;
        }

    }

    private void ClearItems()
    {
        Debug.Log("Start ClearItems");
        //foreach (Button item in buttonList) Destroy(item);
        //foreach (Texture2D item in gamePuzzles) Destroy(item);
        countCorrectGuesses = 0;
        ClearRawImages();
        ResetColors();
        ResetInteractable();
        TurnOffAllCards();
        btns = null;
        gamePuzzles.Clear();
        words.Clear();

        // Reset game logic values to 0 or ""
        countCorrectGuesses = 0;
        countGuesses = 0;
        gameGuesses = 0;
        firstGuessIndex = 0;
        secondGuessIndex = 0;
        firstGuessPuzzle = "";
        secondGuessPuzzle = "";
        Debug.Log("End ClearItems");
        firstGuess = false;
        secondGuess = false;


    }


} //Memory Helper