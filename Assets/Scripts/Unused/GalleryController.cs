// TODO masrk as possible obsolete script

using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Experimental.UIElements;
using System.Collections.Generic;

public class GalleryController : MonoBehaviour {

    public UnityEngine.UI.Button applyButton;
    public UnityEngine.UI.Button refreshButton;
    public UnityEngine.UI.Button cancelButton;
    //public ScrollView fileView;
    public InputField nameInput;

	// Use this for initialization
	void Start () {
        /*
         1. Disable apply button
         2. Get file list
         3. populate scroll view content list
         */
        Debug.Log("In start!");
        applyButton.interactable = false;
        Debug.Log("Entering File accessUtil!");
        List<string> imagePaths = FileAccessUtil.GetAndroidGalleryPaths();

        if (imagePaths == null || imagePaths.Count < 0)
        {
            nameInput.text = "File access did not work!";
        }
        else
        {
            nameInput.text = string.Format("Path is {0}.", imagePaths[0]);
            //int x = 0;
            //foreach (string path in imagePaths)
            //{
            //    x += 1;
            //    nameInput.text = string.Format("The {0}th path is {1}.", x.ToString(), path);
            //}
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
