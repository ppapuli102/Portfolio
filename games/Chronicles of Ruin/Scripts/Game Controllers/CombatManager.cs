using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    public CombatState State;
    
    public static event Action<CombatState> onCombatStateChanged;

    private ResourceManager resourceManager;
    private Initiative initiative;
    private Targeting targeting;

    [SerializeField] private GameObject combatStartPanel;
    [SerializeField] private Transform deckListTransform;

    private void Awake() {
        instance = this;
        resourceManager = GetComponent<ResourceManager>();
        initiative = GetComponent<Initiative>();
        targeting = FindObjectOfType<Targeting>();
        UpdateGameState(CombatState.START);
    }

    private void Start() {
    }

    public void UpdateGameState(CombatState newState) {
        State = newState;

        switch (newState) {
            
            case CombatState.START:
                StartCoroutine(HandleCombatStart());
                break;
            
            case CombatState.PLAYERTURN:
                Debug.Log("Player's Turn");
                break;

            case CombatState.ENEMYTURN:
                Debug.Log("Enemy's Turn");
                break;
            
            case CombatState.WIN:
                break;
            
            case CombatState.LOSE:
                break;
            
            default:
                break;
        }

        onCombatStateChanged?.Invoke(newState);
    }

    private IEnumerator HandleCombatStart() {
        // Debug.Log("Combat Start");
        // combatStartPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        // combatStartPanel.SetActive(false);

        // Should update to whoever has highest initiative - Player or Enemy? 
        UpdateGameState(CombatState.PLAYERTURN);
    }

    public void StartNextInitiative() {
        initiative.NextInitiative();
    }

    public void SetNewActiveUnit(GameObject activeUnit) {
        activeUnit.GetComponent<Unit>().SetActiveUnit(true);
        targeting.activeUnit = activeUnit.GetComponent<Unit>();
    }

    public void ResetActiveUnit(GameObject activeUnit) {
        activeUnit.GetComponent<Unit>().SetActiveUnit(false);
        targeting.activeUnit = null;
    }

    private void OnDisable() {
        resourceManager.SaveResources();
    }

}

public enum CombatState { 
    START,
    PLAYERTURN, 
    ENEMYTURN, 
    WIN, 
    LOSE 
}