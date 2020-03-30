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
        model.ResetSceneData();
    }

    public override void GetCoworkers(MasterClass newMaster)
    {
        model = (MOD_PlayList)master.GetModel("MOD_PlayList");
    }

    public override void SceneChange(string scene)
    {
        master.RequestSceneChange(scene);
    }

    public Dictionary<int, string> GetIdToWordsDict()
    {
        return model.wordDict;
    }

    public List<string> GetWordTagsList()
    {
        return model.wordTagsList;
    }

    public Dictionary<int, string> GetIdToWordTagsDict()
    {   
        return model.tagsDict;
    } 

    /// <summary>
    /// Returns the total number of words currently in the words table of the database.
    /// </summary>
    /// <returns></returns>
    public int GetTotalWordsCount()
    {
        return model.GetTotalWords();
    }

    public string CreateActivityListString(int index, int typeId)
    {       
        StringBuilder builder = new StringBuilder();

        int max;

        switch (typeId)
        {
            case 0:
                DO_WordScramble scramble = JsonUtility.FromJson<DO_WordScramble>(model.GetJsonData(index));

                max = scramble.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[scramble.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[scramble.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 1:
                DO_ChooseReward reward = JsonUtility.FromJson<DO_ChooseReward>(model.GetJsonData(index));
                
                max = reward.rewardIdsList.Count;      

                break;

            case 2:
                DO_FlashCard flash = JsonUtility.FromJson<DO_FlashCard>(model.GetJsonData(index));
                
                max = flash.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {                      
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[flash.wordIdList[idx]]));
                    }
                    else
                    {                       
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[flash.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 3:
                DO_CountingGame counting = JsonUtility.FromJson<DO_CountingGame>(model.GetJsonData(index));
                
                max = counting.wordIds.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[counting.wordIds[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[counting.wordIds[idx]])), ", ");
                    }
                }

                break;

            case 4:
                DO_KeyboardGame keyboard = JsonUtility.FromJson<DO_KeyboardGame>(model.GetJsonData(index));

                max = keyboard.wordIdList.Count;
                
                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[keyboard.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[keyboard.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 5:
                DO_MemoryCards memory = JsonUtility.FromJson<DO_MemoryCards>(model.GetJsonData(index));
                
                max = memory.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[memory.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[memory.wordIdList[idx]])), ", ");
                    }
                }

                break;

            case 6:
                DO_MatchingGame matching = JsonUtility.FromJson<DO_MatchingGame>(model.GetJsonData(index));
                
                max = matching.wordIdList.Count;

                for (int idx = 0; idx < max; idx++)
                {
                    if (idx == (max - 1))
                    {
                        builder.AppendFormat("{0}", TidyCase(model.wordDict[matching.wordIdList[idx]]));
                    }
                    else
                    {
                        builder.AppendFormat("{0}{1}", (TidyCase(model.wordDict[matching.wordIdList[idx]])), ", ");
                    }
                }

                break;

            default:
                // TODO: log an error
                return null;
        }       

        return builder.ToString();
         
            //case 7:
            //    break;
            //case 8:
            //    break;
            //case 10:
            //    break;
            //case 11:
    }

    public bool AddOrEditEntry(string typeStr, int duration, object dataObject)
    {
        string json = "";

        int typeId = model.GetTypeId(typeStr);

        switch (typeId)
        {
            case 0:
                json = JsonUtility.ToJson((DO_WordScramble)dataObject);
                break;

            case 1:                
                json = JsonUtility.ToJson((DO_ChooseReward)dataObject);
                break;

            case 2:
                json = JsonUtility.ToJson((DO_FlashCard)dataObject);               
                break;

            case 3:
                json = JsonUtility.ToJson((DO_CountingGame)dataObject);
                break;

            case 4:
                json = JsonUtility.ToJson((DO_KeyboardGame)dataObject);
                break;

            case 5:
                json = JsonUtility.ToJson((DO_MemoryCards)dataObject);
                break;
           
            case 6:
                json = JsonUtility.ToJson((DO_MatchingGame)dataObject);
                break;

            default:
                // TODO: log an error
                return false;

        }

        return model.AddOrEditEntryData(typeId, duration, json);
              
            //case 7:
            //    break;
            //case 8:
            //    break;        
            //case 10:
            //    break;
            //case 11:
            //    break;         
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

