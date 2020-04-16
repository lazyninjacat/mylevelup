using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingHelper : AB_GameHelper
{

    [SerializeField] GameObject wordimage1;
    [SerializeField] GameObject wordimage2;
    [SerializeField] GameObject wordimage3;
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

    public int word1Errors = 0;
    public int word2Errors = 0;
    public int word3Errors = 0;


    private const int XY_DIMENSION = 300;
    private Texture2D customTexture1;
    private Texture2D customTexture2;
    private Texture2D customTexture3;




    private void Start()
    {
        Debug.Log("matching start method");

    }

    public override void Resume()
    {
        Debug.Log("matching resume method");

        Debug.Log("enter Matching Helper start");

        playlistDataObject = gameLoop.GetCurrentPlay();

        SetupFromString(playlistDataObject.json);

        Debug.Log("done Matching Helper start");
    }


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

        Debug.Log("Words = ");
        foreach (string word in words)
        {
            Debug.Log(word);
        }

        Debug.Log("WordsIDs = ");
        foreach (int id in wordIDs)
        {
            Debug.Log(id);
        }
        Debug.Log("End Set Game Data");

    }


    [SerializeField] GameObject test1RawImg;


    // Set Images
    private void SetImages()
    {
        Debug.Log("Start Set Images");

        byte[] bytes1 = null;
        byte[] bytes2 = null;
        byte[] bytes3 = null;




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
            //bytes1 = FileAccessUtil.LoadWordPic(words[0]);
            //customTexture1 = new Texture2D(XY_DIMENSION, XY_DIMENSION);
            gameLoop.SetImage(words[0], wordimage1);
            wordimage1.name = words[0];
            wordImage1StartPosition = wordimage1.transform.position;
            Debug.Log("wordimage1 set. word = " + words[0]);

            //bytes2 = FileAccessUtil.LoadWordPic(words[1]);
            //customTexture2 = new Texture2D(XY_DIMENSION, XY_DIMENSION);
            gameLoop.SetImage2(words[1], wordimage2);
            wordimage2.name = words[1];
            wordImage2StartPosition = wordimage2.transform.position;
            Debug.Log("wordimage2 set. word = " + words[1]);

            //bytes3 = FileAccessUtil.LoadWordPic(words[2]);
            //customTexture3 = new Texture2D(XY_DIMENSION, XY_DIMENSION);
            gameLoop.SetImage3(words[2], wordimage3);
            wordimage3.name = words[2];
            wordImage3StartPosition = wordimage3.transform.position;
            Debug.Log("wordimage3 set. word = " + words[2]);

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

        wordImage1Container.transform.SetSiblingIndex((Random.Range(0, wordIDs.Count)));
        wordImage2Container.transform.SetSiblingIndex(Random.Range(0, wordIDs.Count));
        wordImage3Container.transform.SetSiblingIndex(Random.Range(0, wordIDs.Count));

        Debug.Log("End Shuffle Images");

    }



    private IEnumerator GameOverPause()
    {
        yield return new WaitForSeconds(1);
        solvedPairs = 0;
        wordimage1.transform.SetParent(wordImage1Container.transform);
        wordimage1.transform.position = wordImage1StartPosition;
        wordimage1.GetComponent<RawImage>().texture = null;
        wordimage1.name = "Image1";
        wordtext1.name = "Text1";
        wordtext1.text = "";
        wordtext1Container.name = "Text1Container";

        wordimage2.transform.SetParent(wordImage2Container.transform);
        wordimage2.transform.position = wordImage2StartPosition;
        wordimage2.GetComponent<RawImage>().texture = null;
        wordimage2.name = "Image2";
        wordtext2.name = "Text2";
        wordtext2.text = "";

        wordtext2Container.name = "Text2Container";




        wordimage3.transform.SetParent(wordImage3Container.transform);
        wordimage3.transform.position = wordImage3StartPosition;
        wordimage3.GetComponent<RawImage>().texture = null;
        wordimage3.name = "Image3";
        wordtext3.name = "Text3";
        wordtext3.text = "";

        wordtext3Container.name = "Text3Container";

        solvedPairs = 0;

        word1 = "";
        word2 = "";
        word3 = "";


        gameLoop.controller.ReportRoundErrorCount("Matching", word1, word1Errors);
        gameLoop.controller.ReportRoundErrorCount("Matching", word2, word2Errors);
        gameLoop.controller.ReportRoundErrorCount("Matching", word3, word3Errors);


        word1Errors = 0;
        word2Errors = 0;
        word3Errors = 0;


        gameLoop.PlayEntryCompleted(playlistDataObject.type_id);
    }


    private void GameOver()
    {
        StartCoroutine(GameOverPause());
         
        
    }







}
