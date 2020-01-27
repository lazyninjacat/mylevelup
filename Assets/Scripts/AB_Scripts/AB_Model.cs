using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This abstract class represents the Model in the MVC design pattern.
/// It is designed to interact with the database, and get references to the other parts of the MVC from the Master. 
///</summary>
///<remarks>
/// The inheriting class must provide implementation of GetCoworkers(MasterClass master).
///</remarks>
public abstract class AB_Model {
    private DataService dataService; // should this be here?
    public AB_Model(MasterClass newMaster) { }
    public abstract void GetCoworkers(MasterClass master);
}
