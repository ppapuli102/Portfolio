using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int scoreValue;
    [SerializeField] float health;

    [Header("Shooting")]
    [SerializeField] GameObject projectile;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 10f;

    [Header("Death VFX")]
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float durationOfExplosion = .02f;
    
    [Header("SFX")]
    [SerializeField] AudioClip[] laserSFX;
    [SerializeField] [Range(0,1)] float laserSFXVolume = 0.8f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.8f;

    //cached variables
    AudioSource myAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        ResetShotCounter();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
        }
    }

    private void Fire()
    {
        GameObject laserObject = Instantiate(projectile, gameObject.transform.position, Quaternion.identity) as GameObject;
        laserObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1*projectileSpeed);
        AudioClip clip = laserSFX[UnityEngine.Random.Range(0, laserSFX.Length)];
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, laserSFXVolume);
        ResetShotCounter();
    }

    private void ResetShotCounter()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        Destroy(gameObject);
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        GameObject explosionClone = Instantiate(explosionVFX, transform.position, transform.rotation);
        Destroy(explosionClone, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        
    }
}
