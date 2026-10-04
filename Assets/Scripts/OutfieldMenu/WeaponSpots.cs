using Unity.Cinemachine;
using UnityEngine;

public class WeaponSpots : MonoBehaviour
{
    [SerializeField] WeaponPicker wp;
    [SerializeField] bool CanUse;
    [SerializeField] CinemachineCamera cam;
    [SerializeField] Transform[] weapons;
    [SerializeField] Vector3 zoomMargin;
    [SerializeField] int CurrentTargetIndex = 0;
    [SerializeField] float cameraSpeed = 10;
    bool makeZoom; Vector3 targetZoomPosition, defaultPosition;

    void Start()
    {
        defaultPosition = cam.transform.position;
        cam.gameObject.SetActive(false);
    }
    public void EnableWeaponSpot()
    {
        CurrentTargetIndex = 0;
        wp.isInWeaponSpot = true;
        CanUse = true;
        cam.gameObject.SetActive(true);
        cam.transform.position = defaultPosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ChangeWeapon(0);
    }
    void LeaveSpot()
    {
        wp.isInWeaponSpot = false;
        CanUse = true;
        cam.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!CanUse) { return; }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeWeapon(-1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            LeaveSpot();
        }

        if (makeZoom)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetZoomPosition + zoomMargin, cameraSpeed * Time.deltaTime);
            if (cam.transform.position == targetZoomPosition)
            {
                makeZoom = false;
            }
        }
    }

    public void ChangeWeapon(int direction)
    {
        CurrentTargetIndex += direction;

        CurrentTargetIndex = CurrentTargetIndex > weapons.Length - 1 ? 0 : (CurrentTargetIndex < 0 ? weapons.Length - 1 : CurrentTargetIndex);

        targetZoomPosition = weapons[CurrentTargetIndex].localPosition;
        makeZoom = true;
    }

}
