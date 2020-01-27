using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

/* Handles controlling the Scramble game logic */
public class GameController : MonoBehaviour, IWordSolved {

    // FOR TESTING
    public Text testText;

	//Reference to the DataService script
	public DataService ds;

	//Tags used to find the ToSolve and Provided game objects in the scene
	private const string STARTING_SLOTS = "StartingSlotsGroup";
	private const string ENDING_SLOTS = "EndingSlotsGroup";
	private const string MAIN_PICTURE = "MainPicture";
	private const string RND_CNTR = "RoundCounter";

    // Get UI refs
    private RawImage mainImage;

	//References to GameController and a getter property
	private GameObject gc;
	public GameObject Gc {
        get {return gc;}
    }

	//The name of the scene of the reward screen for the loadscene script
	private string rewardScreen;
	//The reward screen time limit
	private int rewardTimeLimit;

	//The current round of game play
	private int round;
    public int Round {
        get {return round;}
    }
	//Stars image representing current round
    private Image roundCounter;

	// List of words grabbed from the database
	private List<string> wordListDB;
	public List<string> WordListDB {
		get {return wordListDB;}
	}

	private List<string> wordImages;
	public List<string> WordImages{
		get {return wordImages;}
	}

    private List<string> wordAudio;
    public List<string> WordAudio
    {
        get { return wordAudio; }
    }

	//References to the slots that hold letters	
    private GameObject letterSlots;
	private GameObject solveSlots;
    private AudioSource audioSource;

	private System.Random rnd = new System.Random();

	// -------------- SETTINGS ----------------
	// These variables are set by the GetGameSettings function

	private string gameType;
	private string wordConfig;
	private int gameDifficulty;
	/* Sets whether letters should snap back to their original position if the are dragged
	Touch the incorrect position */
	public bool snapBack = true;

	// ------------- Statistics ---------------

	// Timer that starts as soon as the scene is loaded
	private double gameTimer = 0.0;
	// Stores the times it took to solve each word
	List<double> wordSolveTimes = new List<double>();
	// Stores the total solve time for a game
	double totalSolveTimes = 0.0; 
	// Store the indiviual and total number of tile moves 
	public int numTileMoves = -1;
	private int numTileMovesTotal;

	// Use this for initialization
	void Start () {

		//Create new DataService object and set reference
		ds = StartupScript.ds;

		//Create reference to GameController object for public access to send WordSolved event
		gc = gameObject;

        audioSource = gc.GetComponent<AudioSource>();

        // Get the main image
        mainImage = GameObject.FindGameObjectWithTag("GameMainImage").GetComponent<RawImage>();

        //Set the round to 0, the first round
        round = 0; 

		wordListDB = new List<string>();
		wordImages = new List<string>();
        wordAudio = new List<string>();

		// Get the game settings from the DB
		GetGameSettings ();

		// Load new word list from DB depending on the wordConfig
		if (wordConfig == "manual") {
			RetrieveWordsManual ();
		} else if (wordConfig == "automatic") {
			RetrieveWordsAutomatic ();
		}

		// Used for debugging
		//foreach (string item in WordListDB) {
		//	Debug.Log (item);
		//}

		// Load new word, image and slots
		GenerateRound();
		
	}
	
	// Update is called once per frame
	void Update () {
		gameTimer += Time.deltaTime; //Time.deltaTime will increase the value with 1 every second
	}

	///<summary>Generates the board for a round of the Scramble Game</summary>

