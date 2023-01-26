using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caravan : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] [Range(0f, 100f)] private int maxHealth;
    [SerializeField] [Range(0f, 100f)] private int currentHealth;
    [SerializeField] private int visionRadius;
    
    private OverworldMovement owMovement;
    

    private void Start() {
        owMovement = GetComponent<OverworldMovement>();
        UpdateSightRange(visionRadius);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void ChangeHealth(int amount) {
        if (amount < 0) {
            currentHealth -= amount;
        }
        else if (amount > 0) {
            currentHealth += amount;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void UpdateSightRange(int newVisionRadius) {
        owMovement.sight_range = newVisionRadius;
    }

}
