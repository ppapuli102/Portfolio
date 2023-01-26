using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class Gem : MonoBehaviour
{

    [SerializeField] GemMenu gemMenu;
    public bool equipped;
    private Equipment equipped_to;
    GameObject heroRef;
    UnitStats statRef;
    public int index;

    public enum Stat {maximum_hp, critical_chance, critical_damage, armor, resistance, 
                        evasion, accuracy, physical_damage_min, physical_damage_max, 
                        magical_damage_min, magical_damage_max};
    public enum Element {fire, water, earth, air};

    [Header("Gem Statistics")]
    [SerializeField] string type;
    [SerializeField] public int refraction_req;
    [SerializeField] List<Stat> EquipStats;
    [SerializeField] List<int> EquipValues;
    [SerializeField] List<Element> EquipElements;
    [SerializeField] List<int> ElementValues;

    void Start() {
        heroRef = GameObject.FindGameObjectWithTag("Hero").transform.parent.gameObject;
        statRef = heroRef.GetComponent<UnitStats>();
    }

    public void PointerDownEvent() {
        if (gemMenu.equipmentList.gem_equip != null) {
            if (!equipped) {
                equipped = true;
                gemMenu.EquipGem(this);
            } else if (equipped) {
                equipped = false;
                gemMenu.UnequipGem(this);
            }
        }
    }

    public void UpdateStats(Equipment eq) {
        for (var i = 0; i <= EquipStats.Count - 1; i++) {
            var statistic = EquipStats[i].ToString();
            var value = EquipValues[i];
            if (eq.equipped == true) {
                switch(statistic) {
                    case "maximum_hp":
                        statRef.maximum_hp += value;
                        break;
                    case "critical_chance":
                        statRef.critical_chance += value;
                        break;
                    case "critical_damage":
                        statRef.critical_damage += value;
                        break;
                    case "armor":
                        statRef.armor += value;
                        break;
                    case "resistance":
                        statRef.resistance += value;
                        break;
                    case "evasion":
                        statRef.evasion += value;
                        break;
                    case "accuracy":
                        statRef.accuracy += value;
                        break;
                    case "physical_damage_min":
                        statRef.physical_damage_min += value;
                        break;
                    case "physical_damage_max":
                        statRef.physical_damage_max += value;
                        break;
                    case "magical_damage_min":
                        statRef.magical_damage_min += value;
                        break;
                    case "magical_damage_max":
                        statRef.magical_damage_max += value;
                        break;
                }
            } else {
                switch(statistic) {
                    case "maximum_hp":
                        statRef.maximum_hp -= value;
                        break;
                    case "critical_chance":
                        statRef.critical_chance -= value;
                        break;
                    case "critical_damage":
                        statRef.critical_damage -= value;
                        break;
                    case "armor":
                        statRef.armor -= value;
                        break;
                    case "resistance":
                        statRef.resistance -= value;
                        break;
                    case "evasion":
                        statRef.evasion -= value;
                        break;
                    case "accuracy":
                        statRef.accuracy -= value;
                        break;
                    case "physical_damage_min":
                        statRef.physical_damage_min -= value;
                        break;
                    case "physical_damage_max":
                        statRef.physical_damage_max -= value;
                        break;
                    case "magical_damage_min":
                        statRef.magical_damage_min -= value;
                        break;
                    case "magical_damage_max":
                        statRef.magical_damage_max -= value;
                        break;
                }
            }
        }
    }

    void Update() {
        
    }
}
