using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Targeting : MonoBehaviour
{
    [Header("References")]
    private Grid grid;
    private BezierArrows targetingArrows;
    private Hand handRef;
    private Card activeCard;
    private Pathfinding pathfindingRef;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap gridTilemap;
    [SerializeField] private TileBase attackingTile;
    [SerializeField] private TileBase normalTile;

    [Header("Targeting")]
    public Unit activeUnit;
    [SerializeField] GameObject[] targetingSquareTypes;
    private GameObject targetingSquare;
    private int handOffsetWhileTargeting = -125;
    private Vector3 handOffsetPosition = new Vector3();

    private void Start() {
        grid = FindObjectOfType<Grid>();
        targetingArrows = FindObjectOfType<BezierArrows>();
        handRef = GetComponent<Hand>();
        pathfindingRef = FindObjectOfType<Pathfinding>();
        handOffsetPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + handOffsetWhileTargeting, 0);
    }

    private void Update() {
        if (targetingSquare != null) {
            bool activeTarget = HighlightActiveGrid();
            if (Input.GetMouseButtonDown(0) && activeTarget) {
                handRef.previewRef.activeCard.ResolveEffect();
            }
        }
        // Debug.Log(grid.NodeFromWorldPoint(UtilsClass.GetMouseWorldPosition()).worldPosition);
    }

    public void StartTargeting(GameObject targetAOE) {
        Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
        Node currentNode = grid.NodeFromWorldPoint(mousePos);

        targetingArrows.BeginTargeting();
        Cursor.visible = true;
        gameObject.GetComponent<Image>().raycastTarget = false;

        // Offset the hand to move it out of the way
        transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, handOffsetPosition, 115f);

        // Create the appropriate targeting square AOE based on the spell's AOE component
        targetingSquare = Instantiate(targetAOE, currentNode.worldPosition, Quaternion.identity);
        activeCard = FindObjectOfType<CardPreview>().activeCard;
        CreateAttackingGrid();
    }

    public void StopTargeting() {
        targetingArrows.EndTargeting();
        Cursor.visible = true;
        transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - handOffsetWhileTargeting, 0), 115f);
        gameObject.GetComponent<Image>().raycastTarget = true;
        Destroy(targetingSquare.gameObject);
        DestroyAttackingGrid();
    }

    private bool HighlightActiveGrid() {
        Vector3 mousePos = UtilsClass.GetMouseWorldPosition();
        // Targeting Square snaps to grid while following mouse
        targetingSquare.transform.localPosition = grid.NodeFromWorldPoint(mousePos).worldPosition;

        // Loop through all of the target squares in the targeting AOE
        for (int i = 0; i < targetingSquare.transform.childCount; i ++) {
            Transform currentSquare = targetingSquare.transform.GetChild(i);
            Vector3 childPosition = currentSquare.transform.localPosition;
            Node checkNode = grid.NodeFromWorldPoint(targetingSquare.transform.localPosition + childPosition);
            List<Node> previousNodesChecked = new List<Node>();
            SpriteRenderer currentSpriteRenderer = currentSquare.GetComponent<SpriteRenderer>();
            bool withinRange = checkNode.GetIsValidAttackPosition();

        // Color square based on the node it inhabits, then return whether there is a valid target
            if (checkNode.GetCurrentUnit() != null && withinRange == true) {
                ColorNodesRed();
                return true;
            }
            // else if (checkNode.GetCurrentUnit() == null) {
            //     if (withinRange) {
                    
            //     }    
            //     if (!withinRange) {
            //         ColorNodesYellow();
            //     }
            // }
        }

        if (grid.NodeFromWorldPoint(mousePos).GetIsValidAttackPosition() == true) {
            ColorNodesYellow();
        }
        else if (grid.NodeFromWorldPoint(mousePos).GetIsValidAttackPosition() == false) {
            ColorNodesWhite();
        }

        return false;
    }

    private void ColorNodesRed() {
        for (int i = 0; i < targetingSquare.transform.childCount; i ++) {
            Transform currentSquare = targetingSquare.transform.GetChild(i);
            SpriteRenderer currentSpriteRenderer = currentSquare.GetComponent<SpriteRenderer>();
            currentSpriteRenderer.color = Color.red;
        }
    }
    private void ColorNodesWhite() {
        for (int i = 0; i < targetingSquare.transform.childCount; i ++) {
            Transform currentSquare = targetingSquare.transform.GetChild(i);
            SpriteRenderer currentSpriteRenderer = currentSquare.GetComponent<SpriteRenderer>();
            currentSpriteRenderer.color = Color.white;
        }
    }
    private void ColorNodesYellow() {
        for (int i = 0; i < targetingSquare.transform.childCount; i ++) {
            Transform currentSquare = targetingSquare.transform.GetChild(i);
            SpriteRenderer currentSpriteRenderer = currentSquare.GetComponent<SpriteRenderer>();
            currentSpriteRenderer.color = Color.yellow;
        }
    }

    // Creates a blue grid of valid attacking tiles within range of the unit
    private void CreateAttackingGrid() {
        Vector3Int unitPosition = activeUnit.GetUnitWorldPosition();

        // For each node within our max targeting distance
        for (int x = unitPosition.x - (int)activeCard.range; x <= unitPosition.x + activeCard.range; x++) {
            for (int y = unitPosition.y - (int)activeCard.range; y <= unitPosition.y + activeCard.range; y++) {
                if (x == 0 && y == 0) { continue; }
                
                Node checkNode = grid.NodeFromWorldPoint(new Vector3(x, y, 0));

                if (checkNode.walkable) {
                    // Position is Walkable
                    if (pathfindingRef.HasPath((int)unitPosition.x, (int)unitPosition.y, x, y, activeCard.range)) {
                        // There is a path to the target Node
                        gridTilemap.SetTile(new Vector3Int(x, y, 0), attackingTile);
                        checkNode.SetIsValidAttackPosition(true); // Store valid move positions on the node within grid
                    }
                }
            }
        }
    }

    // Destroys the attacking grid and resets the tiles to their normal tilebase
    private void DestroyAttackingGrid() {
        Vector3Int unitPosition = activeUnit.GetUnitWorldPosition();

        for (int x = unitPosition.x - (int)activeCard.range; x <= unitPosition.x + activeCard.range; x ++) {
            for (int y = unitPosition.y - (int)activeCard.range; y <= unitPosition.y + activeCard.range; y ++){ 
                Node checkNode = grid.NodeFromWorldPoint(new Vector3(x, y, 0)); 
                gridTilemap.SetTile(new Vector3Int(x, y, 0), normalTile);
                checkNode.SetIsValidAttackPosition(false);
            }
        }
    }

}
