using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private GameObject[] alsoHide; // e.g. the gun, if it is NOT a child of the player

    private int currentHealth;
    private bool isDead = false;

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

        currentHealth -= amount;
        Debug.Log(gameObject.name + " health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // stop physics so it can't move, fall or be hit
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (isPlayer)
        {
            // switch off controls
            PlayerMovement move = GetComponent<PlayerMovement>();
            if (move != null) move.enabled = false;

            Gunscript gunScript = GetComponentInChildren<Gunscript>();
            if (gunScript != null) gunScript.enabled = false;

            // make the player (and its children) invisible
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
            // stop the walking script, then remove the enemy
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