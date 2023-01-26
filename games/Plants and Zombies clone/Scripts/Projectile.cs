using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float rotationSpeed;
    static ProjectileVFX ProjectileVFX;
    
    public float damage = 20f; 

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = Random.Range(1f,5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * projectileSpeed * Time.deltaTime;
        GetComponent<Rigidbody2D>().rotation -= rotationSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        ProjectileVFX ProjectileVFX = GetComponent<ProjectileVFX>();

        if (other.tag == "Enemy")
        {
            ProjectileVFX.SpawnVFX();
            DestroyProjectile();
        }
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

}
