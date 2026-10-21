using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Guns : MonoBehaviour
{

    [Header("Properties")]
    public string WeaponName;

    [Header("InGame Requirements")]
    public bool CanUse;
    [SerializeField] CinemachineCamera fpsCam;
    [SerializeField] Transform twoIkRight_Target, twoIkLeft_Target;
    [SerializeField] Transform twoIkRight_Hint, twoIkLeft_Hint;
    [SerializeField] Transform gunHoldingPoint_Left, gunHoldingPoint_Right;

    [SerializeField] Vector3 twoIkRight_Rot, twoIkLeft_Rot;
    [SerializeField] Vector3 twoIkRight_HintPos, twoIkLeft_HintPos;

    [Header("Aiming")]
    public bool isSniper;
    [Tooltip("Automaticially set by settings manager")] public float aimingSens;
    [SerializeField] protected Transform NuzzlePoint;
    [SerializeField] protected Vector3 GunMarginToCamera, GunAimingMarginToCamera;
    protected Vector3 currentGunMargin;
    [SerializeField] protected float gunRealignerSmoothness = 5, gunRealignerAimingSmoothness = 10;
    protected float defaultGunRealignerSmoothness;
    public bool aiming;

    [SerializeField] bool CanFire = true, isAutomatic;
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] Animation anims;
    [SerializeField] AudioClip fireSfx, reloadSfx;
    [SerializeField] TextMeshProUGUI bulletCounter;
    [SerializeField, Tooltip("used for calling animations")] string gunName;
    [SerializeField] int magStack, magIn;
    int defaultMagStack, defaultMagCapacity;
    [SerializeField] protected float maxRange;
    public bool reloading;

    [Header("Bullets")]
    [SerializeField] GunsBullet bullet;
    [SerializeField] Transform bulletEjectPoint;

    [Header("Camera Shake")]
    [SerializeField] float shakeTime = 0.1f, shakeDensity = 1;

    public void PickUpGun()
    {
        SettingsManager.OnSettingsChanged += (GameSettings g) =>
        {
            aimingSens = isSniper ? g.sniperSens : g.aimingSens;
        };
        CanUse = true;
        RefreshBulletCount();
    }
    protected void Start()
    {
        aimingSens = isSniper ? SettingsManager.Instance.CurrentSettings.sniperSens : SettingsManager.Instance.WorkingSettings.aimingSens;
        bullet.gameObject.SetActive(false);
        defaultGunRealignerSmoothness = gunRealignerSmoothness;
        currentGunMargin = GunMarginToCamera;
        defaultMagStack = magStack;
        defaultMagCapacity = magIn;
        RefreshBulletCount();
    }
    public void RefreshBulletCount()
    {
        CanFire = magStack + magIn > 0;
        bulletCounter.text = magStack + "/" + magIn;
    }

    protected virtual void ReloadTriggered() { AudioSource.PlayClipAtPoint(reloadSfx, transform.position); }
    public void RewindAnimations() { anims.Rewind(); anims.Sample(); anims.Stop(); }

    public void FireBullet(Vector3 target, RaycastHit? hit)
    {
        GunsBullet nb = Instantiate(bullet, null);
        nb.gameObject.SetActive(true);
        nb.transform.position = NuzzlePoint.position;
        nb.transform.rotation = transform.rotation;
        nb.Fire(target, bulletEjectPoint.position, hit);
        CameraEffects.ready.EffectShake(shakeTime, shakeDensity);
    }
    /// <summary>
    /// called by reload event
    /// </summary>
    protected virtual void Reloaded()
    {
        magStack = magStack - 1 < 0 ? 0 : magStack - 1;
        magIn = defaultMagCapacity;
        RefreshBulletCount();
        reloading = false;
        if (aiming)
        { // after reload if user still holding the button we gotta reloacte the gun
            currentGunMargin = GunAimingMarginToCamera; defaultGunRealignerSmoothness = gunRealignerAimingSmoothness;
        }
    }

    public void Refill()
    {
        reloading = true;
        ReloadTriggered();
        if (anims.isPlaying) { RewindAnimations(); }
        if (aiming) { currentGunMargin = GunMarginToCamera; defaultGunRealignerSmoothness = gunRealignerSmoothness; }
        magStack = defaultMagStack;
        magIn = defaultMagCapacity;
        anims.Play($"{gunName}_reload");
    }

    protected void Update()
    {
        if (!CanUse) { return; }

        if (!anims.isPlaying && !aiming && Input.GetKeyDown(KeyCode.F)) { anims.Play($"{gunName}_inspect"); }

        if (CanFire && !anims.IsPlaying($"{gunName}_fire") && !anims.IsPlaying($"{gunName}_reload") && magIn == 0 && magStack > 0)
        {
            // wait last fire animation to play and reload automaticially
            reloading = true;
            if (aiming)
            { // set to default margins because i dont want player watch reloading that close
                currentGunMargin = GunMarginToCamera; defaultGunRealignerSmoothness = gunRealignerSmoothness;
            }
            anims.Play($"{gunName}_reload");
            ReloadTriggered();
        }

        if (aiming && anims.IsPlaying($"{gunName}_inspect")) { RewindAnimations(); }

        bool firingType = isAutomatic ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);
        if (CanFire && firingType && !anims.IsPlaying($"{gunName}_fire") && !reloading && magIn + magStack > 0)
        {
            if (anims.IsPlaying($"{gunName}_inspect")) { anims.Stop($"{gunName}_inspect"); }
            AudioSource.PlayClipAtPoint(fireSfx, transform.position);
            anims.Play($"{gunName}_fire");
            fireParticles.Play();
            magIn = magIn - 1 < 0 ? 0 : magIn - 1;
            RefreshBulletCount();

            RaycastHit hit;
            if (Physics.Raycast(NuzzlePoint.position, NuzzlePoint.forward, out hit, maxRange))
            {
                Debug.DrawLine(NuzzlePoint.position, hit.point, Color.green, 0.5f);
                FireBullet(hit.point, hit);
            }
            else
            {
                FireBullet(NuzzlePoint.position + NuzzlePoint.forward * maxRange, null);
            }
        }

        HandsAndAiming();
    }

    void HandsAndAiming()
    {
        transform.position = Vector3.Lerp(transform.position, fpsCam.transform.TransformPoint(currentGunMargin), defaultGunRealignerSmoothness * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, fpsCam.transform.rotation, defaultGunRealignerSmoothness * Time.deltaTime);

        twoIkLeft_Target.position = gunHoldingPoint_Left.position;
        twoIkRight_Target.position = gunHoldingPoint_Right.position;
        twoIkLeft_Target.rotation = transform.rotation * Quaternion.Euler(twoIkLeft_Rot);
        twoIkRight_Target.rotation = transform.rotation * Quaternion.Euler(twoIkRight_Rot);
        twoIkLeft_Hint.localPosition = twoIkLeft_HintPos;
        twoIkRight_Hint.localPosition = twoIkRight_HintPos;

        if (!reloading && !aiming && Input.GetKeyDown(KeyCode.Mouse1))
        {
            aiming = true;
            currentGunMargin = GunAimingMarginToCamera;
            defaultGunRealignerSmoothness = gunRealignerAimingSmoothness;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            aiming = false;
            currentGunMargin = GunMarginToCamera;
            defaultGunRealignerSmoothness = gunRealignerSmoothness;
        }
    }
}