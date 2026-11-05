using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject gunObject;     // the glock
    [SerializeField] private GameObject rayGunObject;  // the raygun
    [SerializeField] private Gunscript gunScript;
    [SerializeField] private Ultbeam ultimate;

    private bool rayGunEquipped = false;

    void Start()
    {
        Equip(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(false);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Equip(true);
    }

    private void Equip(bool rayGun)
    {
        rayGunEquipped = rayGun;

        gunObject.SetActive(!rayGun);
        rayGunObject.SetActive(rayGun);

        gunScript.canShoot = !rayGun;
        ultimate.canFire = rayGun;
    }
}