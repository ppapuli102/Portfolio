using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class OverworldManager : MonoBehaviour
{

    public static OverworldManager instance;

    public ResourceManager resourceManager;

    [SerializeField] Caravan caravan;

    private void Awake() {
        instance = FindObjectOfType<OverworldManager>();
        resourceManager = GetComponent<ResourceManager>();
    }

    public void DisplayCollection() {
        // Show the collection
    }

    public void CaravanChangeHealth(int amount) {
        caravan.ChangeHealth(amount);
    }

    public void CaravanUpdateSight(int visionRadius) {
        caravan.UpdateSightRange(visionRadius);
    }

    private void OnDestroy() {
        resourceManager.SaveResources();
    }

}
