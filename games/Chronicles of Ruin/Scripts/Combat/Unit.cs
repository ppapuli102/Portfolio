using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Tilemaps;
using CodeMonkey.Utils;

public class Unit : MonoBehaviour
{
    public UnitConstructor unit;
    private UnitMovement unitMovement;
    private Grid grid;
    public Node currentNode;

    private Initiative initiative;

    public State unitState;

    [SerializeField] private GameObject bannerPreview;


    private void Awake() {
        // CombatManager.onCombatStateChanged += CombatManagerOnCombatStateChanged;
    }

    private void Start() {
        // Initialize References
        // this.GetComponent<Animator>().runtimeAnimatorController = unit.animator;
        unitState = State.IDLE;
        unitMovement = GetComponent<UnitMovement>();
    }

    private void Update() {
        // On-Click: Validate move position, then request Path to mouse position
        if (unitState == State.ACTIVE) {
            if (Input.GetKeyUp("p")) {
                unitMovement.CreateMovementGrid();
            }
            if (Input.GetMouseButtonDown(0)) {
                unitMovement.CheckForValidMove();
            }
        }
    }

    public void SetActiveUnit(bool value) {
        string unitTag = GetUnitTag();

        if (value == true) {
            unitState = State.ACTIVE;

            if (unitTag == "Hero" && CombatManager.instance.State != CombatState.PLAYERTURN) {
                CombatManager.instance.UpdateGameState(CombatState.PLAYERTURN);
            }
            else if (unitTag == "Enemy" && CombatManager.instance.State != CombatState.ENEMYTURN) {
                CombatManager.instance.UpdateGameState(CombatState.ENEMYTURN);
            }
        }

        else if (value == false) {
            unitState = State.IDLE;
        }
    }

    // private void CombatManagerOnCombatStateChanged(CombatState state) {
    //     string unitTag = GetUnitTag();

    //     switch (unitTag)
    //     {
    //         case "Hero":
    //             if (state == CombatState.PLAYERTURN) {
    //                 Debug.Log("HERO'S TURN");
    //             }
    //             break;
            
    //         case "Enemy":
    //             if (state == CombatState.ENEMYTURN) {

    //                 Debug.Log("ENEMY'S TURN");
    //             }
    //             break;
            
    //         default:
    //             break;
    //     }
    // }

    public Vector3Int GetUnitWorldPosition() {
        return new Vector3Int((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
    }

    public GameObject SpawnPreview() {
        return Instantiate(bannerPreview, transform.position, Quaternion.identity);
    }

    private string GetUnitTag() {
        Transform t = this.transform;
        return t.GetChild(0).tag;
        
    }

    private void OnEnable() {
        initiative = FindObjectOfType<Initiative>();
        if (initiative != null) {
            initiative.AddUnitsToInitiative(new List<GameObject>() { this.gameObject });
        }
    }

    private void OnDisable() {
        initiative = FindObjectOfType<Initiative>();
        if (initiative != null) {
            initiative.RemoveUnitsFromInitiative(new List<GameObject>() { this.gameObject });
        }
    }

    private void OnMouseDown() {
        Destroy(this.gameObject);
    }


}

public enum State { ACTIVE, IDLE }