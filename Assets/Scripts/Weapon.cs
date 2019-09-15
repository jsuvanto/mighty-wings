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

    }

    public virtual void Reload()
    {

    }

}

public enum WeaponType
{
    Projectile, Beam, Missile
}