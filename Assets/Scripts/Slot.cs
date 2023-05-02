using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Slot : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip slotClip;
    SpriteRenderer spriteRenderer;
    bool open = true;
    GameManager gameManager;

    void Start(){
        gameManager = GameManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer.enabled = false;
    }

    void OnEnable(){
        GameManager.MailReadyEvent += SpawnMail;
    }

    void OnDisable(){
        GameManager.MailReadyEvent -= SpawnMail;
    }

    void OnMouseDown(){
        Flip();
        audioSource.PlayOneShot(slotClip);
    }

    public void Flip(){
        open = !open;
        spriteRenderer.enabled = !open;
    }

    public void SpawnMail(Mail mail){
        if(!open) return;
        mail.gameObject.SetActive(true);
        mail.transform.DOMoveY(transform.position.y - 1, 0.5f).SetEase(Ease.OutBounce);
        audioSource.Play();

        gameManager.mails.Add(gameManager.dayMails[0]);
        gameManager.dayMails.RemoveAt(0);
        gameManager.dayMailTimes.RemoveAt(0);
        gameManager.dayMailSeconds.RemoveAt(0);
    }

}
