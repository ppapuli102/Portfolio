using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] public int numEnemiesAlive = 0;
    [SerializeField] GameObject winLabel;
    [SerializeField] GameObject loseLabel;
    [SerializeField] float waitToLoad = 4f;
    public int maxEnemies = 3;
    private EnemySpawner[] enemySpawners;
    private bool levelEnded = false;

    private void Start() {
        enemySpawners = FindObjectsOfType<EnemySpawner>();
        winLabel.SetActive(false);
        loseLabel.SetActive(false);
    }

    public void EnemySpawned()
    {
        numEnemiesAlive ++;
    }

    public void EnemyDied()
    {
        numEnemiesAlive --;
        if (levelEnded && numEnemiesAlive <= 0)
        {
            StartCoroutine("HandleWinCondition");
        }
    }

    public void LevelEnded()
    {
        levelEnded = true;
        StopEnemySpawners();
    }

    public void StopEnemySpawners()
    {
        foreach (EnemySpawner spawner in enemySpawners)
        {
            spawner.StopSpawning();
        }
    }

    public void StartEnemySpawners()
    {
        foreach (EnemySpawner spawner in enemySpawners)
        {
            spawner.StartSpawning();
        }
    }

    private IEnumerator HandleWinCondition()
    {
        winLabel.SetActive(true);
        yield return new WaitForSeconds(waitToLoad);
        FindObjectOfType<LevelController>().GetComponent<SceneLoader>().LoadNextScene();
    }

    public void HandleLoseCondition()
    {
        loseLabel.SetActive(true);
        Time.timeScale = 0; // stop time after loss
    }

}
