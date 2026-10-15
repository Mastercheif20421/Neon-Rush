using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    public void Launch(Vector2 dir)
    {
        transform.right = dir;

        GetComponent<Rigidbody2D>().linearVelocity = dir * speed;
        Destroy(gameObject, lifetime); // removes if hits nothing
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return; // don't hit yourself
        Destroy(gameObject);
    }
}