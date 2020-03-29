using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashHelper : AB_GameHelper
{
    [SerializeField] GameObject picture;
    [SerializeField] Canvas parentCanvas;
    [SerializeField] Text buttonText;
    [SerializeField] Button wordButton;
    [SerializeField] VW_GameLoop gameLoop;
    [SerializeField] GameObject continueModal;


    private const int pauseTime = 1;

    private DO_FlashCard flash;
    private DO_PlayListObject playObj;
    private List<string> words;
    private int currentRound;
    private string word;

    // Use this for initialization
    void Start () {
        Debug.Log("FlashHelper start method");
        Restart();
	}

    /// <summary>
    /// Public facing method that starts the flash card coroutine
    /// </summary>
    public void WordButtonPress() {
        wordButton.interactable = false;
        StartCoroutine(PlayFlashCard());
    }

    private IEnumerator PlayFlashCard()
    {
        // Display the word
        buttonText.text = word;

        // Play the audio of the word and wait for it to finish
        gameLoop.PlayAudio(word.ToLower());
        yield return new WaitWhile(() => gameLoop.audioSource.isPlaying);

        // Pause for three seconds
        yield return new WaitForSeconds(pauseTime);

        currentRound++;

        // Disable the parent canvas
        parentCanvas.enabled = false;
        buttonText.text = "?";

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
    
    public override void Resume()
    {
        Debug.Log("FlashHelper Resume method");
        Restart();
    }

    private void GenerateRound()
    {
        wordButton.interactable = true;

        // Enable the canvas
        parentCanvas.enabled = true;

        // Grab the new word
        word = words[currentRound - 1].ToUpper();

        // Set the image in the picture game object and then activate it
        gameLoop.SetImage(word, picture);
        picture.SetActive(true);
    }

    private void Restart()
    {
        // Grab the play list object for this game
        playObj = gameLoop.GetCurrentPlay();

        // De-serialize the word scramble json
        flash = JsonUtility.FromJson<DO_FlashCard>(playObj.json);

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

        Debug.Log("FLASH: WORDS list count is " + words.Count.ToString());

        currentRound = 1;

        // Start first round
        GenerateRound();
    }

    private void PopulateWordList()
    {
        foreach (int id in flash.wordIdList)
        {
            words.Add(gameLoop.controller.GetWordById(id));
        }
    }
}