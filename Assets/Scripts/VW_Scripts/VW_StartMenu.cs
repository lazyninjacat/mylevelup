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

    public Text childnameText;

    private CON_AdminMenu controller;

    private IEnumerator timerCoroutine;
    private bool isLockOn;

    private DateTime current;
    private DateTime lockOutFrom;
    private DateTime lockOutTo;


    // Use this for initialization
    void Start () {

        // Preload the Masters if they are not loaded
        COM_Director.InitData();

        MAS_Admin tempMaster = (MAS_Admin)COM_Director.GetMaster("MAS_Admin");
        controller = (CON_AdminMenu)tempMaster.GetController("CON_AdminMenu");

        isLockOn = (PlayerPrefs.GetInt("lockOnOff") == 1);

        // Debug.Log("********************* \n isLockOn is " + isLockOn.ToString() + "\n*************************");


        // Create a current time DateTime object
        DateTime current = DateTime.Now;

        // Create a DateTime object for the FROM and TO lock out times
        DateTime lockOutFrom = new DateTime(current.Year, current.Month, current.Day, (PlayerPrefs.GetInt("LockoutFromTimeInt")), 0, 0);
        DateTime lockOutTo = new DateTime(current.Year, current.Month, current.Day, (PlayerPrefs.GetInt("LockoutToTimeInt")), 0, 0);
        timeText.text = (PlayerPrefs.GetString("LockoutToTimeString"));
        DateTime lockOutToOvernight = new DateTime(current.Year, current.Month, current.Day, ((PlayerPrefs.GetInt("LockoutToTimeInt")) + 1), 0, 0);


        //Debug.Log("current = " + current);
        //Debug.Log("lockOutFrom = " + lockOutFrom);
        //Debug.Log("lockOutTo = " + lockOutTo);



       
        //Display ChildName Key saved in PlayerPref in the ChildNameText game object in scene, or display "My" if the childnamekey is blank or null
        if ((PlayerPrefs.GetString("ChildNameKey") == "") || (PlayerPrefs.GetString("ChildNameKey") == null))
        {
            // Debug.Log("ChildName not set");
            childnameText.text = "My";
        }
        else
        {
            // Debug.Log("ChildName is set to: " + PlayerPrefs.GetString("ChildNameKey"));
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
        //Debug.Log("********************* \n SCENE NAME IS " + scene + "\n*************************");

        noAccessPrompt.SetActive(false);
        startButton.interactable = true;

        // Stop the coroutine if it is running
        // if (timerCoroutine != null) { StopCoroutine(timerCoroutine); }

        // Debug.Log("***********************\n Passed coroutine check \n**************************");

        //controller.SceneChange(scene);
    }

    private IEnumerator StartLockTimer(double seconds)
    {
        //Debug.Log("********************* \n TIMER COROUTINE STARTED!\n*************************");
        //Debug.Log("second = " + seconds);
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



}
