using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    private Rigidbody2D rb;

    public void Launch(float direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction * speed, 0f);
        Destroy(gameObject, lifetime); // clean up if it hits nothing
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return; // don't hit yourself
        Destroy(gameObject);
    }
}