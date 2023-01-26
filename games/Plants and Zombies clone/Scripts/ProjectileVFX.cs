using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVFX : MonoBehaviour
{
    public List<ParticleSystem> projectileVFX = new List<ParticleSystem>();

    private void Start() {
        //Destroy(gameObject, 1f);
    }

    public void SpawnVFX()
    {
        if (projectileVFX.Count > 0)
        {
            foreach (ParticleSystem projectile in projectileVFX)
            {
                ParticleSystem Object = Instantiate(projectile, transform.position, transform.rotation);
            }
        }
    }

}
