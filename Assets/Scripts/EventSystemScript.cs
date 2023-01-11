using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemScript : MonoBehaviour
{
    public GameObject transitionPanel = null;
    
    private UnityEngine.EventSystems.EventSystem eventSystem = null;

    private GameObject lastSelectedGameObject = null;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<UnityEngine.EventSystems.EventSystem>();
        lastSelectedGameObject = eventSystem.firstSelectedGameObject;
        eventSystem.firstSelectedGameObject.GetComponent<Animator>().SetBool("Selected", true);
        transitionPanel = GameObject.Find("Transition_Panel");
    }

    // Update is called once per frame
    void Update()
    { 
        if (eventSystem.currentSelectedGameObject && lastSelectedGameObject != eventSystem.currentSelectedGameObject && eventSystem.currentSelectedGameObject.GetComponent<Animator>())
        {
            lastSelectedGameObject.GetComponent<Animator>().SetBool("Selected", false);
            eventSystem.currentSelectedGameObject.GetComponent<Animator>().SetBool("Selected", true);
            lastSelectedGameObject = eventSystem.currentSelectedGameObject; 
        }
    }
}
