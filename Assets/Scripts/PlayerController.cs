﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public uint PlayerNumber;
    public uint ThrottleForce;
    public uint SteeringSpeed;
    public Camera CameraPrefab = new Camera();

    private Rigidbody2D _body;
    private Camera _camera;
    private Vector3 _cameraOffset;

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float throttle = Mathf.Max(Input.GetAxis($"Player {PlayerNumber} Throttle"), 0);
        float steering = Input.GetAxis($"Player {PlayerNumber} Steering");

        _body.AddForce(throttle * ThrottleForce * _body.transform.up);

        _body.transform.Rotate(steering * SteeringSpeed * Vector3.forward);
    }

    private void LateUpdate()
    {
        _camera.transform.position = transform.position - _cameraOffset;
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
        _camera = Instantiate(CameraPrefab, new Vector3(transform.position.x, transform.position.y, 0), new Quaternion());
        _cameraOffset = transform.position - _camera.transform.position;
        _camera.rect = new Rect(x, y, 0.5f, 0.5f);
        _camera.name = $"Player {playerNumber} camera";
    }
}
