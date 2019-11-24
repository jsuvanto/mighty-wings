using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Range(1, 4)]
    public uint NumberOfPlayers;

    public GameObject PlayerShip;
    public Weapon[] Weapons;

    public Canvas PlayerHud;
    public Camera PlayerCamera;

    [Range(0, 100)]
    public uint PlayerHealth;
    [Range(0, 100)]
    public uint PlayerLives;

    private Dictionary<PlayerController, int> playerScores;
    private Dictionary<PlayerController, uint> playerLives;
    private Dictionary<PlayerController, uint> playerHealths;

    

    void Start()
    {

        // TODO: start menu

        Camera.main.enabled = false;

        for (uint playerNumber = 1; playerNumber <= NumberOfPlayers; playerNumber++)
        {
            var playerShip = CreatePlayerShip(playerNumber);
            var playerCamera = CreatePlayerCamera(playerNumber);
            playerCamera.transform.position = playerShip.transform.position - new Vector3(0, 0, 10);
            playerShip.GetComponent<PlayerController>().Camera = playerCamera;
            CreatePlayerHud(playerCamera, playerNumber);
        }
    }

    private void LateUpdate()
    {
        // TODO: update healths
        // TODO: update lives
        // TODO: kill dead players
        // TODO: update score
        // TODO: display victory screen

    }



    private GameObject CreatePlayerShip(uint playerNumber)
    {
        print($"Spawning player {playerNumber}");
        GameObject playerShip = Instantiate(PlayerShip, new Vector3(playerNumber, 0, 1), new Quaternion());
        var playerController = playerShip.GetComponent<PlayerController>();
        playerController.Weapon = Instantiate(Weapons[0], playerShip.transform);
        playerController.Initialize(playerNumber);
        return playerShip;
    }

    private Camera CreatePlayerCamera(uint playerNumber)
    {
        float x = playerNumber % 2 == 0 ? 0.5f : 0;
        float y = playerNumber < 3 ? 0.5f : 0;
        var playerCamera = Instantiate(PlayerCamera, GameObject.Find("Cameras").transform);
        playerCamera.rect = new Rect(x, y, 0.5f, 0.5f);
        playerCamera.name = $"Player {playerNumber} camera";
        return playerCamera;
    }

    private void CreatePlayerHud(Camera playerCamera, uint playerNumber)
    {
        var playerHud = Instantiate(PlayerHud);
        playerHud.renderMode = RenderMode.ScreenSpaceCamera;
        playerHud.worldCamera = playerCamera;
        playerHud.planeDistance = 1;
        playerHud.name = $"Player {playerNumber} HUD";
    }
}
