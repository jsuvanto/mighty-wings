using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string Name;
    public WeaponType Type;
    public int Damage;
    public GameObject Ammunition;
    public int ReloadTime;
    public int MagazineSize;
    public int FireRate;


    public Weapon()
    {

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