using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.10f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    private Vector3 basePos;
    private Vector3 velocity;
    private float shakeTimer, shakeDuration, shakeMagnitude;

    void Start()
    {
        basePos = transform.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desired = target.position + offset;
            basePos = Vector3.SmoothDamp(basePos, desired, ref velocity, smoothTime);
        }

        Vector3 shake = Vector3.zero;
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float fade = shakeTimer / shakeDuration;
            shake = (Vector3)(Random.insideUnitCircle * shakeMagnitude * fade);
        }

        transform.position = basePos + shake;
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}