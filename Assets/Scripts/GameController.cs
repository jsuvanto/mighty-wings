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

    public Canvas PlayerUi;
    public Camera PlayerCamera;

    void Start()
    {

        // start menu

        Camera.main.enabled = false;

        for (uint playerNumber = 1; playerNumber <= NumberOfPlayers; playerNumber++)
        {
            var playerShip = CreatePlayerShip(playerNumber);
            var camera = CreatePlayerCamera(playerNumber, playerShip);
            CreatePlayerUi(camera);            
        }
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

    private Camera CreatePlayerCamera(uint playerNumber, GameObject playerShip)
    {
        float x = playerNumber % 2 == 0 ? 0.5f : 0;
        float y = playerNumber < 3 ? 0.5f : 0;
        var playerCamera = Instantiate(PlayerCamera, GameObject.Find("Cameras").transform);
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        playerCamera.rect = new Rect(x, y, 0.5f, 0.5f);
        playerCamera.name = $"Player {playerNumber} camera";

        playerShip.GetComponent<PlayerController>().Camera = playerCamera;
        return playerCamera;
    }

    private void CreatePlayerUi(Camera playerCamera)
    {
        var playerUi = Instantiate(PlayerUi);
        playerUi.renderMode = RenderMode.ScreenSpaceCamera;
        playerUi.worldCamera = playerCamera;
        playerUi.planeDistance = 1;
    }
}
