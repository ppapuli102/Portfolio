using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DefenderSpawner : MonoBehaviour
{
    GameObject defenderParent;
    Defender defender;
    List<Vector2> placedDefenders = new List<Vector2>();
    const string DEFENDER_PARENT_NAME = "Defenders";
    

    private void Start() {
        CreateDefenderParent();
    }

    private void CreateDefenderParent()
    {
        defenderParent = GameObject.Find(DEFENDER_PARENT_NAME);
        if (!defenderParent)
        {
            defenderParent = new GameObject(DEFENDER_PARENT_NAME);
        }
    }

    private void OnMouseDown() {
        AttemptToPlaceDefenderAt(GetSquareClicked());
    }

    public void SetSelectedDefender(Defender defenderToSelect)
    {
        defender = defenderToSelect;
    }

    private void AttemptToPlaceDefenderAt(Vector2 gridPos)
    {
        var StarDisplay = FindObjectOfType<StarDisplay>();
        if (!defender)
        {
            return;
        }
        else{
            int defenderCost = defender.GetStarCost();
            if ((defenderCost <= StarDisplay.stars) && (!placedDefenders.Contains(gridPos)))
            {
                SpawnDefender(gridPos);
                placedDefenders.Add(gridPos);
                StarDisplay.SpendStars(defenderCost);
            }
        }
    }

    private Vector2 GetSquareClicked()
    {
        Vector2 clickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(clickPos);
        Vector2 gridPos = SnapToGrid(worldPos);
        return gridPos;
    }

    private Vector2 SnapToGrid(Vector2 rawWorldPos)
    {
        float newX = Mathf.RoundToInt(rawWorldPos.x);
        float newY = Mathf.Round(rawWorldPos.y);
        return new Vector2(newX, newY);
    }

    private void SpawnDefender(Vector2 spawnPosition)
    {
        Defender newDefender = Instantiate(defender, spawnPosition, Quaternion.identity) as Defender;
        newDefender.transform.parent = defenderParent.transform;
    }
}
