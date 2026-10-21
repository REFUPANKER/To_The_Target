using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Use <see cref="ready"/> to reach ready to use instance of object <br/>
/// Dont forget to : <br/>
/// + Add CinemachineBasicMultiChannelPerlin to Fps Camera <br/>
/// + Set tag of primary fps camera to fpsCam <br/>
/// </summary>
public class CameraEffects : MonoBehaviour
{
    [SerializeField] CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] CinemachineCamera targetCam;
    public static CameraEffects ready { get; private set; }
    void Awake()
    {
        if (ready != null && ready != this)
        {
            Destroy(gameObject);
            return;
        }
        ready = this;
    }
    void Start()
    {
        GameObject tcam = GameObject.FindGameObjectWithTag("fpsCam");
        targetCam = tcam.GetComponent<CinemachineCamera>();
        noise = tcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void EffectShake(float time, float density = 1)
    {
        StopAllCoroutines(); // Önceki shake veya durdurma coroutine'lerini temizle
        StartCoroutine(SmoothShake(time, density));
    }

    private IEnumerator SmoothShake(float time, float density)
    {
        // Yumuşak giriş (Fade In)
        float elapsed = 0f;
        float fadeInTime = 0.1f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            noise.AmplitudeGain = Mathf.Lerp(0f, density, elapsed / fadeInTime);
            yield return null;
        }
        noise.AmplitudeGain = density;

        // Titreşim süresi
        yield return new WaitForSeconds(time);

        // Yumuşak çıkış (Fade Out / stopSoftness)
        float fadeOutTime = 0.2f;
        elapsed = 0f;
        float startDensity = noise.AmplitudeGain;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            noise.AmplitudeGain = Mathf.Lerp(startDensity, 0f, elapsed / fadeOutTime);
            yield return null;
        }
        noise.AmplitudeGain = 0f;
    }
}
