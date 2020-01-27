using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CON_PlayList : AB_Controller
{
    private MOD_PlayList model;
    private MAS_PlayList master;

    public CON_PlayList(MasterClass newMaster) : base(newMaster)
    {
        master = (MAS_PlayList)newMaster;
    }

    public override void ClearData()
    {
        Debug.Log("CON_PL: Calling model to clear scene data");
        model.ResetSceneData();
    }

    public override void GetCoworkers(MasterClass newMaster)
    {
        model = (MOD_PlayList)master.GetModel("MOD_PlayList");
    }

    public override void SceneChange(string scene)
    {
        Debug.Log("CON_PL: Attempting to change the scene to " + scene);
        master.RequestSceneChange(scene);
    }

    public Dictionary<int, string> GetIdToWordsDict()
    {
        Debug.Log("getting word dictionary from the model. Count = " + model.wordDict.Count);
        return model.wordDict;
    }

    public List<string> GetWordTagsList()
    {
        Debug.Log("getting word tags list from the model. Count = " + model.wordTagsList.Count);
        return model.wordTagsList;
    }

    public Dictionary<int, string> GetIdToWordTagsDict()
    {
        Debug.Log("getting word id to word tags dictionary from the model. Count = " + model.tagsDict.Count);
   
        return model.tagsDict;
    } 

    public string CreateActivityListString(int index, int typeId)
    {
        Debug.Log("CON: INDEX IS " + index.ToString());
        Debug.Log("CON: TYPE IS " + typeId.ToString());

        StringBuilder builder = new StringBuilder();

        int max;

        switch (typeId)
        {
            case 0:
                //Debug.Log("CON: IN CASE 0");

                DO_WordScramble scramble = JsonUtility.FromJson<DO_WordScramble>(model.GetJsonData(index));

                //Debug.Log("CON: DESERIALIZED SCRAMBLE!");

                max = scramble.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    //Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                    if (idx == (max - 1))
                    {
                        //Debug.Log("CON: IDX == MAX - 1");

                        builder.AppendFormat("{0}", TidyCase(model.wordDict[scramble.wordIdList[idx]]));
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                    else
                    {
                        Debug.Log("CON: ELSE!");

                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[scramble.wordIdList[idx]])), ", ");
                        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                }

                break;

            

            case 1:
                //Debug.Log("CON: IN CASE 1");

                DO_ChooseReward reward = JsonUtility.FromJson<DO_ChooseReward>(model.GetJsonData(index));

                //Debug.Log("CON: DESERIALIZED REWARD!");

                max = reward.rewardIdsList.Count;

                //for (int idx = 0; idx < max; idx++)
                //{
                //    Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                //    if (idx == (max - 1))
                //    {
                //        Debug.Log("CON: IDX == MAX - 1");

                //        builder.AppendFormat("{0}", model.rewardDict[reward.rewardIdsList[idx]]);
                //        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                //    }
                //    else
                //    {
                //        Debug.Log("CON: ELSE!");

                //        builder.AppendFormat("{0}{1}", model.rewardDict[reward.rewardIdsList[idx]], ", ");
                //        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                //    }
                //}

                break;

            case 2:
                //Debug.Log("CON: IN CASE 2");

                DO_FlashCard flash = JsonUtility.FromJson<DO_FlashCard>(model.GetJsonData(index));
                Debug.Log("flashcard word at idx 0 is: " + flash.wordIdList[0]);

                //Debug.Log("CON: DESERIALIZED FLASH!");

                max = flash.wordIdList.Count;
                Debug.Log("max = " + max);

                for (int idx = 0; idx < max; idx++)
                {
                    //Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                    if (idx == (max - 1))
                    {
                        Debug.Log("CON: IDX == MAX - 1");
                        Debug.Log("word is : " + model.wordDict[flash.wordIdList[idx]]);
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[flash.wordIdList[idx]]));
                        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                    else
                    {
                        //Debug.Log("CON: ELSE!");

                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[flash.wordIdList[idx]])), ", ");
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                }

                break;

            case 3:
                //Debug.Log("CON: IN CASE 3");

                DO_CountingGame counting = JsonUtility.FromJson<DO_CountingGame>(model.GetJsonData(index));

                //Debug.Log("CON: DESERIALIZED COUNTING!");

                max = counting.wordIds.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    //Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                    if (idx == (max - 1))
                    {
                        //Debug.Log("CON: IDX == MAX - 1");

                        builder.AppendFormat("{0}", TidyCase(model.wordDict[counting.wordIds[idx]]));
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                    else
                    {
                        //Debug.Log("CON: ELSE!");

                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[counting.wordIds[idx]])), ", ");
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                }

                break;

            case 4:
                //Debug.Log("******************************************************************************CON: IN CASE 4");

                DO_KeyboardGame keyboard = JsonUtility.FromJson<DO_KeyboardGame>(model.GetJsonData(index));

                //Debug.Log("CON: DESERIALIZED KEYBOARD!");

                max = keyboard.wordIdList.Count;

                //Debug.Log("max = " + max);

                for (int idx = 0; idx < max; idx++)
                {
                    //Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                    if (idx == (max - 1))
                    {
                        //Debug.Log("CON: IDX == MAX - 1");

                        builder.AppendFormat("{0}", TidyCase(model.wordDict[keyboard.wordIdList[idx]]));
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                    else
                    {
                        //Debug.Log("CON: ELSE!");

                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[keyboard.wordIdList[idx]])), ", ");
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                }

                break;

            case 5:
                //Debug.Log("CON: IN CASE 5");

                DO_MemoryCards memory = JsonUtility.FromJson<DO_MemoryCards>(model.GetJsonData(index));

                //Debug.Log("CON: DESERIALIZED MEMORY GAME!");

                max = memory.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    //Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                    if (idx == (max - 1))
                    {
                        //Debug.Log("CON: IDX == MAX - 1");

                        builder.AppendFormat("{0}", TidyCase(model.wordDict[memory.wordIdList[idx]]));
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                    else
                    {
                        //Debug.Log("CON: ELSE!");

                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[memory.wordIdList[idx]])), ", ");
                        //Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                }

                break;
            default:
                // TODO: throw and log an error
                return null;
        }

        return builder.ToString();

        /*       
                     
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 10:
                break;
            case 11:
                break;
         */

    }

    public bool AddOrEditEntry(string typeStr, int duration, object dataObject)
    {
        string json = "";
        int typeId = model.GetTypeId(typeStr);
        //Debug.Log("made it to the int typeID");


        //Debug.Log("CON_PL: Add or Edit requested for activity of type " + typeStr);

        switch (typeId)
        {
            case 0:
                json = JsonUtility.ToJson((DO_WordScramble)dataObject);
                //Debug.Log("CON_PL: Case 0 SCRAMBLE reached");
                break;

            case 1:                
                json = JsonUtility.ToJson((DO_ChooseReward)dataObject);
                //Debug.Log("CON_PL: Case 1 REWARD reached");
                break;
            case 2:
                json = JsonUtility.ToJson((DO_FlashCard)dataObject);
                //Debug.Log("CON_PL: Case 2 FLASH reached");
                //Debug.Log("Flash json = " + json);
                break;

            case 3:
                json = JsonUtility.ToJson((DO_CountingGame)dataObject);
                //Debug.Log("CON_PL: Case 3 COUNTING reached");
                break;

            case 4:
                json = JsonUtility.ToJson((DO_KeyboardGame)dataObject);
                //Debug.Log("CON_PL: Case 4 KEYBOARD reached");
                break;

            case 5:
                json = JsonUtility.ToJson((DO_MemoryCards)dataObject);
                //Debug.Log("CON PL: Case 5 MEMORY reached");
                break;
            default:
                // TODO: throw and log an error
                return false;
        }

        // On change

        //Debug.Log("CON_PL: calling model for AddOrEdit of type " + typeStr);
        return model.AddOrEditEntryData(typeId, duration, json);

        /*       
                     
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;        
            case 10:
                break;
            case 11:
                break;
         */
    }

    //########################### Choose Reward Functionality ####################
    public Dictionary<int, string> RequestRewards()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();

        foreach (var row in model.GetRewardsTable())
        {
            dict.Add(row.reward_id, row.reward_name);
        }

        return dict;
    }

    /// <summary>
    /// Takes in a string a returns it with the first letter upper case and the rest lower case.
    /// </summary>
    /// <param name="sourceStr"></param>
    /// <returns></returns>
    public static string TidyCase(string sourceStr)
    {
        Debug.Log("TidyCase for " + sourceStr);
        sourceStr.Trim();
        if (!string.IsNullOrEmpty(sourceStr))
        {
            char[] allCharacters = sourceStr.ToCharArray();

            for (int i = 0; i < allCharacters.Length; i++)
            {
                char character = allCharacters[i];
                if (i == 0)
                {
                    if (char.IsLower(character))
                    {
                        allCharacters[i] = char.ToUpper(character);
                    }
                }
                else
                {
                    if (char.IsUpper(character))
                    {
                        allCharacters[i] = char.ToLower(character);
                    }
                }
            }
            return new string(allCharacters);
        }
        return sourceStr;
    }


    // Return a copy of the play list
    public int GetDurationByIndex(int idx) { return model.GetDurationValue(idx); }
    public string GetJsonByIndex(int idx) { return model.GetJsonData(idx); }
    public string[] GetTypeStrings() { return model.GetTypeStrings(); }
    public string GetTypeString(int typeId) { return model.GetTypeString(typeId); }
    public List<DO_PlayListObject> GetPlayList() { return model.GetPlayListObjects(); }
    public bool CheckIfNewEntry() { return model.creatingNewEntry; }
    public void CreatingNew() { model.creatingNewEntry = true; }
    public void EditingEntry() { model.creatingNewEntry = false; }
    public void SetActiveEntryIndex(int currentIdx) { model.activeEntryIndex = currentIdx; }
    public int GetActiveContextIndex() { return model.activeEntryIndex; }
    public bool SwapListEntries(int idxA, int idxB) { return model.SwapEntryData(idxA, idxB); }
    public bool RemoveEntryData(int idx) { return model.RemoveEntryData(idx); }
    public bool DeleteAllPlaylist() { return model.DeletePlaylist(); }
    public bool CheckIfInfiniteLoop() { return model.infiniteLoops; }
    public int GetLoopNumber() { return model.loopIterations; }
    public bool CheckIfPassLocked() { return model.passLocked; }
    public bool SetLoopData(int playId, bool infinite, int iterations, bool passLocked) { return model.SetLoopData(playId, infinite, iterations, passLocked); }
    public void VerifyData() { model.VerifyData(); }
}

