using UnityEngine;
using System.Collections;

public class HTW_SubtitleSignalTrigger : MonoBehaviour
{
    public typingSubtitleManager typingSubtitle; //
    public GameObject subtitleCanvas; // assign your Canvas or Panel here

    public void ShowAllMessagesLineByLine()
    {
        string[] lines = new string[]
        {
            "Hello, friend! Let’s practice using your controllers. See those cubes on the table?",
            "Your task is to pick them up and place them in the trashcan to your left.",
            "This is a simple training exercise to get you ready for your journey.",
            

        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

        public void Step1_LeftJoystick()
    {
        string[] lines = new string[]
        {
            "Number one, use the joystick on your left controller for movement. ",
            "Push it forward to walk, pull it back to step backward, and tilt it left or right to move sideways.",

        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

            public void Step2_RightJoystick()
    {
        string[] lines = new string[]
        {
            "Number two, use the joystick on your right controller for navigation.",
            "This helps you turn your view, look around, and explore in every direction.",

        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

            public void Step3_TriggerButton()
    {
        string[] lines = new string[]
        {
            "Number three, press the trigger button with your index finger to select or interact. ",
            "This is like clicking a mouse, use it to choose items, or interact with other object like bones, door etc.",

        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

          public void Step4_GripButton()
    {
        string[] lines = new string[]
        {
            "Finally, number four, squeeze the grip button on the side of your controller to hold and carry things.",
            "Use this when you need to lift cubes, tools, or bones.",

        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

          public void FinalTest()
    {
        string[] lines = new string[]
        {
            "Now, let’s put your skills to the test! Pick those three cubes one by one and toss them into the trashcan,",
            "starting from the largest cube down to the smallest.",

        };

        subtitleCanvas.SetActive(true); // make sure canvas is visible
        StartCoroutine(ShowLinesSequentially(lines, true)); // hide after intro
    }

              public void Congratulation()
    {
        string[] lines = new string[]
        {
            "Congratulations! You’ve completed your practice.",
            " Now the real fun begins. Look to your right, there’s a door waiting just for you!",

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
            yield return new WaitForSeconds(line.Length * typingSubtitle.typingSpeed + 3f);
        }

        if (hideAfter)
        {
            typingSubtitle.HideSubtitle();
            subtitleCanvas.SetActive(false); // hide the panel after intro
        }
    }
}
