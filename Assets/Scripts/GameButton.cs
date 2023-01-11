using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    [SerializeField]
    private float waitTimeBeforeAction = 0.3f;

    [SerializeField]
    private bool playButton = true;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ClickButtonCallback);
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
        if (!playButton)
            Player.instance.eventSystemScript.transitionPanel.GetComponent<Animator>().SetTrigger("ActivateTransition");

        for (float currentTime = 0f; currentTime <= waitTimeBeforeAction; currentTime += Time.deltaTime)
            yield return null;

        if (playButton)
            ResumeGame();
        else
            ExitGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        GameObject.Find("Pause-Menu").SetActive(false);
    }
}
