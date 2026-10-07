using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class Guns : MonoBehaviour
{
    public bool CanUse;
    [SerializeField] Transform twoIkRight_Target, twoIkLeft_Target;
    [SerializeField] Transform twoIkRight_Hint, twoIkLeft_Hint;
    [SerializeField] Transform gunHoldingPoint_Left, gunHoldingPoint_Right;

    [SerializeField] Vector3 twoIkRight_Rot, twoIkLeft_Rot;
    [SerializeField] Vector3 twoIkRight_HintPos, twoIkLeft_HintPos;

    [SerializeField] Transform NuzzlePoint;
    [SerializeField] Vector3 GunMarginToCamera, GunAimingMarginToCamera;
    Vector3 currentGunMargin;
    [SerializeField] float gunRealignerSmoothness = 5, gunRealignerAimingSmoothness = 10;
    float defaultGunRealignerSmoothness;
    bool aiming;

    [SerializeField] bool CanFire = true, isAutomatic;
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] Animation anims;
    [SerializeField] AudioClip fireSfx;
    [SerializeField] TextMeshProUGUI bulletCounter;
    [SerializeField] string gunName;
    [SerializeField] int magStack, magIn;
    int defaultMagCapacity;
    [SerializeField] float maxRange;
    bool reloading;

    public void PickUpGun()
    {
        CanUse = true;
        RefreshBulletCount();
    }
    void Start()
    {
        defaultGunRealignerSmoothness = gunRealignerSmoothness;
        currentGunMargin = GunMarginToCamera;
        defaultMagCapacity = magIn;
    }
    void RefreshBulletCount()
    {
        if (magStack + magIn <= 0) { CanFire = false; }
        bulletCounter.text = magStack + "/" + magIn;
    }
    /// <summary>
    /// called by reload event
    /// </summary>
    void Reloaded()
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
    void Update()
    {
        if (!CanUse) { return; }

        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.TransformPoint(currentGunMargin), defaultGunRealignerSmoothness * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Camera.main.transform.rotation, defaultGunRealignerSmoothness * Time.deltaTime);

        if (!anims.isPlaying && !aiming && Input.GetKeyDown(KeyCode.F)) { anims.Play($"{gunName}_inspect"); }

        if (aiming && anims.IsPlaying($"{gunName}_inspect")) { anims.Rewind($"{gunName}_inspect"); anims.Sample(); anims.Stop(); }//TODO: fix : when inspecting its also aiming and its looking bad
        bool firingType = isAutomatic ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);
        if (CanFire && firingType && !anims.IsPlaying($"{gunName}_fire") && !reloading)
        {
            if (anims.IsPlaying($"{gunName}_inspect")) { anims.Stop($"{gunName}_inspect"); }
            AudioSource.PlayClipAtPoint(fireSfx, transform.position);
            anims.Play($"{gunName}_fire");
            fireParticles.Play();
            magIn = magIn - 1 < 0 ? 0 : magIn - 1;
            if (magIn == 0 && magStack > 0)
            {
                reloading = true;
                if (aiming)
                { // set to default margins because i dont want player watch reloading that close
                    currentGunMargin = GunMarginToCamera; defaultGunRealignerSmoothness = gunRealignerSmoothness;
                }
                anims.Play($"{gunName}_reload");
            }
            RefreshBulletCount();

            RaycastHit hit;
            if (Physics.Raycast(NuzzlePoint.position, NuzzlePoint.forward, out hit, maxRange))
            {
                Debug.DrawLine(NuzzlePoint.position, hit.point, Color.green, 0.5f);

                switch (hit.transform.gameObject.tag)
                {
                    case "ScoreTarget":
                        ScoreTarget st = hit.transform.GetComponent<ScoreTarget>();
                        st.Fall(hit.point);
                        break;
                    case "ExplosiveTarget":
                        ExplosiveTarget expT = hit.transform.GetComponent<ExplosiveTarget>();
                        expT.Explode();
                        break;
                }
            }
        }

        HandsAndAiming();
    }

    void HandsAndAiming()
    {
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