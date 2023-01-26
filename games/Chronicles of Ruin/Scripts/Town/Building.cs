using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private SpriteRenderer glow;
    [SerializeField] private BuildingMenu menu;
    [SerializeField] private GameObject speechBubble;
    // private TownManager townManager;

    private void Start() {
        // townManager = FindObjectOfType<TownManager>();
    }

    private void OnMouseEnter() {
        // Debug.Log("Mouse Enter");
        if (!menu.isActiveAndEnabled) {
            glow.color = new Color(1f,1f,1f,0.07f);
        }
    }

    private void OnMouseExit() {
        // Debug.Log("Mouse Exit");
        if (!menu.isActiveAndEnabled) {
            glow.color = new Color(1f,1f,1f,0f);
        }
    }

    private void OnMouseDown() {
        // Debug.Log("Mouse Down");
        if (!menu.isActiveAndEnabled) {
            menu.gameObject.SetActive(true);
            TownManager.instance.ToggleVentureButton();
        }
    }

    public void CloseSpeechBubble() {
        speechBubble.SetActive(false);
    }

    public void OpenSpeechBubble() {
        speechBubble.SetActive(true);
    }

}
