using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonAmmunitionBehaviour : MonoBehaviour
{
    [Tooltip("Damage dealt to a player ship per round of ammunition")]
    public uint Damage;

    [Tooltip("Minimum impulse required to penetrate a player ship")]
    public float MinimumPenetrationImpulse;

    private Vector2 velocity = new Vector2();

    void FixedUpdate()
    {
        velocity = GetComponent<Rigidbody2D>().velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var normal = collision.GetContact(0).normal;
            var impulse = collision.GetContact(0).normalImpulse;

            var result = "deflected";

            Debug.Log("velocity: " + velocity.ToString() + " normal : " + normal.ToString());

            var angleOfImpact = Vector2.Angle(velocity, -normal);

            if (impulse > MinimumPenetrationImpulse)
            {
                result = "penetration";
                collision.gameObject.GetComponent<PlayerController>().Damage(Damage);
                gameObject.SetActive(false);
                // TODO: add effect
            }
            Debug.Log("angle: " + angleOfImpact + " impulse: " + impulse + " " + result);
            
        }
        else if (collision.gameObject.tag == "Cave")
        {            
            gameObject.SetActive(false);
            Debug.Log("hit cave");
            // TODO: add effect
        }
    }
}
