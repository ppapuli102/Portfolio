using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogGrid : MonoBehaviour
{
    private int grid_width, grid_height;
    private Vector3Int to_set, sight_set, max_sight, mouseCell, heroPos;
    public Tilemap fGrid;
    public Tile tile_black, tile_gray, visible;
    private OverworldMovement ovMove;
    private HexGrid overworld;

    // Start is called before the first frame update
    void Start() {
        grid_width = 11;
        grid_height = 8;
        visible = null;
        fGrid = GetComponent<Tilemap>();
        ovMove = FindObjectOfType<OverworldMovement>();
        overworld = FindObjectOfType<HexGrid>();
        heroPos = ovMove.heroMove;
        InitializeFog();
    }

    public void InitializeFog() {
        for (var i = -grid_width; i < grid_width; i++) {
            for (var j = -grid_height; j < grid_height; j++) {
                to_set = new Vector3Int(i, j, 0);
                fGrid.SetTile(to_set, tile_black);
            }
        }
        //Reveal hero sight range
        RevealSight(heroPos, ovMove.sight_range, visible);
    }

    public void RevealSight(Vector3Int cell, int range, Tile tile_opacity) {
        fGrid.SetTile(cell, tile_opacity);
        to_set = overworld.FindHexDirection(cell, "UpLeft");
        fGrid.SetTile(cell + to_set, tile_opacity);
        if (range >= 2) {
            sight_set = overworld.FindHexDirection(cell + to_set, "UpLeft");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "UpLeft");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "UpRight");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Left");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "UpRight");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >=3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "UpLeft");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "UpRight");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "Left");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Left");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
        }
        to_set = overworld.FindHexDirection(cell, "UpRight");
        fGrid.SetTile(cell + to_set, tile_opacity);
        if (range >= 2) {
            sight_set = overworld.FindHexDirection(cell + to_set, "UpLeft");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            sight_set = overworld.FindHexDirection(cell + to_set, "UpRight");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "UpRight");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Right");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "Right");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Right");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
        }
        to_set = overworld.FindHexDirection(cell, "Right");
        fGrid.SetTile(cell + to_set, tile_opacity);
        if (range >= 2) {
            sight_set = overworld.FindHexDirection(cell + to_set, "UpRight");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            sight_set = overworld.FindHexDirection(cell + to_set, "Right");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Right");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "DownRight");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Right");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "DownRight");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
        }
        to_set = overworld.FindHexDirection(cell, "DownRight");
        fGrid.SetTile(cell + to_set, tile_opacity);
        if (range >= 2) {
            sight_set = overworld.FindHexDirection(cell + to_set, "Right");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            sight_set = overworld.FindHexDirection(cell + to_set, "DownRight");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "DownRight");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "DownLeft");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "DownRight");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "DownLeft");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
        }
        to_set = overworld.FindHexDirection(cell, "DownLeft");
        fGrid.SetTile(cell + to_set, tile_opacity);
        if (range >= 2) {
            sight_set = overworld.FindHexDirection(cell + to_set, "Left");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "DownLeft");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Left");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "DownLeft");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "DownLeft");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "DownRight");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
        }
        to_set = overworld.FindHexDirection(cell, "Left");
        fGrid.SetTile(cell + to_set, tile_opacity);
        if (range >= 2) {
            sight_set = overworld.FindHexDirection(cell + to_set, "UpLeft");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            sight_set = overworld.FindHexDirection(cell + to_set, "Left");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
            if (range >= 3) {
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "Left");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
                max_sight = overworld.FindHexDirection(cell + to_set + sight_set, "UpLeft");
                fGrid.SetTile(cell + to_set + sight_set + max_sight, tile_opacity);
            }
            sight_set = overworld.FindHexDirection(cell + to_set, "DownLeft");
            fGrid.SetTile(cell + to_set + sight_set, tile_opacity);
        }
    }

    // Update is called once per frame
    void Update() {
        mouseCell = fGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        heroPos = ovMove.heroMove;
        fGrid.SetTile(heroPos, null);
    }
}
