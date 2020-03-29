using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public class DO_PlayListObject : IComparable<DO_PlayListObject>
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int order_id { get; set; }
    public int duration { get; set; }
    public int type_id { get; set; }
    public string json { get; set; }
    public string custom_json { get; set; }

    public DO_PlayListObject() { }

    public DO_PlayListObject(int order, int length, int typeId, string jsonStr, string customStr = null)
    {
        order_id = order;
        duration = length;
        type_id = typeId;
        json = jsonStr;
        custom_json = customStr;
    }

    public DO_PlayListObject(int dbId, int order, int length, int typeId, string jsonStr, string customStr = null)
    {
        id = dbId;
        order_id = order;
        duration = length;
        type_id = typeId;
        json = jsonStr;
        custom_json = customStr;
    }

    public int CompareTo(DO_PlayListObject other)
    {
        // Check if other is null return 1
        if (other == null) return 1;

        return this.order_id.CompareTo(other.order_id);
    }

    public bool ValidateChild(DataService data)
    {
        switch (type_id)
        {
            case 0:
                DO_WordScramble scramble = JsonUtility.FromJson<DO_WordScramble>(json);
                return scramble.ValidateData(data);
            case 1:
                // TODO: FINISH THE CHECK
                //DO_Reward reward = DictionarySerializeUtil.JsonToRewardDictionary(json);
                //return reward.ValidateData(data);
                return true;
            case 2:
                // TODO: FINISH THE CHECK
                DO_FlashCard flash = JsonUtility.FromJson<DO_FlashCard>(json);
                return flash.ValidateData(data);
            case 3:
                DO_CountingGame counting = JsonUtility.FromJson<DO_CountingGame>(json);
                return counting.ValidateData(data);
            case 4:
                DO_KeyboardGame keyboard = JsonUtility.FromJson<DO_KeyboardGame>(json);
                return keyboard.ValidateData(data);
            case 5:
                DO_MemoryCards memory = JsonUtility.FromJson<DO_MemoryCards>(json);
                return memory.ValidateData(data);
            case 6:
                DO_MatchingGame matching = JsonUtility.FromJson<DO_MatchingGame>(json);
                return matching.ValidateData(data);
            default:
                // TODO: throw and log an error
                Debug.Log("Error in DO_PlaylistObject");
                return false;
        }
    }
}