/*
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CON_PlayList : AB_Controller
{
    private MOD_PlayList model;
    private MAS_PlayList master;

    public CON_PlayList(MasterClass newMaster) : base(newMaster)
    {
        master = (MAS_PlayList)newMaster;
    }

    public override void ClearData()
    { 
        Debug.Log("CON_PL: Calling model to clear scene data");
        model.ResetSceneData();
    }

    public override void GetCoworkers(MasterClass newMaster)
    {
        model = (MOD_PlayList)master.GetModel("MOD_PlayList");
    }

    public override void SceneChange(string scene)
    {
        Debug.Log("CON_PL: Attempting to change the scene to " + scene);
        master.RequestSceneChange(scene);
    }

    public Dictionary<int, string> GetIdToWordsDict() { return model.wordDict; }

    public string CreateActivityListString(int index, int typeId)
    {
        Debug.Log("CON: INDEX IS " + index.ToString());
        Debug.Log("CON: TYPE IS " + typeId.ToString());

        StringBuilder builder = new StringBuilder();
        int max;

        switch (typeId)
        {
            case 0:
                Debug.Log("CON: IN CASE 0");

                DO_WordScramble scramble = JsonUtility.FromJson<DO_WordScramble>(model.GetJsonData(index));

                Debug.Log("CON: DESERIALIZED SCRAMBLE!");

                max = scramble.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                    if (idx == (max - 1))
                    {
                        Debug.Log("CON: IDX == MAX - 1");

                        builder.AppendFormat("{0}", model.wordDict[scramble.wordIdList[idx]]);
                        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                    else
                    {
                        Debug.Log("CON: ELSE!");

                        builder.AppendFormat("{0}{1}", model.wordDict[scramble.wordIdList[idx]], ", ");
                        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                }

                break;
            case 1:
                Dictionary<string, bool> dict = DictionarySerializeUtil.JsonToRewardDictionary(model.GetJsonData(index));

                int count = dict.Count;
                int itemNum = 0;

                foreach (var entry in dict){
                    itemNum++;
                    if (itemNum == dict.Count){
                        builder.Append(entry.Key);
                    }
                    else{
                        builder.Append(entry.Key + ", ");
                    }
                }

                break;
            case 2:
                Debug.Log("CON: IN CASE 0");

                DO_FlashCard flash = JsonUtility.FromJson<DO_FlashCard>(model.GetJsonData(index));

                Debug.Log("CON: DESERIALIZED SCRAMBLE!");

                max = flash.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    Debug.Log("CON: IN FOR AT IDX " + idx.ToString());
                    if (idx == (max - 1))
                    {
                        Debug.Log("CON: IDX == MAX - 1");

                        builder.AppendFormat("{0}", model.wordDict[flash.wordIdList[idx]]);
                        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                    else
                    {
                        Debug.Log("CON: ELSE!");

                        builder.AppendFormat("{0}{1}", model.wordDict[flash.wordIdList[idx]], ", ");
                        Debug.Log("CON: BUILDER IS NOW " + builder.ToString());
                    }
                }

                break;
            default:
                // TODO: throw and log an error
                return null;
        }

        return builder.ToString();
        
    }

    /*
    public bool AddOrEditEntry(string typeStr, int duration, object dataObject)
    {
        string json = "";
        int typeId = model.GetTypeId(typeStr);

        Debug.Log("CON_PL: Add or Edit requested for activity of type " + typeStr);

        switch (typeId)
        {
            case 0:
                json = JsonUtility.ToJson((DO_WordScramble)dataObject);
                Debug.Log("CON_PL: Case 0 SCRAMBLE reached");
                break;
            case 1:
                Dictionary<string, bool> dict = (Dictionary<string, bool>) dataObject;
                json = DictionarySerializeUtil.RewardDictionaryToJson(dict);
                
                Debug.Log("CON_PL: Case 1 REWARD reached");
                break;
            case 2:
                json = JsonUtility.ToJson((DO_FlashCard)dataObject);
                Debug.Log("CON_PL: Case 2 FLASH reached");
                break;
            default:
                // TODO: throw and log an error
                return false;
        }

        Debug.Log("CON_PL: calling model for AddOrEdit of type " + typeStr);
        return model.AddOrEditEntryData(typeId, duration, json);
        
    }

/*

    //########################### Choose Reward Functionality ####################
    public Dictionary<string, int> RequestRewards(){
        Dictionary<string, int> dict = new Dictionary<string, int>();

        foreach (var row in model.GetRewardsTable()){
            dict.Add(row.reward_name, row.reward_id);
        }

        return dict;
    }

    // Return a copy of the play list
    public int GetDurationByIndex(int idx) { return model.GetDurationValue(idx); }
    public string GetJsonByIndex(int idx) { return model.GetJsonData(idx); }
    public string[] GetTypeStrings() { return model.GetTypeStrings(); }
    public string GetTypeString(int typeId) { return model.GetTypeString(typeId); }
    public List<DO_PlayListObject> GetPlayList() { return model.GetPlayListObjects(); }
    public bool CheckIfNewEntry() { return model.creatingNewEntry; }
    public void CreatingNew() { model.creatingNewEntry = true; }
    public void EditingEntry() { model.creatingNewEntry = false; }
    public void SetActiveEntryIndex(int currentIdx) { model.activeEntryIndex = currentIdx; }
    public int GetActiveContextIndex() { return model.activeEntryIndex; }
    public bool SwapListEntries(int idxA, int idxB) { return model.SwapEntryData(idxA, idxB); }
    public bool RemoveEntryData(int idx) { return model.RemoveEntryData(idx); }
    public bool CheckIfInfiniteLoop() { return model.infiniteLoops; }
    public int GetLoopNumber() { return model.loopIterations; }
    public bool CheckIfPassLocked() { return model.passLocked; }
    public bool SetLoopData(int playId, bool infinite, int iterations, bool passLocked) { return model.SetLoopData(playId, infinite, iterations, passLocked); }
    public void VerifyData() { model.VerifyData(); }
}
*/
