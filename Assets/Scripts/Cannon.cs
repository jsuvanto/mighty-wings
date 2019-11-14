using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Weapon
{

    public override void Fire()
    {
        
        base.Fire();

        if (Time.time > lastFired + FireRate)
        {
            lastFired = Time.time;
            var direction = transform.up;




            var bullet = Instantiate(Ammunition);
            bullet.transform.position = transform.position;


            bullet.GetComponent<Rigidbody2D>().velocity = gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity;
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * Force);

            //Destroy(bulletClone, 3.0f); // TODO replace with object pooling
        }
    }
}
