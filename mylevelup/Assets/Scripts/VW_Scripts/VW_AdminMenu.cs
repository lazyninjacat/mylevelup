using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VW_AdminMenu : MonoBehaviour {
        
    private bool animIsPlaying;
    private CON_AdminMenu controller;
    private GameObject butt;

    private List<GameObject> tutorialObjects;

    [SerializeField] GameObject tutorialPanel1;
    [SerializeField] GameObject tutorialButtonIconOn;
    [SerializeField] GameObject tutorialButtonIconOff;


    void Start ()
    {
        tutorialObjects = GameObject.FindGameObjectsWithTag("tutorial").ToList();

        foreach (GameObject obj in tutorialObjects)
        {
            obj.SetActive(false);
        }

        if (PlayerPrefs.GetInt("isTutorial") == 1)
        {
            tutorialPanel1.SetActive(true);
            tutorialButtonIconOff.SetActive(true);
            tutorialButtonIconOn.SetActive(false);
        }
        else
        {
            tutorialButtonIconOff.SetActive(false);
            tutorialButtonIconOn.SetActive(true);
        }       

        MAS_Admin tempMaster = (MAS_Admin)COM_Director.GetMaster("MAS_Admin");
        controller = (CON_AdminMenu)tempMaster.GetController("CON_AdminMenu");
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
            tutorialButtonIconOff.SetActive(true);
            tutorialButtonIconOn.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetInt("isTutorial", 0);
            foreach (GameObject obj in tutorialObjects)
            {
                obj.SetActive(false);
            }
            tutorialButtonIconOff.SetActive(false);
            tutorialButtonIconOn.SetActive(true);
        }
    }


    public void ChangeScene(string scene)
    {
        GetComponent<Animation>().Play();
        GameObject butt = gameObject;
        StartCoroutine(WaitForAnimation(scene, butt));
    }


    private IEnumerator WaitForAnimation(string sceneRequest, GameObject button)
    {
        animIsPlaying = button.GetComponent<Animation>().isPlaying;
        yield return new WaitUntil(() => (animIsPlaying = false));               
        controller.SceneChange(sceneRequest);                     
    }
}
