using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillCard : MonoBehaviour
{
    public CardConstructor CardtoFill;

    public Text Description;
    public Text Name;
    public Text NameDeck;
    public Text Type;

    public Image Image;
    public Image TypeArt;
    public Image TypeDeck;

    public Text Cost;
    public Text CostDeck;

    // Start is called before the first frame update
    void Start()
    {
        Description.text = CardtoFill.cardDescription;
        Name.text = CardtoFill.cardName;
        NameDeck.text = CardtoFill.cardName;
        Type.text = CardtoFill.cardType;

        Image.sprite = CardtoFill.cardArt;
        TypeArt.sprite = CardtoFill.cardTypeArt;
        TypeDeck.sprite = CardtoFill.cardTypeArt;

        Cost.text = CardtoFill.cardCost.ToString();
        CostDeck.text = CardtoFill.cardCost.ToString();
    }

}
