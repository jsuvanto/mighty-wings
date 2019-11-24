using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonAmmunitionBehaviour : MonoBehaviour
{
    [Tooltip("Damage dealt to a player ship per round of ammunition")]
    public uint Damage;

    [Tooltip("Minimum impulse required to penetrate a player ship")]
    public float MinimumPenetrationImpulse;

    public GameObject PenetrationEffect;
    public GameObject WallCollisionEffect;
    public GameObject RicochetEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var impulse = collision.GetContact(0).normalImpulse; // TODO: replace with average, min, max or other characteristic value for impulse

            if (impulse > MinimumPenetrationImpulse)
            {
                collision.gameObject.GetComponent<PlayerController>().Damage(Damage);
                gameObject.SetActive(false);
                //Instantiate(PenetrationEffect, collision.transform);
                // TODO: sfx
            }
            else
            {
              //  Instantiate(RicochetEffect, collision.transform);
                // TODO: sfx
            }
        }
        else if (collision.gameObject.tag == "Cave")
        {            
            gameObject.SetActive(false);
            Debug.Log("hit cave");
         //   Instantiate(WallCollisionEffect, collision.transform);
            // TODO: sfx
        }
    }
}
