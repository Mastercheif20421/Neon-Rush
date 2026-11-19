using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private GameObject[] alsoHide;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer flashSprite;
    [SerializeField] private float flashTime = 0.1f;
    [SerializeField] private float invincibleTime = 1f;   // player only
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    private int currentHealth;
    private bool isDead = false;
    private float invincibleUntil = 0f;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private static HashSet<string> deadEnemies = new HashSet<string>();

    void Awake()
    {
        currentHealth = maxHealth;

        if (!isPlayer && deadEnemies.Contains(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isPlayer && isDead && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (isPlayer && Time.time < invincibleUntil) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (isPlayer) invincibleUntil = Time.time + invincibleTime;
        PlaySound(hurtClip);
        if (flashSprite != null) StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        Color original = flashSprite.color;
        flashSprite.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        flashSprite.color = original;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || Camera.main == null) return;
        // plays at the camera so it is full volume, and survives this object being destroyed
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    private void Die()
    {
        isDead = true;
        PlaySound(deathClip);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (isPlayer)
        {
            PlayerMovement move = GetComponent<PlayerMovement>();
            if (move != null) move.enabled = false;

            Gunscript gunScript = GetComponentInChildren<Gunscript>();
            if (gunScript != null) gunScript.enabled = false;

            Ultbeam ult = GetComponentInChildren<Ultbeam>();
            if (ult != null) ult.enabled = false;

            foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            {
                r.enabled = false;
            }
            foreach (GameObject obj in alsoHide)
            {
                if (obj != null) obj.SetActive(false);
            }

            Debug.Log("You died. Press R to restart.");
        }
        else
        {
            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null) enemy.enabled = false;

            deadEnemies.Add(gameObject.name);
            Destroy(gameObject);
        }
    }

    [ContextMenu("Take 1 Damage")]
    private void TestDamage()
    {
        TakeDamage(1);
    }
}