using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] Sprite pressedSprite;
    Sprite defaultSprite;

    void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    void OnMouseOver(){
        spriteRenderer.sprite = pressedSprite;
    }

    void OnMouseExit(){
        spriteRenderer.sprite = defaultSprite;
    }

    void OnMouseDown(){
        Application.Quit();
    }
}
