using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Weapon
{

    [Tooltip("Time to reload the magazine, in seconds")]
    public float ReloadTime;

    [Tooltip("0 for infinite")]
    public int MagazineSize;

    [Tooltip("Time between shots in seconds")]
    public float FireRate;

    [Tooltip("Initial force of ammunition, affects recoil")]
    public float Force;

    public override void Fire()
    {
        
        base.Fire();

        if (Time.time > lastFired + FireRate)
        {
            lastFired = Time.time;
            var direction = transform.up;

            var bullet = Instantiate(Ammunition); // TODO replace with object pooling
            bullet.transform.position = transform.position;


            bullet.GetComponent<Rigidbody2D>().velocity = gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity;
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * Force);

            //Destroy(bulletClone, 3.0f); // TODO replace with object pooling

            Recoil();
        }
    }

    public void Recoil()
    {
        GetComponent<Rigidbody2D>().AddForce(-transform.up * Force);
    }
}
