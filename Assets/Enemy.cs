using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private Transform edgeCheckLeft;
    [SerializeField] private List<Transform> edgeChecks;
     [SerializeField]private BoxCollider2D boxCollider;


void FixedUpdate()
{
    bool groundAhead = true;
    var vecRight = new Vector2(edgeCheck.position.x, edgeCheck.position.y);
    var vecLeft = new Vector2(edgeCheckLeft.position.x, edgeCheckLeft.position.y);
    var overlap = boxCollider.OverlapPoint(vecRight) || boxCollider.OverlapPoint(vecLeft);
    print(overlap);
    if (overlap)
        {
            Turn();
        }

        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y); 
}

private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x*= -1f;
        transform.localScale = scale;
    }
private void OnCollisionEnter2D(Collision2D collision)
{
    if (!collision.gameObject.CompareTag("Player")) return;

    Health playerHealth = collision.gameObject.GetComponentInParent<Health>();
    if (playerHealth != null)
    {
        playerHealth.TakeDamage(999); // enough to kill the player in one touch
    }
}

}