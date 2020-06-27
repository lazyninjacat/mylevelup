using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Slot : MonoBehaviour , IDropHandler
{
    private ScrambleHelper helper;
	private bool snap = true;

	public GameObject letter
	{
		get {
			// If letter has a child return it otherwise return null
			if (transform.childCount > 0) {
				return transform.GetChild(0).gameObject;
			} 

			return null;
		}
	}

	#region IdropHandler implementation

	///<summary>System event called when object being dragged is dropped on a slot</summary>
	///<param name="eventData">the data collected about the event by Unity</param>
	
	public void OnDrop(PointerEventData eventData)
	{
        Debug.Log("SLOT: Triggered OnDrop!");

        // Get the parent of the letter being dragged
        Transform parent = DragHandler.letterBeingDragged.transform.parent;

		// Get the letter contained in the letter being dragged
		char guessedLetter = char.ToLower(DragHandler.letterBeingDragged.name [7]);
		// Get the correct letter that should be on the current slot
		char correctLetter = char.ToLower(transform.name[0]);

        Debug.Log("SLOT: Guessed letter is " + guessedLetter);
        Debug.Log("SLOT: Correct letter is " + correctLetter);

        // If slot doesn't contain a letter accept the new letter, unless snapback is activated
        if (!letter)
        {
            Debug.Log("SLOT: Not letter!");

            if (snap == false || guessedLetter == correctLetter) {
				DragHandler.letterBeingDragged.transform.SetParent (transform);

				/* Calls on every game object above the one you have called it on until it finds 
				one that it can handle, pass it the current game object, null for data then use
				a lambda function to call HasChanged in letterCollection */
				ExecuteEvents.ExecuteHierarchy<IHasChanged> (gameObject, null, (x, y) => x.HasChanged ());
                Debug.Log("SLOT: Called HasChanged!");
            }
		// If slot contains a letter swap with the new letter, unless snapback is activated  
		}
        else
        {
			if (snap == false || char.ToLower (letter.name [7]) != correctLetter) {
				DragHandler.letterBeingDragged.transform.SetParent (transform);
				// Get the first child of the destination slot and set it back to the parent of the letter being dragged
				transform.GetChild (0).SetParent (parent);
				Debug.Log ("A letter was already on this slot");
				ExecuteEvents.ExecuteHierarchy<IHasChanged> (gameObject, null, (x, y) => x.HasChanged ());
                Debug.Log("SLOT: Called HasChanged!");
                helper.numErrors++;
                print("numErrors = " + helper.numErrors);
            }
		}
	}
	#endregion
}﻿
