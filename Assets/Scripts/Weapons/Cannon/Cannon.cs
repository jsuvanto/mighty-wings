using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Weapon
{
    [Tooltip("Time between shots in seconds")]
    public float TimeBetweenShots;

    [Tooltip("Initial force of ammunition, affects recoil")]
    public float Force;

    public int AmmunitionPoolSize;

    private GameObjectPool ammoPool;

    private void Start()
    {
        ammoPool = new GameObjectPool(Ammunition, AmmunitionPoolSize);
    }

    public override void Fire()
    {
        
        base.Fire();

        if (Time.time > lastFired + TimeBetweenShots)
        {
            lastFired = Time.time;
            var direction = transform.up;

            var bullet = ammoPool.GetPooledObject();
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.GetComponent<Rigidbody2D>().velocity = gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity;
                bullet.GetComponent<Rigidbody2D>().AddForce(direction * Force);
            }

            Recoil();
        }
    }

    public void Recoil()
    {
        GetComponent<Rigidbody2D>().AddForce(-transform.up * Force);
    }
}
