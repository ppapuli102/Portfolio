using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardPreview : MonoBehaviour
{
    public bool hasPreview = false;
    // public int previewTimer = 0;
    private Card previewRef;
    private Card preview;
    private Vector3 previewShift = new Vector3(150, -100, 0);
    public Card activeCard;

    void Update() {
        // Destroy the big Card preview if you press either mouse button
        if (Input.GetMouseButtonDown(0)) {
            if (hasPreview) {
                DestroyPreview();
            }
        }
        if (preview != null && preview.isInDeckListPreview) {
            preview.transform.position = Input.mousePosition + previewShift;
        }
        // if (preview) {
        //     previewTimer++;
        // }
    }
    // Create reference within decklist collection
    public void CreateReference(Card cardToPreview) {
        previewRef = cardToPreview;
        if (preview) {
            DestroyPreview();
        }
        if (!previewRef.isInDeckListPreview) {
            CreatePreview();
        } else {
            CreatePreviewDeckList();
        }
    }
    
    // Create large card preview after right-clicking on a card in hand
    public void CreatePreview() {
        if (activeCard == null) {
            hasPreview = true;
            preview = Instantiate(previewRef, gameObject.transform.position, Quaternion.identity);
            preview.EnableKeyWord();
            preview.transform.SetParent(this.transform);
            //preview.transform.position = cardPreview.transform.parent.position;
            preview.gameObject.GetComponent<Card>().enabled = false;
            preview.gameObject.GetComponent<DragDrop>().enabled = false;
            // preview.isInHand = false;
            // preview.isPreviewing = false;
            Vector3 previewScale = new Vector3(2, 2, 2);
            preview.transform.localScale = previewScale;
            // previewTimer = 0;
        }
    }

    public void CreatePreviewDeckList() {
        preview = Instantiate(previewRef, Input.mousePosition + previewShift, Quaternion.identity);
        preview.EnableKeyWord();
        preview.transform.GetChild(1).gameObject.SetActive(true);
        preview.transform.GetChild(3).gameObject.SetActive(false);
        preview.transform.SetParent(this.transform);
        preview.gameObject.GetComponent<Card>().enabled = false;
        preview.gameObject.GetComponent<DragDrop>().enabled = false;
        hasPreview = true;
    }

    public void SetActiveCard(Card activeCard) {
        this.activeCard = activeCard;
    }

    public void DestroyPreview() {
        if (preview != null) {
            Destroy(preview.gameObject);
            hasPreview = false;
        }
    }

}
