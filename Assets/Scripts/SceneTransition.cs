using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Image transitionImage;
    [SerializeField] private TextMeshProUGUI transitionText, gainText, lossText, penaltyText, houseText, borderText, resultText;
    [SerializeField] private float transitionTime;
    [SerializeField] private float textPauseTime;
    [SerializeField] private int pigeonFee = -600;

    AudioSource audioSource;

    public int currentSceneIndex = 1;

    [SerializeField] AudioClip textSound;

    public static SceneTransition Instance { get; private set; }

    public List<MailInfo> mailInfos;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Transform[] objects = FindObjectsOfType<Transform>();
        foreach (Transform obj in objects)
        {
            if (obj.parent == transform)
            {
                DontDestroyOnLoad(obj.gameObject);
            }
        }
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 0);
        transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, 0);
    }

    public void LoadScene(int scene, int gain, int loss, int penalty, Mail[] leftOvers)
    {
        StartCoroutine(Transition(scene, gain, loss, penalty, leftOvers));
    }

    IEnumerator Transition(int scene, int gain, int loss, int penalty, Mail[] leftOvers)
    {
        audioSource.Play();
        transitionText.text = "Day " + (currentSceneIndex - 1) + " FINISHED";
        gainText.text = "CORRECT MAILS = " + gain.ToString() + " COINS";
        lossText.text = "INCORRECT MAILS = " + loss.ToString() + " COINS";
        penaltyText.text = "UNHANDLED MAILS = " + penalty.ToString() + " COINS";
        houseText.text = "PIGEON CARE = " + pigeonFee + " COINS";
        resultText.text = "TOTAL = " + (gain + loss + penalty + pigeonFee) + " COINS";

        // Fade out
        while (transitionImage.color.a < 1)
        {
            transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, transitionImage.color.a + Time.deltaTime / transitionTime);
            transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, transitionText.color.a + Time.deltaTime / transitionTime);
            yield return null;
        }

        yield return new WaitForSeconds(1);
        gainText.gameObject.SetActive(true);
        audioSource.PlayOneShot(textSound);
        yield return new WaitForSeconds(textPauseTime);
        lossText.gameObject.SetActive(true);
        audioSource.PlayOneShot(textSound);
        yield return new WaitForSeconds(textPauseTime);
        penaltyText.gameObject.SetActive(true);
        audioSource.PlayOneShot(textSound);
        yield return new WaitForSeconds(textPauseTime);
        houseText.gameObject.SetActive(true);
        audioSource.PlayOneShot(textSound);
        yield return new WaitForSeconds(textPauseTime);
        borderText.gameObject.SetActive(true);
        resultText.gameObject.SetActive(true);
        audioSource.PlayOneShot(textSound);
        yield return new WaitForSeconds(7);

        if(gain + loss + penalty + pigeonFee < 0)
        {
            SceneManager.LoadScene("Lose");
            // currentSceneIndex = 0;
            yield break;
        }

        mailInfos = new List<MailInfo>();
        foreach (Mail mail in leftOvers)
        {
            MailInfo temp = new MailInfo(mail.location, mail.weight, mail.priority, mail.type);
            mailInfos.Add(temp);
        }

        // Load new scene
        currentSceneIndex++;
        SceneManager.LoadScene(currentSceneIndex);

        // Fade in
        while (transitionImage.color.a > 0)
        {
            transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, transitionImage.color.a - Time.deltaTime / transitionTime);
            transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, transitionText.color.a - Time.deltaTime / transitionTime);
            yield return null;
        }

        gainText.gameObject.SetActive(false);
        lossText.gameObject.SetActive(false);
        penaltyText.gameObject.SetActive(false);
        houseText.gameObject.SetActive(false);
        borderText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
    }
}
