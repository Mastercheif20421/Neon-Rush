using UnityEngine;
using UnityEngine.InputSystem;

public class playerscript : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody2D rb;
    Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var k = Keyboard.current;
        if (k == null) return; // no keyboard connected

        float x = 0f;
        float y = 0f;

        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.wKey.isPressed || k.upArrowKey.isPressed) y += 1f;
        if (k.sKey.isPressed || k.downArrowKey.isPressed) y -= 1f;

        input = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}