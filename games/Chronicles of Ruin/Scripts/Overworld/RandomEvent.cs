using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{

    private OverworldMovement movementRef;
    [SerializeField] GameObject caravan;

    // Start is called before the first frame update
    void Start() {
        movementRef = caravan.GetComponent<OverworldMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Caravan>() != null) {
            movementRef.random_event_trigger = true;
        }
    }

}
