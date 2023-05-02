using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bird : MonoBehaviour
{
    public string location;
    public string birdName;
    public float maxWeight = 30f;

    [SerializeField] int peckRandom;

    Animator animator;
    GameManager gameManager;
    AudioSource audioSource;

    [SerializeField] TextMeshProUGUI heavyText;

    void Start(){
        gameManager = GameManager.Instance;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        InvokeRepeating("Peck", peckRandom, 2f);
    }

    private void Peck(){
        if(Random.Range(0, peckRandom) != 1) return;
        animator.SetTrigger("Peck");
    }

    public void SendBird(List<Mail> mails){
        foreach(Mail mail in mails){
            if(location == mail.location) gameManager.CorrectMail();
            else gameManager.IncorrectMail();
            gameManager.mails.Remove(mail);
        }
        audioSource.Play();
        StartCoroutine(FadeBird());
    }

    IEnumerator FadeBird(){
        animator.SetTrigger("Fly");
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        while(spriteRenderer.color.a > 0){
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void ComplainHeavy(){
        StartCoroutine(ComplainHeavyRoutine());
    }

    IEnumerator ComplainHeavyRoutine(){
        heavyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        heavyText.gameObject.SetActive(false);
    }
}
