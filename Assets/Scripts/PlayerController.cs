using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public uint PlayerNumber;
    [Range(0,100)]
    public uint Health;
    [Range(0, 100)]
    public uint Lives;

    public uint ThrottleForce;
    public uint SteeringSpeed;
    public Camera CameraPrefab;
    public Weapon Weapon;

    private Rigidbody2D body;
    private Camera playerCamera;
    private Vector3 cameraOffset;
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();    
    }

    private void FixedUpdate()
    {
        // Steering
        float throttle = Mathf.Max(Input.GetAxis($"Player {PlayerNumber} Throttle"), 0);
        float steering = Input.GetAxis($"Player {PlayerNumber} Steering");
        body.AddForce(throttle * ThrottleForce * body.transform.up);
        body.transform.Rotate(steering * SteeringSpeed * Vector3.forward);


        float fire = Input.GetAxis($"Player {PlayerNumber} Fire");
        if (fire > 0)
        {
            Weapon.Fire();
        }

        if (Health <= 0)
        {
            Die();
        }
    }

    private void LateUpdate()
    {
        playerCamera.transform.position = transform.position - cameraOffset;       
    }

    public void Initialize(uint playerNumber)
    {
        PlayerNumber = playerNumber;
        name = $"Player {playerNumber}";

        var spriteRenderer = GetComponent<SpriteRenderer>();

        switch (playerNumber)
        {
            case 1:
                spriteRenderer.color = Color.red;
                break;
            case 2:
                spriteRenderer.color = Color.blue;
                break;
            case 3:
                spriteRenderer.color = Color.green;
                break;
            case 4:
                spriteRenderer.color = Color.yellow;
                break;
        }

        float x = playerNumber % 2 == 0 ? 0.5f : 0;
        float y = playerNumber < 3 ? 0.5f : 0;
        playerCamera = Instantiate(CameraPrefab, GameObject.Find("Cameras").transform);
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        cameraOffset = transform.position - playerCamera.transform.position;
        playerCamera.rect = new Rect(x, y, 0.5f, 0.5f);
        playerCamera.name = $"Player {playerNumber} camera";
    }

    private void Die()
    {
        print($"Player {PlayerNumber} died");
        Destroy(gameObject);
    }

    public void Damage(uint amount)
    {
        Health -= amount;
        print($"Player {PlayerNumber} has {Health} health");
    }
}
