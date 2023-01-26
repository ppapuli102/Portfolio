using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InitiativeTracker : MonoBehaviour
{
    
    [SerializeField] private List<UnitInitiativePreview> preivews = new List<UnitInitiativePreview>();
    private Initiative initiative;

    private List<int> initiatives; 

    private void Start() {
        initiative = CombatManager.instance.GetComponent<Initiative>();
        // int i = GetHighestInitiative();
        // SortInitiativeTracker();
    }

    // private int GetHighestInitiative() {
        // initiatives = initiative.GetInitiatives();
        // return initiatives.Last();
    // }

    public void UpdateInitiativeTracker(List<GameObject> units) {
        ClearTracker();
        foreach (GameObject unit in units) {
            var preview = unit.GetComponent<Unit>().SpawnPreview();
            UpdatePreviewStats(preview, unit);
            preview.transform.SetParent(this.transform);
        }
    }

    private void UpdatePreviewStats(GameObject preview, GameObject unit) {
        var unitPreview = preview.GetComponent<UnitInitiativePreview>();
        unitPreview.unitStat = unit.GetComponent<UnitStats>();
    }

    private void ClearTracker() {
        foreach (Transform child in this.transform) {
            if (child != null)
                GameObject.Destroy(child.gameObject);
        }
    }
}
