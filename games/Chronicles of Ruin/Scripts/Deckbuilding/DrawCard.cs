using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DrawCard : MonoBehaviour
{
    public GameObject cardToDraw;
    public GameObject Hand;
    private Hand hand;
    private Deck deckRef;

    private void Start() {
        hand = FindObjectOfType<Hand>();
        deckRef = FindObjectOfType<Deck>();
    }

    public void DrawNewCard(Card cardToDraw) {
        if (hand.handState == HandState.IDLE) {
            hand.handState = HandState.ORGANIZING;
            deckRef.TopCard = deckRef.DeckOrder.Peek();
            if (deckRef.DeckOrder.Count == 1) {
                cardToDraw = deckRef.TopCard;
                cardToDraw.gameObject.SetActive(true);
                deckRef.DeckOrder.Dequeue();
                cardToDraw.isInHand = true;
                hand.DrawCard(cardToDraw);
                deckRef.DeckOrder.Clear();
                deckRef.TopCard = null;
            } else {
                cardToDraw = deckRef.TopCard;
                cardToDraw.gameObject.SetActive(true);
                deckRef.DeckOrder.Dequeue();
                cardToDraw.isInHand = true;
                hand.DrawCard(cardToDraw);
            }
            
            //handOrganizer.Organize();
        }
        
    }
}
