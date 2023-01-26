using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipList : MonoBehaviour
{

    public Equipment gem_equip;
    public PlayerMenus menuRef;
    public List<GameObject> EquipmentList = new List<GameObject>();
    public int equips_number;
    public bool Helmet_filled = false;
    public bool Body_filled = false;
    public bool Boots_filled = false;
    public bool Ring_filled = false;
    public bool Amulet_filled = false;
    public bool Weapon_filled = false;
    public bool OffHand_filled = false;

    // Start is called before the first frame update
    void Start() {
        equips_number = this.transform.childCount;
        DisplayEquipment();
    }

    public void DisplayEquipment() {
        for (var i = 0; i < equips_number; i++) {
            EquipmentList.Add(this.transform.GetChild(i).gameObject);
        }
    }

    public void EquipSlot(string equipped) {
        switch (equipped) {
            case "Helmet":
                Helmet_filled = true;
                break;
            case "Body":
                Body_filled = true;
                break;            
            case "Boots":
                Boots_filled = true;
                break;
            case "Ring":
                Ring_filled = true;
                break;
            case "Amulet":
                Amulet_filled = true;
                break;
            case "Weapon":
                Weapon_filled = true;
                break;
            case "OffHand":
                OffHand_filled = true;
                break;
        }
    }

    public void UnEquipSlot(string equipped) {
        switch (equipped) {
            case "Helmet":
                Helmet_filled = false;
                break;
            case "Body":
                Body_filled = false;
                break;            
            case "Boots":
                Boots_filled = false;
                break;
            case "Ring":
                Ring_filled = false;
                break;
            case "Amulet":
                Amulet_filled = false;
                break;
            case "Weapon":
                Weapon_filled = false;
                break;
            case "OffHand":
                OffHand_filled = false;
                break;
        }
    }

    public bool CheckEquipped(string equipped) {
        switch (equipped) {
            case "Helmet":
                if (Helmet_filled) {
                    return true;
                } else {return false;}
            case "Body":
                if (Body_filled) {
                    return true;
                } else {return false;}
            case "Boots":
                if (Boots_filled) {
                    return true;
                } else {return false;}
            case "Ring":
                if (Ring_filled) {
                    return true;
                } else {return false;}
            case "Amulet":
                if (Amulet_filled) {
                    return true;
                } else {return false;}
            case "Weapon":
                if (Weapon_filled) {
                    return true;
                } else {return false;}
            case "OffHand":
                if (OffHand_filled) {
                    return true;
                } else {return false;}
        }
        return true;
    }

    // Update is called once per frame
    void Update() {
        equips_number = this.transform.childCount;
    }
}
