using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAS_PlayList : MasterClass
{
    public override void CreateWorkers()
    {
        Debug.Log("MAS_PlayList: Instantiating mod and cons");
        //AddController("CON_PlayList", new CON_PlayList(this));
        AddModel("MOD_PlayList", new MOD_PlayList(this));
    }
}
