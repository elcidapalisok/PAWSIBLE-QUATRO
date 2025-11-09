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
    public void StartOfTaskTwo()
    {
        string[] lines = new string[]
        {
            "Great job completing your safety preparation!",
            "Now, let’s begin our guided activity with the pelvic limb bones.",
            "Please start by placing the pelvic bone on the medical table",

        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }

    public void PelvicDescription()
    {
        string[] lines = new string[]
        {
            "The pelvis connects the spine to the hind limbs and supports body weight during standing and walking"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Femur()
    {
        string[] lines = new string[]
        {
            "Great start!\nNow grab the femur,  the thick thigh bone, and set it on the table"

        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void FemurDescription()
    {
        string[] lines = new string[]
        {
            "The femur is the strongest bone in the hind limb and links the hip to the knee joint"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Tibia()
    {
        string[] lines = new string[]
        {
            "Awesome progress!\nNow, Grab the tibia next and place it correctly on the table"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void TibiaDescription()
    {
        string[] lines = new string[]
        {
            "The tibia is the main weight-bearing bone of the lower hind leg, forming the shin area"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Fibia()
    {
        string[] lines = new string[]
        {
            "Nice work!\nPick up the fibia, the slender bone beside the tibia, and organize it on the table"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void FibiaDescription()
    {
        string[] lines = new string[]
        {
            "The fibula provides balance and is important for muscle attachments in the hind leg"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Ankle()
    {
        string[] lines = new string[]
        {
            "You're doing great! Let’s place the tarsal bones, the ankle area"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void AnkleDescription()
    {
        string[] lines = new string[]
        {
            "Ankle help the dog push off the ground, essential for running and jumping"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Metatarsal()
    {
        string[] lines = new string[]
        {
            "Almost finished! Organize the Metatarsal bones nicely on the table"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void MetatarsalDescription()
    {
        string[] lines = new string[]
        {
            "These bones form the paw and toes, providing balance, grip, and flexible movement"

        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void StartOfTheThoracicBone()
    {
        string[] lines = new string[]
        {
            "Fantastic work! You’ve successfully arranged the thoracic limb bones!",
            "Now, let’s move on to our next challenge, the bones of the thoracic limb!",
            "Start by picking up the scapula and placing it on the same table with the pelvic limbs"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }

    public void ScapulaDescription()
    {
        string[] lines = new string[]
        {
            "The scapula is the shoulder blade",
            "It helps attach the forelimb to the body and allows wide, flexible movement of the shoulder"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Humerus()
    {
        string[] lines = new string[]
        {
            "Great! Now grab the humerus, the dog’s upper arm bone, and place it carefully on the table"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void HumerusDescription()
    {
        string[] lines = new string[]
        {
            "The humerus connects the shoulder to the elbow and plays a major role in supporting body movement"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Radius()
    {
        string[] lines = new string[]
        {
            "Awesome! Next is the radius. Go ahead and position it neatly beside the humerus"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void RadiusDescription()
    {
        string[] lines = new string[]
        {
             "The radius is the main weight-bearing bone of the forearm,",
             "helping support the dog while walking and running"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Ulna()
    {
        string[] lines = new string[]
        {
            "Nice work! Now locate the ulna, the thin bone behind the radius, and place it properly on the table"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void UlnaDescription()
    {
        string[] lines = new string[]
        {
            "The ulna forms the elbow joint and allows the forearm to bend and rotate"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Carpal()
    {
        string[] lines = new string[]
        {
            "Great job! Now, let’s move on to the carpals, the wrist bones. Please place them carefully on the table"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void CarpalDescription()
    {
        string[] lines = new string[]
        {
            "The carpals form the wrist joint of the forelimb and allow the dog to bend, absorb impact,",
            "and stabilize the front paw during movement"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Metacarpals()
    {
        string[] lines = new string[]
        {
            "Almost done! Lastly, pick up the metacarpals, the paw’s hand-like bones, and organize them on the table."
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void MetacarpalsDescription()
    {
        string[] lines = new string[]
        {
            "The metacarpals support the dog’s paw and help with balance during movement"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }

    public void LastSectionOfSkeletalSystem()
    {
        string[] lines = new string[]
        {
            "Fantastic job! You’ve successfully arranged all the bones of the thoracic limb!",
            "Now, we’re down to the final section of the skeletal system, exciting, right?",
            "Next, we will explore and organize the vertebral column, ribs, and other important parts",
            "Let’s begin with the skull! Carefully grab it and place it on the trolley, left side of the medical table"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void SkullDescription()
    {
        string[] lines = new string[]
        {
            "The skull protects the dog’s brain and supports important senses like vision, smell, and chewing"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
     public void Jaw()
    {
        string[] lines = new string[]
        {
            "Great start! Now pick up the mandible, the lower jaw, and position it beside of the skull"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
     public void JawDescription()
    {
        string[] lines = new string[]
        {
            "The jaw helps dogs chew food and express emotions like barking and growling"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Neck()
    {
        string[] lines = new string[]
        {
            "Awesome! Find the cervical vertebrae, the neck bones, and line them up right after the skull and jaw"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void NeckDescription()
    {
        string[] lines = new string[]
        {
            "These bones support the head and allow flexible motion like turning and nodding"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Ribs()
    {
        string[] lines = new string[]
        {
            "You’re doing great! Place the ribs and sternum set onto the middle section of the trolley"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void RibsDescription()
    {
        string[] lines = new string[]
        {
            "This region protects the heart and lungs and plays a big role in breathing"
        };

        subtitleCanvas.SetActive(true); // show canvas again
        StartCoroutine(ShowLinesSequentially(lines, true));
    }
    public void Heart()
{
    string[] lines = new string[]
    {

        " Let's start with the heart, place it at the center of the visceral inside the freezer."
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void HeartDescription()
{
    string[] lines = new string[]
    {
        "The heart pumps blood throughout the body, delivering oxygen and nutrients to keep the dog alive and active.",
    
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void Intestine()
{
    string[] lines = new string[]
    {
        "Great! Now pick up the intestines and gently place them inside the freezer."
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void IntestineDescription()
{
    string[] lines = new string[]
    {
        "The intestines help break down and absorb nutrients from the food the dog eats.",

    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void Penis()
{
    string[] lines = new string[]
    {
        "Awesome! Go ahead and place the penis model inside the second freezer"
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void PenisDescription()
{
    string[] lines = new string[]
    {
        "The penis is part of the male reproductive system,",
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void Testicles()
{
    string[] lines = new string[]
    {
        "You’re doing great! Now grab the testicles and place them near the penis."
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void TesticlesDescription()
{
    string[] lines = new string[]
    {
        "The testicles produce sperm and male hormones like testosterone.",

    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void Colon()
{
    string[] lines = new string[]
    {
        "Next, grab the colon. Place it inside the freezer"
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void ColonDescription()
{
    string[] lines = new string[]
    {
        "The colon absorbs water from waste and prepares stool for removal.",

    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void UrinaryBladder()
{
    string[] lines = new string[]
    {
        "Finally, grab the urinary bladder and put it inside the freezer."
    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

public void UrinaryBladderDescription()
{
    string[] lines = new string[]
    {
        "The urinary bladder stores urine produced by the kidneys until the dog is ready to pee.",

    };

    subtitleCanvas.SetActive(true);
    StartCoroutine(ShowLinesSequentially(lines, true));
}

    public void Congratulations()
    {
        string[] lines = new string[]
        {
            "Congratulations! ",
            "You’ve successfully completed all the required tasks to help you get familiar with this module.",
            "You can now freely explore the Anatomy Module at your own pace.",
            "When you’re ready to move forward, just come and talk to me,",
            "and we’ll begin the simulation together!"
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
