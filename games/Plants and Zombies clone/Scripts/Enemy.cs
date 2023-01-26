using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] [Range(0f, 5f)] float walkSpeed = 1f;
    [SerializeField] float enemyHealth = 20f;
    GameObject currentTarget;

    private void Awake() 
    {
        FindObjectOfType<LevelController>().EnemySpawned();
    }

    private void OnDestroy()
    {
        LevelController levelController = FindObjectOfType<LevelController>();
        if (levelController != null)
        {
            levelController.EnemyDied();
        }
    }

    void Update()
    {
        transform.Translate(Vector2.left * walkSpeed * Time.deltaTime);
        UpdateAnimationState();
    }

    public void SetMovementSpeed(float speed)
    {
        walkSpeed = speed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Projectile")
        {
            var hitBy = FindObjectOfType<Projectile>();
            ProcessHit(hitBy.damage);
        }
    }

    public void ProcessHit(float dmg)
    {
        enemyHealth -= dmg;
        if (enemyHealth <= 0)
        {
            SetMovementSpeed(0);
        }
        StartCoroutine("FlashRedThenDie");
    }

    private IEnumerator FlashRedThenDie()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Attack(GameObject target)
    {
        GetComponent<Animator>().SetBool("isAttacking", true);
        currentTarget = target;
    }

    public void StrikeCurrentTarget(float dmg)
    {
        if (!currentTarget) { return; }
        Defender defender = currentTarget.GetComponent<Defender>();
        if (defender)
        {
            defender.DealDamage(dmg);
        }
    }

    private void UpdateAnimationState()
    {
        if (!currentTarget)
        {
            GetComponent<Animator>().SetBool("isAttacking", false);
        }
    }

}
