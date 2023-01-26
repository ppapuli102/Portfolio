using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Equipment : MonoBehaviour
{

    public bool equipped = false;
    GameObject heroRef;
    UnitStats statRef;
    EquipPositions equipPos;
    public GameObject eMenu, pMenu;
    [SerializeField] EquipList equipmentList;
    RectTransform equipRef;
    Deck deckRef;
    Collection collRef;
    Card loadedCard;
    public GemMenu gemRef;
    public int refraction_equipped = 0;
    public List<Gem> gems_equipped = new List<Gem>();
    List<Card> AddedCopy = new List<Card>();
    private Vector3 equipMenuAnchor = new Vector3(-514f, 220f, 0);
    private int equipMenuXOffset = 130, equipMenuYOffset = -130;

    public enum Stat {maximum_hp, critical_chance, critical_damage, armor, resistance, 
                        evasion, accuracy, physical_damage_min, physical_damage_max, 
                        magical_damage_min, magical_damage_max};
    public enum Element {fire, water, earth, air};

    [Header("Equipment Statistics")]
    [SerializeField] string type;
    [SerializeField] public int refraction;
    [SerializeField] List<Stat> EquipStats;
    [SerializeField] List<int> EquipValues;
    [SerializeField] List<Element> EquipElements;
    [SerializeField] List<int> ElementValues;
    [SerializeField] List<Card> AddedCards;

    // Start is called before the first frame update
    void Start() {
        heroRef = GameObject.FindGameObjectWithTag("Hero").transform.parent.gameObject;
        statRef = heroRef.GetComponent<UnitStats>();
        eMenu = this.transform.root.GetChild(2).transform.GetChild(2).gameObject;
        pMenu = this.transform.root.GetChild(2).transform.GetChild(0).gameObject;
        equipPos = this.transform.root.transform.GetChild(2).transform.Find("Player Stats").transform.GetChild(0).GetComponent<EquipPositions>();
        equipRef = GetComponent<RectTransform>();
        deckRef = FindObjectOfType<Deck>();        
        collRef = FindObjectOfType<Collection>();
    }

    private void SortEquipMenu() {
        for (var i = 0; i < equipmentList.EquipmentList.Count; i++) {
            if (i <= 8) {
                equipmentList.EquipmentList[i].gameObject.GetComponent<RectTransform>().localPosition = 
                equipMenuAnchor + new Vector3(i*equipMenuXOffset, 0, 0);
            }
            if (9 <= i && i <= 17) {
                equipmentList.EquipmentList[i].gameObject.GetComponent<RectTransform>().localPosition = 
                equipMenuAnchor + new Vector3((i-9)*equipMenuXOffset, equipMenuYOffset, 0);
            }
            if (18 <= i && i <= 26) {
                equipmentList.EquipmentList[i].gameObject.GetComponent<RectTransform>().localPosition = 
                equipMenuAnchor + new Vector3((i-18)*equipMenuXOffset, 2*equipMenuYOffset, 0);
            }
            if (27 <= i && i <= 36) {
                equipmentList.EquipmentList[i].gameObject.GetComponent<RectTransform>().localPosition = 
                equipMenuAnchor + new Vector3((i-27)*equipMenuXOffset, 3*equipMenuYOffset, 0);
            }
            
        }
    }

    //Display equipment in the proper slot.
    private void SetEquipPosition() {
        if (this.equipped == true) {
            switch(type) {
                case "Helmet":
                    equipRef.localPosition = equipPos.Weapon;
                    break;
                case "Body":
                    equipRef.localPosition = equipPos.Body;
                    break;
                case "Boots":
                    equipRef.localPosition = equipPos.Boots;
                    break;
                case "Ring":
                    equipRef.localPosition = equipPos.Ring;
                    break;
                case "Amulet":
                    equipRef.localPosition = equipPos.Amulet;
                    break;
                case "Weapon": 
                    equipRef.localPosition = equipPos.Weapon;
                    break;
                case "OffHand":
                    equipRef.localPosition = equipPos.OffHand;
                    break;
            }
        } else {
            SortEquipMenu();
        }
    }

    void UpdateStats() {
        for (var i = 0; i <= EquipStats.Count - 1; i++) {
            var statistic = EquipStats[i].ToString();
            var value = EquipValues[i];
            if (equipped == true) {
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

    public void AddCards() {        
        for (var i = 0; i < AddedCards.Count; i++) {
            loadedCard = Instantiate(AddedCards[i], transform.position, Quaternion.identity, deckRef.transform);
            loadedCard.AddToDeck(loadedCard, false);
            AddedCopy.Add(loadedCard);
        }
    }

    public void RemoveCards() {
        for (var i = 0; i < AddedCards.Count; i++) {
            AddedCopy[i].RemoveFromDeck(AddedCopy[i], false);
            AddedCopy.Remove(AddedCopy[i]);
        }
    }

    //Equip from the menu.
    public void PointerDownEvent() {
        if (equipmentList.gem_equip == null) {
            if (Input.GetButtonDown("Fire1")) {
                if (this.equipped == false && !equipmentList.CheckEquipped(this.type)) {
                    equipped = true;
                    equipmentList.EquipSlot(type);
                    this.gameObject.transform.SetParent(pMenu.transform.GetChild(0));
                    equipmentList.EquipmentList.Remove(this.gameObject);
                    SetEquipPosition();
                    SortEquipMenu();
                    UpdateStats();
                    AddCards();
                    foreach (Gem gem in gems_equipped) {
                        gem.UpdateStats(this);
                    }
                } else if (this.equipped == true) {
                    equipped = false;
                    equipmentList.UnEquipSlot(type);
                    this.gameObject.transform.SetParent(eMenu.transform);
                    equipmentList.EquipmentList.Add(this.gameObject);
                    SetEquipPosition();
                    SortEquipMenu();
                    UpdateStats();
                    RemoveCards();
                    foreach (Gem gem in gems_equipped) {
                        gem.UpdateStats(this);
                    }
                }
            } else if (Input.GetButtonDown("Fire2")) {
                if (equipmentList.gem_equip == null) {
                    gemRef.gameObject.SetActive(true);
                    gemRef.EquipPreview(this);
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
