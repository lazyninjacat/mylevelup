using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This abstract class represents the Controller in the MVC design pattern.
/// It is designed to clear data from the scene, request a scene change from the master, and get
/// references to the other parts of the MVC from the Master.
///</summary>
///<remarks>
/// The inheriting class must provide implementation of ClearData(), SceneChange(string scene), and GetCoworkers(MasterClass master).
///</remarks>
public abstract class AB_Controller {
    public AB_Controller (MasterClass master) { }
    public abstract void ClearData();
    public abstract void SceneChange(string scene);
    public abstract void GetCoworkers(MasterClass newMaster);
}