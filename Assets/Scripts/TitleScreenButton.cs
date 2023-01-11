using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenButton : MonoBehaviour
{
    [SerializeField]
    private float waitTimeBeforeAction = 0.3f;

    [SerializeField]
    private bool playButton = true;
 
    [SerializeField]
    private string sceneToLaunch = null;

    private EventSystemScript eventSystemScript = null;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ClickButtonCallback);
        eventSystemScript = GameObject.Find("EventSystem").GetComponent<EventSystemScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickButtonCallback()
    {
        StartCoroutine(ClickButton());
    }

    IEnumerator ClickButton()
    {
        GetComponent<Animator>().SetTrigger("Pressed");
        eventSystemScript.transitionPanel.GetComponent<Animator>().SetTrigger("ActivateTransition");

        for (float currentTime = 0f; currentTime <= waitTimeBeforeAction; currentTime += Time.deltaTime)
            yield return null;

        if (playButton)
            LaunchScene();
        else
            ExitGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LaunchScene()
    {
        SceneManager.LoadScene(sceneToLaunch);
    }
}
