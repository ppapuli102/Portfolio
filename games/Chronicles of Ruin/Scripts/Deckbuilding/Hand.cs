using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public enum HandState {
    IDLE,
    ORGANIZING
}

public class Hand : MonoBehaviour
{
    [Header("Variables")]
    public int numCardsInHand = 0;
    public List<Card> cardsInHandList;

    [Header("References")]
    public HandState handState;
    public CardPreview previewRef;
    private Grid grid;
    public Targeting targetingRef;

    [Header("Targeting Box Colors")]
    [SerializeField] private Color targetingColorNeutral = new Color(0.772549f, 0.7635396f, 0f, 0.8f);
    [SerializeField] private Color targetingColorNull = new Color(255, 255, 255, 0);
    [SerializeField] private Color targetingColorValid = new Color(197, 6, 0, 199);


    private void Start() {
        handState = HandState.IDLE;
        RectTransform handRectTransform = GetComponent<RectTransform>();
        grid = FindObjectOfType<Grid>();
        float handWidth = handRectTransform.sizeDelta.x;
        previewRef = FindObjectOfType<CardPreview>();
        targetingRef = GetComponent<Targeting>();
    }

    private void Update() {
        
    }

    public void OrganizeHand() {
        for (int i = 0; i < numCardsInHand; i++) {
            cardsInHandList[i].transform.SetParent(gameObject.transform.GetChild(numCardsInHand-1).GetChild(i));
            cardsInHandList[i].SetTargetTransform(
                cardsInHandList[i].gameObject.transform.parent,
                cardsInHandList[i].gameObject.transform.parent.rotation
            );
        }
        handState = HandState.IDLE;
    }

    public void DrawCard(Card newCard) {
        if (!newCard.cardIsMoving && handState == HandState.ORGANIZING){
            numCardsInHand ++;
            AddCardToHand(newCard);
        }
    }

    private void AddCardToHand(Card newCard) {
        Transform spawnLocation = gameObject.transform.GetChild(numCardsInHand-1).GetChild(numCardsInHand-1);
        /*Card spawnedCard = 
            Instantiate(
                newCard, 
                spawnLocation.position, 
                spawnLocation.transform.rotation
            );*/
        newCard.transform.position = spawnLocation.transform.position;
        newCard.transform.rotation = spawnLocation.transform.rotation;
        cardsInHandList.Add(newCard);
        OrganizeHand();
    }

    public void RemoveCardFromHand(Card cardToRemove) {
        handState = HandState.ORGANIZING;
        cardsInHandList.Remove(cardToRemove);
        numCardsInHand -= 1;
        OrganizeHand();
    }

    public IEnumerator PlayCard(Card card) {
        // Move card from the hand to the play zone
        card.SetTargetTransform(card.PlayCard_callback, previewRef.transform, Quaternion.identity);
        previewRef.SetActiveCard(card);
        previewRef.DestroyPreview();
        // handRef.RemoveCardFromHand(this);
        yield return null;

        targetingRef.StartTargeting(card.targetAOE);
        // isInPlayZone = true;
        // isInHand = false;
    }

    
}