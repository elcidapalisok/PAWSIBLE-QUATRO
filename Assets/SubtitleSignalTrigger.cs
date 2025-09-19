using UnityEngine;
using System.Collections;

public class SubtitleSignalTrigger : MonoBehaviour
{
    public TypingSubtitle typingSubtitle;
    public GameObject subtitleCanvas; // assign your Canvas or Panel here

    public void ShowAllMessagesLineByLine()
    {
        string[] lines = new string[]
        {
            "Welcome to Pawsible, your virtual companion in learning veterinary anatomy and surgery.",
            "I’ll be your guide, Dr. Paws! \nTogether, we’ll explore the different rooms.",
            "Follow along, and don’t worry, I’ll help you every step of the way.",
            "Now, let’s start! \nHead to the first door on your right and open it."
        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

    public void ShowDoorMessage()
    {
        string[] lines = new string[]
        {
            "Great job opening the door! Let’s move forward and see what’s inside.",
             "This is the Anatomy Room, where you’ll study the skeletal, muscular, and visceral systems",
             "of animals. Look around, and you’ll see models and specimens ready for you to interact with.",
             "Check your left side, there’s a lab coat waiting for you. Grab it, put it on,",
            "and get ready to step into the role of Dr. Paws’ assistant!"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); // keep visible after door lines
    }

        public void ShowGrabBone()
    {
        string[] lines = new string[]
        {
            "Good job, assistant! Head over to the canine skeleton and pick up the femur bone.",
             "Watch closely, the bone description will appear as soon as you hold it!",
    
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); // keep visible after door lines
    }

           public void ShowVisceral()
    {
        string[] lines = new string[]
        {
            "Well done! Now, move on to the visceral system and pick up the liver to examine it.",
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); // keep visible after door lines
    }

          public void EndOfAnatomyRoom()
    {
        string[] lines = new string[]
        {
            "Great job, assistant! Take a moment to enjoy the Anatomy Room, interact, experiment, and discover",
            "After you’ve had your fun, we’ll head over to the Surgery Room for some hands-on practice!"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); // keep visible after door lines
    }

              public void StartOfSurgeryRoom()
    {
        string[] lines = new string[]
        {
            "Now, let’s move to the Surgery Room. Here, you’ll practice real-life procedures",
            "in a safe and guided environment"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); // keep visible after door lines
    }

             public void StartOfLibraryRoom()
    {
        string[] lines = new string[]
        {
            "Great work so far! Now, let’s step into the Library Room. This is your treasure chest of knowledge,",
            "explore books, models, and diagrams to test yourself and learn more"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); // keep visible after door lines
    }

    private IEnumerator ShowLinesSequentially(string[] lines, bool hideAfter)
    {
        foreach (string line in lines)
        {
            typingSubtitle.ShowSubtitle(line);

            // Wait for typing + 2s before next line
            yield return new WaitForSeconds(line.Length * typingSubtitle.typingSpeed + 3f);
        }

        if (hideAfter)
        {
            typingSubtitle.HideSubtitle();
            subtitleCanvas.SetActive(false); // hide the panel after intro
        }
    }
}
