using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public uint PlayerNumber;
    public uint ThrottleForce;
    public uint SteeringSpeed;
    public Camera CameraPrefab;

    private Rigidbody2D body;
    private new Camera camera;
    private Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        camera = Instantiate(CameraPrefab, new Vector3(0,0,0), new Quaternion());
        camera.transform.position = new Vector3(0,0,0);
        
        cameraOffset = transform.position - camera.transform.position;
        print(cameraOffset);
    }

    // Update is called once per frame
    void Update()
    {
        float throttle = Input.GetAxis($"Player {PlayerNumber} Throttle") * ThrottleForce;
        float steering = Input.GetAxis($"Player {PlayerNumber} Steering") * SteeringSpeed;

        body.AddForce(throttle * body.transform.up);

        transform.Rotate(Vector3.forward * steering);

        camera.transform.position = transform.position - cameraOffset;
    }

    void SetPlayerNumber(uint number)
    {
        PlayerNumber = number;  
    }

    void SetCamera(Camera camera)
    {
        CameraPrefab = camera;
    }

}
