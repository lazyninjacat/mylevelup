
using SQLite4Unity3d;
using UnityEngine;
using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService  {

	private SQLiteConnection _connection;

	public DataService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
	}

    //########## Table Return queries - returns all data in each table ##########//

    ///<summary> Gets all rows from the Game_Config table</summary>
    ///<returns>IEnumerable object of the Game_Config type</returns>
    public IEnumerable<Game_Config> GetGameConfigTable()
    {
        return _connection.Table<Game_Config>();
    }

	///<summary>Gets all rows from the Rewards table</summary>
	///<returns>IEnumerable object of the Reward type</returns>
	public IEnumerable<Rewards> GetRewardsTable(){
		return _connection.Table<Rewards>();
	}

	///<summary>Gets all rows from the Words table</summary>
	///<returns>IEnumerable object of the Word type</returns>
	public IEnumerable<Words> GetWordsTable(){
		return _connection.Table<Words>();
	}

	///<summary>Gets all word id's and word name's of all the words in the Words_List table</summary>
	///<returns>IEnumerable object of the Words type</returns>
	public IEnumerable<Words> GetWordsInWordsList(){
		string q = "SELECT Words.word_name, Words.word_image, Words.word_sound FROM Words INNER JOIN Words_List ON Words.word_id = Words_List.word_id";
		return _connection.Query<Words>(q);
	}	

	///<summary>Inserts a word into the Words_List table</summary>
	///<param name="wordId">the word_id fk of the word</param>
	///<param name="wlId">the word_list_id to add the word to
	///<returns>an int value of how many rows were successfully inserted</returns>
	public int InsertIntoWordList(int wordId, int wlId){
		string query = "INSERT INTO Words_List (word_id, word_list_id) VALUES(?, ?)";
		return _connection.Execute(query, wordId, wlId);
	}
	
	///<summary>Deletes a word from the Words_List table</summary>
	///<param name="wordId">the word_id of the word to be deleted</param>
	///<param name="wlId">the word_list_id that the word is to be removed from</param>
	///<returns>an int value of how many rows were successfully deleted</returns>
	public int DeleteFromWordList(int wordId, int wlId){
		string query = "DELETE FROM Words_List WHERE word_id = ? AND word_list_id = ?";
		return _connection.Execute(query, wordId, wlId);
	}

	///<summary>SQL query to reset the auto increment sequence number back to the value given</summary>
	///<param name="tableName">the name of the table to reset the sequence number</param>
	///<param name="resetToInt">the number to reset the sequence to</param>
	public void ReseedTable(string tableName, int resetToInt){
		string cmd = "UPDATE SQLITE_SEQUENCE SET seq = ? WHERE name = ?";
		_connection.Execute(cmd, resetToInt, tableName);
        
	}

	///<summary>Creates an MD5 hash of a string and returns the string</summary>
	///<param name="strToEncrypt">the string you want to encrypt</param>
	///<returns>a hashed string</returns>
	public string Md5Sum(string strToEncrypt){
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
 
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
 
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
 
		for (int i = 0; i < hashBytes.Length; i++){
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
 
		return hashString.PadLeft(32, '0');
	}

	///<summary>Checks if a given word already exists in the DB</summary>
	///<param name="wordToAdd">the word we are checking for existance</param>
	///<returns>a boolean to signify whether it exists or not</returns>
	public bool CheckWordExistance(string wordToAdd){
		string query = "SELECT word_name FROM Words WHERE word_name = \'" + wordToAdd.ToLower() + "\'";
		IEnumerable<Words> results = _connection.Query<Words>(query);
		foreach (var row in results){
			if (string.Equals(row.word_name, wordToAdd, System.StringComparison.OrdinalIgnoreCase)){
				return true;
			}
		}
		return false;
	}

    ///<summary>Creates a new word in the DB</summary>
    ///<param name="wordName">the word being added</param>
    ///<param name="stockCustom">whether the word is stock or custom</param>
    ///<parem name="wordTags"> any descriptor tags</parem>
    ///<parem name="wordSound"> filename of word audio clip</parem>
    ///<parem name="wordImage"> filename of primary image</parem>
    ///<returns>an int representing how many rows were successfully inserted</returns>
    public int CreateWord(string wordName, string stockCustom, string wordTags, string wordSound, string wordImage){
        string query = "INSERT INTO Words (word_name, stock_custom, word_tags, word_sound, word_image) VALUES(?, ?, ?, ?, ?)";
		return _connection.Execute(query, wordName, stockCustom, wordTags, wordSound, wordImage);
	}
    
    public IEnumerable<DO_PlayListObject> GetPlayList()
    {
        string query = "SELECT id, order_id, duration, type_id, json, custom_json FROM Play_List";
        return _connection.Query<DO_PlayListObject>(query);
    }

    public int DeleteFromPlayList(int id)
    {
        string query = "DELETE FROM Play_List WHERE id = ?";
        return _connection.Execute(query, id);
    }

    public int DeleteAllPlaylist()
    {
        string query = "DELETE FROM Play_List";
        return _connection.Execute(query);
    }

    public int UpdatePlayListAutoIds()
    {
        string query = "UPDATE SQLITE_SEQUENCE SET SEQ = 0 WHERE NAME = 'Play_List'";
        return _connection.Execute(query);
    }

    public int ChangeOrderIdValue(int dbId, int newOrderId)
    {
        string query = "UPDATE Play_List SET order_id = ? WHERE id = ?";
        return _connection.Execute(query, newOrderId, dbId);
    }

    public int AddNewPlayListRow(DO_PlayListObject data)
    {
        string query = "INSERT INTO Play_List (order_id, duration, type_id, json, custom_json) VALUES(?,?,?,?,?)";
        return _connection.Execute(query, data.order_id, data.duration, data.type_id, data.json, data.custom_json);
    }

    public int EditPlayListRow(DO_PlayListObject data)
    {
        string query = "UPDATE Play_List SET order_id = ?, duration = ?, json = ?, custom_json = ? WHERE id = ?";
        return _connection.Execute(query, data.order_id, data.duration, data.json, data.custom_json, data.id);
    }

    public IEnumerable<DO_PlayListObject> GetLastInsertedRowId(int orderId)
    {
        string query = "SELECT id FROM Play_List WHERE order_id = \'" + orderId + "\'";
        return _connection.Query<DO_PlayListObject>(query);
    }

    public int UpdateConfingData(int playId, int infinite, int iterations, int passLocked)
    {
        string query = "UPDATE Game_Config SET infinite_loop = ?, iteration_number = ?, pass_locked = ? WHERE play_list_id = ?";
        return _connection.Execute(query, infinite, iterations, passLocked, playId);
    }

    public int UpdateConfingData(Game_Config data)
    {
        string query = "UPDATE Game_Config SET infinite_loop = ?, iteration_number = ?, pass_locked = ? WHERE play_list_id = ?";
        return _connection.Execute(query, data.infinite_loop, data.iteration_number, data.pass_locked, data.play_list_id);
    }

    public int CreateNewConfigData(int playId, int infinite, int iterations, int passLocked)
    {
        string query = "INSERT INTO Game_Config (play_list_id, infinite_loop, iteration_number, pass_locked) VALUES(?,?,?,?)";
        return _connection.Execute(query, playId, infinite, iterations, passLocked);
    } 

    public int CreateNewConfigData(Game_Config data)
    {
        string query = "INSERT INTO Game_Config (play_list_id, infinite_loop, iteration_number, pass_locked) VALUES(?,?,?,?)";
        return _connection.Execute(query, data.play_list_id, data.infinite_loop, data.iteration_number, data.pass_locked);
    }

    public IEnumerable<Game_Config> GetConfigByPlayListId(int playId)
    {
        string query = "SELECT infinite_loop, iteration_number, pass_locked FROM Game_Config WHERE play_list_id = \'" + playId + "\'";
        return _connection.Query<Game_Config>(query);
    }

    // Edits the data in a database word entry
    public int EditWordEntry(WordDO wordData)
    {
        string query = "UPDATE words SET stock_custom = ?, word_tags = ? WHERE word_id = ?";
        return _connection.Execute(query, wordData.StockCustom, wordData.WordTags, wordData.IdNum);
    }

    public int AddWordEntry(string word, WordDO wordData)
    {
        string query = "INSERT INTO Words (word_name, stock_custom, word_tags) VALUES(?, ?, ?)";
        return _connection.Execute(query, word, wordData.StockCustom, wordData.WordTags);
    }

    public int DeleteFromWords(int wordId)
    {
        string query = "DELETE FROM Words WHERE word_id = ?";
        return _connection.Execute(query, wordId);
    }

    public float GetWordMastery(string word)
    {
        string query = "SELECT AVG(total_errors) FROM Mastery WHERE word = ?";
        return _connection.ExecuteScalar<float>(query, word);
    }

    public int GetWordPlayCount(string word)
    {
        string query = "SELECT COUNT(total_errors) FROM Mastery WHERE word = ? ";
        return _connection.ExecuteScalar<int>(query, word);
    }

    public int GetLastIdInWords()
    {
        string query = "SELECT word_id FROM Words WHERE word_id = (SELECT MAX(word_id) FROM Words)";
        return _connection.ExecuteScalar<int>(query);
    }

    public IEnumerable<Words> GetWordsAndIdsOnly()
    {
        string query = "SELECT word_id, word_name FROM Words";
        return _connection.Query<Words>(query);
    }     

    public IEnumerable<Words> GetLastById()
    {
        string query = "SELECT * FROM Words WHERE word_id = (SELECT MAX(word_id) FROM Words)";
        return _connection.Query<Words>(query);
    }

    /// <summary>
    /// Checks the DB for a word entry with the given id. Returns 1 if found and 0 if not.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>int</returns>
    public int DoesWordIdExist(int id)
    {
        string query = "SELECT EXISTS (SELECT * FROM Words WHERE word_id = ? LIMIT 1)";
        return _connection.ExecuteScalar<int>(query, id);
    }

    /// <summary>
    /// Checks the DB for a play list entry with the given id. Returns 1 if found and 0 if not.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>int</returns>
    public int DoesPlayEntryExist(int id)
    {
        string query = "SELECT EXISTS (SELECT * FROM Play_List WHERE id = ? LIMIT 1)";
        return _connection.ExecuteScalar<int>(query, id);
    }

    /// <summary>
    /// Returns the last added play list entry id from the database.
    /// </summary>
    /// <returns>int</returns>
    public int GetLastPlayEntryId()
    {
        string query = "SELECT id FROM Play_List WHERE id = (SELECT MAX(id) FROM Play_List)";
        return _connection.ExecuteScalar<int>(query);
    }

	public IEnumerable<Rewards> SearchForReward(string rewardName){
		string query = "SELECT reward_name FROM Rewards WHERE reward_name = \'" + rewardName + "\'";
		return _connection.Query<Rewards>(query);
	}

    public IEnumerable<Rewards> FindRewardType(string rewardName)
    {
        string query = "SELECT reward_type FROM Rewards WHERE reward_name = \'" + rewardName + "\'";
        return _connection.Query<Rewards>(query);
    }

    public IEnumerable<Rewards> FindRewardUrl(string rewardName)
    {
        string query = "SELECT reward_url FROM Rewards WHERE reward_name = \'" + rewardName + "\'";
        return _connection.Query<Rewards>(query);
    }

	public int AddReward(string rewardName, string reward_type, string reward_url)
    {
        string query = "INSERT INTO Rewards (reward_name, reward_type, reward_url) VALUES(?,?,?)";
        return _connection.Execute(query, rewardName, reward_type, reward_url);
    }

    public int RecordRoundData(string gameType, string word, int totalErrors)
    {
        string query = "INSERT INTO Mastery (game_type, word, total_errors) VALUES(?,?,?)";
        return _connection.Execute(query, gameType, word, totalErrors);
    }

    public int RecordWebRewardData(string url, DateTime startTime, DateTime endTime)
    {
        string query = "INSERT INTO WebRewardLog (time_start, time_end, url) VALUES(?,?,?)";
        return _connection.Execute(query, startTime, endTime, url);
    }


    public int DeleteReward(string rewardName){
		string query = "DELETE FROM Rewards WHERE reward_name = ?";
        return _connection.Execute(query, rewardName);
	}

   public int InsertUrlIntoRewards(string rewardName, string reward_url)
    {
        string query = "UPDATE Rewards SET reward_url = ? WHERE reward_name = ?";
        return _connection.Execute(query, reward_url, rewardName);
    }
    
}

  /*  © 2018 GitHub, Inc.
    Terms
    Privacy
    Security
    Status
    Help

    Contact GitHub
    API
    Training
    Shop
    Blog
    About

Press h to open a hovercard with more details.
*/