using UnityEngine;
using TMPro;
using System.Collections;

public class TypingSubtitle : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;  // Assign in inspector
    public float typingSpeed = 0.01f;     // Speed of typing

    private Coroutine typingCoroutine;

    public void ShowSubtitle(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line)
    {
        // Show panel (parent of text)
        subtitleText.transform.parent.gameObject.SetActive(true);

        subtitleText.text = "";

        foreach (char c in line)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideSubtitle()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        subtitleText.text = "";
        subtitleText.transform.parent.gameObject.SetActive(false);
    }
}
