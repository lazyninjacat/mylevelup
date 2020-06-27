using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAS_WordEditing : MasterClass
{ 
    public override void CreateWorkers()
    {
        AddController("CON_WordEditing", new CON_WordEditing(this));
        AddModel("MOD_WordEditing", new MOD_WordEditing(this));
    }
}
