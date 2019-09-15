using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [Range(1, 4)]
    public uint NumberOfPlayers;

    public MapGenerator mapGenerator;
    public GameObject PlayerShip;
    public Weapon[] Weapons;

    void Start()
    {

        // start menu

        Camera.main.enabled = false;

        for (uint playerNumber = 1; playerNumber <= NumberOfPlayers; playerNumber++)
        {
            print($"Spawning player {playerNumber}");
            GameObject playerShip = Instantiate(PlayerShip, new Vector3(playerNumber, 0, 1), new Quaternion());
            var playerController = playerShip.GetComponent<PlayerController>();
            playerController.Weapon = Instantiate(Weapons[0], playerShip.transform);
            playerController.Initialize(playerNumber);
        }
    }
}
