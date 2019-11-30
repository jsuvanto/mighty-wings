using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFinished : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        Destroy(transform.parent.gameObject);
    }
}
