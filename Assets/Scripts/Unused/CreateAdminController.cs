using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAdminController : MonoBehaviour {

    private const string PIN_PREF_KEY = "pinCode";

	//Reference to DataService script
	private DataService ds;

	// Reference to SceneController script
	private SceneController sc;

	//UI References
	private InputField usernameInput;
	private InputField passwordInput;
	private InputField emailInput;

	//private Button invalidButtonComponent;

    [SerializeField] InputField pinField;
    [SerializeField] GameObject invalidPopup;
    [SerializeField] Button submitButton;

    void Start () {

		//Set reference to DataService script
		ds = StartupScript.ds;

		//Set reference to SceneController script
		sc = GameObject.FindGameObjectWithTag("CreateAdminController").GetComponent<SceneController>();

		//Set references to UI objects
		//usernameInput = GameObject.FindGameObjectWithTag("UserInputField").GetComponent<InputField>();
		//passwordInput = GameObject.FindGameObjectWithTag("PassInputField").GetComponent<InputField>();
		//emailInput = GameObject.FindGameObjectWithTag("EmailInputField").GetComponent<InputField>();

		//submitButton = GameObject.FindGameObjectWithTag("forwardbutton").GetComponent<Button>();
		submitButton.interactable = false;

		//invalidPopup = GameObject.Find("InvalidAttempt");
		//invalidButtonComponent = invalidPopup.GetComponent<Button>();
		//invalidButtonComponent.onClick.AddListener(ClosePopup);
		invalidPopup.SetActive(false);
	}

	///<summary> Handles checking if a user exists and displaying the result, also creates a new user</summary>
	public void AccountCreation(){
		// Retrieve Admins table data
		IEnumerable<Admins> result = ds.SearchForAdmin(usernameInput.text);

		// Check Admins table for username, if exists 
		bool userIsThere = false;
		foreach (var row in result) {
			userIsThere = true;
		}

		if (userIsThere){
			invalidPopup.SetActive(true);
		}
		else{
			int rowsInserted = 0;
			try{
				//Hash password
				string hashedPw = ds.Md5Sum(passwordInput.text);
				//Insert user
				rowsInserted = ds.CreateAdmin(usernameInput.text, hashedPw, emailInput.text);
			}
			catch{
				Debug.Log("Hash Failed");
			}
			Debug.Log("Num of rows inserted: " + rowsInserted.ToString());

			if (rowsInserted > 0){
				Text t = invalidPopup.GetComponentInChildren<Text>();
				t.text = "SUCCESS !\n  Admin account created\n\n(Click to continue)";
				//invalidButtonComponent.onClick.RemoveListener(ClosePopup);
				//invalidButtonComponent.onClick.AddListener(BackToLogin);
				invalidPopup.SetActive(true);
			}
		}
	}

    // TODO: MAKE MORE SECURE PIN CODE
    public void CreatePinAccount()
    {
        int pin = short.Parse(pinField.text);

        //
        if (ds.DoesPinExist(pin) == 1)
        {
            invalidPopup.SetActive(true);
        }
        else
        {
            //PlayerPrefs.SetInt(PIN_PREF_KEY, short.Parse(pinField.text));
            ds.InsertPin(pin);
            Text t = invalidPopup.GetComponentInChildren<Text>();
            t.text = "SUCCESS !\n  Admin account created\n\n(Click to continue)";
            //invalidButtonComponent.onClick.RemoveListener(ClosePopup);
            //invalidButtonComponent.onClick.AddListener(BackToLogin);
            invalidPopup.SetActive(true);
        }

        /*
        if (ds.DoesAdminExist(usernameInput.text) == 1)
        {
            invalidPopup.SetActive(true);
        }
        else
        {
            int rowsInserted = 0;
            try
            {
                //Insert user
                rowsInserted = ds.CreateAdminPin(usernameInput.text, short.Parse(pinField.text));
            }
            catch
            {
                Debug.Log("Hash Failed");
            }

            Debug.Log("Num of rows inserted: " + rowsInserted.ToString());

            if (rowsInserted > 0)
            {
                Text t = invalidPopup.GetComponentInChildren<Text>();
                t.text = "SUCCESS !\n  Admin account created\n\n(Click to continue)";
                //invalidButtonComponent.onClick.RemoveListener(ClosePopup);
                //invalidButtonComponent.onClick.AddListener(BackToLogin);
                invalidPopup.SetActive(true);
            }
        }
        */
    }

	///<summary> Handles closing the invalid login popup </summary>
	public void ClosePopup(){
		//usernameInput.text = "";
        pinField.text = "";
		//passwordInput.text = "";
		//emailInput.text = "";
		//submitButton.interactable = false;
		invalidPopup.SetActive(false);
	}

	///<summary> Returns the game to the Login screen </summary>
	public void BackToLogin(){
		COM_Director.LoadSceneByName("admin_login");
	}
	
	// Update is called once per frame
	void Update () {
		//if ((submitButton.interactable) && (usernameInput.text.Equals("") || passwordInput.text.Equals("") || emailInput.text.Equals(""))){
		if ((submitButton.interactable) || pinField.text.Equals("")) {
			submitButton.interactable = false;
		}

		//if ((!submitButton.interactable) && (!usernameInput.text.Equals("") || !passwordInput.text.Equals("") || !emailInput.text.Equals(""))){
		if ((!submitButton.interactable) || !pinField.text.Equals("")) {
			submitButton.interactable = true;
		}

		// Allow easier transition between input fields with Tab button
		if (Input.GetKeyDown(KeyCode.Tab)) {
			if (usernameInput.isFocused) {
				passwordInput.Select();
			}
			if (passwordInput.isFocused){
				emailInput.Select();
			}
			if (emailInput.isFocused){
				usernameInput.Select();
			}

		}
	}
}
