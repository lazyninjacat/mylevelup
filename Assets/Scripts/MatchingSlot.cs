using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MatchingSlot : MonoBehaviour , IDropHandler
{




	#region IdropHandler implementation

	///<summary>System event called when object being dragged is dropped on a slot</summary>
	///<param name="eventData">the data collected about the event by Unity</param>
	
	public void OnDrop(PointerEventData eventData)
	{
        Debug.Log("SLOT: Triggered OnDrop!");

        // Get the parent of the letter being dragged
        Transform parent = MatchingDragHandler.ImageBeingDragged.transform.parent;

		// Get the name of the image being dragged
		string imageName = MatchingDragHandler.ImageBeingDragged.name;
		// Get the correct letter that should be on the current slot
		string textName = transform.name;




        if (imageName != textName)
        {
          
            Debug.Log("Nope");

        }
        else
        {
            Debug.Log("COrrect!!!!1");
            MatchingDragHandler.ImageBeingDragged.transform.SetParent(transform);
            // Get the first child of the destination slot and set it back to the parent of the image being dragged
            //transform.GetChild(0).SetParent(parent);
        }

    }
	#endregion
}﻿
