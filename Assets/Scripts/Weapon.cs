using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string Name;
    public WeaponType Type;
    public int Damage;
    public GameObject Ammunition;
    public Vector2[] AmmunitionSpawnPoints;
    
    [Tooltip("Time to reload the magazine, in seconds")]
    public float ReloadTime;
    
    [Tooltip("0 for infinite")]
    public int MagazineSize;
    
    [Tooltip("Time between shots in seconds")]
    public float FireRate;
    
    [Tooltip("Initial force of ammunition, affects recoil")]
    public float Force;

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