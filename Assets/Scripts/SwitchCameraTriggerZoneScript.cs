using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchCameraTriggerZoneScript : MonoBehaviour
{
    public GameObject destination = null;

    private GameObject UIChangeZone = null;

    public bool isADoor = true;

    public GameObject door = null;

    // Start is called before the first frame update
    void Start()
    {
        UIChangeZone = GameObject.Find("PlayerCanvas").transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.nextZone = destination.transform.parent.gameObject;
            Player.instance.destination = destination.transform.position;
            Player.instance.currentSwitchZone = this;
            UIChangeZone.SetActive(true);
            UIChangeZone.GetComponent<UnityEngine.UI.Text>().text = "Aller Vers";
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.nextZone = null;
            Player.instance.currentSwitchZone = null;
            UIChangeZone.SetActive(false);
        }
    }
}