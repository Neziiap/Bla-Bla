using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private string itemToReceive = "No item";

    private GameObject UITalk = null;

    private GameObject UIReadMind = null;

    #pragma warning disable 0649                                    // permet d'eviter l'erreur de la non affectation du variable

    [Header("Généralités")]
    #pragma warning disable 0414                                    // permet d'eviter l'erreur de la non affectation du nom
    [SerializeField] bool triggerOnStart;                           // Doit se jouer au démarrage de la scene ?
    [SerializeField] bool deactiveAfterTriggered;                   // Doit se désactiver après la scene ?
    [SerializeField] string characterName = "";                     // Nom de l'élément Important de le set pour l'avancée
    #pragma warning restore 0414
        
    DialogueRunner dialogueRunner;

    #pragma warning restore 0649
    private 


    void Start()
    {
        dialogueRunner = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueRunner>();
        UITalk = GameObject.Find("PlayerCanvas").transform.GetChild(0).gameObject;
        UIReadMind = GameObject.Find("PlayerCanvas").transform.GetChild(1).gameObject;
    }

    public void StartTalking()
    {
        if (dialogueRunner.IsDialogueRunning == false)
        {
            dialogueRunner.yarnScripts[0] = Player.instance.currentYarnFile;
            Player.instance.dialogueHolder.sprite = Player.instance.talkBubble;   
            dialogueRunner.StartDialogue(characterName + ".Talk." + Player.instance.currentQuestState.ToString());  
        }
        Player.instance.NPCIsTalking = true;
    }

    public void StartReadingMind()
    {
        if (dialogueRunner.IsDialogueRunning == false)
        {
            dialogueRunner.yarnScripts[0] = Player.instance.currentYarnFile;
            Player.instance.dialogueHolder.sprite = Player.instance.telepathyBubble ;   
            dialogueRunner.StartDialogue(characterName + ".Telepathy." + Player.instance.currentQuestState.ToString() );   /// Telep a la place de Talk)
        }
        Player.instance.isReadingNPCMind = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().NPCToTalk = gameObject.GetComponent<NPC>();
            UITalk.SetActive(true);
            UITalk.GetComponent<UnityEngine.UI.Text>().text = "Discuter";
            UIReadMind.SetActive(true);
            UIReadMind.GetComponent<UnityEngine.UI.Text>().text = "Télépathie";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().NPCToTalk = null;
            UITalk.SetActive(false);
            UIReadMind.SetActive(false);
        }
    }

    public void ReceiveItem()
    {
        if (itemToReceive == Player.instance.itemTakenName)
        {
            Player.instance.itemTakenName = "";
        }
    }
}