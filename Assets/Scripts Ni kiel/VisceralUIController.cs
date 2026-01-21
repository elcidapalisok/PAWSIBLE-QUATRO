using UnityEngine;
using TMPro;

public class VisceralUIController : MonoBehaviour
{
    public VisceralOrganData organData;

    public TextMeshProUGUI organNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI functionText;

    private int currentIndex = 0;

    void Start()
    {
        ShowOrgan(currentIndex);
    }

    void ShowOrgan(int index)
    {
        organNameText.text = organData.organs[index].organName;
        descriptionText.text = organData.organs[index].description;
        functionText.text = organData.organs[index].function;
    }

    // Called by NEXT button
    public void NextOrgan()
    {
        currentIndex++;
        if (currentIndex >= organData.organs.Length)
            currentIndex = 0;

        ShowOrgan(currentIndex);
    }

    // Called by PREVIOUS button
    public void PreviousOrgan()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = organData.organs.Length - 1;

        ShowOrgan(currentIndex);
    }
}
