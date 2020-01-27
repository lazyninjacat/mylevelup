using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the View script for the word audio recording scene. It is in charge of 
/// taking in recorded audio from the user and playing it back to the user.
/// </summary>
public class VW_WordMic : MonoBehaviour
{
    private const int RECORD_LENGTH = 3;

    // Get all scene buttons, text, and input fields
    public Button recordButton;
    public Button stopButton;
    public Button previewButton;
    public Button applyButton;
    public Text recordingText;

    // Get the component's container
    public GameObject components;

    private bool inputFieldFilled, isRecording;
    private string micName;
    private AudioSource sourceAudio;
    private AudioClip clip;
    private CON_WordEditing controller;

    // Create consts
    const string RECORDING = "Recording...";
    const string NO_RECORDING = "No Recording";
    const string DONE_RECORD = "Done!";
    const string RECORD_ERROR = "Error, Try Again";

    // Use this for initialization
    void Start()
    {
        MAS_WordEditing tempMaster = (MAS_WordEditing)COM_Director.GetMaster("MAS_WordEditing");
        controller = (CON_WordEditing)tempMaster.GetController("CON_WordEditing");

        Debug.Log("MIC: Target word is " + controller.GetTargetWord());

        stopButton.interactable = false;
        previewButton.interactable = false;
        applyButton.interactable = false;
        sourceAudio = components.GetComponent<AudioSource>();

        if (Microphone.devices.Length < 0)
        {
            micName = Microphone.devices[0];
        }
        else
        {
            Debug.Log("*****\n Could not find any microphone devices*****\n");
        }
        
    }

    /// <summary>
    /// Starts recording audio.
    /// </summary>
    public void StartRecording()
    {
        stopButton.interactable = true;
        recordingText.text = RECORDING;
        clip = Microphone.Start(micName, false, RECORD_LENGTH, 44100);
    }

    /// <summary>
    /// Stops audio recording and sets the source audio clip to the newly made clip.
    /// </summary>
    public void StopRecording()
    {
        stopButton.interactable = false;
        Microphone.End(micName);

        if (clip != null)
        {
            recordingText.text = DONE_RECORD;
        }
        else
        {
            recordingText.text = RECORD_ERROR;
            return;
        }

        applyButton.interactable = true;
        previewButton.interactable = true;

        sourceAudio.clip = clip;
    }

    /// <summary>
    /// Plays recorded audio.
    /// </summary>
    public void PlayAudio()
    {
        sourceAudio.Play();
    }

    /// <summary>
    /// Calls the controller to save the currently recorded clip.
    /// </summary>
    public void SaveClip()
    {
        controller.SetCurrentClip(clip);
        controller.SceneChange("word_edit_add");
    }

    public void CancelAndClose() { controller.SceneChange("word_edit_add"); }
}