using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class TutorialHelper : MonoBehaviour
{
    [SerializeField] GameObject tutorialPanel1;
    [SerializeField] GameObject helpButtonIconOn;
    [SerializeField] GameObject helpButtonIconOff;

    List<GameObject> tutorialObjects;

    // Start is called before the first frame update
    void Start()
    {
        tutorialObjects = GameObject.FindGameObjectsWithTag("tutorial").ToList();
        foreach (GameObject obj in tutorialObjects)
        {
            obj.SetActive(false);
        }

        if (PlayerPrefs.GetInt("isTutorial") == 1)
        {
            tutorialPanel1.SetActive(true);
            helpButtonIconOff.SetActive(true);
            helpButtonIconOn.SetActive(false);
        }
        else
        {
            helpButtonIconOff.SetActive(false);
            helpButtonIconOn.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("isTutorial") == 0)
        {
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    public void ToggleTutorialButton()
    {
        if (PlayerPrefs.GetInt("isTutorial") == 0)
        {
            PlayerPrefs.SetInt("isTutorial", 1);
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
            tutorialPanel1.SetActive(true);
            helpButtonIconOn.SetActive(false);
            helpButtonIconOff.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("isTutorial", 0);
            tutorialPanel1.SetActive(false);

            helpButtonIconOn.SetActive(true);
            helpButtonIconOff.SetActive(false);
        }
    }

}
