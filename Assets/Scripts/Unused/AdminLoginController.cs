using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminLoginController : MonoBehaviour {

    private const string PIN_PREF_KEY = "pinCode";

    // Reference to DataService script
    public DataService ds;
    
	private InputField passwordInput;
	private Text passwordValue;

    [SerializeField] Button loginButton;
	[SerializeField] Button createNewButton;
	[SerializeField] GameObject invalidPopup;
    [SerializeField] InputField inputPinField;
    [SerializeField] InputField usernameInput;
    [SerializeField] Text usernameValue;
    [SerializeField] GameObject noPinModal;

    // Use this for initialization
    void Start () {
        Debug.Log("**************************************\nADMIN: in START \n ***********************************");
		ds = StartupScript.ds;
    }

    public void PinModalClose() { noPinModal.SetActive(false); }

    //################## LOADING DATA FUNCTIONS ##################//

    ///<summary> Calls the DB, retrieves the Admins table, and sets the values of all the UI elements </summary>
    public void LoginAttempt() {

        //TODO: Alter this function to salt and hash

		string hashedPw = "";
		//Hash the pw
		try{
			hashedPw = ds.Md5Sum(passwordInput.text);
		}
		catch{
			Debug.Log("Hash failed");
		}
		//check hashed password with one in the DB
		bool matches = ds.CheckPassword(usernameInput.text, hashedPw);
		//if matches, call to open admin_menu
		if(matches){
			COM_Director.LoadSceneByName("admin_menu");
		}
		else{invalidPopup.SetActive(true);}
	}

    // TODO: Replace with more secure pin code
    public void PinLogin()
    {
        /*
        //int truePin = ds.GetUserPin(usernameInput.text);

        if (truePin > -1)
        {
            if (truePin == Int16.Parse(inputPinField.text))
            {
                COM_Director.activeAdminPin = truePin;
                COM_Director.LoadSceneByName("admin_menu");
            }
            else
            {
                invalidPopup.SetActive(true);
            }
        }
        else
        {
            // TODO: TOSS ERROR
        }
        */

        int pin = short.Parse(inputPinField.text);

        if (ds.DoesPinExist(pin) == 1)
        {
            COM_Director.LoadSceneByName("admin_menu");
        }
        //else if ()
        //{
        //    noPinModal.SetActive(true);
        //}
        else
        {
            invalidPopup.SetActive(true);
        }
    }

	//################## INSERTING DATA FUNCTIONS ##################//

	///<summary> Handles closing the invalid login popup </summary>
	public void ClosePopup(){
		//usernameInput.text = "";
		//passwordInput.text = "";
		loginButton.interactable = false;
		invalidPopup.SetActive(false);
	}

	// Update is called once per frame
	void Update () {

        //Makes login button not interactable if either of the input fields is empty and the button is interactable
        //if (((usernameValue.text.Equals("")) || (passwordValue.text.Equals(""))) && loginButton.interactable) {
        if (((inputPinField.text.Equals(""))) && loginButton.interactable)
        {
            loginButton.interactable = false;
			//Debug.Log ("Missing Username or Password. Please try again.");
		}
		//Makes login button interactable if there are values in both input fields and the button is non interactable
		//if ((!loginButton.interactable) && ((!usernameValue.text.Equals("")) && (!passwordValue.text.Equals("")))){
		if ((!loginButton.interactable) && (!inputPinField.text.Equals(""))){
			loginButton.interactable = true;
			//Debug.Log ("Login success!");
		}
		
		// Allow easier transition between input fields with Tab button
		if (Input.GetKeyDown(KeyCode.Tab)) {
			if (usernameInput.isFocused) {
				passwordInput.Select();
			}
			if (passwordInput.isFocused){
				usernameInput.Select();
			}
		}
	}
}
