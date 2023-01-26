using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EventGrid : MonoBehaviour
{
    public Tile enemy;
    public Tilemap eGrid;

    // Start is called before the first frame update
    void Start() {
        eGrid = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
