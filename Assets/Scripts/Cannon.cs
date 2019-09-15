using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Weapon
{
    public override void Fire()
    {
        base.Fire();
        print($"{nameof(Cannon)} firing");
    }

}
