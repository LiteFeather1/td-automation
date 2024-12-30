using System;
using UnityEngine;

public class ParticleStoppedCallback : MonoBehaviour
{
    public Action<ParticleStoppedCallback> OnStopped;

    private void OnParticleSystemStopped()
    {
        OnStopped?.Invoke(this);
    }
}
