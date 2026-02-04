using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BookAnatomyDebugOverlay : MonoBehaviour
{
    [SerializeField] private BookAnatomyController book;

    private GUIStyle style;

    private void Awake()
    {
        if (!book)
            book = GetComponent<BookAnatomyController>();

        style = new GUIStyle
        {
            fontSize = 18,
            normal = { textColor = Color.white }
        };
    }

    private void OnGUI()
    {
        if (!book) return;

        GUILayout.BeginArea(new Rect(20, 20, 450, 250), GUI.skin.box);

        GUILayout.Label("📘 Book Anatomy Debug", style);
        GUILayout.Space(5);

        GUILayout.Label($"On Podium: {book.Debug_IsOnPodium}", style);
        GUILayout.Label($"Grabbed: {book.Debug_IsGrabbed}", style);
        GUILayout.Label($"Swapped To Open-Flat: {book.Debug_Swapped}", style);
        GUILayout.Label($"Open-Flat Active: {book.Debug_OpenFlatActive}", style);
        GUILayout.Label($"Swap Coroutine Running: {book.Debug_SwapRoutineRunning}", style);

        GUILayout.EndArea();
    }
}
