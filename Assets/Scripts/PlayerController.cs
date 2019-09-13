using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public uint PlayerNumber;
    public uint ThrottleForce;
    public uint SteeringSpeed;
    public Camera CameraPrefab;

    private Rigidbody2D _body;
    private Camera _camera;
    private Vector3 _cameraOffset;

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _camera = Instantiate(CameraPrefab);
        _camera.transform.position = new Vector3(transform.position.x, transform.position.y ,0);
        
        _cameraOffset = transform.position - _camera.transform.position;
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

    void SetPlayerNumber(uint number)
    {
        PlayerNumber = number;  
    }

    void SetCamera(Camera camera)
    {
        _camera = camera;
    }

}
