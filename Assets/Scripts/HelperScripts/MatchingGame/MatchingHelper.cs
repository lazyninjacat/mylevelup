using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingHelper : AB_GameHelper
{

    [SerializeField] RawImage wordimage1;
    [SerializeField] RawImage wordimage2;
    [SerializeField] RawImage wordimage3;
    [SerializeField] GameObject wordImage1Container;
    [SerializeField] GameObject wordImage2Container;
    [SerializeField] GameObject wordImage3Container;

    public Vector2 wordImage1StartPosition;
    public Vector2 wordImage2StartPosition;
    public Vector2 wordImage3StartPosition;


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

        wordImage1StartPosition = new Vector2(0, 0);
        wordImage2StartPosition = new Vector2(0, 0);
        wordImage3StartPosition = new Vector2(0, 0);


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
            GameOver();
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
            wordImage1StartPosition = wordimage1.transform.position;

        }
        else if (words.Count == 2)
        {
            gameLoop.SetImage(words[0], wordimage1.gameObject);
            wordimage1.gameObject.name = words[0];
            wordImage1StartPosition = wordimage1.transform.position;


            gameLoop.SetImage(words[1], wordimage2.gameObject);
            wordimage2.gameObject.name = words[1];
            wordImage2StartPosition = wordimage2.transform.position;


        }
        else if (words.Count == 3)
        {
            gameLoop.SetImage(words[0], wordimage1.gameObject);
            wordimage1.gameObject.name = words[0];
            wordImage1StartPosition = wordimage1.transform.position;


            gameLoop.SetImage(words[1], wordimage2.gameObject);
            wordimage2.gameObject.name = words[1];
            wordImage2StartPosition = wordimage2.transform.position;


            gameLoop.SetImage(words[2], wordimage3.gameObject);
            wordimage3.gameObject.name = words[2];
            wordImage3StartPosition = wordimage3.transform.position;


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


    private IEnumerator GameOverPause()
    {
        yield return new WaitForSeconds(1);
        solvedPairs = 0;
        wordimage1.transform.SetParent(wordImage1Container.transform);
        wordimage1.transform.position = wordImage1StartPosition;

        wordimage2.transform.SetParent(wordImage2Container.transform);
        wordimage2.transform.position = wordImage2StartPosition;

        wordimage3.transform.SetParent(wordImage3Container.transform);
        wordimage3.transform.position = wordImage3StartPosition;


        gameLoop.PlayEntryCompleted(playlistDataObject.type_id);
    }


    private void GameOver()
    {
        StartCoroutine(GameOverPause());
         
        
    }
}
