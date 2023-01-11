using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct Sounds
    {
        public AudioClip doorOpening;
        public AudioClip takingItem;
        public AudioClip catMeowing;
        public AudioClip speechBubble;
        public AudioClip readingMind;
        public AudioClip changingZone;
        public AudioClip feet;
        public AudioClip click;
        public AudioClip stairs;
    }

    public Image dialogueHolder;
    public Sprite talkBubble;
    public Sprite telepathyBubble;

    [HideInInspector]
    static public Player instance = null;

    public int currentQuestState = 0;

    public GameObject currentZone = null;

    public GameObject nextZone = null;

    [HideInInspector]
    public Vector3 destination = new Vector3();

    public string itemTakenName;

    public Collectible itemToTake = null;

    public NPC NPCToTalk = null;

    [HideInInspector]
    public bool canMove = true;

    public bool NPCIsTalking = false;

    [HideInInspector]
    public bool isReadingNPCMind = false;

    [HideInInspector]
    public bool isWatchingItem = false;

    public Sounds sounds;

    public YarnProgram currentYarnFile = null;

    public SwitchCameraTriggerZoneScript currentSwitchZone = null;

    public EventSystemScript eventSystemScript = null;

    [SerializeField]
    private float speed = 1.0f;

    [SerializeField]
    private float stopDuration = 1.0f;

    [SerializeField]
    private float beforeTalkDuration = 1.0f;

    [SerializeField]
    private Vector3 offsetWhenTalking = new Vector3();

    [SerializeField]
    private GameObject initZone = null;

    [SerializeField]
    private float timeBeforeTakeItem = 1.0f;

    private Rigidbody rb;

    private SpriteRenderer spriteRenderer;

    private Color spriteColor;

    private Animator animator;

    private AudioSource audioSource;

    private Cinemachine.CinemachineVirtualCamera talkingVcam;

    public UnityEvent[] events;

    private GameObject playerCanvas = null;

    private bool canTalk = true;

    private float talkCooldown = 0.5f;

    private DialogueRunner dialogueRunner;

    private GameObject pauseMenu = null;

    private bool canTp = true;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        talkingVcam = GameObject.Find("Talking Virtual Cam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        currentZone = initZone;
        playerCanvas = GetComponentInChildren<UnityEngine.Canvas>().gameObject;
        dialogueRunner = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueRunner>();
        canMove = false;
        pauseMenu = GameObject.Find("Pause-Menu");
        pauseMenu.SetActive(false);
        eventSystemScript = GameObject.Find("EventSystem").GetComponent<EventSystemScript>();
        SetProgression(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 6") || Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);            

        if (Input.GetKeyDown("joystick button 7") || Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pauseMenu.transform.GetChild(1).gameObject);
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            pauseMenu.transform.GetChild(1).GetComponent<Animator>().SetBool("Selected", true);
            animator.SetFloat("movingLat", 0.0f);
            animator.SetBool("isMovingFront", false);
        }
        if (!pauseMenu.activeSelf)
        {
            for (int i = 256; i < 266; i++)
                if (Input.GetKeyDown((KeyCode)i))
                    SetProgression(i - 256);

            if (canMove)
            {
                Vector3 inputDirection = GetInputDirection() * Time.deltaTime * speed;

                if (!(inputDirection.sqrMagnitude > 0.0f))
                {
                    animator.SetFloat("movingLat", 0.0f);
                    animator.SetBool("isMovingFront", false);
                    rb.velocity = Vector3.zero;
                }

                else
                {
                    if (inputDirection.x > 0.01f)
                    {
                        animator.SetFloat("movingLat", 1.0f);
                        if (spriteRenderer.flipX)
                            spriteRenderer.flipX = false;
                    }
                    else if (inputDirection.x < -0.01f)
                    {
                        animator.SetFloat("movingLat", -1.0f);
                        if (!spriteRenderer.flipX)
                            spriteRenderer.flipX = true;
                    }
                    if (inputDirection.z > 0.01f || inputDirection.z < -0.01f)
                        animator.SetBool("isMovingFront", true);

                    else
                        animator.SetBool("isMovingFront", false);

                    rb.MovePosition(transform.position + inputDirection);
                }
            }

            if (Input.GetKeyDown("joystick button 0") || Input.GetKeyDown(KeyCode.A))
            {
                if (nextZone && canTp)
                    ChangeZone();

                else if (itemToTake && canTalk)
                    StartCoroutine(NeedToTakeItem());

                else if (NPCToTalk && !isReadingNPCMind && !NPCIsTalking && canTalk)
                    StartCoroutine(StartTalking(true));

                else if (dialogueRunner.IsDialogueRunning)
                    dialogueRunner.GetComponent<DialogueUI>().MarkLineComplete();
            }

            if (Input.GetKeyDown("joystick button 2") || Input.GetKeyDown(KeyCode.X))
            {
                if (NPCToTalk && !isReadingNPCMind && !NPCIsTalking && canTalk)
                    StartCoroutine(StartTalking(false));
            }
        }
    }

    Vector3 GetInputDirection()
    {
        return (Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.forward * Input.GetAxisRaw("Vertical")).normalized;
    }

    public void AmyStartTalking()
    {
        canMove = false;
        canTalk = false;
       // if (dialogueRunner.IsDialogueRunning == false)
       // {
            //dialogueRunner.yarnScripts[0] = currentYarnFile;
        animator.SetFloat("movingLat", 0.0f);
        animator.SetBool("isMovingFront", false);

            dialogueHolder.sprite = telepathyBubble;
            dialogueRunner.StartDialogue("Amy.Self." + currentQuestState.ToString());
        //}
    }

    public void ChangeZone()
    {
        if (currentZone)
            currentZone.transform.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Priority = 1;
        nextZone.transform.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Priority = 2;

        StartCoroutine(ChangeRoom());
    }

    IEnumerator ChangeRoom()
    {
        canMove = false;
        canTp = false;
        animator.SetFloat("movingLat", 0.0f);
        animator.SetBool("isMovingFront", false);
        playerCanvas.SetActive(false);
        float coeff = Time.deltaTime * (stopDuration * 2.0f);
        if (currentSwitchZone.isADoor)
        {
            if (currentSwitchZone.door)
                currentSwitchZone.door.GetComponent<Animator>().SetTrigger("OpenDoor");
            audioSource.PlayOneShot(sounds.doorOpening);
        }
        else
            audioSource.PlayOneShot(sounds.stairs);

        for (float currentTime = 0f; currentTime <= stopDuration; currentTime += Time.deltaTime)
        {
            if (currentTime < stopDuration / 2.0f)
            {
                spriteColor.a -= coeff;
                spriteRenderer.color = spriteColor;
            }
            yield return null;
        }

        transform.position = destination;
        currentZone = nextZone;
        audioSource.PlayOneShot(sounds.changingZone);

        for (float currentTime = 0f; currentTime <= stopDuration / 2.0f; currentTime += Time.deltaTime)
        {
            spriteColor.a += coeff;
            spriteRenderer.color = spriteColor;
            yield return null;
        }
        canMove = true;
        spriteColor.a = 1.0f;
        spriteRenderer.color = spriteColor;
        playerCanvas.SetActive(true);
        StartCoroutine(WaitForNextTp());
    }

    IEnumerator StartTalking(bool talking)
    {
        currentZone.transform.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Priority = 1;
        talkingVcam.Follow = NPCToTalk.transform;
        talkingVcam.Priority = 2;
        float coeff = Time.deltaTime * (beforeTalkDuration * 2.0f);
        playerCanvas.SetActive(false);
        canMove = false;
        canTalk = false;
        animator.SetFloat("movingLat", 0.0f);
        animator.SetBool("isMovingFront", false);

        for (float currentTime = 0f; currentTime <= beforeTalkDuration; currentTime += Time.deltaTime)
        {
            if (currentTime < beforeTalkDuration / 2.0f)
            {
                spriteColor.a -= coeff;
                spriteRenderer.color = spriteColor;
            }
            yield return null;
        }

        transform.position = NPCToTalk.transform.position + offsetWhenTalking;

        for (float currentTime = 0f; currentTime <= beforeTalkDuration / 2.0f; currentTime += Time.deltaTime)
        {
            spriteColor.a += coeff;
            spriteRenderer.color = spriteColor;
            yield return null;
        }
        spriteColor.a = 1.0f;
        spriteRenderer.color = spriteColor;

        if (talking)
            NPCToTalk.StartTalking();

        else
        {
            NPCToTalk.StartReadingMind();
            animator.SetBool("isReadingMind", true);
            audioSource.PlayOneShot(sounds.readingMind);
        }
    }

    IEnumerator NeedToTakeItem()
    {
        canMove = false;
        canTalk = false;
        animator.SetFloat("movingLat", 0.0f);
        animator.SetBool("isMovingFront", false);
        if (itemToTake.takeItem)
        {
            animator.SetTrigger("isGrabbing");
            for (float currentTime = 0f; currentTime <= timeBeforeTakeItem; currentTime += Time.deltaTime)
                yield return null;
        }
        itemToTake.StartWatching();
            
        if (itemToTake.takeItem)
            TakeItem();
    }

    public void TakeItem()
    {
        itemTakenName = itemToTake.name;
        Destroy(itemToTake.gameObject);
        itemToTake = null;
        audioSource.PlayOneShot(sounds.takingItem);
    }

    public void PlayerCanMove()
    {
        canMove = true; 
    }

    public void StopDialogue()
    {
        if (animator.GetBool("isReadingMind"))
            animator.SetBool("isReadingMind", false);

        currentZone.transform.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Priority = 2;
        talkingVcam.Priority = 1;
        isReadingNPCMind = false;
        NPCIsTalking = false;
        playerCanvas.SetActive(true);
        StartCoroutine(WaitForNextTalk());
    }

    IEnumerator WaitForNextTalk()
    {
        for (float currentTime = 0f; currentTime <= talkCooldown; currentTime += Time.deltaTime)
            yield return null;

        canTalk = true;
    }

    IEnumerator WaitForNextTp()
    {
        for (float currentTime = 0f; currentTime <= talkCooldown; currentTime += Time.deltaTime)
            yield return null;

        canTp = true;
    }

    public void SetProgression(int newProgression)
    {
        currentQuestState = newProgression;
        Debug.Log("SetProgressions     " + "ValueWanted:" +newProgression + "_" + "CurrentQuestState:"+ currentQuestState);

        events[currentQuestState].Invoke();
    }

    IEnumerator fadeOutBeforeChangeScene(string sceneToLaunch)
    {
        eventSystemScript.transitionPanel.GetComponent<Animator>().SetTrigger("ActivateTransition");

        for (float currentTime = 0f; currentTime <= 1.0f; currentTime += Time.deltaTime)
            yield return null;

        SceneManager.LoadScene(sceneToLaunch);
    }

    public void LaunchScene(string sceneToLaunch)
    {
        StartCoroutine(fadeOutBeforeChangeScene(sceneToLaunch));
    }
}
