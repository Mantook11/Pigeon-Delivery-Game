using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    public float initialWeight = 5f;
    public float weight = 0f;

    bool held = false;
    bool capped = false;

    AudioSource audioSource;
    [SerializeField] AudioClip capClip, uncapClip, dropClip, pickupClip;

    List<Mail> mails = new List<Mail>();
    Transform wholeTopLeft, wholeBottomRight, topLeft, bottomRight;

    InputManager inputManager;
    GameManager gameManager;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite cappedSprite, uncappedSprite;

    [SerializeField] private LayerMask ScaleLayer;
    [SerializeField] private LayerMask BirdLayer;

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
        inputManager = InputManager.Instance;
        gameManager = GameManager.Instance;
        wholeTopLeft = gameManager.wholeTopLeft;
        wholeBottomRight = gameManager.wholeBottomRight;
        topLeft = gameManager.topLeft;
        bottomRight = gameManager.bottomRight;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        weight = initialWeight;
    }

    void Update(){
        if(held){
            Vector3 gamePos = Camera.main.ScreenToWorldPoint(inputManager.Position);
            transform.position = new Vector3(gamePos.x, gamePos.y, transform.position.z);
            if(transform.position.x < wholeTopLeft.position.x) transform.position = new Vector3(wholeTopLeft.position.x, transform.position.y, transform.position.z);
            if(transform.position.x > wholeBottomRight.position.x) transform.position = new Vector3(wholeBottomRight.position.x, transform.position.y, transform.position.z);
            if(transform.position.y > wholeTopLeft.position.y) transform.position = new Vector3(transform.position.x, wholeTopLeft.position.y, transform.position.z);
            if(transform.position.y < wholeBottomRight.position.y) transform.position = new Vector3(transform.position.x, wholeBottomRight.position.y, transform.position.z);
        }
    }

    public void RegisterClick(){
        Ray ray = Camera.main.ScreenPointToRay(inputManager.Position);
        RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
        if(hit.collider != null && hit.collider.TryGetComponent<Tube>(out Tube tube)){
            if(tube != null && tube == this){
                OnTubeClicked();
            }
        }
    }

    public void RegisterHold(){
        Ray ray = Camera.main.ScreenPointToRay(inputManager.Position);
        RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);
        if(hit.collider != null && hit.collider.TryGetComponent<Tube>(out Tube tube)){
            if(tube != null && tube == this){
                OnTubeHeld();
            }
        }
    }

    public void RegisterStopHold(){
        if(!held) return;

        held = false;
        Ray ray = Camera.main.ScreenPointToRay(inputManager.Position);
        RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity, ScaleLayer);
        if(hit.collider != null && hit.collider.TryGetComponent<Scale>(out Scale scale)){
            if(scale != null && mails.Count == 0){
                mails = scale.GetMails();
                if(mails.Count > 0){
                    float totalWeight = 0;
                    foreach(Mail mail in mails){
                        totalWeight += mail.weight;
                        mail.gameObject.SetActive(false);
                    }
                    totalWeight += initialWeight;
                    weight = totalWeight;
                    capped = true;
                    spriteRenderer.sprite = cappedSprite;
                    audioSource.PlayOneShot(capClip);
                }
            }
        }

        RaycastHit2D birdHit = Physics2D.GetRayIntersection (ray, Mathf.Infinity, BirdLayer);
        if(birdHit.collider != null && birdHit.collider.TryGetComponent<Bird>(out Bird bird)){
            if(bird != null && weight > initialWeight && bird.maxWeight >= weight){
                bird.SendBird(mails);
                Destroy(gameObject);
            }
            else if(bird != null && weight > initialWeight && bird.maxWeight < weight){
                bird.ComplainHeavy();
            }
        }

        if(hit.collider == null && birdHit.collider == null){
            audioSource.PlayOneShot(dropClip);
        }
    }

    public void OnTubeHeld(){
        held = true;
        audioSource.PlayOneShot(pickupClip);
    }

    public void OnTubeClicked(){
        if(!capped) return;

        if(transform.position.x < topLeft.position.x || transform.position.x > bottomRight.position.x || transform.position.y > topLeft.position.y || transform.position.y < bottomRight.position.y){
            return;
        }

        audioSource.PlayOneShot(uncapClip);

        capped = false;
        spriteRenderer.sprite = uncappedSprite;
        foreach(Mail mail in mails){
            mail.transform.position = new Vector3(transform.position.x, transform.position.y, mail.transform.position.z);
            mail.gameObject.SetActive(true);
        }
        weight = initialWeight;
        mails.Clear();
    }
}
