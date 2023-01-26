using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    
    public List<Card> CardCollection = new List<Card> {};
    public List<Card> CardCollectionActive = new List<Card> {};
    public List<Card> DeckList = new List<Card> {};
    public bool CollectionOpen = false, filtering = false;
    public Vector3 cardShift = new Vector3(0, 0, 0);
    float cardShiftX, cardShiftY, cardShiftZ;
    public string type;
    public string typeRef;
    public string typeRefLast = null;
    public List<string> filteringByTypes = new List<string> {};
    public string[] cardTypes = new string[0];
    Collection[] ParentObject = new Collection[1];
    Collection Parent;
    Deck deckRef;
    

    // Start is called before the first frame update
    void Start() {
        ParentObject = Resources.FindObjectsOfTypeAll<Collection>();
        deckRef = FindObjectOfType<Deck>();
        Parent = ParentObject[0];
        cardShiftX = Parent.transform.position.x - 150;
        cardShiftY = Parent.transform.position.y + 50;
        cardShiftZ = Parent.transform.position.z;
    }

    public void ToggleCollectionUI() {
        if (CollectionOpen == false) {
            CollectionOpen = true;
            filteringByTypes.Clear();
            var toggle = gameObject.transform.GetChild(0);
            toggle.transform.gameObject.SetActive(true);
            toggle = gameObject.transform.GetChild(1);
            toggle.transform.gameObject.SetActive(true);
            toggle = gameObject.transform.GetChild(2);
            toggle.transform.gameObject.SetActive(true);
        }
        else {
            CollectionOpen = false;
            filtering = false;
            var toggle = gameObject.transform.GetChild(0);
            toggle.transform.gameObject.SetActive(false);
            toggle = gameObject.transform.GetChild(1);
            toggle.transform.gameObject.SetActive(false);
            toggle = gameObject.transform.GetChild(2);
            toggle.transform.gameObject.SetActive(false);
        }
    }

    public void PopulateCollection() {
        if (CollectionOpen == true) {
            for (var i = 0; i <= CardCollection.Count - 1; i++) {
                cardShift.Set(cardShiftX + i*200, cardShiftY, cardShiftZ);
                CardCollection[i].transform.position = cardShift;
                CardCollection[i].gameObject.SetActive(true);
            }
        }
    }

    public void PopulateFilter() {
        for (var i = 0; i <= CardCollectionActive.Count - 1; i++) {
            cardShift.Set(cardShiftX + i*200, cardShiftY, cardShiftZ);
            CardCollectionActive[i].transform.position = cardShift;
        }
    }

    public void PopulateDeckList() {
        for (var i = 0; i <= DeckList.Count - 1; i++) {
            var index = DeckList.Count - 1;
            Vector3 deckShift = new Vector3(0, -50*i, 0);
            Vector3 deckOffset = new Vector3(0, 160, 0);
            if (DeckList[i] != null) {
                DeckList[i].transform.position = this.transform.GetChild(1).position + deckShift + deckOffset;
            }
        }
    }

    public void FilterByType(string type) {
        CardCollectionActive.Clear();
        //Untoggle filtering by cards of type if re-selected
        if (filteringByTypes.Contains(type)) {
            typeRefLast = null;
            filteringByTypes.Remove(type);
            for (var i = 0; i <= CardCollection.Count - 1; i++) {
                typeRef = CardCollection[i].GetComponentInChildren<FillCard>().CardtoFill.cardType.ToString();
                if (typeRef == type) {
                    CardCollection[i].gameObject.SetActive(false);
                    CardCollectionActive.Remove(CardCollection[i]);
                }
            }
            foreach (string typing in filteringByTypes) {
                for (var i = 0; i <= CardCollection.Count - 1; i++) {
                    typeRef = CardCollection[i].GetComponentInChildren<FillCard>().CardtoFill.cardType.ToString();
                    if (typeRef == typing) {
                        CardCollection[i].gameObject.SetActive(true);
                        CardCollectionActive.Add(CardCollection[i]);
                    }
                }
            }
            PopulateFilter();
            if (filteringByTypes.Count == 0) {
                PopulateCollection();
            }
            return;
        }
        //Add cards of "type" to filter list
        for (var i = 0; i <= CardCollection.Count - 1; i++) {
            typeRef = CardCollection[i].GetComponentInChildren<FillCard>().CardtoFill.cardType.ToString();
            if (typeRef == type) {
                CardCollection[i].gameObject.SetActive(true);
                CardCollectionActive.Add(CardCollection[i]);
            } else {
                CardCollection[i].gameObject.SetActive(false);
            }
        }
        //Re-add cards of "type" back into filter list
        foreach (string typing in filteringByTypes) {
            for (var i = 0; i <= CardCollection.Count - 1; i++) {
                typeRef = CardCollection[i].GetComponentInChildren<FillCard>().CardtoFill.cardType.ToString();
                if (typeRef == typing) {
                    CardCollection[i].gameObject.SetActive(true);
                    CardCollectionActive.Add(CardCollection[i]);
                }
            }
        }
        PopulateFilter();
        typeRefLast = type;
        filtering = true;
        if (!filteringByTypes.Contains(type)) {
            filteringByTypes.Add(type);
        }
    }

    // Update is called once per frame
    void Update() {
        if (filteringByTypes.Count == 0) {
            filtering = false;
        }
        if (Input.GetKeyUp("o") == true) {
            ToggleCollectionUI();
            PopulateCollection();
        }
    }
}
