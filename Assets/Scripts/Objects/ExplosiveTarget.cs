using System;
using System.Collections;
using UnityEngine;

public class ExplosiveTarget : MonoBehaviour
{
    [SerializeField] AudioSource sfx;
    [SerializeField] ParticleSystem vfx;
    [SerializeField, Header("Enabled if its Delayed Explosion"), Tooltip("Vfx plays after delay if this enabled")] bool isProgressive;
    [SerializeField] float explodeAfter;
    [SerializeField] bool triggered;
    public void Explode()
    {
        if (triggered) { return; }
        triggered = true;
        if (!isProgressive)
        {
            Explosion();
        }
        else
        {
            StartCoroutine(DelayedExplosion());
        }
    }

    void Explosion()
    {
        sfx.Play();
        vfx.Play();
    }

    IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(explodeAfter);
        Explosion();
    }
}
