using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public uint ThrottleForce;
    public uint SteeringSpeed;
    public GameObject DeathEffect;

    [HideInInspector]
    public uint PlayerNumber;
    [HideInInspector]
    public uint Health;
    [HideInInspector]
    public uint Lives;
    [HideInInspector]
    public Weapon Weapon;
    [HideInInspector]
    public Camera Camera;
    [HideInInspector]
    public Canvas Hud;

    private Rigidbody2D body;
    private Vector3 cameraOffset;    

    [HideInInspector]
    public Text HealthText;
    [HideInInspector]
    public Text LivesText;
    [HideInInspector]
    public float TimeOfDeath;

    private AudioSource rocketSound;

    private ParticleSystem rocketTrail;
    private float emissionRate;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        cameraOffset = transform.position - Camera.transform.position;

        rocketTrail = GetComponent<ParticleSystem>();
        emissionRate = rocketTrail.emission.rateOverTimeMultiplier;
        rocketSound = GetComponent<AudioSource>();

        HealthText = Camera.GetComponentsInChildren<Text>()[0];
        LivesText = Camera.GetComponentsInChildren<Text>()[1];
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

        rocketSound.volume = throttle;

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
        Instantiate(DeathEffect, transform.position, new Quaternion());
        TimeOfDeath = Time.time;
        Lives -= 1;
        gameObject.SetActive(false);
    }

    public void Damage(uint amount)
    {
        Health -= amount;
    }

    public void Respawn(Vector3 location, uint health)
    {
        Health = health;
        transform.position = location;
        transform.rotation = new Quaternion();
        gameObject.SetActive(true);
        rocketTrail.Play();
    }
}
