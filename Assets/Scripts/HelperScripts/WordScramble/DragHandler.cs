using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static GameObject letterBeingDragged;
	protected Vector3 startPosition;
	protected Transform startParent;

    private AudioSource audioSource;
    private Animation animation;

    private VW_GameLoop gameloop;


    #region IBeginDragHandler implementation

    ///<summary>System event called when dragging an object begins</summary>
    ///<param name="eventData">the data collected about the event by Unity</param>
    ///

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animation = GetComponent<Animation>();

    }




    public void OnBeginDrag (PointerEventData eventData)
	{
       
        //Set pointer to object being dragged
        letterBeingDragged = gameObject;

		//Set startPosition to letterBeingDragged's current position
		startPosition = transform.position;

		//Set startParent to the slot the word starts in
		startParent = transform.parent;

		//Disable raycasting to the letter
		GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<Animation>().Play("ButtonWiggleAnim");

        
    }
	#endregion

	#region IDragHandler implementation

	///<summary>System event called when dragging an object</summary>
	///<param name="eventData">the data collected about the event by Unity</param>

	public void OnDrag (PointerEventData eventData)
	{
		//Continuously sets the position of the letter to the position of the pointer(mouse/finger)
		transform.position = Input.mousePosition;
	}

	#endregion

	#region IEndDragHandler implementation

	///<summary>System event called when dragging an object ends</summary>
	///<param name="eventData">the data collected about the event by Unity</param>
	public virtual void OnEndDrag (PointerEventData eventData)
	{
        string letter;



        letter = letterBeingDragged.name.Replace("_", " ").Replace("(Clone)", "");

        Debug.Log("Letter dropped is " + letter);


        //Unload letterBeingDragged
        letterBeingDragged = null;
		//Re-enables raycasting to the letter
		GetComponent<CanvasGroup>().blocksRaycasts = true;

		//Check if this object's parent is the starting slot _or_ the canvas
		//if (transform.parent == startParent || transform.parent == transform.root) {
		if (transform.parent == startParent) {
			//Sets the object's position back to the starting slot
			transform.position = startPosition;
            //Sets the object's parent back to the starting slot
            //transform.SetParent (startParent);
            ExecuteEvents.ExecuteHierarchy<IPostAnError>(gameObject, null, (x, y) => x.PostAnError());
            print("posting an error");

        }

        else
        {
            //Play soundclip

            //audioSource.Play();




            AudioClip tempClip = FileAccessUtil.LoadWordAudio(letter + ".wav");

            if (tempClip != null)
            {
                audioSource.clip = tempClip;
                audioSource.Play();
            }
            else
            {
                tempClip = Resources.Load<AudioClip>("Sound/" + letter);


                if (tempClip != null)
                {
                    audioSource.clip = tempClip;
                    audioSource.Play();
                }
                else
                {
                    //Do nothing
                }

                audioSource.clip = tempClip;
                audioSource.Play();
            }


            //Play animation
            animation.Play();
        }

    }

	#endregion

}
