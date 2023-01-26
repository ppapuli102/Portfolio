using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DepthUpdate : MonoBehaviour
{
    private SpriteRenderer rend;
    private Tilemap ground; 
    private GameObject gridRef;
    public Vector3Int groundPos, lastPos;
    public int depth;
    private Vector3 groundOffset;

    // Start is called before the first frame update
    void Start() {
        rend = GetComponent<SpriteRenderer>();
        gridRef = GameObject.Find("Ground Tilemap");
        ground = gridRef.GetComponent<Tilemap>();
        groundPos = ground.WorldToCell(gameObject.transform.position);
        lastPos = new Vector3Int (-999, -999, -999);
    }

    // Update is called once per frame
    void Update() {
        depth = -groundPos.y;
        rend.sortingOrder = depth;
        groundPos = ground.WorldToCell(gameObject.transform.position);
        if (groundPos != lastPos) {
            groundOffset = ground.CellToWorld(groundPos) + new Vector3(0.5f, .99f, 0);
            gameObject.transform.position = groundOffset;
        }
        lastPos = groundPos;
    }
}
