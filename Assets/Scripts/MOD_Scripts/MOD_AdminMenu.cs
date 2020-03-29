using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOD_AdminMenu : AB_Model {

    private DataService dataService;
    private CON_AdminMenu controller;

    private const string LOCK_KEY = "lockOutPref";
    private const string ON_OFF_KEY = "lockOnOff";

    // Create the struct for proper serialization of the lock out prefs
    public struct LockOutPrefs
    {
        public int fromTime, toTime, fromSelection, toSelection;

        public LockOutPrefs(int fTime, int tTime, int fSelect, int tSelect)
        {
            fromTime = fTime;
            toTime = tTime;
            fromSelection = fSelect;
            toSelection = tSelect;
        }
    }

    public MOD_AdminMenu(MasterClass newMaster) : base(newMaster)
    {
        dataService = StartupScript.ds;

        if (!PlayerPrefs.HasKey(ON_OFF_KEY)) PlayerPrefs.SetInt(ON_OFF_KEY, 0);

        // Debug.Log("********************* \n IN MODEL ON OFF KEY IS " + PlayerPrefs.GetInt(ON_OFF_KEY).ToString() + "\n*************************");
    }

    public override void GetCoworkers(MasterClass master) { }

    public void SetLockOutPref(List<int> prefs)
    {
        // Debug.Log("********************* \n IN MODEL SETTING LOCK PREFS \n*************************");

        LockOutPrefs tempStruct = new LockOutPrefs(prefs[0], prefs[1], prefs[2], prefs[3]);

        PlayerPrefs.SetString(LOCK_KEY, JsonUtility.ToJson(tempStruct));
        PlayerPrefs.Save();

        // Debug.Log("********************* \n IN MODEL LOCK VALUE IS " + PlayerPrefs.GetString(LOCK_KEY) + "\n*************************");
    }
    public bool CheckLockOutPref() { return PlayerPrefs.HasKey(LOCK_KEY); }

    public void LockOutOnOff(bool lockOn)
    {
        PlayerPrefs.SetInt(ON_OFF_KEY , lockOn == true ? 1 : 0);
        PlayerPrefs.Save();
    }
    public bool CheckLockStatus() { return PlayerPrefs.GetInt(ON_OFF_KEY) == 1 ? true : false; }

    public List<int> GetLockOutPrefs()
    {
        if (PlayerPrefs.HasKey(LOCK_KEY))
        {
            List<int> tempList = new List<int>();
            LockOutPrefs prefs = JsonUtility.FromJson<LockOutPrefs>(PlayerPrefs.GetString(LOCK_KEY));

            // Add all the prefs data to tempList
            tempList.Add(prefs.fromTime);
            tempList.Add(prefs.toTime);
            tempList.Add(prefs.fromSelection);
            tempList.Add(prefs.toSelection);

            return tempList;
        }

        return null;
    }
}
