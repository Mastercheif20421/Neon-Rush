using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;

    private Image[] hearts;

    void Start()
    {
        hearts = GetComponentsInChildren<Image>(true);
    }

    void Update()
    {
        if (playerHealth == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < playerHealth.CurrentHealth;
        }
    }
}