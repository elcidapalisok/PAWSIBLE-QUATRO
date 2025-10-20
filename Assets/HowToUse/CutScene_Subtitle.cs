using UnityEngine;
using System.Collections;

public class CutScene_Subtitle : MonoBehaviour
{
    public typingSubtitleManager typingSubtitle; //
    public GameObject subtitleCanvas; // assign your Canvas or Panel here

    public void Introduction()
    {
        string[] lines = new string[]
        {
            "Welcome to Pawsible, your virtual companion in learning veterinary anatomy and surgery.",
            "I’ll be your guide, Dr. Paws! \nTogether, we’ll explore the different rooms.",
            "Follow along, and don’t worry, I’ll help you every step of the way.",

        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

        public void OpeningAnatomyRoom()
    {
        string[] lines = new string[]
        {
            "This is the Anatomy Room, where you’ll study the skeletal, muscular, and visceral systems",
            "of animals. Look around, and you’ll see models and specimens ready for you to interact with.",
            "Before diving into the lessons, make sure to follow all laboratory safety measures,",
            "wear your lab coat and medical gloves. Safety first in every experiment.",
            "We even have a freezer, used to preserve the visceral and muscular specimens of canines,",
            "and a trolley specially prepared for organizing and transporting the bones.", 
            "You’ll also notice this timer, it keeps track of your performance in each section,",
            "such as the skeletal, muscular, and visceral modules.",
            "Think of it as your personal progress clock, measuring how efficiently you complete every task."




        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
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
