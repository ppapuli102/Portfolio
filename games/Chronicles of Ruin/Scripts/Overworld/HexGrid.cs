using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using System.Linq;

public class HexGrid : MonoBehaviour
{
    public Tilemap tileMap;
    private Vector3Int townPos;
    private Vector3 mousePos, mousePosWorld;
    private Vector3 pathPos;
    private string randomDirection;
    [SerializeField] HexTile tileRef;
    HexTile tiling, tiling2, tiling3;
    public HexTile destination_tile;
    public HexTile heroTile;
    public HexTile temp;
    [SerializeField] DetectTile tile_detector;
    DetectTile detective;
    private int hexLength, hexWidth;
    public int steps_taken = 0;

    [SerializeField] OverworldMovement ovMove;

    public Dictionary<string, Vector3Int> HexDirection = new Dictionary<string, Vector3Int>();
    public List<HexTile> openTiles = new List<HexTile>();
    public List<HexTile> closedTiles = new List<HexTile>();
    public List<HexTile> deadEnd = new List<HexTile>();
    public List<HexTile> finalPath = new List<HexTile>();

    // Start is called before the first frame update
    void Start() {
        tileMap = GetComponent<Tilemap>();
        townPos = tileMap.WorldToCell(tileMap.tileAnchor);
        //tileRef = this.transform.parent.transform.Find("Hex Tile").GetComponent<HexTile>();
        hexLength = 10;
        hexWidth = 10;
    }

//Returns coordinates of a specified direction from a cell
    public Vector3Int FindHexDirection(Vector3Int cell, string direction) {
        HexDirection.Clear();
        HexDirection.Add("Right", new Vector3Int(1, 0, 0));
        HexDirection.Add("Left", new Vector3Int(-1, 0, 0));
        if (cell.y % 2 == 0) {
            HexDirection.Add("UpLeft", new Vector3Int (-1, 1, 0));
            HexDirection.Add("UpRight", new Vector3Int (0, 1, 0));
            HexDirection.Add("DownLeft", new Vector3Int (-1, -1, 0));
            HexDirection.Add("DownRight", new Vector3Int (0, -1, 0));
        } else {
            HexDirection.Add("UpLeft", new Vector3Int (0, 1, 0));
            HexDirection.Add("UpRight", new Vector3Int (1, 1, 0));
            HexDirection.Add("DownLeft", new Vector3Int (0, -1, 0));
            HexDirection.Add("DownRight", new Vector3Int (1, -1, 0));
        }
        return HexDirection[direction];
    }

//Generate a random cardinal direction using hex tiles
    public string RandomDirection() {
        switch (Random.Range(0, 5)) {
            case 0: 
                randomDirection = "UpLeft";
                break;
            case 1: 
                randomDirection = "UpRight";
                break;
            case 2: 
                randomDirection = "Right";
                break;
            case 3:
                randomDirection = "DownRight";
                break;
            case 4: 
                randomDirection = "DownLeft";
                break;
            case 5:
                randomDirection = "Left";
                break;
        }
        return randomDirection;
    }
    
    void GenerateHexPath() {
        for (var i = -hexWidth; i <= hexWidth; i++) {
            tiling = Instantiate(tileRef, tileMap.WorldToCell(new Vector3(i, 0, 0)), Quaternion.identity, this.transform);
            tiling.x = i;
            tiling.z = 0;
            tiling.name = "x:" + tiling.x.ToString() + " " + "z:" + tiling.z.ToString();
            tiling2 = tiling;
            tiling3 = tiling;
            for (var j = 0; j <= hexLength; j++) {
                tiling2 = Instantiate(tileRef, tileMap.CellToWorld(tileMap.WorldToCell(tiling2.transform.position) + FindHexDirection(tileMap.WorldToCell(tiling2.transform.position), "UpRight")), Quaternion.identity, this.transform);
                tiling2.z = j + 1;
                tiling2.x = i;
                tiling2.name = "x:" + tiling2.x.ToString() + " " + "z:" + tiling2.z.ToString();
            }
            for (var j = 0; j <= hexLength; j++) {
                tiling3 = Instantiate(tileRef, tileMap.CellToWorld(tileMap.WorldToCell(tiling3.transform.position) + FindHexDirection(tileMap.WorldToCell(tiling3.transform.position), "DownLeft")), Quaternion.identity, this.transform);
                tiling3.z = -j - 1;
                tiling3.x = i;
                tiling3.name = "x:" + tiling3.x.ToString() + " " + "z:" + tiling3.z.ToString();
            }
        }
    }

    public int CalculateDistance(HexTile beginning, HexTile end) {
        var dx = Mathf.Abs(beginning.x - end.x);
        var dy = Mathf.Abs(beginning.y - end.y);
        var dz = Mathf.Abs(beginning.z - end.z);
        beginning.H = Mathf.Max(dx, dy, dz);
        return beginning.H;
    }

    public void FindAdjacentPath(HexTile point) {
        detective = Instantiate(tile_detector, tileMap.CellToWorld(tileMap.WorldToCell(point.transform.position)), Quaternion.identity);
        destination_tile.sorted = false;
    }

    public void FindPath() {
        int valid = 0;
        temp = openTiles[0];
        closedTiles.Add(openTiles[0]);
        openTiles.Remove(openTiles[0]);
        foreach (HexTile tile in temp.adjacent_tiles) {
            if (tile.walkable && !tile.considered) {
                valid++;
            }
        }
        if (valid == 0) {
            FindPath();
        }
    }

    public void FindFinalPath() {
        finalPath.Add(destination_tile);
        for (int i = destination_tile.G - 1; i >= 0; i--) {
            destination_tile = closedTiles.Find(x => x.G == i && destination_tile.adjacent_tiles.Contains(x));
            finalPath.Add(destination_tile);
        }
        finalPath.Reverse();
    }

    public void ResetPath() {
        foreach (HexTile tile in openTiles) {
            tile.G = 0;
            tile.H = 0;
            tile.considered = false;
        }
        foreach (HexTile tile in closedTiles) {
            tile.G = 0;
            tile.H = 0;
            tile.considered = false;
        }
        openTiles.Clear();
        closedTiles.Clear();
        finalPath.Clear();
        destination_tile.sorted = false;
        destination_tile.sorted2 = false;
        ovMove.moving = false;
        destination_tile = null;
        temp = null;
        steps_taken = 0;
    }

    // Update is called once per frame
    void Update() {
        mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        if (Input.GetKeyDown("q")) {
            GenerateHexPath();
        }
        if (Input.GetKeyDown("w")) {
            FindPath();
        }
    }
}
