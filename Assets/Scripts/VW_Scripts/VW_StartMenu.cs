using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;

public class VW_StartMenu : MonoBehaviour {
      
    [SerializeField] GameObject noAccessPrompt;
    [SerializeField] Button startButton;
    [SerializeField] Text timeText;
    [SerializeField] Text childnameText;
    private CON_AdminMenu adminController;
    private IEnumerator timerCoroutine;
    private bool isLockOn;
    private DateTime current;
    private DateTime lockOutFrom;
    private DateTime lockOutTo;
    private int checkIfNewInstall;

    private DataService dataService;


    void Start () {

        // Preload the Masters if they are not loaded
        COM_Director.InitData();

        MAS_Admin tempMasterAdmin = (MAS_Admin)COM_Director.GetMaster("MAS_Admin");

        adminController = (CON_AdminMenu)tempMasterAdmin.GetController("CON_AdminMenu");

        isLockOn = (PlayerPrefs.GetInt("lockOnOff") == 1);

        // Create a current time DateTime object
        DateTime current = DateTime.Now;

        // Create a DateTime object for the FROM and TO lock out times
        DateTime lockOutFrom = new DateTime(current.Year, current.Month, current.Day, (PlayerPrefs.GetInt("LockoutFromTimeInt")), 0, 0);

        DateTime lockOutTo = new DateTime(current.Year, current.Month, current.Day, (PlayerPrefs.GetInt("LockoutToTimeInt")), 0, 0);

        timeText.text = (PlayerPrefs.GetString("LockoutToTimeString"));

        DateTime lockOutToOvernight = new DateTime(current.Year, current.Month, current.Day, ((PlayerPrefs.GetInt("LockoutToTimeInt")) + 1), 0, 0);

        dataService = StartupScript.ds;


        //Display ChildName Key saved in PlayerPref in the ChildNameText game object in scene, or display "My" if the childnamekey is blank or null
        if ((PlayerPrefs.GetString("ChildNameKey") == "") || (PlayerPrefs.GetString("ChildNameKey") == null))
        {
            childnameText.text = "My";
        }
        else
        {
            childnameText.text = PlayerPrefs.GetString("ChildNameKey") + "'s";
        }                
   
        if (!RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }

        // Check if lock out prefs exist
        if (isLockOn)
        {
            if (lockOutFrom < lockOutTo)
            {
                if ((current > lockOutFrom) && (current < lockOutTo))
                {
                    noAccessPrompt.SetActive(true);
                }

            }
            else if (lockOutTo < lockOutFrom)
            {
                if ((current > lockOutFrom) || (current < (lockOutTo)))
                {
                    noAccessPrompt.SetActive(true);
                }
            }
        }

        CheckIfFirstOpenAfterInstall();

    }

    private void Update()
    {
        // Check if lock out prefs exist
        if (isLockOn)
        {
            if (lockOutFrom < lockOutTo)
            {
                if (!((current > lockOutFrom) && (current < lockOutTo)))
                {
                    noAccessPrompt.SetActive(false);
                }

            }
            else if (lockOutTo < lockOutFrom)
            {
                if (!((current > lockOutFrom) || (current < (lockOutTo))))
                {
                    noAccessPrompt.SetActive(false);
                }
            }

        }
    }

    public void ChangeScene(/*string scene*/)
    {
        noAccessPrompt.SetActive(false);

        startButton.interactable = true;

        // Stop the coroutine if it is running

        // if (timerCoroutine != null) { StopCoroutine(timerCoroutine); }

        //controller.SceneChange(scene);
    }

    private IEnumerator StartLockTimer(double seconds)
    {
        PromptButtonSwitch();

        while (seconds > 0)
        {
            yield return new WaitForSeconds(1);

            seconds -= 1;
        }
    }

    private void PromptButtonSwitch()
    {
        // Switch the noAccessPrompt
        noAccessPrompt.SetActive(!noAccessPrompt.activeSelf);

        // Switch the start button interactable
        startButton.interactable = !startButton.IsInteractable();
    }

    // Create a bool to keep track of whether or not this is the first open after install.
    private void CheckIfFirstOpenAfterInstall()
    {
        checkIfNewInstall = PlayerPrefs.GetInt("isFirstKey");
        Debug.Log("isFirst = " + checkIfNewInstall);

        if (checkIfNewInstall == 0)
        {
            dataService.DeleteAllPlaylist();
            PlayerPrefs.SetInt("isFirstKey", 1);
            checkIfNewInstall = PlayerPrefs.GetInt("isFirstKey");
            Debug.Log("First open after install detected. isFirstKey is now set to : " + PlayerPrefs.GetInt("isFirstKey"));
        }
        else
        {
            Debug.Log("Not first open after install.");
        }

    }


    // Check if this is the first open after install


    // If it is the first open, then reset the playlist to blank

}
