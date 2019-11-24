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
    public Weapon Weapon;

    public GameObject DeathEffect;

    private Rigidbody2D body;
    public Camera Camera;
    private Vector3 cameraOffset;
    private ParticleSystem rocketTrail;
    private float emissionRate;
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        cameraOffset = transform.position - Camera.transform.position;
        rocketTrail = GetComponent<ParticleSystem>();
        emissionRate = rocketTrail.emission.rateOverTimeMultiplier;
    }

    private void FixedUpdate()
    {
        // Steering
        float throttle = Mathf.Max(Input.GetAxis($"Player {PlayerNumber} Throttle"), 0);
        float steering = Input.GetAxis($"Player {PlayerNumber} Steering");
        body.AddForce(throttle * ThrottleForce * body.transform.up);
        body.transform.Rotate(steering * SteeringSpeed * Vector3.forward);

        var emission = rocketTrail.emission;
        emission.rateOverTimeMultiplier = throttle * emissionRate;


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
        Camera.transform.position = transform.position - cameraOffset;       
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
    }

    private void Die()
    {
        print($"Player {PlayerNumber} died");
        Instantiate(DeathEffect, transform.position, new Quaternion());
        Destroy(gameObject);
    }

    public void Damage(uint amount)
    {
        Health -= amount;
        print($"Player {PlayerNumber} has {Health} health");
    }
}
