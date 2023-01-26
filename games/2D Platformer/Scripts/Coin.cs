using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioClip coinPickUpSFX;
    [SerializeField] int scoreAmount = 250;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(coinPickUpSFX, Camera.main.transform.position);
            FindObjectOfType<GameSession>().AddScore(scoreAmount);
            Destroy(gameObject);
        }
    }
}
