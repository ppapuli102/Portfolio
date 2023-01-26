using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{

    [SerializeField] private NPC npc;
    // [SerializeField] private TownManager townManager;

    private void Awake() {
        // FindObjectOfType<TownManager>();
    }

    public void ExitMenu() {
        TownManager.instance.ToggleVentureButton();
        this.gameObject.SetActive(false);
    }


}