	void GenerateRound(){
		
		letterSlots = GameObject.FindGameObjectWithTag(STARTING_SLOTS);
		solveSlots = GameObject.FindGameObjectWithTag(ENDING_SLOTS);

        testText.text = testText.text + "GENERATING ROUND\n";

        // Generate Picture
        SetWordImage(wordImages[round]);

        mainImage.enabled = true;

        // Update the star progess bar in the top right
        UpdateProgressBar();

		//generate all slots
		string word = wordListDB[round];
        testText.text = testText.text + "WORD IS " + word + "\n";
        string scrambledWord = ScrambleWord(word);
		for (int i = 0; i < word.Length; ++i){
			//Debug.Log("For loop entered");

			//Creates a starting slot GameObject from the slot prefab
			GameObject obj = Instantiate((GameObject)Resources.Load("Slot"), letterSlots.transform);

			string letter = scrambledWord[i].ToString().ToUpper();
			//Names the object the same as the letter
			obj.name = letter;
			//Sets the parent of the slot to the letterSlots object
			obj.transform.SetParent(letterSlots.transform);
			//Creates a letter from the letter prefab 
			GameObject l = Instantiate((GameObject)Resources.Load(("Letter Prefabs/Letter_" + letter)));		
			//Sets the parent of the letter to its starting slot with a false flag to use the letter's own scaling
			l.transform.SetParent(obj.transform, false);
		

			//Generates the solve slots associated with the word and in the correct order
			GameObject obj2 = Instantiate((GameObject)Resources.Load("Slot"), solveSlots.transform);
			obj2.name = word[i].ToString().ToUpper();
			obj2.transform.SetParent(solveSlots.transform);
		}

        if (wordAudio[round] != null)
        {
            testText.text = testText.text + "AUDIO IS " + wordAudio[round] + "\n";

            AudioClip tempClip = FileAccessUtil.LoadWordAudio(wordAudio[round]);

            if (tempClip == null)
            {
                testText.text = testText.text + "CLIP WAS NULL\n";
                return;
            }

            audioSource.clip = tempClip;
            audioSource.Play();

            // Set clip to null and let GC handle it
            tempClip = null;
        }
        else
        {
            testText.text = testText.text + "AUDIO WORD WAS NULL!!!\n";
        }
    }

    ///<summary>Fisher-Yates Shuffle algorithim used to help scramble the words</summary>

    private static void Shuffle(int[] array) {
        int arraySize = array.Length;
        int random;
        int temp;

        for (int i = 0; i < arraySize; i++) {
            System.Random rnd = new System.Random();
            random = i + (int)(rnd.NextDouble() * (arraySize - i));
            temp = array[random];
            array[random] = array[i];
            array[i] = temp;
        }
    }

	///<summary>Takes a string and scrambles the letters around</summary>
	///<param name="word">a string representing a word</param>
	///<returns>a string containing the scrambled word</returns>

	private string ScrambleWord(string word){
		Debug.Log("ScrambleWord entered");

        string scrambledWord = "";
        int arraySize = word.Length;
        int[] randomArray = new int[arraySize];

        for (int i = 0; i < arraySize; i++) {
            randomArray[i] = i;
        }

        Shuffle(randomArray);

        for (int i = 0; i < arraySize; i++) {
            scrambledWord += word[randomArray[i]];
        }

		if (scrambledWord == word) {
			Debug.Log ("Word and Scrambled word were the same");
			return ScrambleWord (word);
		} else {
			return scrambledWord;
		}
	}

	///<summary> Clears the child transforms from the transform given</summary>
	///<param name="t">the transform to clear the children from</param>

	private void ClearSlots(Transform t){
		foreach(Transform child in t){
			GameObject.Destroy(child.gameObject);
		}
	}

	///<summary> Handles the functionaly of the game when a word is solved by either starting a new word or ending the game</summary>



    public void ToggleMainImage()
    {
        mainImage.enabled = !mainImage.enabled;
    }

    public void WordSolved()
    {

        Debug.Log("total time game has run: " + gameTimer);                 // The total time that the game has run
        double currentSolveTime = 0.0;

        if (wordSolveTimes.Count > 0)
        {
            totalSolveTimes += wordSolveTimes[wordSolveTimes.Count - 1];
        }
        // Get the current solve time
        currentSolveTime = gameTimer - totalSolveTimes;
        // Update the wordSolveTimes 
        wordSolveTimes.Add(currentSolveTime);
        Debug.Log("solve time: " + wordSolveTimes[wordSolveTimes.Count - 1]);   // The time it took to solve the last word

        numTileMovesTotal += numTileMoves;
        numTileMoves = 0;

        //Check if it is the last round
        if (round == (wordListDB.Count - 1))
        {
            string wordsSolved = "";
            foreach (string word in WordListDB)
            {
                wordsSolved += (word + " ");
            }

            ds.InsertIntoGameHistory(1, (Round + 1), wordsSolved, Convert.ToInt32(totalSolveTimes), numTileMovesTotal);

            Debug.Log("You won! Reward screen next....");
            SceneController s = gameObject.GetComponent<SceneController>();
            s.LoadSceneByName("reward_selection");
            //Play some sort of you completed the game notification
            //Call scene load on reward scene
        }
        else
        {
            round++;
            ClearSlots(letterSlots.transform);
            ClearSlots(solveSlots.transform);
            GenerateRound();
        }
    }

    ///<summary>When the game is set to manual words, grabs all the words in the word listfrom the DB and saves them in a list</summary>

