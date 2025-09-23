using UnityEngine;
using TMPro;
using System.Collections;

public class TypingSubtitle : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public float typingSpeed = 0.03f; // adjust speed

    private Coroutine typingCoroutine;

    public void ShowSubtitle(string line)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line)
    {
        subtitleText.text = "";
        subtitleText.gameObject.SetActive(true);

        foreach (char c in line)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideSubtitle()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        subtitleText.text = "";
        subtitleText.gameObject.SetActive(false);
    }
}
