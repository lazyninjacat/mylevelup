using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyPressHandler : MonoBehaviour, IPointerClickHandler
{
    #region IBeginKeyPressHandler implementation

    ///<summary>System event called when pressing a letter begins</summary>
    ///<param name="eventData">the data collected about the event by Unity</param>

    // Broadcast the key that was pressed to the KeyCollection script
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        string key = name;
        Debug.Log(name + " Game Object Clicked!");
        Messenger<string>.Broadcast("KeyboardPressed", name);     
    }
    #endregion
    
}
