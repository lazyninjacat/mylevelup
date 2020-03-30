using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class ParentalGateControl : MonoBehaviour {

	public Text task;
	private string taskDescription;
	private int correctAnswer;
	private string textFalseAnswer = "Incorrect, try again\n";
    private bool firstTry;
	Delegate callback;

    private void Start()
    {
        firstTry = true;

        GenerateAndShowTask();
    }
          
    public void CheckAnswer(Text userAnswer)
	{
        if (userAnswer.text.Equals(correctAnswer.ToString())) 
		{       
            Close();

            InvokeCallback(true);
        }
        else
		{
            firstTry = false;

            GenerateAndShowTask();

            task.GetComponent<Animation>().Play();

			InvokeCallback(false);
		}
	}
	
	public void Close()
	{
        StartCoroutine(CloseHelper());
	}
    private IEnumerator CloseHelper()
    {
        task.text = "Correct!";

        task.GetComponent<Animation>().Play("ButtonWiggleAnim");

        yield return new WaitForSeconds(0.5f);

        gameObject.GetComponent<Animation>().Play("ShrinkGoneAnim");

        yield return new WaitForSeconds(0.5f);

        firstTry = true;

        GenerateAndShowTask();

        if (SceneManager.GetActiveScene().name == "game_loop")
        {
            gameObject.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "admin_login")
        {
            Transform parentalGatePanel = gameObject.transform.parent;

            parentalGatePanel.gameObject.SetActive(false);

            gameObject.SetActive(false);

            SceneManager.LoadScene("admin_menu");
        }
    }

	public void Show(Action<bool> callback)
	{
		gameObject.SetActive(true);

		GenerateAndShowTask();

		SetCallback(callback);
	}

	private void GenerateAndShowTask()
	{
        int a = UnityEngine.Random.Range(0, 4);

		int b = UnityEngine.Random.Range(1, 5);	
        
		correctAnswer = a + b;

        if (firstTry == true)
        {
            task.text = a.ToString() + " + " + b.ToString();
        }
        else
        {
            task.text = textFalseAnswer + a.ToString() + " + " + b.ToString();
        }
    }

	void SetCallback(Delegate callback)
	{ 
		if (callback != null)
		{				
			this.callback = callback;
		}
	}

	void InvokeCallback(object result)
	{
		if (this.callback != null)
		{
			this.callback.DynamicInvoke(result);
		}
	}		
}
