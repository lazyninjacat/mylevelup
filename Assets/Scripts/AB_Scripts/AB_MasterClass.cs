using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This abstract class represents the Master in the MVC design pattern.
/// It is responsible for handling managing the models and controllers (view management not yet enabled).
///</summary>
///<remarks>
/// Functionality such as Creation and Deletion of workers, Adding MVC components, accessing them, removing
/// them, and getting a status check on whether or not the master has generated any MVC components.
///</remarks>
public abstract class MasterClass {

    private Dictionary<string, AB_Model> models;
    private Dictionary<string, AB_Controller> controllers;
    public bool WorkersAlive { get; set; }

    public MasterClass()
    {
        Debug.Log("MAS: creating new master");
        controllers = new Dictionary<string, AB_Controller>();
        models = new Dictionary<string, AB_Model>();
        TagForKeepAlive();
    }

    ///<summary>
    /// Adds a model and a key representing the class name to the <paramref name="models"/> dictionary.
    ///</summary>
    ///<param name="key"> A string representing the class name of the model </param>
    ///<param name="value"> An AB_Model object. </param>
    public void AddModel(string key, AB_Model value) { models.Add(key, value); }

    ///<summary>
    /// Adds a controller and a key representing the class name to the <paramref name="controllers"/> dictionary.
    ///</summary>
    ///<param name="key"> A string representing the class name of the controller </param>
    ///<param name="value"> An AB_Controller object. </param>
    public void AddController(string key, AB_Controller value) { controllers.Add(key, value); }

    //<summary>
    // Adds a view and a key representing the class name to the <paramref name="views"> dictionary.
    //</summary>
    //<param name="key"> A string representing the class name of the view </param>
    //<param name="value"> An AB_View object. </param>
    //public void AddView(string key, object value) { views.Add(key, value); }

    ///<summary>
    /// Returns the model in the dictionary based on the key passed.
    ///</summary>
    ///<param name="key"> A string representing the key of the model you want to retrieve.</param>
    ///<returns> The model object belonging to the key passed from the models dictionary.</returns>
    public object GetModel(string key) {
        Debug.Log("MASW: In GetModel trying to fetch model " + key);
        int test = models.Count;
        Debug.Log("MASW: number of models in models is: " + test);
        test = controllers.Count;
        Debug.Log("MASW: number of controllers in controllers is: " + test);

        return models[key]; }

    ///<summary>
    /// Returns the controller in the dictionary based on the key passed.
    ///</summary>
    ///<param name="key"> A string representing the key of the controller you want to retrieve.</param>
    ///<returns> The controller object belonging to the key passed from the controllers dictionary.</returns>
    public object GetController(string key) { return controllers[key]; }

    //<summary>
    //Returns the view in the dictionary based on the key passed.
    //</summary>
    //<param name="key"> A string representing the key of the view you want to retrieve.</param>
    //<returns> The view object belonging to the key passed from the views dictionary.</returns>
    //public object GetView(string key) { return views[key]; }

    ///<summary>
    /// Removes all entries in all the dictionaries by calling .Clear() and nulling the collections.
    ///</summary>
    public void KillWorkers()
    {
        Debug.Log("MAS: KILLING WORKERS!");
        models.Clear();
        controllers.Clear();
        WorkersAlive = false;
        Debug.Log("MAS: WORKERS ARE DEAD COLLECTIONS ARE NULL!");
    }

    ///<summary>
    /// Calls to the Master to create the models and controllers, then calls on the Model and Controllers to create references to each other.
    ///</summary>
    ///<remarks>
    /// Calls the abstract CreateWorkers() function provided by a Master implementation, then for each model and controller it
    /// calls the AB_Model.GetCoworkers(MasterClass master) abstract method provided by a Model implementation.
    ///</remarks>
    public void CreateAndAssignWorkers()
    {
        Debug.Log("MAS: In create and assign workers!");

        // Create the workers and then assign them to their coworkers
        CreateWorkers();

        // Switch the workers alive bool to on
        WorkersAlive = true;

        foreach (AB_Model mod in models.Values)
        {
            mod.GetCoworkers(this);
        }

        foreach (AB_Controller con in controllers.Values)
        {
            con.GetCoworkers(this);
        }
    }

    /// <summary>
    /// Tags the model and controller collections for protection from 
    /// garbage collection.
    /// </summary>
    private void TagForKeepAlive()
    {
        System.GC.KeepAlive(models);
        System.GC.KeepAlive(controllers);
    }

    ///<summary>
    /// Sends a request to the director to change the scene.
    ///</summary>
    ///<param name="scene"> A string representing the name of the scene you are requesting to change to.</param>
    public void RequestSceneChange(string scene) { COM_Director.LoadSceneByName(scene); }

    // Domain specific abstract classes 
    public abstract void CreateWorkers();

    // Temp Test Methods
    public bool ModelsAlive() { return models.Count > 0; }
    public bool ControllersAlive() { return controllers.Count > 0; }
    //
    
}