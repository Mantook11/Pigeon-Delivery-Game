using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scale : MonoBehaviour
{
    public LayerMask mailLayer;
    public LayerMask tubeLayer;
    private float weight;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] Vector2 weighArea;


    void Update()
    {
        weight = 0;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, weighArea, 0, Vector2.zero, 0, mailLayer);
        if(hits.Length > 0){
            foreach(RaycastHit2D hit in hits){
                if(hit.collider.TryGetComponent<Mail>(out Mail mail)){
                    if(mail != null){
                        weight += mail.weight;
                    }
                }
            }
        }

        RaycastHit2D[] tubeHits = Physics2D.BoxCastAll(transform.position, weighArea, 0, Vector2.zero, 0, tubeLayer);
        if(tubeHits.Length > 0){
            foreach(RaycastHit2D hit in tubeHits){
                if(hit.collider.TryGetComponent<Tube>(out Tube tube)){
                    if(tube != null){
                        weight += tube.weight;
                    }
                }
            }
        }

        weightText.text = weight.ToString() + "G";
    }

    public List<Mail> GetMails(){
        List<Mail> mails = new List<Mail>();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, weighArea, 0, Vector2.zero, 0, mailLayer);
        if(hits.Length > 0){
            foreach(RaycastHit2D hit in hits){
                if(hit.collider.TryGetComponent<Mail>(out Mail mail)){
                    if(mail != null){
                        mails.Add(mail);
                    }
                }
            }
        }
        return mails;
    }
}
