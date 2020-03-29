using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragHandlerCounting : DragHandler {

    private GameObject countingCanvas;
    private CountingHelper countingScript;

    private void Start() {
        countingCanvas = GameObject.Find("Canvas_3");
        countingScript = countingCanvas.GetComponent<CountingHelper>();
    }

    public override void OnEndDrag(PointerEventData eventData) {
        countingScript.HandleItemDrop(gameObject);
    }

    public void ResetPos() {
        Debug.Log("Position Reset: " + name);
        transform.position = startPosition;
        Clean();
    }

    public void Clean() {
        letterBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
