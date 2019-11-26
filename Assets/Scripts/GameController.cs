using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Range(2, 4)]
    public uint NumberOfPlayers;

    public GameObject PlayerShip;
    public Weapon[] Weapons;

    public Canvas PlayerHud;
    public Camera PlayerCamera;
    
    public uint PlayerHealth;
    public uint PlayerLives;

    [Tooltip("Respawn time after death in seconds")]
    public uint RespawnTime;

    private List<PlayerController> players = new List<PlayerController>();

    private bool isPaused = false;
    private bool isRunning = false;

    public GameObject MenuController;
    private MenuController menuController;
    public Camera MainCamera;

    private void Start()
    {
        menuController = MenuController.GetComponent<MenuController>();
    }

    public void StartGame()
    {
        MainCamera.gameObject.SetActive(false);

        for (uint playerNumber = 1; playerNumber <= NumberOfPlayers; playerNumber++)
        {
            var playerShip = CreatePlayerShip(playerNumber);
            var playerController = playerShip.GetComponent<PlayerController>();
            
            var playerCamera = CreatePlayerCamera(playerNumber);
            playerCamera.transform.position = playerShip.transform.position - new Vector3(0, 0, 10);
            playerShip.GetComponent<PlayerController>().Camera = playerCamera;
            var playerHud = CreatePlayerHud(playerCamera, playerNumber);
            playerHud.transform.SetParent(playerCamera.transform);

            playerController.Camera = playerCamera;
            playerController.Hud = playerHud;
            playerController.Health = PlayerHealth;
            playerController.Lives = PlayerLives;
            
            players.Add(playerController);
        }
        isPaused = false;
        isRunning = true;
        Time.timeScale = 1;
    }

    private void LateUpdate()
    {
        if (isRunning)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseGame();
            }

            foreach (var player in players)
            {
                player.LivesText.text = player.Lives.ToString();
                player.HealthText.text = player.Health.ToString();

                if (player.Lives == 0) continue;

                if (!player.isActiveAndEnabled && player.TimeOfDeath + RespawnTime < Time.time)
                {
                    player.gameObject.transform.position = RandomLocation(player.PlayerNumber);
                    player.Health = PlayerHealth;
                    player.gameObject.SetActive(true);
                }

                // TODO: update score
            }

            if (players.Where(p => p.Lives > 0).Count() == 1)
            {
                var winner = players.Single(p => p.Lives > 0).PlayerNumber;
                // TODO: change to victory screen
                DeclareWinner(winner);
            }
        }
    }

    private GameObject CreatePlayerShip(uint playerNumber)
    {
        GameObject playerShip = Instantiate(PlayerShip, RandomLocation(playerNumber), new Quaternion());        
        var playerController = playerShip.GetComponent<PlayerController>();
        playerController.Weapon = Instantiate(Weapons[0], playerShip.transform);
        playerController.Initialize(playerNumber);
        playerController.Health = PlayerHealth;
        playerController.Lives = PlayerLives;
        return playerShip;
    }

    private Vector3 RandomLocation(uint playerNumber)
    {
        // TODO: implement properly
        return new Vector3(playerNumber, 0, 1);
    }

    private Camera CreatePlayerCamera(uint playerNumber)
    {
        float x = playerNumber % 2 == 0 ? 0.5f : 0;
        float y = (NumberOfPlayers > 2 && playerNumber < 3) ? 0.5f : 0;
        float width = (playerNumber == 3 && NumberOfPlayers == 3) ? 1 : 0.5f;
        float height = NumberOfPlayers == 2 ? 1 : 0.5f;

        var playerCamera = Instantiate(PlayerCamera, GameObject.Find("Cameras").transform);
        playerCamera.rect = new Rect(x, y, width, height);
        playerCamera.name = $"Player {playerNumber} camera";
        return playerCamera;
    }

    private Canvas CreatePlayerHud(Camera playerCamera, uint playerNumber)
    {
        var playerHud = Instantiate(PlayerHud);
        playerHud.renderMode = RenderMode.ScreenSpaceCamera;
        playerHud.worldCamera = playerCamera;
        playerHud.planeDistance = 1;
        playerHud.name = $"Player {playerNumber} HUD";
        return playerHud;
    }


    public void TogglePauseGame()
    {
        if (isPaused)
        {
            menuController.TogglePauseMenu();
            isPaused = false;
            Time.timeScale = 1;
        }
        else
        {
            menuController.TogglePauseMenu();
            isPaused = true;
            Time.timeScale = 0;
        }
    }

    private void DeclareWinner(uint winner)
    {
        isRunning = false;
        menuController.ShowVictoryMenu(winner);
        // TODO: disable controls
    }


    public void ResetGame()
    {
        Time.timeScale = 0;
        isRunning = false;
        MainCamera.gameObject.SetActive(true);

        foreach (var player in players)
        {
            Destroy(player.Camera.gameObject);
            Destroy(player.gameObject);

            // TODO: remove ammo pools
        }
        players.Clear();
    }
}
