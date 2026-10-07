using UnityEngine;

public class WeaponPicker : MonoBehaviour
{
    public bool CanUse, isInWeaponSpot;
    [Header("Empty Hand Settings")]
    [SerializeField] bool isHandEmpty = false;
    [SerializeField] Transform twoIkRight_Target, twoIkLeft_Target;
    [SerializeField] Transform twoIkRight_Hint, twoIkLeft_Hint;
    [SerializeField] Transform emptyHandPoint_Left, emptyHandPoint_Right;
    [SerializeField] Vector3 emptyHandRight_Rot, emptyHandLeft_Rot;
    [SerializeField] Vector3 emptyHandRight_HintPos, emptyHandLeft_HintPos;
    [SerializeField] Transform emptyHand;
    [SerializeField] float emptyHandRealignerSmoothness = 5;
    [SerializeField] Vector3 emptyHandMargin;

    [Header("Gun Settings")]
    [SerializeField] Transform InHand;
    [SerializeField] LayerMask weaponSpotLayer;
    [SerializeField] float weaponSpotAccessRange = 2f;
    [SerializeField] Guns ActiveGun;
    void Update()
    {
        if (!CanUse) { return; }
        if (Input.GetKeyDown(KeyCode.E) && !isInWeaponSpot)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, weaponSpotAccessRange, weaponSpotLayer))
            {
                if (ActiveGun != null)
                {
                    ActiveGun.enabled = false;
                }
                WeaponSpots spot = hit.collider.GetComponent<WeaponSpots>();
                spot.EnableWeaponSpot();
            }
        }
    }

    public void PickWeapon(Transform weapon)
    {
        if (ActiveGun != null) { Destroy(ActiveGun.gameObject); }
        Transform InstWeapon = Instantiate(weapon, InHand);
        Guns getGunS = InstWeapon.GetComponent<Guns>();
        if (getGunS != null)
        {
            ActiveGun = getGunS;
            getGunS.PickUpGun();
        }
    }
    public void LeaveSpot()
    {
        if (ActiveGun != null)
        {
            ActiveGun.enabled = true;
        }
        isInWeaponSpot = false;
    }
    void LateUpdate()
    {
        if (!CanUse) { return; }
        EmptyHand();
    }

    private void EmptyHand()
    {
        if (!isInWeaponSpot && isHandEmpty)
        {
            emptyHand.position = Vector3.Lerp(emptyHand.position, Camera.main.transform.TransformPoint(emptyHandMargin), emptyHandRealignerSmoothness * Time.deltaTime);
            emptyHand.rotation = Quaternion.Lerp(emptyHand.rotation, Camera.main.transform.rotation, emptyHandRealignerSmoothness * Time.deltaTime);

            twoIkLeft_Target.position = emptyHandPoint_Left.position;
            twoIkLeft_Target.rotation = emptyHandPoint_Left.rotation * Quaternion.Euler(emptyHandLeft_Rot);

            twoIkRight_Target.position = emptyHandPoint_Right.position;
            twoIkRight_Target.rotation = emptyHandPoint_Right.rotation * Quaternion.Euler(emptyHandRight_Rot);

            twoIkLeft_Hint.localPosition = emptyHandLeft_HintPos;
            twoIkRight_Hint.localPosition = emptyHandRight_HintPos;
        }
    }
}
