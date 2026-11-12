using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Ultbeam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject beamObject;   // Laser02
    [SerializeField] private LineRenderer beamLine;   // the "Line" object under Laser02
    [SerializeField] private TMP_Text cooldownText;

    [Header("Ultimate")]
    [SerializeField] private KeyCode ultKey = KeyCode.Q;
    [SerializeField] private float cooldown = 60f;
    [SerializeField] private float beamLength = 40f;
    [SerializeField] private float beamThickness = 0.8f;
    [SerializeField] private int damage = 9999;       // one-shot kill

    [Header("Fade")]
    [SerializeField] private float holdTime = 0.15f;  // stays fully visible
    [SerializeField] private float fadeTime = 0.6f;   // then fades out
    [SerializeField] private ParticleSystem muzzleParticles;

    [Header("Charge")]
    [SerializeField] private float chargeTime = 0.6f;          // seconds of charge-up
    [SerializeField] private ParticleSystem chargeParticles;   // sparks that gather at the barrel
    [SerializeField] private ParticleSystem sparksParticles;

    [Header("Charge Ramp")]
    [SerializeField] private float rampPower = 2.5f;        // higher = slower start, harder finish
    [SerializeField] private float speedStart = 0.4f;       // ChargeSparks playback speed at start
    [SerializeField] private float speedEnd = 3f;           // ...and right before firing
    [SerializeField] private float rateStart = 10f;         // ChargeSparks per second at start
    [SerializeField] private float rateEnd = 150f;          // ...and at the end
    [SerializeField] private float burstStart = 5f;         // outward Sparks per second at start
    [SerializeField] private float burstEnd = 120f;         // ...and at the end  

    public bool canFire = false;

    private float nextReadyTime = 0f;
    private bool isFiring = false;

    // remembered so the beam can go back to the gun afterwards
    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;
    private float originalWidth;
    private Color originalStartColor;
    private Color originalEndColor;

    void Start()
    {
        originalParent = beamObject.transform.parent;
        originalLocalPos = beamObject.transform.localPosition;
        originalLocalRot = beamObject.transform.localRotation;
        originalLocalScale = beamObject.transform.localScale;
        originalWidth = beamLine.widthMultiplier;
        originalStartColor = beamLine.startColor;
        originalEndColor = beamLine.endColor;

        beamObject.SetActive(false);
    }

    void Update()
    {
        if (canFire && !isFiring && Time.time >= nextReadyTime && Input.GetKeyDown(ultKey))
        {
            nextReadyTime = Time.time + cooldown;
            StartCoroutine(Fire());
        }

        UpdateUI();
    }

    IEnumerator Fire()
    {
        if (chargeParticles != null) chargeParticles.Play();

    float elapsed = 0f;
    float sparkAccumulator = 0f;

    while (elapsed < chargeTime)
    {
        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / chargeTime);
        float ramp = Mathf.Pow(progress, rampPower);   // 0 -> 1, slow at first, fast at the end

    
        if (chargeParticles != null)
        {
            var main = chargeParticles.main;
            main.simulationSpeed = Mathf.Lerp(speedStart, speedEnd, ramp);

            var emission = chargeParticles.emission;
            emission.rateOverTime = Mathf.Lerp(rateStart, rateEnd, ramp);
        }

    
        if (sparksParticles != null)
        {
            sparkAccumulator += Mathf.Lerp(burstStart, burstEnd, ramp) * Time.deltaTime;
            int count = Mathf.FloorToInt(sparkAccumulator);
            if (count > 0)
            {
                sparksParticles.Emit(count);
                sparkAccumulator -= count;
            }
        }

        yield return null;
    }

if (chargeParticles != null)
{
    chargeParticles.Stop();
    var main = chargeParticles.main;
    main.simulationSpeed = 1f;   // reset for next time
}

if (muzzleParticles != null) muzzleParticles.Play();

        // 1. remember exactly where and which way we fired
        Vector2 origin = firePoint.position;
        Vector2 direction = firePoint.right;
        float angle = firePoint.eulerAngles.z;

        // 2. kill everything in the beam, once
        DealDamage(origin, direction, angle);

        // 3. show the beam, then detach it so it stays in this spot
        beamObject.SetActive(true);
        beamObject.transform.SetParent(null, true); // true = keep world position

        // 4. stay fully visible for a moment
        yield return new WaitForSeconds(holdTime);

        // 5. fade out
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = 1f - Mathf.Clamp01(t / fadeTime); // 1 -> 0

            beamLine.widthMultiplier = originalWidth * k;
            beamLine.startColor = new Color(originalStartColor.r, originalStartColor.g, originalStartColor.b, originalStartColor.a * k);
            beamLine.endColor = new Color(originalEndColor.r, originalEndColor.g, originalEndColor.b, originalEndColor.a * k);

            yield return null;
        }

        // 6. hide it and put it back on the gun for next time
        beamObject.SetActive(false);
        beamObject.transform.SetParent(originalParent, false);
        beamObject.transform.localPosition = originalLocalPos;
        beamObject.transform.localRotation = originalLocalRot;
        beamObject.transform.localScale = originalLocalScale;

        beamLine.widthMultiplier = originalWidth;
        beamLine.startColor = originalStartColor;
        beamLine.endColor = originalEndColor;

        isFiring = false;
    }

    void DealDamage(Vector2 origin, Vector2 direction, float angle)
    {
        Vector2 center = origin + direction * (beamLength / 2f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            new Vector2(beamLength, beamThickness),
            angle);

        HashSet<Health> alreadyHit = new HashSet<Health>();

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) continue;

            Health health = hit.GetComponentInParent<Health>();
            if (health != null && alreadyHit.Add(health))
            {
                health.TakeDamage(damage);
            }
        }
    }

    void UpdateUI()
    {
        if (cooldownText == null) return;

        float remaining = nextReadyTime - Time.time;
        if (remaining <= 0f)
        {
            cooldownText.text = "ULT READY [Q]";
            cooldownText.color = Color.green;
        }
        else
        {
            cooldownText.text = "ULT " + Mathf.CeilToInt(remaining) + "s";
            cooldownText.color = Color.gray;
        }
    }
}