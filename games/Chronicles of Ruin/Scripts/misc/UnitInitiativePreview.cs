using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInitiativePreview : MonoBehaviour
{
    
    public UnitStats unitStat;
    [SerializeField] private GameObject unit;
    
    public int initiative;
    public int maxHealth;
    public int currentHealth;


    private void Start() {
        initiative = unitStat.initiative;
        maxHealth = unitStat.maximum_hp;
        currentHealth = unitStat.current_hp;
    }

    public int GetInitiative() {
        return initiative;
    }




}
