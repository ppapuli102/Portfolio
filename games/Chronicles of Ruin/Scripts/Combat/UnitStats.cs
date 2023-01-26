using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitStats : MonoBehaviour
{

    Initiative iniRef;
    // public bool holds_initiative = false;

    [Header("Stats")]
    public int maximum_hp;
    public int current_hp;
    public int critical_chance, critical_damage;
    public int armor, resistance;
    public int evasion, accuracy;
    public int physical_damage_mod, magical_damage_mod;
    public int elemental_affinity;
    public int initiative;
    public int physical_damage_min;
    public int physical_damage_max;
    public int magical_damage_min;
    public int magical_damage_max;
    
    
    // Start is called before the first frame update
    void Start() {
        iniRef = FindObjectOfType<Initiative>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
