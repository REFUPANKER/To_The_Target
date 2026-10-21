using UnityEngine;

public class WeaponPicker : MonoBehaviour
{
    public bool CanUse, isInWeaponSpot;
    public InfoTextHUD infoTxt;
    [SerializeField] MenuManager mm;
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
    [SerializeField] string weaponSpotLayer, ammoRefillLayerMask, ShootingRangeExitDoor;
    [SerializeField] float accessRange = 5f;
    public Guns ActiveGun;
    void Update()
    {
        if (!CanUse) { return; }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, accessRange))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer(weaponSpotLayer) && !isInWeaponSpot)
            {
                if (Input.GetKeyDown(KeyCode.E) && !isInWeaponSpot)
                {
                    if (ActiveGun != null) { ActiveGun.enabled = false; }
                    WeaponSpots spot = hit.collider.GetComponent<WeaponSpots>();
                    spot.EnableWeaponSpot();
                }
                else
                {
                    infoTxt.Show("Press E to select weapon");
                }
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer(ammoRefillLayerMask) && ActiveGun != null && !ActiveGun.reloading)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ActiveGun.Refill();
                }
                else
                {
                    infoTxt.Show("Press E to refill");
                }
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer(ShootingRangeExitDoor))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CanUse = false;
                    if (ActiveGun != null) { Destroy(ActiveGun.gameObject); }
                    infoTxt.Hide();
                    mm.TurnBackFrom("Training");
                }
                else
                {
                    infoTxt.Show("Press E to Exit Shooting Range");
                }
            }
            else
            {
                infoTxt.Hide();
            }
        }
        else
        {
            infoTxt.Hide();
        }
    }

    public void PickWeapon(Guns weapon)
    {
        if (ActiveGun != null) { Destroy(ActiveGun.gameObject); }
        Guns InstWeapon = Instantiate(weapon, InHand);
        ActiveGun = InstWeapon;
        ActiveGun.PickUpGun();
    }
    public void LeaveSpot()
    {
        if (ActiveGun != null) { ActiveGun.enabled = true; }
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

    public void PauseResume(bool paused)
    {
        if (paused)
        {
            CanUse = false;
            if (ActiveGun != null)
            {
                ActiveGun.CanUse = false;
            }
        }
        else
        {
            CanUse = true;
            if (ActiveGun != null) { ActiveGun.CanUse = true; }
        }
    }
}
