using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniTypewriter : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private string text;

    private void Start() {
        StartCoroutine(SpeechBubbleText());
    }

    public IEnumerator SpeechBubbleText() {
        textLabel.text = string.Empty;
        // int charIndex = 0;
        // float t = 0;

        for (int i = 0; i <= text.Length; i ++) {
            textLabel.text = text.Substring(0, i);
            yield return new WaitForSeconds(.2f);
        }
    }


}
