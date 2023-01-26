using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CodeMonkey.Utils;

public class DragDrop : MonoBehaviour, 
                        IPointerDownHandler,
                        IPointerUpHandler,
                        IBeginDragHandler, 
                        IEndDragHandler, 
                        IDragHandler,
                        IPointerEnterHandler,
                        IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Card cardRef;
    private Hand handRef;
    private Vector3 initialPosition;
    public bool draggable = true;
    [SerializeField] AudioClip returnToHandAudioClip;
    

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        cardRef = GetComponent<Card>();
        canvas = FindObjectOfType<Canvas>();
        handRef = FindObjectOfType<Hand>();
        /*if (cardRef.isInCollectionPreview || cardRef.isInDeckListPreview ) {
            this.GetComponent<DragDrop>().enabled = false;
        } else {this.GetComponent<DragDrop>().enabled = true;}*/
    }

    // If you're dragging and right-click, send the card back to the hand
    public void OnPointerDown(PointerEventData eventData) {
        //Debug.Log("OnPointerDown");
        if (Input.GetMouseButtonDown(1) && !cardRef.isInHand) {
            AudioSource.PlayClipAtPoint(returnToHandAudioClip, Camera.main.transform.position);
            draggable = false;
            cardRef.isDragging = false;
            rectTransform.localPosition = initialPosition;
            rectTransform.localRotation = new Quaternion(0,0,0,0);
        }
        if (!cardRef.isInPlayZone)
            draggable = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        // Debug.Log("OnPointerUp");
    }

    public void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log("OnPointerEnter");
        if (cardRef.transform.localPosition.y <= 1 && !cardRef.isDragging && cardRef.isInHand) {
            // Preview the Card in Hand
            cardRef.isPreviewing = true;
            cardRef.transform.rotation = Quaternion.identity;
            cardRef.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            cardRef.savedParentPosition = cardRef.transform.parent.transform.localPosition;
            cardRef.transform.parent.transform.localPosition = new Vector3(
                cardRef.transform.parent.transform.localPosition.x,
                60,
                cardRef.transform.parent.transform.localPosition.z
            );
        }
    }
    // Reset the size of the card after your pointer leaves
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit");
        if (cardRef.isInHand && !cardRef.isInPlayZone) {
            cardRef.transform.localScale = new Vector3(1, 1, 1);
            cardRef.ResetParentTransform();
            cardRef.transform.rotation = new Quaternion(0,0,0,0);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //Debug.Log("OnBeginDrag");
        if (draggable) {
            cardRef.transform.localScale = new Vector3(1,1,1);
            cardRef.isInHand = false;
            cardRef.isPreviewing = false;
            cardRef.isDragging = true;
            cardRef.ResetParentTransform();
        }
        
    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("OnDrag");
        if (draggable && cardRef.isDragging) {
            this.transform.position = eventData.position;
        }
        else if (!draggable) {
            eventData.pointerDrag = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        // Debug.Log("OnEndDrag: " + eventData.position);
        cardRef.isInHand = true;
        if (cardRef.isBorderHighlighted && cardRef.isDragging) {
            cardRef.PlayCardFromHand();
        }
        cardRef.isDragging = false;
        cardRef.UnHighlightBorder();
        cardRef.SetTargetTransform(cardRef.targetTransform, cardRef.targetQuaternion);
    }

}
// cardRef.PlayCard();