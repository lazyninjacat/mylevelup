using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Tracks what order the letters are in to see if the word has been solved
public class LetterCollection : MonoBehaviour, IHasChanged, IPostAnError {

    //private GameController gc;
    private ScrambleHelper helper;
    // Set the continue prompt to off by default
	//public bool ContinuePromptOn = false;

	[SerializeField] Transform slots;	// Reference to the solving slots
	//[SerializeField] GameObject continuePromptUI;

	void Start () {
        Debug.Log("Letter Collection starting!");
		//gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        helper = gameObject.GetComponent<ScrambleHelper>();
        HasChanged (); 
	}

    public void PostAnError()
    {
        helper.numErrors++;
        print("error posted. numerrors is now " + helper.numErrors);
    }

    #region IHasChanged implementation
    public void HasChanged (){
        Debug.Log("LC: HAS CHANGED");
        Debug.Log("LC: Scramble Helper says " + helper.GetType().ToString());

        // Tracks how many tiles have been moved for each word
        helper.numTileMoves++;
        print("numTileMoves = " + helper.numTileMoves);

		// String to store the order of the letters
		System.Text.StringBuilder builder = new System.Text.StringBuilder ();

		// Loop through each letter contained in the solving section
		foreach (Transform slotTransform in slots) {
			// Get the letter stored in the slot
			GameObject letter = slotTransform.GetComponent<Slot> ().letter;
			// If the slot contains a letter, append it to the string
			if (letter) {
				// Extract the letter from the full letter string Ex. Letter_A(Clone) position 7 = a
				char letterName = char.ToLower(letter.name [7]);	
				// Append the letter to the builder
				builder.Append (letterName);
			}
		}

		// Finalize the string
		string wordProgress = builder.ToString ();

        Debug.Log("LC: Word progress is " + wordProgress);

        // Detect if the word has been solved
        if (helper.CheckForMatch(wordProgress)) {
            Debug.Log("LC: Found a match!");
            ExecuteEvents.Execute<IWordSolved>(helper.gameObject, null, (x, y) => x.WordSolved());
            //Debug.Log("Word has been solved!");
        }
	}
    #endregion
    /*
	///<summary> Handles quitting the game if chosen in the UI popup</summary>

	public void QuitPrompt() {
		continuePromptUI.SetActive (false);
		Time.timeScale = 1f;
		//ContinuePromptOn = false;
        ExecuteEvents.Execute<IWordSolved>(helper.gameObject, null, (x, y) => x.WordSolved());
		//
	}

	///<summary> Handles displaying the Quit prompt </summary>
	
        
	public void DisplayPrompt(){
		continuePromptUI.SetActive (true);
		Time.timeScale = 0;
		//ContinuePromptOn = true;
	}
    */
}

// Using UnityEngine.EventSystems namespace is optional, just keeps interfaces in the same place
namespace UnityEngine.EventSystems {
	// Declare a IHasChanged interface
	public interface IHasChanged : IEventSystemHandler {
		void HasChanged ();
	}

    public interface IPostAnError : IEventSystemHandler
    {
        void PostAnError();
    }
}
