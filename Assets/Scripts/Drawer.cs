using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Drawer : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] GameObject tubePrefab;
    [SerializeField] AudioClip openClip, closeClip;
    SpriteRenderer spriteRenderer;
    bool open = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Flip(){
        open = !open;
        audioSource.clip = open ? openClip : closeClip;
        audioSource.Play();
        spriteRenderer.enabled = open;

        if(open){
            GameObject tube = Instantiate(tubePrefab, transform.position, Quaternion.identity);
            tube.transform.position += new Vector3(0, 0, -5);
            tube.transform.DOMoveY(transform.position.y + 0.5f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => tube.transform.DOMoveY(transform.position.y - Random.Range(1.5f, 2.5f), 0.5f).SetEase(Ease.OutBounce));
        }
    }

    void OnMouseDown(){
        Flip();
    }

}
