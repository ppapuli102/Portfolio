using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TownManager : MonoBehaviour
{

    public static TownManager instance;

    public ResourceManager resourceManager;

    [Header("UI References")]
    [SerializeField] private Button ventureButton;
    [SerializeField] private SpriteRenderer speechBubble;
    [SerializeField] private Building building;

    public bool isMenuOpen = false;


    // Set the instance to the town manager in heirarchy
    private void Awake() {
        instance = FindObjectOfType<TownManager>();
        resourceManager = GetComponent<ResourceManager>();
    }

    public void ToggleVentureButton() {
        // toggle button OFF on menu is opened
        if (!isMenuOpen) {
            ventureButton.interactable = false;
            isMenuOpen = true;
        }
        // toggle button ON if menu is closed
        else if (isMenuOpen) {
            ventureButton.interactable = true;
            isMenuOpen = false;
        }
    }

    public void ToggleSpeechBubble() {
        if (speechBubble.gameObject.activeInHierarchy) {
            building.CloseSpeechBubble();
        }
        else if (!speechBubble.gameObject.activeInHierarchy) {
            building.OpenSpeechBubble();
        }
    }

    private void OnDestroy() {
        resourceManager.SaveResources();
    }

}
