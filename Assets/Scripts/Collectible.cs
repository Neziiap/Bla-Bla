using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Collectible : MonoBehaviour
{
    public bool takeItem = true;

    private GameObject UITakeItem = null;
    
    private DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
        UITakeItem = GameObject.Find("PlayerCanvas").transform.GetChild(0).gameObject;
        dialogueRunner = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().itemToTake = this;
            UITakeItem.SetActive(true);
            if (takeItem)
                UITakeItem.GetComponent<UnityEngine.UI.Text>().text = "Ramasser";
            else
                UITakeItem.GetComponent<UnityEngine.UI.Text>().text = "Observer";
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().itemToTake = null;
            UITakeItem.SetActive(false);
        }
    }
    public void StartWatching()
    {
        if (dialogueRunner.IsDialogueRunning == false)
        {
            dialogueRunner.yarnScripts[0] = Player.instance.currentYarnFile;
            Player.instance.dialogueHolder.sprite = Player.instance.telepathyBubble;
            dialogueRunner.StartDialogue(name);
        }

        Player.instance.canMove = false;
        Player.instance.isWatchingItem = true;
    }

    private void OnDestroy()
    {
        if (UITakeItem)
            UITakeItem.SetActive(false);
    }

    public void SetGettable(bool value )
    {
        takeItem = value;
    }

}
