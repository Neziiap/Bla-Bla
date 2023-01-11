using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class DialoguePanel : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    [SerializeField]
    private RectTransform positionDialoguePanel1 = null;

    [SerializeField]
    private RectTransform positionDialoguePanel2 = null;

    [SerializeField]
    private Vector3 playerPosition1 = new Vector3();

    [SerializeField]
    private Vector3 playerPosition2 = new Vector3();

    [SerializeField]
    private Vector3 NPCPosition1 = new Vector3();

    [SerializeField]
    private Vector3 NPCPosition2 = new Vector3();

    public void Awake()
    {
        dialogueRunner.AddCommandHandler("Progression", UpdateProgression);
        dialogueRunner.AddCommandHandler("SpeakerName", SpeakerName);
    }

    public void UpdateProgression(string[] parameters)
    {
        int newProgression = 0;
        int.TryParse(parameters[0], out newProgression);
        Player.instance.SetProgression(newProgression); 
    }

    public void SpeakerName(string[] parameters)
    {
        if (parameters[0] == "Amy")
        {
            positionDialoguePanel1.position = playerPosition1;
            positionDialoguePanel2.position = playerPosition2;
        }
        else
        {
            positionDialoguePanel1.position = NPCPosition1;
            positionDialoguePanel2.position = NPCPosition2;
        }
    }
}