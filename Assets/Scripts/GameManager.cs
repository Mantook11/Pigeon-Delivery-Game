using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum GameState{
    Morning,
    Playing,
    Night,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set;}
    public GameState gameState;
    [SerializeField] GameObject mailsParent;
    public List<Mail> mails = new List<Mail>();
    public List<Mail> dayMails = new List<Mail>();
    public List<int> dayMailSeconds = new List<int>();
    public List<int> dayMailTimes = new List<int>();
    [SerializeField] List<AudioClip> pigeonClips = new List<AudioClip>();

    public Transform wholeTopLeft, wholeBottomRight, topLeft, bottomRight;

    private float timer;
    public float timerMultiplier = 2f;
    [SerializeField] private TextMeshProUGUI timerText;

    public delegate void MailReady(Mail mail);
    public static event MailReady MailReadyEvent;
    
    [SerializeField] float colorMultiplier;
    [SerializeField] float cloudMultiplier;

    [SerializeField] SpriteRenderer skyRenderer;
    [SerializeField] SpriteRenderer cloudsRenderer;
    [SerializeField] Transform clouds;

    [SerializeField] GameObject mailPrefab;
    public Sprite[] mailSprites;
    public Sprite[] insigniaSprites;

    int correctMail, incorrectMail = 0;

    int previousSeconds = 0;

    private SceneTransition sceneTransition;
    AudioSource audioSource;
    GameObject birdObject;

    void Awake(){
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    void Start(){
        gameState = GameState.Morning;
        sceneTransition = SceneTransition.Instance;
        audioSource = GetComponent<AudioSource>();
        HandleLeftovers();
        birdObject = GameObject.Find("Birds");
    }

    public void HandleLeftovers(){
        if(SceneTransition.Instance.mailInfos == null) return;
        foreach(MailInfo mailInfo in SceneTransition.Instance.mailInfos){
            Mail mail = Instantiate(mailPrefab, new Vector3(0f, -2f, -0.01f), Quaternion.identity).GetComponent<Mail>();
            mail.location = mailInfo.location;
            mail.priority = mailInfo.priority;
            mail.weight = mailInfo.weight;
            mail.type = mailInfo.type;

            mails.Add(mail);
        }
        SceneTransition.Instance.mailInfos = null;
    }

    void Update(){
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Return)){
            EndDay();
        }
        #endif

        if(birdObject.transform.childCount == 0){
            EndDay();
        }

        timer += timerMultiplier*Time.deltaTime;


        int minutes = Mathf.FloorToInt(timer / 60) + 9;
        int seconds = Mathf.FloorToInt(timer % 60);

        clouds.position += Vector3.right * Time.deltaTime * timerMultiplier * cloudMultiplier;

        if(minutes > 12){
            cloudsRenderer.color = new Color(cloudsRenderer.color.r - timer/colorMultiplier, cloudsRenderer.color.g - timer/colorMultiplier, cloudsRenderer.color.b - timer/colorMultiplier);
            skyRenderer.color = new Color(skyRenderer.color.r - timer/colorMultiplier, skyRenderer.color.g - timer/colorMultiplier, skyRenderer.color.b - timer/colorMultiplier);
        }
 
        if(dayMails.Count > 0){
            if(minutes >= dayMailTimes[0] && seconds >= dayMailSeconds[0]){
                MailReadyEvent?.Invoke(dayMails[0]);
            }
        }

        if (seconds % 15 == 0)
        {
            timerText.text = minutes >= 12 ? string.Format("{0:00}:{1:00} PM", minutes, seconds) : string.Format("{0:00}:{1:00} AM", minutes, seconds);
        }

        if(seconds % 60 == 0 && seconds != previousSeconds && birdObject.transform.childCount > 0){
            audioSource.PlayOneShot(pigeonClips[UnityEngine.Random.Range(0, pigeonClips.Count)]);
        }

        if(minutes >= 17){
            EndDay();
        }

        previousSeconds = seconds;
    }

    void EndDay(){
        int correctPoints = correctMail * 100;
        int incorrectPoints = incorrectMail * 50;

        int penaltyPoints = 0;
        foreach(Mail mail in mails){
            penaltyPoints += mail.priority * 25;
        }

        sceneTransition.LoadScene(sceneTransition.currentSceneIndex + 1, correctPoints, -incorrectPoints, -penaltyPoints, mails.ToArray());
        gameObject.SetActive(false);
    }

    void OnEnable(){
        Mail.MailDropEvent += UpdateMails;
    }

    void OnDisable(){
        Mail.MailDropEvent -= UpdateMails;
    }

    void UpdateMails(Mail droppedMail){
        float minZ = Mathf.Infinity;
        foreach(Mail mail in mails){
            if(mail.transform.position.z < minZ){
                minZ = mail.transform.position.z;
            }
        }
        droppedMail.transform.position = new Vector3(droppedMail.transform.position.x, droppedMail.transform.position.y, minZ - 0.001f);
    }

    public void CorrectMail(){
        correctMail++;
    }

    public void IncorrectMail(){
        incorrectMail++;
    }

}
