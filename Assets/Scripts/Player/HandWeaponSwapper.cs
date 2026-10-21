using System.Collections.Generic;
using UnityEngine;

public class HandWeaponSwapper : MonoBehaviour
{
    [SerializeField] List<Guns> weapons;
    [SerializeField] Guns activeGun;
    [SerializeField] int weaponIndex = 0;
    [SerializeField] Movement m;
    void Start()
    {
        foreach (var item in transform.GetComponentsInChildren<Guns>())
        {
            weapons.Add(item);
            item.gameObject.SetActive(false);
        }
        SwitchTo(0);
    }

    void Update()
    {
        if (!m.CanMove || Input.GetKey(KeyCode.Mouse1)) { return; }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) { UpdateIndex(scroll > 0 ? 1 : -1); }
    }

    void UpdateIndex(int direction)
    {
        weaponIndex += direction;
        if (weaponIndex >= weapons.Count)
        {
            weaponIndex = 0;
        }
        if (weaponIndex < 0)
        {
            weaponIndex = weapons.Count - 1;
        }
        SwitchTo(weaponIndex);
    }

    void SwitchTo(int index)
    {
        if (activeGun != null)
        {
            activeGun.RewindAnimations();
            activeGun.gameObject.SetActive(false);
        }
        weapons[index].gameObject.SetActive(true);
        activeGun = weapons[weaponIndex];
        m.ActiveGun = activeGun;
        activeGun.RefreshBulletCount();
    }
}
