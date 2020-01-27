using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonWiggle : MonoBehaviour
{


    public void PlayButtonWiggleAnimation()
    {
        GetComponent<Animation>().Play("ButtonWiggleAnim");
    }


}
