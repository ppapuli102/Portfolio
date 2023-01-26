using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float rotationSpeed;
    public float damage = 40f; 

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = -Random.Range(1f,5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * projectileSpeed * Time.deltaTime;
        GetComponent<Rigidbody2D>().rotation += rotationSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Enemy")
        {
            //SpawnVFX();
            Destroy(gameObject, .5f);
        }
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    private void SpawnVFX()
    {
        
    }
}
