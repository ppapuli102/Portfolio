using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "NewCard")]
public class CardConstructor : ScriptableObject
{
    
    public string cardName;
    public string cardDescription;
    public string cardType;

    public Sprite cardArt;
    public Sprite cardTypeArt;

    public int cardCost;
    
}
