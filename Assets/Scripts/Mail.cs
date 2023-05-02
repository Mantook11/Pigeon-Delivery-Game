using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public struct MailInfo{
    public string location;
    public float weight;
    public int priority;
    public int type;

    public MailInfo(string location, float weight, int priority, int type){
        this.location = location;
        this.weight = weight;
        this.priority = priority;
        this.type = type;
    }
}

public class Mail : MonoBehaviour
{
    public string location;
    public float weight;
    [Range(0, 3)]
    public int priority;
    [Range(0, 6)]
    public int type;

    bool held = false;
    Vector2 holdOffset;

    public float heldZ = 0;
    public float normalZ;

    InputManager inputManager;
    GameManager gameManager;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    Transform topLeft, bottomRight;

    [SerializeField] TextMeshProUGUI locationText;
    [SerializeField] Image insignia;

    public delegate void MailDrop(Mail mail);
    public static event MailDrop MailDropEvent;

    void OnEnable(){
        InputManager.RegisterClickEvent += RegisterClick;
        InputManager.RegisterHoldEvent += RegisterHold;
        InputManager.RegisterStopHoldEvent += RegisterStopHold;
    }

    void OnDisable(){
        InputManager.RegisterClickEvent -= RegisterClick;
        InputManager.RegisterHoldEvent -= RegisterHold;
        InputManager.RegisterStopHoldEvent -= RegisterStopHold;
    }

    void Start(){
        locationText.text = "DEST: " + location;
        inputManager = InputManager.Instance;
        gameManager = GameManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        topLeft = gameManager.topLeft;
        bottomRight = gameManager.bottomRight;
        normalZ = transform.position.z;
        spriteRenderer.sprite = gameManager.mailSprites[type];
        insignia.sprite = gameManager.insigniaSprites[priority];
    }

    void Update(){
        if(held){
            Vector3 gamePos = Camera.main.ScreenToWorldPoint(inputManager.Position);
            transform.position = new Vector3(gamePos.x, gamePos.y, heldZ) + (Vector3)holdOffset;
            if(transform.position.x < topLeft.position.x) transform.position = new Vector3(topLeft.position.x, transform.position.y, transform.position.z);
            if(transform.position.x > bottomRight.position.x) transform.position = new Vector3(bottomRight.position.x, transform.position.y, transform.position.z);
            if(transform.position.y > topLeft.position.y) transform.position = new Vector3(transform.position.x, topLeft.position.y, transform.position.z);
            if(transform.position.y < bottomRight.position.y) transform.position = new Vector3(transform.position.x, bottomRight.position.y, transform.position.z);
        }
    }

    public void RegisterClick(){
        Ray ray = Camera.main.ScreenPointToRay(inputManager.Position);
        RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
        if(hit.collider != null && hit.collider.TryGetComponent<Mail>(out Mail mail)){
            if(mail != null && mail == this){
                OnMailClicked();
            }
        }
    }

    public void RegisterHold(){
        Ray ray = Camera.main.ScreenPointToRay(inputManager.Position);
        RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
        if(hit.collider != null && hit.collider.TryGetComponent<Mail>(out Mail mail)){
            if(mail != null && mail == this){
                OnMailHeld();
            }
        }
    }

    public void RegisterStopHold(){
        if(held){
            held = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, normalZ);
            MailDropEvent?.Invoke(this);
        }
    }

    public void OnMailHeld(){
        audioSource.Play();
        held = true;
        holdOffset = (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(inputManager.Position);
    }

    public void OnMailClicked(){
    }
}
