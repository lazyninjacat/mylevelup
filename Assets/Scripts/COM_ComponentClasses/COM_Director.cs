using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

///<summary>
/// This class handles managing the domains, transitions between them, and directing a Master on when to create its MVC objects or kill them. 
///</summary>
public static class COM_Director {

    private static Dictionary<string, string> sceneDomains;
    private static Dictionary<string, object> masterPool;
    private static string currentDomain;
    private static bool initialized = false;
    private const string START_SCENE = "startup_menu";
    private const string NO_DOMAIN = "nd";
    public static string CurrentDomain {get {return currentDomain;}}

    ///<summary>
    /// This function handles managing which Domain Master should be running and whether a Domain transition should occur
    ///</summary>
    ///<param name="sceneName"> The name of the scene the Master has requested to change to</param>
    public static void LoadSceneByName(string sceneName)
    {
        string nextDomain = "";

        // Check if we are trastioning to the start menu and if so kill all previously created workers and call garbage collection
        if (sceneName == START_SCENE)
        {
            KillAndClear();
        }        

        // Check to see if the sceneName has been added to the list of sceneDomains
        if (sceneDomains.ContainsKey(sceneName))
        {
            nextDomain = sceneDomains[sceneName];
        }
        else
        {
            nextDomain = NO_DOMAIN;
        }      

        // Check to see if a domain transition is needed If so, kill the MVC workers of the domain that owns the scene being changed to.        
        if (IsDomainTransition(nextDomain))
        {
            CreateWorkers(nextDomain);
        }

        //Set the current domain of the scene to load
        currentDomain = nextDomain;

        SceneManager.LoadScene(sceneName);
    }

    ///<summary>
    /// Get the requested Master from the MasterPool
    ///</summary>
    ///<param name="key"> The name of the Master </param>
    ///<returns>Returns a MasterClass object </returns>
    public static object GetMaster(string key) { return masterPool[key]; }

    public static void AddToPool(string key, object master) { masterPool[key] = master; }

    public static bool IsEmpty() { return (masterPool == null || masterPool.Count == 0); }

    ///<summary>
    /// Checks whether a domain transition is occuring.
    ///</summary>
    ///<param name="domainName">The domain name to check against the current domain</param>
    ///<returns>Returns a boolean indicating whether a domain transition is occuring</returns> 
    private static bool IsDomainTransition(string domainName) { return domainName != currentDomain; }

    ///<summary>
    /// Initializes all the Domains and their associated scenes
    ///</summary>
    public static void InitData()
    {
        
        initialized = true;
        //TODO: Add the master and scenes from the config file

        // Set current domain and last scene to default
        currentDomain = START_SCENE;
        
        // Create new collections for the master pool and scene domains
        masterPool = new Dictionary<string, object>();

        sceneDomains = new Dictionary<string, string>();

        // Instanstiate and add all masters to the pool
        masterPool.Add("MAS_WordEditing", new MAS_WordEditing());

        masterPool.Add("MAS_PlayList", new MAS_PlayList());

        masterPool.Add("MAS_GameLoop", new MAS_GameLoop());

        masterPool.Add("MAS_RewardsList", new MAS_RewardsList());

        masterPool.Add("MAS_Admin", new MAS_Admin());



        // TODO: Tie this to a config or something.

        // Add the Admin domain master and the scenes it is attached to
        string value = "MAS_Admin";

        sceneDomains.Add("admin_menu", value);

        // Create the workers for the Admin Master
        CreateWorkers("MAS_Admin");

        // Add the Settings domain master and the scenes it is attached to
        value = "MAS_Settings";

        sceneDomains.Add("settings", value);

        sceneDomains.Add(START_SCENE, value);

        // Create the workers for the Settings Master
        CreateWorkers("MAS_Settings");

        // Add the WordEditing domain master and the scenes it is attached to
        value = "MAS_WordEditing";

        sceneDomains.Add("words_list", value);

        sceneDomains.Add("word_edit_add", value);

        sceneDomains.Add("image_camera", value);

        sceneDomains.Add("record_audio", value);


        // Add the PlayList domain master and the scenes it is attached to
        value = "MAS_PlayList";

        sceneDomains.Add("play_list", value);

        sceneDomains.Add("word_scramble", value);

        sceneDomains.Add("browser_reward", value);

        sceneDomains.Add("counting_game", value);

        sceneDomains.Add("keyboard_game", value);

        sceneDomains.Add("memory_cards", value);

        sceneDomains.Add("matching_game", value);

        // Add RewardsList domain master and the scene it is attached to
        value = "MAS_RewardsList";

        sceneDomains.Add("rewards_list", value);

        sceneDomains.Add("reward_edit_add", value);

        sceneDomains.Add("reward_camera", value);

        // Add Game Loop domain master and the scene it is attached to
        value = "MAS_GameLoop";

        sceneDomains.Add("game_loop", value);

        TagAsKeepAlive();
    }

    /// <summary>
    /// Kills the workers belonging to each master in the list and calls garbage collection to free heap memory.
    /// </summary>
    private static void KillAndClear()
    {
        MasterClass master;

        foreach (var pair in masterPool)
        {
            if (pair.Key != "MAS_AdminMenu")
            {
                master = (MasterClass)pair.Value;

                master.KillWorkers();
            }
        }

        System.GC.Collect();
    }

    /// <summary>
    /// Returns the initialized bool value.
    /// </summary>
    /// <returns>bool</returns>
    public static bool CheckIfInitDone() { return initialized; }

    ///<summary>
    /// Creates the model(s) and controller(s) workers of the next provided Domain.
    ///</summary>
    ///<param name="nextDomain">The name of the next domain.</param>
    private static void CreateWorkers(string nextDomain)
    {
        //If the domain is in the masterpool, attempt to kill its workers
        if (masterPool.ContainsKey(nextDomain))
        {
            MasterClass next = (MasterClass)masterPool[nextDomain];
            
            if (!next.WorkersAlive)
            {
                next.CreateAndAssignWorkers();
            }
        }
    }

    /// <summary>
    /// Tags the two dictionaries that are used by the Director.
    /// </summary>
    private static void TagAsKeepAlive()
    {
        System.GC.KeepAlive(sceneDomains);

        System.GC.KeepAlive(masterPool);
    }
}
