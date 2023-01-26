using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    Vector3 shift;
    public List<Card> StaticDeck;
    public List<Card> PlayDeck = new List<Card> {};
    public List<Card> PlayDeckCopy = new List<Card> {};
    public Queue<Card> DeckOrder = new Queue<Card> {};
    public List<Card> DiscardPile = new List<Card> {};
    public Card TopCard = null;
    Card cardLoad = null;
    public Card removing = null;
    public int removeIndex;


    // Start is called before the first frame update
    void Start() {
        FillPlayDeck();
        ShuffleDeck();
    }

    //Play Deck is the deck used for encounters.
    public void FillPlayDeck() {
        PlayDeck.Clear();
        for (var i = 0; i <= StaticDeck.Count - 1; i++) {
            PlayDeck.Add(StaticDeck[i]);
            DiscardPile.Clear();
            DeckOrder.Enqueue(StaticDeck[i]);
        }
        ShuffleDeck();
    }

    private void ShuffleDeck() {
        PlayDeck.TrimExcess();
        DeckOrder.Clear();
        for (var i = 0; i <= PlayDeck.Count - 1; i++) {
            PlayDeckCopy.Add(PlayDeck[i]);
        }
        var loops = PlayDeckCopy.Count - 1;
        for (var i = 0; i <= loops; i++) {
            int randomCard = Random.Range(0, PlayDeckCopy.Count);
            DeckOrder.Enqueue(PlayDeckCopy[randomCard]);
            PlayDeckCopy.Remove(PlayDeckCopy[randomCard]);
            PlayDeckCopy.TrimExcess();
        }
        
    }

    private void DiscardTopCard() {
        if (DeckOrder.Peek() == null) {return;}
        if (DeckOrder.Count == 1) {
            DiscardPile.Add(DeckOrder.Peek());
            PlayDeck.Remove(DeckOrder.Peek());
            DeckOrder.Dequeue();
            DeckOrder.Clear();
            DeckOrder.Enqueue(null);
        } else {
            DiscardPile.Add(DeckOrder.Peek());
            PlayDeck.Remove(DeckOrder.Peek());
            DeckOrder.Dequeue();
        }
    }

    //Static Deck is what shows in the deck list in the collection menu; does not change throughout encounters.
    public void AddtoStaticDeck(string cardname) {
        var prefabLoad = LoadPrefabFromFile(cardname);
        cardLoad = (Card) Instantiate(prefabLoad, transform.position, Quaternion.identity, this.transform);
        cardLoad.transform.GetChild(1).gameObject.SetActive(true);
        cardLoad.transform.GetChild(3).gameObject.SetActive(false);
        if (StaticDeck.Count == 0) {
            StaticDeck.Clear();
            PlayDeck.Clear();
        }
        StaticDeck.Insert(StaticDeck.Count, cardLoad);
        cardLoad.gameObject.SetActive(false);
    }

    public int RemoveFromStaticDeck(string cardname) {
        var loops = StaticDeck.Count - 1;
        for (var i = 0; i <= loops; i ++) {
            removing = (Card) GameObject.Find("Deck List").transform.GetChild(i).gameObject.GetComponent<Card>();
            if ((cardname.ToString()) == removing.ToString()) {
                removeIndex = i;
                return removeIndex;
            }
        }
        return -1;
    }

    public Card LoadPrefabFromFile(string filename) {
        var loadedObject = Resources.Load<Card>("Prefabs/Card Factory/Card Library/" + filename);
        
        if (loadedObject == null) {
            Debug.Log("NOT WORKING");
        }
        return loadedObject;
    }

    void Update() {
        if (DeckOrder.Count != 0) {
            TopCard = DeckOrder.Peek();
            if (DeckOrder.Peek() == null) {
                if (TopCard != null) {
                    TopCard.gameObject.SetActive(false); 
                }
            }   
        }
        if (Input.GetKeyUp("space") == true) {
            FillPlayDeck();
        }
        if (Input.GetKeyUp("d") == true) {
            DiscardTopCard();
        }
    }
}
