using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMenu : MonoBehaviour
{

    public List<Gem> gemList = new List<Gem>();
    public List<Gem> gemDisplay = new List<Gem>();
    public Equipment equipRef;
    public EquipList equipmentList;
    public Equipment equipCopy;
    public Gem gemCopy;
    Transform gem_equip_menu;
    public int gemNum;
    private Gem remove_index;

    // Start is called before the first frame update
    void Start() {
        gemNum = this.transform.childCount - 1;
        for (var i = 1; i <= gemNum; i++) {
            gemList.Add(this.transform.GetChild(i).GetComponent<Gem>());
        }
    }

    void Awake() {
        gem_equip_menu = this.transform.GetChild(0).transform;
        equipmentList = this.transform.parent.transform.Find("Equipment Menu").GetComponent<EquipList>();
    }

    public void EquipPreview(Equipment equip) {
        equipmentList.gem_equip = equip;
        equipRef = equipmentList.gem_equip;
        equipmentList.menuRef.EnableGemMenu();
        equipmentList.menuRef.DisableEquipmentMenu();
        equipCopy = Instantiate(equipRef, Input.mousePosition, Quaternion.identity, this.transform);
        equipCopy.transform.SetParent(gem_equip_menu);
        equipCopy.transform.localScale = new Vector3(1,1,1);
        equipCopy.transform.localPosition = new Vector3(-508, 0, 0);
        for (var i = 0; i < equipRef.gems_equipped.Count; i++) {
            gemCopy = Instantiate(equipRef.gems_equipped[i], this.transform.position, Quaternion.identity, gem_equip_menu);
            gemCopy.gameObject.SetActive(true);
            gemDisplay.Add(gemCopy);
        }
        SortGemMenu();
        SortGemEquip();
    }

    public void ExitPreview() {
        Destroy(equipCopy.gameObject);
        for (var i = 0; i <gemDisplay.Count; i++) {
            Destroy(gemDisplay[i].gameObject);
        }
        gemDisplay.Clear();
        equipmentList.gem_equip = null;
        equipmentList.menuRef.DisableGemMenu();
        equipmentList.menuRef.EnableEquipmentMenu();
    }

    public void EquipGem(Gem gem) {
        if ((equipRef.refraction - equipRef.refraction_equipped) >= gem.refraction_req) {
            if (gemList.Contains(gem)) {
                gemList.Remove(gem);
                gemNum--;
            }
            gem.index = equipRef.gems_equipped.Count;
            equipRef.gems_equipped.Add(gem);
            equipRef.refraction_equipped += gem.refraction_req;
            gem.transform.SetParent(equipRef.transform);
            gemCopy = Instantiate(gem, this.transform.position, Quaternion.identity, gem_equip_menu);
            gemCopy.index = gem.index;
            gemDisplay.Add(gemCopy);
            gem.gameObject.SetActive(false);
            SortGemMenu();
            SortGemEquip();
        }
    }

    public void UnequipGem(Gem gem) {
        gemNum++;
        foreach (Gem gemvar in equipRef.gems_equipped) {
            if (gemvar.index == gem.index) {
                remove_index = gemvar;
                gemvar.equipped = false;
                gemDisplay.Remove(gem);
                Destroy(gem.gameObject);
                gemvar.transform.SetParent(this.transform);
                gemvar.gameObject.SetActive(true);
                gemList.Add(gemvar);
            }
        }
        equipRef.gems_equipped.Remove(remove_index);
        equipRef.gems_equipped.Remove(gem);
        equipRef.refraction_equipped -= gem.refraction_req;
        SortGemMenu();
        SortGemEquip();
    }

    public void SortGemMenu() {
        for (var i = 0; i < gemNum; i++) {
            if (i <= 9) {
                gemList[i].transform.localPosition = new Vector3(-524 + (i*110), 100, 0);
            }
            if (i > 9 && i <= 19) {
                gemList[i].transform.localPosition = new Vector3(-524 + ((i-10)*110), -10, 0);
            }
            if (i > 19 && i <= 29) {
                gemList[i].transform.localPosition = new Vector3(-524 + ((i-20)*110), -120, 0);
            }
            if (i > 29 && i <= 39) {
                gemList[i].transform.localPosition = new Vector3(-524 + ((i-30)*110), -230, 0);
            }
        }
    }

    public void SortGemEquip() {
        for (var i = 0; i < equipRef.gems_equipped.Count; i++) {
            gemDisplay[i].transform.localPosition = new Vector3((-345 + (120*i)), 0, 0);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Cancel") && (equipmentList.gem_equip != null)) {
            ExitPreview();
        }
        Debug.Log(equipRef);
    }
}
