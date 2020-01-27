using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DO_ChooseReward : IValidateData
{
    // RewardsList DO members
    public List<int> rewardIdsList;
    public int duration;
    

    public DO_ChooseReward(List<int> rewards, int duration)
    {
        rewardIdsList = rewards;
        this.duration = duration;
    }

    public bool ValidateData(DataService data)
    { 
        // TODO: Complete this!
        return true;
    }
}