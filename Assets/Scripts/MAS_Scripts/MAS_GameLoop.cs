using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The master for the GameLoop
/// </summary>
public class MAS_GameLoop : MasterClass
{
    public override void CreateWorkers()
    {
        AddController("CON_GameLoop", new CON_GameLoop(this));
        AddModel("MOD_GameLoop", new MOD_GameLoop(this));
    }
}