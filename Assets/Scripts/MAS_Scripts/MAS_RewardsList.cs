using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This class represents the Master for the RewardsList domain. It handles
/// creating the Controller and Model workers for the domain.
///</summary>
public class MAS_RewardsList : MasterClass {

    private const string CLASSNAME = "MAS_RewardsList: ";

    ///<summary>
    /// Creates the controller and model workers for the domain
    ///</summary>
    public override void CreateWorkers()
    {
        Debug.Log(CLASSNAME + "Creating CON & MOD workers");
        AddController("CON_RewardsList", new CON_RewardsList(this));
        AddModel("MOD_RewardsList", new MOD_RewardsList(this));
    }

    /*
    public int NumOfMods()
    {
        Debug.Log("MASW: Getting number of mods from inside Child.");
        return this.Models.Count;
    }
    */
}
