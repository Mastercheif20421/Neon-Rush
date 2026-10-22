using UnityEngine;

public class Gunscript : MonoBehaviour
{
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Transform gun; //object that move and rotate    
    [SerializeField] private Transform pivot; //center where i rotates around
    [SerializeField] private float distance = 1f; //distance from player
    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firepoint; 
    [SerializeField] private float fireCooldown = 0.1f; //seconds between shots
    private float nextFireTime; 

    private Vector2 AimDirection = Vector2.right; 

    void Update()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 start = pivot.position;

        Vector2 v = target - start;       
        float length = v.magnitude;       
        if (length < 0.01f) return;      

        AimDirection = v.normalized;       

        float a = Mathf.Atan2(v.y, v.x);  
        if (a < 0f) a += 2f * Mathf.PI;   

        float x = Mathf.Cos(a) * distance;
        float y = Mathf.Sin(a) * distance;

        gun.position = start + new Vector2(x, y);

        gun.rotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg);

        Vector3 scale = gun.localScale;
        scale.y = Mathf.Cos(a) < 0f ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        gun.localScale = scale;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            Shoot();
        }
    }

    public void Shoot()
    {
        if (gunAnim != null) gunAnim.SetTrigger("Shoot");

        GameObject bullet = Instantiate(bulletPrefab, firepoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Launch(AimDirection);
    }
}