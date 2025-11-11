using UnityEngine;
using System.Collections;

public class AnatomyQuizSubtitle : MonoBehaviour
{
    public typingSubtitleManager typingSubtitle; //
    public GameObject subtitleCanvas; // assign your Canvas or Panel here

   
             public void StartOfQuiz()
    {
        string[] lines = new string[]
        {
            "Hello there, future veterinarian! Welcome to the PAWSIBLE Quiz Training Room!",
            "Before you dive into surgery, let’s test what you’ve learned so far",
            "On your left, you’ll find the Skeletal Table. Here, you’ll identify bones and learn their",
            "roles in keeping dogs strong. And on your right, we have the Visceral Table. ",
            "This is where you’ll explore the internal organs that keep the body alive and healthy! ",
            "Interact with a table to begin its quiz. Answer as many questions correctly as you can!",


        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }

    private IEnumerator ShowLinesSequentially(string[] lines, bool hideAfter)
    {
        foreach (string line in lines)
        {
            typingSubtitle.ShowSubtitle(line);

            // Wait for typing + 2s before next line
            yield return new WaitForSeconds(line.Length * typingSubtitle.typingSpeed + 2f);
        }

        if (hideAfter)
        {
            typingSubtitle.HideSubtitle();
            subtitleCanvas.SetActive(false); // hide the panel after intro
        }
    }
}
