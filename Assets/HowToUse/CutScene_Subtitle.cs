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
            "Before I tour you around the anatomy room,",
            "I’d like to highlight the intended learning outcomes of this module.",
            "This is the Anatomy Room, where you’ll study the skeletal, muscular, and visceral systems",
            "of animals. Look around, and you’ll see models and specimens ready for you to interact with.",
            "Before diving into the lessons, make sure to follow all laboratory safety measures,",
            "wear your lab coat and medical gloves. Safety first in every experiment.",
            "We even have a freezer, used to preserve the visceral and muscular specimens of canines,",
            "and a medical table specially prepared for organizing the bones.",
            "You’ll also notice this timer, it keeps track of your performance in each section,",
            "such as the skeletal, muscular, and visceral modules.",
            "Think of it as your personal progress clock, measuring how efficiently you complete every task.",
            "Now, to help you easily familiarize yourself with our animal’s limbs,",
            "let’s take a quick look at these two whiteboards.",
            "They show the major bone structures of the front and back legs, the thoracic and pelvic limbs.",
            "These are the thoracic limb bones, the front legs",
            "The humerus connects to the radius and ulna, and down to the carpal bones, metacarpals, and Phalanges.",
            "And here are the pelvic limb bones, the back legs",
            "The femur, tibia, fibula, and the bones of the paws help support movement and speed.",
            "Remember these parts, you’ll need them in the assessment. Let’s start with the skeletal system ",
            "Gently lift the bone, like this femur, and place it neatly on the preparation table.",
            "Always remember, correct handling prevents damage and keeps everything ready for study or reconstruction.",
            "Now, moving on to the visceral organs.Take each one, such as the stomach, and carefully",
            "store it inside the freezer. This ensures proper preservation and maintains the specimen’s quality for examination.",


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
