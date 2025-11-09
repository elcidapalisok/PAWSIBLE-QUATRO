using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button startButton;

    public Button storyModeButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;
    [Header("Background")]
    public Image backgroundImage; // Drag your Image object here
    public Sprite mainMenuBg;
    public Sprite optionBg;
    public Sprite aboutBg;


    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(StartGame);
        storyModeButton.onClick.AddListener(StoryMode);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

       public void StoryMode()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(3);
    }


    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

  public void EnableMainMenu()
    {
        HideAll();
        mainMenu.SetActive(true);
        backgroundImage.sprite = mainMenuBg;
    }

    public void EnableOption()
    {
        HideAll();
        options.SetActive(true);
        backgroundImage.sprite = optionBg;
    }

    public void EnableAbout()
    {
        HideAll();
        about.SetActive(true);
        backgroundImage.sprite = aboutBg;
    }
}