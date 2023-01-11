using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] 
    private UnityEvent updateProgressionEvent = null;

    public int progressionNeeded;
    public bool isProgressionTriggered;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isProgressionTriggered == true)
        {
            if (other.CompareTag("Player") && Player.instance.currentQuestState == progressionNeeded)
            {
                updateProgressionEvent.Invoke();
            }
        }
        else
        {
            updateProgressionEvent.Invoke();
        }
    }
}
