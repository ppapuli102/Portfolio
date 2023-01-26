using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using CodeMonkey.Utils;

public class UnitMovement : MonoBehaviour
{

    // Pathfinding Variables
    private int maxMoveDistance = 2;
    [SerializeField] Vector3[] path; // Serialized for Debug
    int targetIndex;
    public Node currentNode;
    [SerializeField] TileBase highlightedTile;
    [SerializeField] TileBase unhighlightedTile;

    // References
    [SerializeField] private Tilemap gridTilemap;
    Grid grid;
    Pathfinding pathfinding;
    Unit thisUnit;

    // Gameplay Variables
    private float moveSpeed = 10;

    private void Start() {
        grid = FindObjectOfType<Grid>();
        pathfinding = FindObjectOfType<Pathfinding>();
        thisUnit = GetComponent<Unit>();
        currentNode = grid.NodeFromWorldPoint(this.transform.position);
        currentNode.SetCurrentUnit(thisUnit);
        ResetValidMovePositions();
    }

    private void Update() {
        // Testing the Available Moves Sub-Grid. (Remove and add to separate class after testing complete)
    }

    public void CreateMovementGrid() {
        Vector3Int unitPosition = GetWorldPosition();
        // For each node within our max movement distance
        for (int x = unitPosition.x - (int)maxMoveDistance; x <= unitPosition.x + maxMoveDistance; x++) {
            for (int y = unitPosition.y - (int)maxMoveDistance; y <= unitPosition.y + maxMoveDistance; y++) {
                Node currentNode = grid.NodeFromWorldPoint(new Vector3(x, y, 0));

                if (currentNode.walkable && !currentNode.hasUnit) {
                    // Position is Walkable
                    if (pathfinding.HasPath((int)transform.position.x, (int)transform.position.y, x, y, maxMoveDistance)) {
                        // There is a path to the target Node
                        gridTilemap.SetTile(new Vector3Int(x, y, 0), highlightedTile);
                        currentNode.SetIsValidMovePosition(true); // Store valid move positions on the node within grid
                    }
                }
            }
        }
    }

    public void CheckForValidMove() {
        Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
        Node currentNode = grid.NodeFromWorldPoint(mousePos);
        bool withinRange = currentNode.GetIsValidMovePosition();
        if (withinRange)
        {
            ResetValidMovePositions();
            MoveToMouse(mousePos);
        }
    }

    private void MoveToMouse(Vector3 mouseWorldPos) {
        Vector3Int targetPos = gridTilemap.WorldToCell(mouseWorldPos);
        PathRequestManager.RequestPath(transform.position, targetPos, OnPathFound);
    }

    private void ResetValidMovePositions() {
        for (int x = 0; x < grid.GetGridX(); x ++) {
            for (int y = 0; y < grid.GetGridY(); y ++) {
                grid.grid[x, y].SetIsValidMovePosition(false);
                gridTilemap.SetTile(new Vector3Int((int)grid.grid[x,y].worldPosition.x, (int)grid.grid[x,y].worldPosition.y, 0), unhighlightedTile);
            }
        }
    }

    // Callback for when the path is found and populated
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            path = newPath;
            StopCoroutine(FollowPath(UpdateCurrentNode_callback));    // Stop first just in case
            StartCoroutine(FollowPath(UpdateCurrentNode_callback));
        }
    }
    // Travel from one waypoint in our path list to the next
    public IEnumerator FollowPath(Action UpdateCurrentNode_callback) {
        Vector3 currentWaypoint = path[0];
        targetIndex = 0;

        while (true) {
            if (transform.position == currentWaypoint) {
                targetIndex ++;
                if (targetIndex >= path.Length) {
                    UpdateCurrentNode_callback();
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void UpdateCurrentNode_callback() {
        currentNode.ClearCurrentUnit();
        currentNode = grid.NodeFromWorldPoint(this.transform.position);
        currentNode.SetCurrentUnit(thisUnit);

        ResetValidMovePositions();

        CombatManager.instance.StartNextInitiative();
    }

    private Vector3Int GetWorldPosition() {
        return new Vector3Int((int)transform.position.x,(int)transform.position.y,(int)transform.position.z);
    }

// Show Pathfinding Waypoint Visuals
    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }


}
