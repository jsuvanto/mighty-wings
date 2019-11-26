using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string Name;
    public WeaponType Type;
    public GameObject Ammunition;

    protected float lastFired;

    private void Start()
    {
        lastFired = Time.time;
    }

    public virtual void Fire()
    {
    }

    public virtual void Reload()
    {
    }

}

public enum WeaponType
{
    Projectile, Beam, Missile
}