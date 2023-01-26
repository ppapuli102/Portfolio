using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour,
                    IPointerDownHandler,
                    IPointerEnterHandler,
                    IPointerExitHandler
{
    #region Class Variables

    [Header("Card Stats")]
    public int range = 0;
    public int cost = 0;

    [Header("States")]
    public bool isPreviewing = false;
    public bool isDragging = false;
    public bool isInHand = true;
    public bool isBorderHighlighted = false;
    public bool isInCollectionPreview = false;
    public bool isInDeckListPreview = false;
    public bool isInPlayZone = false;
    public bool cardIsMoving = false;

    [Header("References")]
    private Hand handRef;
    private Collection collRef;
    private Deck deckRef;
    private Card cardPreview;
    private CardPreview previewRef;
    private DragDrop dragRef;
    public GameObject dlTransform;

    [Header("Variables")]
    public int defaultSpeed = 15;
    [SerializeField] int playZoneThreshold = 125;
    public Vector3 savedParentPosition;
    public GameObject targetAOE;

    [Header("Transform")]
    private RectTransform rectTransform;
    public Vector3 currentPosition;
    public Transform targetTransform;
    public Quaternion targetQuaternion;

    #endregion

    private void Awake() {
        handRef = FindObjectOfType<Hand>();
        rectTransform = GetComponent<RectTransform>();
        previewRef = FindObjectOfType<CardPreview>();
        collRef = FindObjectOfType<Collection>();
        deckRef = FindObjectOfType<Deck>();
        dragRef = GetComponent<DragDrop>();
        
        dlTransform = GameObject.Find("Deck List");
    }

    private void Start() {
        GetCardPosition();
        dlTransform = this.transform.root.transform.GetChild(0).transform.GetChild(0).transform.Find("Deck List").gameObject;
    }

    private void Update() {
        if (collRef.CollectionOpen == true) {
            if (!isInDeckListPreview && !isInHand && !isDragging) {
                isInCollectionPreview = true;
                dragRef.enabled = false;
            }
        } else {
            isInCollectionPreview = false;
            dragRef.enabled = true;    
        }
        if (Input.GetMouseButtonDown(1) && !cardIsMoving && isInPlayZone) {
            ResetActiveCard();
        }
    }

    public void ResolveEffect() {
        Debug.Log("RESOLVING");
    }

    private void ResetActiveCard() {
        previewRef.SetActiveCard(null);
        isInPlayZone = false;
        SetTargetTransform(this.transform.parent, this.transform.parent.rotation);
        isInHand = true;
        handRef.targetingRef.StopTargeting();
    }

    public void PlayCardFromHand() {
        if (!previewRef.activeCard)
            StartCoroutine(handRef.PlayCard(this));
    }

    public void PlayCard_callback() {
        cardIsMoving = false;
        isInPlayZone = true;
        dragRef.draggable = false;
        isInHand = false;
    }

// Hand Smoothing

    // Move this Card to its Target position/rotation 
    public IEnumerator ResetPosition(Action ResetPosition_callback, Transform targetTransform, Quaternion targetQuaternion) {
        if (this.rectTransform != null && previewRef.activeCard != this){
            cardIsMoving = true;
            while (Vector3.Distance(rectTransform.position, targetTransform.position) > 0.05) {
                // Debug.Log(Quaternion.Angle(transform.rotation, targetQuaternion));
                rectTransform.position = Vector3.Lerp(rectTransform.position, targetTransform.position, defaultSpeed * Time.deltaTime);
                rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, targetQuaternion, 10*defaultSpeed * Time.deltaTime);
                yield return null;
            }
            ResetPosition_callback();
        }
    }

    private void ResetPosition_callback() {
        cardIsMoving = false;
    }

    // Set Target Position of this Card object
    public void SetTargetTransform(Transform targetPosition, Quaternion targetRotation) {
        Action callback = ResetPosition_callback;
        targetTransform = targetPosition;
        targetQuaternion = targetRotation;
        StopCoroutine(ResetPosition(callback, targetTransform, targetQuaternion));
        StartCoroutine(ResetPosition(callback, targetTransform, targetQuaternion));
    }
    public void SetTargetTransform(Action callback, Transform targetPosition, Quaternion targetRotation) {
        targetTransform = targetPosition;
        targetQuaternion = targetRotation;
        StopCoroutine(ResetPosition(callback, targetTransform, targetQuaternion));
        StartCoroutine(ResetPosition(callback, targetTransform, targetQuaternion));
    }

    // Reset Parent Transform back to its initial position after previewing
    public void ResetParentTransform() {
        if (isInHand) {
            isPreviewing = false;
            transform.parent.transform.localPosition = savedParentPosition;
        }
        if (!isInHand) {
            transform.parent.transform.localPosition = savedParentPosition;
        }
    }

    // Return current position of this Card object
    public Vector3 GetCardPosition() {
        currentPosition = rectTransform.position;
        return currentPosition;
    }

    // Enable Keyword Helper gameObject
    public void EnableKeyWord() {
        this.transform.GetChild(2).gameObject.SetActive(true);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(1) && isInHand && !isInPlayZone)
        {
            // Pass this Card as a reference to create a preview
            previewRef.CreateReference(this);
        }
        if (!isInHand) {
            if (isInCollectionPreview) {
                AddToDeck(this, true);
            } else if (isInDeckListPreview) {
                RemoveFromDeck(this, true);
            }
        }
    }

