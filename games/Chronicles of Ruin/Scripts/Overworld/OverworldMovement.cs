using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OverworldMovement : MonoBehaviour
{
    public HexGrid Overworld;
    public EventGrid EventMap;
    public FogGrid Fog;
    public Vector3Int vectorMove, heroMove, mouseCell;
    public Vector3 heroPos;
    public int sight_range;
    public bool random_event_trigger = false;
    public bool enemy_event_trigger = false;
    public float move_speed;
    public bool moving = false;
    // private SceneLoader sceneManager;

    // Start is called before the first frame update
    void Start() {
        // sight_range = 3;
        Overworld = FindObjectOfType<HexGrid>();
        EventMap = FindObjectOfType<EventGrid>();
        Fog = FindObjectOfType<FogGrid>();
        // sceneManager = FindObjectOfType<SceneLoader>();
        heroPos = Overworld.tileMap.WorldToCell(gameObject.transform.position);
    }

    private void MoveHero(Vector3Int moveto) {
        if (Overworld.tileMap.HasTile(mouseCell) == true) {
            //Check for Enemy
            if (EventMap.eGrid.GetTile(mouseCell) == EventMap.enemy) {
                enemy_event_trigger = true;
            }
            // Update Fog of War tiles *DON'T Change the VARIABLES*
            Fog.RevealSight(heroMove, sight_range, Fog.tile_gray);
            heroMove = moveto;
            heroPos = Overworld.tileMap.CellToWorld(heroMove);
            Fog.RevealSight(heroMove, sight_range, Fog.visible);
        }
    }

    private void TryLoadEnemyEvent() {
        if (enemy_event_trigger) {
            enemy_event_trigger = false;
            SceneLoader.LoadTestScene();
        }
    }

    private void TryLoadRandomEvent() {
        if (random_event_trigger) {
            random_event_trigger = false;
            SceneLoader.LoadRandomEncounter();
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.GetComponent<HexTile>()) {
            if (!moving) {
                Overworld.heroTile = other.gameObject.GetComponent<HexTile>();
            }
        }
    }

    // Update is called once per frame
    void Update() {
        //Store the mouse position in terms of grid cell
        mouseCell = Overworld.tileMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButtonDown(0)) {
            MoveHero(mouseCell);
        }

        //Check for Event Triggers.
        TryLoadRandomEvent();

        //**Update hero position on the map**//
        
        heroPos = gameObject.transform.position;
        //Debug.Log(mouseCell);
    }

    void FixedUpdate() {
        if (Overworld.finalPath.Count > 0) {
            moving = true;
            Vector3 a = transform.position;
            Vector3 b = Overworld.finalPath[0].transform.position;
            transform.position = Vector3.Lerp(a, b, move_speed*Time.deltaTime);
            if (Vector3.Distance(transform.position, b) < 0.001f) {
                if (Overworld.destination_tile.reset_please) {
                    Overworld.destination_tile.reset_please = false;
                    Overworld.ResetPath();
                    return;
                }
                Overworld.finalPath.Remove(Overworld.finalPath[0]);
                if (Overworld.finalPath.Count == 0) {
                    moving = false;
                    Overworld.ResetPath();
                    if (enemy_event_trigger) {TryLoadEnemyEvent();}
                }
            }
        }
    }
}
