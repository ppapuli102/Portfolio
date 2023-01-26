using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUpdater : MonoBehaviour
{
    
    private TMP_Text textLabel;

    private void Start() {
        textLabel = GetComponent<TMP_Text>();
    } 

    public void UpdateText(string text) {
        textLabel.text = text;
    }

}
