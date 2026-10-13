using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Transform gun;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - gun.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0f, 0f, angle);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        gunAnim.SetTrigger("Shoot");
    }
}