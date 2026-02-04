using UnityEngine;
using UnityEngine.UI;

public class StarRatingUI : MonoBehaviour
{
    [Header("Star Images (size must be 5)")]
    public Image[] stars = new Image[5];

    [Header("Sprites")]
    public Sprite starOn;
    public Sprite starOff;

    public void SetRating(int starsOn)
    {
        if (stars == null) return;

        int clamped = Mathf.Clamp(starsOn, 0, 5);

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;
            stars[i].sprite = (i < clamped) ? starOn : starOff;
        }
    }
}
