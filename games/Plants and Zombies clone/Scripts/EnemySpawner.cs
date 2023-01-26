using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public bool spawn_bool = false;
    [SerializeField] Enemy[] enemies;
    [SerializeField] [Range(1f, 5f)] float spawnTimeRandomness = 10f;
    private float minSpawnDelay = 2f;

    IEnumerator Start()
        {
            while (spawn_bool)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnDelay, spawnTimeRandomness));
                SpawnEnemy();
            }
        }

    public void StopSpawning()
    {
        spawn_bool = false;
    }

    public void StartSpawning()
    {
        spawn_bool = true;
    }

    private void SpawnEnemy()
    {
        int i = Random.Range(0, enemies.Length);
        Enemy newEnemy = Instantiate(enemies[i], transform.position + new Vector3(0, .25f, 0), transform.rotation) as Enemy;
        newEnemy.transform.parent = transform;
    }

}
