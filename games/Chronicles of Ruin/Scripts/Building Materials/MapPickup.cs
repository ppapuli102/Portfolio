using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapPickup : MonoBehaviour
{

    [SerializeField] resourceType type;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Caravan")) {
            OverworldManager.instance.resourceManager.AddResource(type.ToString(), 5);
            Destroy(this.gameObject);
        }
    }

}

public enum resourceType { coal, crystal, gold, loyalty, moonshine, timber }
