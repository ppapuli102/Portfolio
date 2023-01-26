using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenus : MonoBehaviour
{

    private bool cMenu, eMenu, gMenu, mSwap;
    private EquipList equipmentList;

    // Start is called before the first frame update
    void Start() {
        equipmentList = this.transform.Find("Equipment Menu").GetComponent<EquipList>();
        cMenu = false;
        eMenu = false;
        gMenu = false;
        mSwap = false;
    }

    public void ToggleCharacterMenu() {
        if (cMenu == false) {
            cMenu = true;
        } else {cMenu = false;}
        this.gameObject.transform.GetChild(0).gameObject.SetActive(cMenu);
    }

    public void ToggleEquipmentMenu() {
        if (equipmentList.gem_equip == null) {
            if (eMenu == false) {
                eMenu = true;
            } else {eMenu = false;}
            this.gameObject.transform.GetChild(2).gameObject.SetActive(eMenu);
        }
    }

    public void ToggleGemMenu() {
        if (equipmentList.gem_equip == null) {
            if (gMenu == false) {
                gMenu = true;
            } else {gMenu = false;}
            this.gameObject.transform.GetChild(1).gameObject.SetActive(gMenu);
        }
    }

    public void ToggleMenuSwap() {
        if (mSwap == false) {
            mSwap = true;
        } else {mSwap = false;}
        this.gameObject.transform.GetChild(3).gameObject.SetActive(mSwap);
    }

    public void EnableCharacterMenu() {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        cMenu = true;
    }

    public void DisableCharcterMenu() {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        cMenu = false;
    }

    public void EnableGemMenu() {
        this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gMenu = true;
    }

    public void DisableGemMenu() {
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gMenu = false;
    }

    public void EnableEquipmentMenu() {
        this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
        eMenu = true;
    }

    public void DisableEquipmentMenu() {
        this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        eMenu = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("c")) {
            ToggleCharacterMenu();
        }
        if (Input.GetKeyDown("i")) {
            if (eMenu == false) {
                if (gMenu == true) {
                    DisableGemMenu();
                } else {EnableEquipmentMenu();}
            } else if (eMenu == true) {
                DisableEquipmentMenu();
            }
            ToggleMenuSwap();
        }
    }
}
