using UnityEngine;
using TMPro;
using System.Collections;

public class LetterInteraction : MonoBehaviour
{
    public GameObject letterUI; 
    public TextMeshProUGUI letterText; 
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI newLetterContent; 
    public GameObject firstPanel;
    public GameObject secondPanel; 

    public AudioSource audioSource1;
    public AudioSource audioSource2;

    private bool isNearLetter = false;
    private bool isReadingLetter = false;
    private bool hasReadLetter = false; 
    private bool isLetterCompleted = false; 
    private GameObject currentCharacter = null;

    void Start()
    {
        letterUI.SetActive(false); 
        interactionText.gameObject.SetActive(false); 
        newLetterContent.gameObject.SetActive(false);
        firstPanel.SetActive(false); 
        secondPanel.SetActive(false); 

        audioSource1.Stop();
        audioSource2.Stop(); 
    }

    void Update()
    {
        if (isNearLetter && Input.GetKeyDown(KeyCode.E) && !hasReadLetter)
        {
            letterUI.SetActive(!letterUI.activeSelf);

            if (letterUI.activeSelf)
            {
                interactionText.gameObject.SetActive(false);
                isReadingLetter = true;

                // Bắt đầu quá trình thay đổi nội dung và hiển thị panel
                StartCoroutine(ChangeLetterContentAndShowFirstPanel(2));
            }
            else
            {
                interactionText.gameObject.SetActive(true);
                isReadingLetter = false;
                StopAllCoroutines();
                letterText.gameObject.SetActive(true);
                newLetterContent.gameObject.SetActive(false);
                firstPanel.SetActive(false);
                secondPanel.SetActive(false);
                audioSource1.Stop();
                audioSource2.Stop();
            }
        }
    }

    IEnumerator ChangeLetterContentAndShowFirstPanel(float delay)
    {
        yield return new WaitForSeconds(2); 
        if (isReadingLetter)
        {
            if (!audioSource1.isPlaying)
            {
                audioSource1.Play();
            }

            yield return StartCoroutine(FadeOut(letterText)); 
            letterText.gameObject.SetActive(false); 
            newLetterContent.gameObject.SetActive(true);
            firstPanel.SetActive(true); 

            
            StartCoroutine(HideFirstPanelAndShowSecondPanel(2));
        }
    }

    IEnumerator HideFirstPanelAndShowSecondPanel(float delay)
    {
        yield return new WaitForSeconds(delay); 
        if (isReadingLetter)
        {
            firstPanel.SetActive(false); 
            if (!audioSource2.isPlaying) 
            {
                audioSource2.Play();
            }
            secondPanel.SetActive(true); 
            isLetterCompleted = true; 
        }
    }

    IEnumerator FadeOut(TextMeshProUGUI text, float fadeDuration = 1f)
    {
        Color originalColor = text.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - normalizedTime); 
            yield return null;
        }
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasReadLetter) 
        {
            if (!letterUI.activeSelf) 
            {
                interactionText.gameObject.SetActive(true);
            }
            isNearLetter = true;
            currentCharacter = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentCharacter)
        {
            if (!isLetterCompleted) 
            {
                return;
            }

            interactionText.gameObject.SetActive(false); 
            isNearLetter = false;
            currentCharacter = null;

            if (letterUI.activeSelf)
            {
                letterUI.SetActive(false); 
                isReadingLetter = false; 
                StopAllCoroutines(); 
                letterText.gameObject.SetActive(true);
                newLetterContent.gameObject.SetActive(false); 
                firstPanel.SetActive(false); 
                secondPanel.SetActive(false); 

                // Tắt cả hai âm thanh khi người chơi rời xa
                if (audioSource1.isPlaying)
                {
                    audioSource1.Stop();
                }
                if (audioSource2.isPlaying)
                {
                    audioSource2.Stop();
                }
            }
        }
    }
}
