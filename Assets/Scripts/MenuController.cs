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

    public Canvas VictoryMenu;
    public Canvas PauseMenu;
    public Canvas StartMenu;

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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void TogglePauseMenu()
    {
        if (PauseMenu.gameObject.activeInHierarchy)
        {
            PauseMenu.gameObject.SetActive(false);
        }
        else
        {
            PauseMenu.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(PauseMenu.transform.Find("ResumeButton").gameObject);
        }
    }

    public void ShowVictoryMenu(uint winner)
    {
        VictoryMenu.gameObject.SetActive(true);
        VictoryMenu.GetComponentInChildren<Text>().text = $"Player {winner} wins!";
        eventSystem.SetSelectedGameObject(VictoryMenu.transform.Find("ReturnToMenuButton").gameObject);
    }


    public void ReturnToStartMenu()
    {
        print("haloo");
        PauseMenu.gameObject.SetActive(false);
        VictoryMenu.gameObject.SetActive(false);
        StartMenu.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(StartMenu.transform.Find("PlayerSlider").gameObject);
    }
}
