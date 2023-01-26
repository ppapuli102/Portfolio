using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int currentHealth = 20;
    [SerializeField] int damage = 2;
    Text healthText;

    private void Start() 
    {
        healthText = GetComponent<Text>();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        healthText.text = currentHealth.ToString();
    }

    public void SubtractHealth()
    {
        currentHealth -= damage;
        UpdateDisplay();

        if (currentHealth <= 0)
        {
            FindObjectOfType<LevelController>().HandleLoseCondition();
        }
    }
}
