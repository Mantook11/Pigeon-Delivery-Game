using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject[] tutorials;

    int index = 0;

    void Start(){
        tutorials[index].gameObject.SetActive(true);
        GameManager.Instance.timerMultiplier = 0f;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            tutorials[index].gameObject.SetActive(false);
            index++;
            if(index < tutorials.Length){
                tutorials[index].gameObject.SetActive(true);
            }else{
                SceneManager.LoadScene("Day1");
            }
        }

    }

}
