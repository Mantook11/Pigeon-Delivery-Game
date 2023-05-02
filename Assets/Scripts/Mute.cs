using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mute : MonoBehaviour
{
    [SerializeField] AudioListener audioListener;
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite mutedSprite, unmutedSprite;

    void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Switch(){
        audioListener.enabled = !audioListener.enabled;

        if(!audioListener.enabled){
            spriteRenderer.sprite = mutedSprite;
        } else {
            spriteRenderer.sprite = unmutedSprite;
        }
    }

    void OnMouseDown(){
        Switch();
    }
}
