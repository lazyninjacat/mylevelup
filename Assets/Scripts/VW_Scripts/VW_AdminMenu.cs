using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VW_AdminMenu : MonoBehaviour {
        
    private bool animIsPlaying;
    private CON_AdminMenu controller;
    private GameObject butt;

    [SerializeField] GameObject gettingStartedPanel;


    void Start ()
    {
        if (PlayerPrefs.GetInt("isTutorial") == 1)
        {
            gettingStartedPanel.SetActive(true);
        }

        MAS_Admin tempMaster = (MAS_Admin)COM_Director.GetMaster("MAS_Admin");
        controller = (CON_AdminMenu)tempMaster.GetController("CON_AdminMenu");
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
