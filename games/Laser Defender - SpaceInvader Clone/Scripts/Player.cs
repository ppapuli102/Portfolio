using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // configuration parameters
    [Header("Player")]
    [SerializeField] float playerSpeed = 10f;
    [SerializeField] float padding = 1f;
    static public int health = 300;
    private HealthTracker[] healthUI;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float firingFrequency = 0.1f;

    [Header("Sound Effects")]
    [SerializeField] AudioClip[] laserSFX;
    [SerializeField] [Range(0,1)] float laserSFXVolume = 0.8f;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.8f;

    Coroutine firingCoroutine;

    float xMin; float xMax; float yMin; float yMax;

    //cached variables
    AudioSource myAudioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    private IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(
                laserPrefab, 
                transform.position, 
                Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(laserSFX[0], Camera.main.transform.position, laserSFXVolume);
            yield return new WaitForSeconds(firingFrequency);
        }
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1,0,0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0,1,0)).y - padding;
    }

    private void Move()
    {
        float dX = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        float dY = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;

        float newXPos = Mathf.Clamp(transform.position.x + dX, xMin, xMax);        
        float newYPos = Mathf.Clamp(transform.position.y + dY, yMin, yMax);
        
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
        {
            DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
            if (!damageDealer) { return; }
            ProcessHit(damageDealer);
        }

    private void ProcessHit(DamageDealer damageDealer)
    {
        StartCoroutine("DamagedColor");
        health -= damageDealer.GetDamage();
        updateHealthUI(damageDealer.GetDamage());
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
            health = 300;
        }
    }

    private IEnumerator DamagedColor()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(.1f);
        GetComponent<SpriteRenderer>().color = Color.red;     
        yield return new WaitForSeconds(.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void updateHealthUI(int damage)
    {
        HealthTracker healthUI = FindObjectOfType<HealthTracker>();
        healthUI.reduceHealthUI(damage);

    }

    private void Die()
    {
        FindObjectOfType<LevelController>().LoadGameOver();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
    }
}

