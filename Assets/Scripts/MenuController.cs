using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public Slider PlayerSlider;
    public GameObject GameController;
    private GameController gameController;

    public Canvas VictoryScreen;
    public Canvas PauseMenu;
    public Canvas MainMenu;

    private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameController.GetComponent<GameController>();
        eventSystem = GetComponentInChildren<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: replace with callback
        gameController.NumberOfPlayers = (uint) PlayerSlider.value;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void TogglePauseMenu()
    {
        if (!PauseMenu.gameObject.activeInHierarchy)
        {
            PauseMenu.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(PauseMenu.transform.Find("ResumeButton").gameObject);
        }
        else
        {
            PauseMenu.gameObject.SetActive(false);
        }

    }

    public void ShowVictoryScreen(uint winner)
    {
        VictoryScreen.gameObject.SetActive(true);
        VictoryScreen.GetComponent<Text>().text = $"Player {winner} wins!";
        eventSystem.SetSelectedGameObject(PauseMenu.transform.Find("ReturnToMenuButton").gameObject);
    }


    public void ReturnToMainMenu()
    {
        PauseMenu.gameObject.SetActive(false);
        VictoryScreen.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);
    }
}
