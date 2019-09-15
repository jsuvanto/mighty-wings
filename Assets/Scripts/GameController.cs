using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [Range(1, 4)]
    public uint NumberOfPlayers;

    public MapGenerator mapGenerator;
    public GameObject PlayerShip;

    void Start()
    {

        // start menu

        // spawn players

        Camera.main.enabled = false;

        for (uint playerNumber = 1; playerNumber <= NumberOfPlayers; playerNumber++)
        {
            print($"Spawning player {playerNumber}");
            GameObject playerShip = Instantiate(PlayerShip, new Vector3(playerNumber, 0, 1), new Quaternion());
            var playerController = playerShip.GetComponent<PlayerController>();
            playerController.Initialize(playerNumber);
        }


        // assign controls to players

        // disable main camera, spawn cameras for each player
    }

    // Update is called once per frame
    void Update()
    {

    }
}
