using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class Guns : MonoBehaviour
{
    [SerializeField] Movement player;
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

    [SerializeField] bool CanFire = true;
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] Animation anims;
    [SerializeField] AudioClip fireSfx;
    [SerializeField] TextMeshProUGUI bulletCounter;
    [SerializeField] string gunName;
    [SerializeField] int magStack, magIn;
    int defaultMagCapacity;
    [SerializeField] float maxRange;
    bool reloading;

    void Start()
    {
        defaultGunRealignerSmoothness = gunRealignerSmoothness;
        currentGunMargin = GunMarginToCamera;
        defaultMagCapacity = magIn;
        RefreshBulletCount();
    }
    void RefreshBulletCount()
    {
        if (magStack + magIn <= 0) { CanFire = false; }
        bulletCounter.text = magStack + "/" + magIn;
    }
    void Reloaded()
    {
        magStack = magStack - 1 < 0 ? 0 : magStack - 1;
        magIn = defaultMagCapacity;
        RefreshBulletCount();
        reloading = false;
    }
    void Update()
    {
        if (!player.CanMove) { return; }

        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.TransformPoint(currentGunMargin), defaultGunRealignerSmoothness * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Camera.main.transform.rotation, defaultGunRealignerSmoothness * Time.deltaTime);

        if (CanFire && Input.GetKeyDown(KeyCode.Mouse0) && !anims.IsPlaying($"{gunName}_fire") && !reloading)
        {
            anims.Play($"{gunName}_fire");
            fireParticles.Play();
            magIn = magIn - 1 < 0 ? 0 : magIn - 1;
            if (magIn == 0 && magStack > 0)
            {
                reloading = true;
                anims.Play($"{gunName}_reload");
            }
            RefreshBulletCount();

            RaycastHit hit;
            if (Physics.Raycast(NuzzlePoint.position, NuzzlePoint.forward, out hit, maxRange))
            {
                AudioSource.PlayClipAtPoint(fireSfx, transform.position);
                Debug.DrawLine(NuzzlePoint.position, hit.point, Color.green, 0.5f);
                if (hit.transform.gameObject.tag == "ScoreTarget")
                {
                    ScoreTarget st = hit.transform.GetComponent<ScoreTarget>();
                    st.Fall(hit.point);
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

        if (!aiming && Input.GetKeyDown(KeyCode.Mouse1))
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