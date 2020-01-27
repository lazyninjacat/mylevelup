using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preloader : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Debug.Log("In Preloader Start!");
        if(!COM_Director.CheckIfInitDone()) COM_Director.InitData();
    }
}