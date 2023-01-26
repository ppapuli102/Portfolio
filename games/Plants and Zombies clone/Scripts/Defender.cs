using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    [SerializeField] int starCost = 100;
    [SerializeField] float defenderHealth = 100;
    

    public void AddStars(int amount)
    {
        FindObjectOfType<StarDisplay>().AddStars(amount);
    }

    public int GetStarCost()
    {
        return starCost;
    }

    public void DealDamage(float dmg)
    {
        defenderHealth -= dmg;
        if (defenderHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

}
