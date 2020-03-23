using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingHelper : AB_GameHelper
{

    [SerializeField] RawImage wordimage1;
    [SerializeField] RawImage wordimage2;
    [SerializeField] RawImage wordimage3;
    [SerializeField] Text wordtext1;
    [SerializeField] Text wordtext2;
    [SerializeField] Text wordtext3;
    [SerializeField] GameObject wordtext1Container;
    [SerializeField] GameObject wordtext2Container;
    [SerializeField] GameObject wordtext3Container;




    [SerializeField] VW_GameLoop gameLoop;
    [SerializeField] Canvas parentCanvas;

    private DO_MatchingGame matchingGameDataObject;
    private DO_PlayListObject playlistDataObject;

    public string word1;
    public string word2;
    public string word3;

    public List<string> words;
    public List<int> wordIDs;

    public int solvedPairs = 0;



    //// Start is called before the first frame update
    //void Start()
    //{
    //    Resume();

    //}

    public void SetupFromString(string json)
    {
        Debug.Log(json);
        matchingGameDataObject = JsonUtility.FromJson<DO_MatchingGame>(json);
        SetGameData();    
        SetImages();
        SetTexts();
        Shuffle();
        
        // TODO: Set up extra features  
        Debug.Log("done setupFromString");
    }
       
    // Update is called once per frame
    void Update()
    {
        if (solvedPairs == words.Count)
        {
            words.Clear();
            wordIDs.Clear();
            SolveAllMatching();
        }
    }

    public void IterateSolvedPairs()
    {
        solvedPairs++;
        Debug.Log("Iterate SolvedPairs. Now solvedPairs = " + solvedPairs);
    }

    // Set up game data
    private void SetGameData()
    {
        Debug.Log("Start Set Game Data");

        foreach (int id in matchingGameDataObject.wordIdList)
        {
            words.Add(gameLoop.controller.GetWordById(id));
            wordIDs.Add(id);
        }
        Debug.Log("Words = " + words);
        Debug.Log("WordIds = " + wordIDs);
        Debug.Log("End Set Game Data");

    }

    // Set Images
    private void SetImages()
    {
        Debug.Log("Start Set Images");

        if (words.Count == 1)
        {
            gameLoop.SetImage(words[0], wordimage1.gameObject);
            wordimage1.gameObject.name = words[0];
        }
        else if (words.Count == 2)
        {
            gameLoop.SetImage(words[0], wordimage1.gameObject);
            wordimage1.gameObject.name = words[0];

            gameLoop.SetImage(words[1], wordimage2.gameObject);
            wordimage2.gameObject.name = words[1];

        }
        else if (words.Count == 3)
        {
            gameLoop.SetImage(words[0], wordimage1.gameObject);
            wordimage1.gameObject.name = words[0];

            gameLoop.SetImage(words[1], wordimage2.gameObject);
            wordimage2.gameObject.name = words[1];

            gameLoop.SetImage(words[2], wordimage3.gameObject);
            wordimage3.gameObject.name = words[2];

        }

        Debug.Log("End Set Images");

    }

    // Set Texts
    private void SetTexts()
    {
        Debug.Log("Start Set Texts");

        if (words.Count == 1)
        {
            wordtext1.text = words[0];
            wordtext1Container.gameObject.name = words[0];

        }
        else if (words.Count == 2)
        {
            wordtext1.text = words[0];
            wordtext1Container.gameObject.name = words[0];

            wordtext2.text = words[1];
            wordtext2Container.gameObject.name = words[1];

        }
        else if (words.Count == 3)
        {
            wordtext1.text = words[0];
            wordtext1Container.gameObject.name = words[0];

            wordtext2.text = words[1];
            wordtext2Container.gameObject.name = words[1];

            wordtext3.text = words[2];
            wordtext3Container.gameObject.name = words[2];

        }
        Debug.Log("End Set Texts");

    }


    // Shuffle
    private void Shuffle()
    {
        Debug.Log("Start Shuffle Images");

        Debug.Log("End Shuffle Images");

    }

    public override void Resume()
    {
        Debug.Log("enter Matching Helper start");

        playlistDataObject = gameLoop.GetCurrentPlay();

        SetupFromString(playlistDataObject.json);

        Debug.Log("done Matching Helper start");
    }


    private void SolveAllMatching()
    {
        solvedPairs = 0;
        gameLoop.PlayEntryCompleted(playlistDataObject.type_id);    
        
    }
}
