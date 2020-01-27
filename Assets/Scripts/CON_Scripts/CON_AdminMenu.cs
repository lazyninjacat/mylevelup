using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CON_AdminMenu : AB_Controller {

    private MAS_Admin master;
    private MOD_AdminMenu model;

    public CON_AdminMenu(MasterClass newMaster) : base(newMaster) { master = (MAS_Admin)newMaster; }

    public override void ClearData()
    {
        throw new System.NotImplementedException();
    }

    public override void GetCoworkers(MasterClass newMaster) { model = (MOD_AdminMenu)master.GetModel("MOD_AdminMenu"); }
    public override void SceneChange(string scene) { master.RequestSceneChange(scene); }
    //public bool LockOutPrefsExist() { return model.CheckLockOutPref(); }
    //public bool IsLockOutOn() { return model.CheckLockStatus(); }
    //public void SetLockOnOff(bool onOff) { model.LockOutOnOff(onOff); }
    //public List<int> GetLockOutPrefs() { return model.GetLockOutPrefs(); }

    //public void SaveLockOutPrefs(int fromTime, int toTime, int fromSelection, int toSelection)
    //{
    //    Debug.Log("********************* \n IN CON SAVING LOCK OUT PREFS\n*************************");

    //    List<int> tempList = new List<int>();

    //    Debug.Log("********************* \n FROMTIME IS " + fromTime.ToString() + "\n*************************");
    //    Debug.Log("********************* \n TOTIME IS " + toTime.ToString() + "\n*************************");
    //    Debug.Log("********************* \n SELECTION FROM IS " + fromSelection.ToString() + "\n*************************");
    //    Debug.Log("********************* \n SELECTION TO IS " + toSelection.ToString() + "\n*************************");

    //    tempList.Add(fromTime);
    //    tempList.Add(toTime);
    //    tempList.Add(fromSelection);
    //    tempList.Add(toSelection);

    //    model.SetLockOutPref(tempList);
    //}
}
