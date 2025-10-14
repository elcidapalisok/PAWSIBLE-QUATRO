using UnityEngine;
using System.Collections;

public class SubtitleSignalTrigger : MonoBehaviour
{
    public typingSubtitleManager typingSubtitle; //
    public GameObject subtitleCanvas; // assign your Canvas or Panel here

    public void AnatomyStart()
    {
        string[] lines = new string[]
        {

            "Now that you’re ready, let’s begin!",
            "Head to the first door on your right and open it to start your anatomy journey."
        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

    public void ShowDoorMessage()
    {
        string[] lines = new string[]
        {
            "Great job opening the door! Let’s move forward and see what’s inside.",
             "Here’s your checklist for the anatomy room! Once you finish everything,",
             "we’ll have a short quiz to test what you’ve learned.",
             "Check your left side, you’ll see a lab coat and gloves. Wear them both,",
            "and get ready to step into your role as Dr. Paws’ assistant!"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }

        public void ShowGrabBone()
    {
        string[] lines = new string[]
        {
            "Good job, assistant! Head over to the canine skeleton and pick up the femur bone.",
             "Watch closely, the bone description will appear as soon as you hold it!",
             "Now, carefully put the femur on the trolley located on the freezer’s left side.",
    
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }
 
               public void ShowGrabBone_3()
    {
        string[] lines = new string[]
        {
            "Excellent work! Next, pick up the humerus and put it carefully on the trolley",

        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }

                public void EndOfCL2()
    {
        string[] lines = new string[]
        {
            "Awesome! Now, let’s look at your checklist and see what’s next.",
            "Place the Liver in the freezer.",

        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }
                  public void StartOfCheckList3()
    {
        string[] lines = new string[]
        {
            "Good job! Only three more to go. Keep it up, you’re almost there!",
            "Now, in the same freezer, place the stomach gently along with the liver.",

        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }

                  public void StartOfCheckList4()
    {
        string[] lines = new string[]
        {
            "Task complete. The stomach has been properly stored. Now, we will proceed to the muscular system.",
            "Insert the triceps muscle into the second freezer to keep it preserved."

        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }



          public void EndOfAnatomyRoom()
    {
        string[] lines = new string[]
        {
            "Great job, assistant! Take a moment to enjoy the Anatomy Room, interact, experiment, and discover",
            "After you’ve had your fun, we’ll head over to the Surgery Room for some hands-on practice!"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }

              public void StartOfSurgeryRoom()
    {
        string[] lines = new string[]
        {
            "Now, let’s move to the Surgery Room. Here, you’ll practice real-life procedures",
            "in a safe and guided environment"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true)); 
    }

             public void StartOfLibraryRoom()
    {
        string[] lines = new string[]
        {
            "Great work so far! Now, let’s step into the Library Room. This is your treasure chest of knowledge,",
            "explore books, models, and diagrams to test yourself and learn more"
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
