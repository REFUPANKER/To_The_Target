using System;
using System.Collections;
using UnityEngine;

public class ExplosiveTarget : MonoBehaviour
{
    [SerializeField] AudioSource sfx;
    [SerializeField] ParticleSystem vfx, progressionVfx;
    [SerializeField, Header("Enabled if its Delayed Explosion"), Tooltip("Vfx plays after delay if this enabled")] bool isProgressive;
    [SerializeField] float explodeAfter, explosionForce = 500, disappearAfter = 5;
    [SerializeField] bool triggered;
    [SerializeField] Transform flyAroundObjHolder;
    [SerializeField] BoxCollider parentCollider;

    [Header("Camera Shake")]
    [SerializeField] float shakeTime = 0.1f, shakeDensity = 1;
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
        CameraEffects.ready.EffectShake(shakeTime,shakeDensity);
        parentCollider.enabled = false;
        Rigidbody[] rbs = flyAroundObjHolder.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            Vector3 randomDir = UnityEngine.Random.insideUnitSphere;
            randomDir.y = Mathf.Abs(randomDir.y) + 1f;
            rb.AddForce(randomDir.normalized * explosionForce, ForceMode.Impulse);
        }

        StartCoroutine(disappearCountdown());
    }

    IEnumerator DelayedExplosion()
    {
        progressionVfx.Play();
        yield return new WaitForSeconds(explodeAfter);
        progressionVfx.Stop();
        Explosion();
    }
    IEnumerator disappearCountdown()
    {
        yield return new WaitForSeconds(disappearAfter);
        Destroy(gameObject);
    }
}