// Add and remove from collection to Deck

    // Add to deck from collection
    public void AddToDeck(Card add, bool from_collection) {
        if (collRef.DeckList[0] == null) {collRef.DeckList.Clear();}
        collRef.DeckList.Add(add);
        deckRef.AddtoStaticDeck(add.transform.GetComponentInChildren<FillCard>().CardtoFill.cardName);
        if (from_collection == true) {
            collRef.CardCollection.Remove(add);    
            if (collRef.filtering == true) {
                collRef.CardCollectionActive.Remove(add);
                collRef.PopulateFilter();
            } else {collRef.PopulateCollection();}    
        }
        isInCollectionPreview = false;
        isInDeckListPreview = true;
        add.transform.GetChild(1).gameObject.SetActive(false);
        add.transform.GetChild(3).gameObject.SetActive(true);
        add.transform.SetParent(dlTransform.transform);
        collRef.PopulateDeckList();
        deckRef.FillPlayDeck();
    }
    // Remove card from the decklist
    public void RemoveFromDeck(Card rem, bool to_collection) {
        collRef.DeckList.Remove(rem);
        deckRef.removeIndex = deckRef.RemoveFromStaticDeck(rem.ToString());
        Destroy(deckRef.transform.GetChild(1 + deckRef.removeIndex).gameObject);
        deckRef.StaticDeck.Remove(deckRef.StaticDeck[deckRef.removeIndex]);
        deckRef.PlayDeck.Remove(deckRef.PlayDeck[deckRef.removeIndex]);
        if (collRef.DeckList.Count == 0) {
            collRef.DeckList.Add(null);
        }
        if (to_collection == true) {
            collRef.CardCollection.Add(rem);
            isInDeckListPreview = false;
            isInCollectionPreview = true;
            if (collRef.filtering == true) {
                if (collRef.filteringByTypes.Contains(rem.transform.GetComponentInChildren<FillCard>().CardtoFill.cardType.ToString())) {
                    collRef.CardCollectionActive.Add(rem);
                    rem.transform.GetChild(1).gameObject.SetActive(true);
                    rem.transform.GetChild(3).gameObject.SetActive(false);
                    collRef.PopulateFilter();
                } else {
                    rem.transform.GetChild(1).gameObject.SetActive(true);
                    rem.transform.GetChild(3).gameObject.SetActive(false);
                    rem.gameObject.SetActive(false);
                }
            } else if (collRef.filtering == false) {
                rem.transform.GetChild(1).gameObject.SetActive(true);
                rem.transform.GetChild(3).gameObject.SetActive(false);
                collRef.PopulateCollection();
            }
            previewRef.DestroyPreview();
            rem.transform.SetParent(GameObject.Find("Collection List").transform);
        } else if (to_collection == false) {
            collRef.DeckList.Remove(rem);
            DestroyFromEquip(deckRef.removeIndex);
        }
        collRef.PopulateDeckList();
        deckRef.FillPlayDeck();        
    }

    public void DestroyFromEquip(int index) {
        Destroy(collRef.transform.GetChild(1).transform.GetChild(index).gameObject);
    }

// Create a preview of a card while mousing over it in the deck list preview in collections

    public void OnPointerEnter(PointerEventData eventData) {
        if (isInDeckListPreview) {
            previewRef.CreateReference(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (isInDeckListPreview) {
            previewRef.DestroyPreview();
        }
    }

// Highlighting Card Border while Being Played

    // Set the card border child active on the Card object
    public void HighlightBorder() {
        Card[] cards = FindObjectsOfType<Card>();
        isBorderHighlighted = true;
        foreach (Card card in cards)
        {
            card.transform.GetChild(0).gameObject.SetActive(false);
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    // Set the card border child inactive on the Card object
    public void UnHighlightBorder() {
        isBorderHighlighted = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Highlight the card when entering the play zone from hand
    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Hand" && isDragging && transform.position.y > playZoneThreshold) {
            if (!isBorderHighlighted) {
                HighlightBorder();
            }
        }
    }

    // Unhighlight the selected card when moving card from play zone to hand
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Hand" && isDragging) {
            if (isBorderHighlighted) {
                UnHighlightBorder();
            }
        }
    }

}