    private void RetrieveWordsManual() { 
		//Gets the word and it's image filepath and saves to both lists
		IEnumerable<Words> words = ds.GetWordsInWordsList();
        testText.text = testText.text + "LOADING WORDS MANUALLY!";
        foreach (var row in words){
			wordListDB.Add(row.word_name);
			wordImages.Add(row.word_image);
            wordAudio.Add(row.word_sound);
            testText.text = testText.text + row.word_sound + "\n";
        }
        //Scramble the order of the words
        ScrambleWordOrder();
	}

	///<summary>When the game is set to automatic words, grabs all words from the DB with the same difficulty as set in the settings</summary>

	private void RetrieveWordsAutomatic() {
		//Gets the word and it's image filepath and saves to both lists
		IEnumerable<Words> words = ds.GetWordsWithDifficulty(gameDifficulty);

        foreach (var row in words) {
			wordListDB.Add (row.word_name);
			wordImages.Add (row.word_image);
            wordAudio.Add(row.word_sound);
        }
        //Scramble the order of the words
        ScrambleWordOrder();
	}

	///<summary>Scrambles the order of the words in the wordListDB list</summary>

	private void ScrambleWordOrder(){
				// Scramble the list
		for (int i = 0; i < wordListDB.Count; i++) {
			string tempWord = wordListDB[i];
			string tempImage = wordImages[i];
            string tempAudio = wordAudio[i];

			int randomIndex = UnityEngine.Random.Range(i, wordListDB.Count);
			wordListDB[i] = wordListDB[randomIndex];
			wordImages[i] = wordImages[randomIndex];
            wordAudio[i] = wordAudio[randomIndex];

			wordListDB[randomIndex] = tempWord;
			wordImages[randomIndex] = tempImage;
            wordAudio[randomIndex] = tempAudio;
		}
	}

	///<summary> Calls the DB and retrieves all the game settings, then sets the values to the apporpriate variables</summary>

	private void GetGameSettings() {
		IEnumerable<Game_Settings> gameSettings = ds.GetGameSettingsTable();
		foreach (var row in gameSettings){
			gameType = row.game_type;
			gameDifficulty = row.game_difficulty;
			wordConfig = row.word_config;
			snapBack = Convert.ToBoolean(row.letter_snap);
            rewardTimeLimit = row.reward_time_limit;
		}
	}

	///<summary> Adjusts the visual round counter based on how many words have been solved</summary>
	
	private void UpdateProgressBar() {
		//generate round counter
		roundCounter = GameObject.FindGameObjectWithTag(RND_CNTR).GetComponent<Image>();
		float totalWords = (float)wordListDB.Count;
		float percentToFill = ((float)round + 1.00f) / totalWords;	
		roundCounter.fillAmount = percentToFill;
	}

    private void SetWordImage(string imageName)
    {
        StringBuilder sb = new StringBuilder();

        Debug.Log("CHECK: " + wordImages[round]);
        Texture2D texture = new Texture2D(325, 325); 

        bool success = true;
        byte[] bytes = null;

        sb.Append("New image name is " + wordImages[round] + "\n");
        testText.text = sb.ToString();

        if (imageName.Contains(".png"))
        {
            sb.Append("Image contains .png going into load photo.\n");
            testText.text = sb.ToString();
            bytes = FileAccessUtil.LoadWordPic(imageName);

            if (bytes == null)
            {
                sb.Append("GAME: Returned bytes were nil!\n");
                testText.text = sb.ToString();
                return;
            }

            sb.Append("GAME: Bytes are clear. Loading image!\n");
            testText.text = sb.ToString();

            success = texture.LoadImage(bytes);
        }
        else
        {
            texture = Resources.Load<Texture2D>("WordPictures/" + wordImages[round]);
        }

        sb.Append("GAME: Checking texture if null. \n");
        testText.text = sb.ToString();

        if (texture == null)
        {
            sb.Append("GAME: Texture was NULL!\n");
            testText.text = sb.ToString();
            // TODO: User error feedback
            return;
        } else if (!success)
        {
            sb.Append("GAME: Success was false!\n");
            testText.text = sb.ToString();
            // TODO: User error for texture.LoadImage
            return;
        }

        sb.Append("GAME: Success true and texture clear! Resizing!\n");
        testText.text = sb.ToString();

        mainImage.texture = texture;
        RectTransform rt = mainImage.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(325, 325);
    }
}