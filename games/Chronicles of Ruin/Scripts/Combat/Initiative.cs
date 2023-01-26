using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Initiative : MonoBehaviour
{

    [SerializeField] private List<GameObject> unitsInScene = new List<GameObject>();
    [SerializeField] private GameObject activeUnit;
    [SerializeField] private InitiativeTracker tracker;

    private SortedSet<int> initiatives = new SortedSet<int>();

    [SerializeField] private List<GameObject> turnOrder = new List<GameObject>();


    private void Start() {
        SortUnitsByInitiative();
        UpdateTracker(turnOrder);
    }

    private void Update() {
        // if (Input.GetKeyDown("x")) {
        //     NextInitiative();
        // }
    }

    public void OrderInitiatives() {
        if (unitsInScene.Count > 0) {
            // Add every initiative to a list
            foreach (GameObject unit in unitsInScene) {
                int initiative = unit.GetComponent<UnitStats>().initiative;
                initiatives.Add(initiative);
            }
        }
    }

    private void SortUnitsByInitiative() {
        OrderInitiatives();
        for (int h = 0; h < unitsInScene.Count; h ++) {
            for (int i = 0; i < unitsInScene.Count; i ++) {
                int currentUnitInitiative = unitsInScene[i].GetComponent<UnitStats>().initiative;
                if (initiatives.Max == (currentUnitInitiative)) {
                    turnOrder.Add(unitsInScene[i]);
                }
            }
            initiatives.Remove(initiatives.Max);
        }
    }

    public void NextInitiative() {
        GameObject first = turnOrder[0];
        turnOrder.Remove(turnOrder[0]);
        turnOrder.Add(first);
        UpdateTracker(turnOrder);
    }

    public void AddUnitsToInitiative(List<GameObject> add) {
        for (int i = 0; i < add.Count; i ++) {
            unitsInScene.Add(add[i]);

            if (turnOrder.Count > 0) {
                var addInitiative = add[i].GetComponent<UnitStats>().initiative;
                // Counting backwards within the turn order list
                for (int j = 0; j < turnOrder.Count; j ++) {
                    var unitInitiative = turnOrder[turnOrder.Count - j - 1].GetComponent<UnitStats>().initiative;

                    if (unitsInScene.Contains(add[i])) {
                        // If the unit we're adding has the same initiative as the unit we're checking
                        if (addInitiative == unitInitiative) {
                            // Then insert the unit to the right of that unit's initiative
                            turnOrder.Insert(turnOrder.Count-j-1+1, add[i]);
                            UpdateTracker(turnOrder);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void RemoveUnitsFromInitiative(List<GameObject> remove) {
        for (int i = 0; i < remove.Count; i ++) {
            if (turnOrder.Contains(remove[i])) {
                turnOrder.Remove(remove[i]);
            }
        }
        UpdateTracker(turnOrder);
    }

    private void UpdateTracker(List<GameObject> order) {
        if (order.Count > 0) {
            tracker.UpdateInitiativeTracker(order);
            GivePriorityToActiveUnit();
        }
    }

    private void GivePriorityToActiveUnit() {
        if (activeUnit != null) {
            CombatManager.instance.ResetActiveUnit(activeUnit);
        }
        activeUnit = turnOrder[0];
        CombatManager.instance.SetNewActiveUnit(activeUnit);
    }

    public GameObject GetActiveUnit() {
        return activeUnit;
    }

    private void OnDisable() {
        ClearInitiativeOrder();
    }

    public void ClearInitiativeOrder() {
        unitsInScene.Clear();
        turnOrder.Clear();
        initiatives.Clear();
    }

}
