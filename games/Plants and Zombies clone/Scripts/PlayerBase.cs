using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Start() {
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (other.tag == "Enemy")
        {
            playerHealth.SubtractHealth();
        }
    }

}
