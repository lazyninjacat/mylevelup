using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

///<summary>Controls the flow of all scenes within the game</summary>
public class SceneController : MonoBehaviour {



    private bool animIsPlaying;
    private GameObject butt;


    public void LoadSceneByName(string sceneName)
    {
        GameObject butt = gameObject;
        butt.GetComponent<Animation>().Play("ButtonWiggleAnim");
        StartCoroutine(WaitForAnimation(sceneName, butt));

    }


    private IEnumerator WaitForAnimation(string scene, GameObject button)
    {
        //animIsPlaying = button.GetComponent<Animation>().isPlaying;
        //yield return new WaitUntil(() => (animIsPlaying = false));
        yield return new WaitForSeconds(0.3f);
        COM_Director.LoadSceneByName(scene);

    }


}