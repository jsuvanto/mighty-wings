using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public Slider PlayerSlider;
    public GameObject GameController;
    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameController.GetComponent<GameController>();
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
}
