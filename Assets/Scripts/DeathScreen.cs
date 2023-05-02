using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void Update(){
        if(Input.GetMouseButtonDown(0) || Input.anyKeyDown){
            if(SceneTransition.Instance != null) Destroy(SceneTransition.Instance.gameObject);
            SceneManager.LoadScene("Menu");
        }
    }
}
