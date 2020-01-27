﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class DO_Reward : IValidateData
{
    // RewardsList DO members
    public int id;// {get; set;}
    public string reward_name;
    public string reward_type;
    public string reward_url;
    public DO_Reward(int rid, string rn, string rt, string ru)
    {
        id = rid;
        reward_name = rn;
        reward_type = rt;
        reward_url = ru;
    }
    public bool ValidateData(DataService data)
    {
        // TODO: Complete this!
        return true;
    }
}