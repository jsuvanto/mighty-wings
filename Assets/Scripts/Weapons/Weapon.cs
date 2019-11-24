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
        print($"Player {transform.parent.gameObject.GetComponent<PlayerController>().PlayerNumber} firing.");
    }

    public virtual void Reload()
    {
        print($"Player {transform.parent.gameObject.GetComponent<PlayerController>().PlayerNumber} reloading.");
    }

}

public enum WeaponType
{
    Projectile, Beam, Missile
}