using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{

    [SerializeField] [Range(10, 50)] private float textSpd;
    private Coroutine typingCoroutine;

    public bool isRunning { get; private set; }
    public bool pausingDialogue { get; private set; }

    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>() {
        {new HashSet<char>(){'.', '!', '?'}, 0.6f},
        {new HashSet<char>() {',', ';', ':'}, 0.3f},
    };

    private void Update() {
        // Press Space to skip the typewriter effect
        ReadStopEffect();
    }

    private void ReadStopEffect() {
        if (isRunning) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                StopEffect();
            }
        }
    }

    public void RunEffect(string textToType, TMP_Text textLabel) {
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    public void StopEffect() {
        StopCoroutine(typingCoroutine);
        isRunning = false;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel) {
        isRunning = true;
        textLabel.text = string.Empty;
        float t = 0;
        int charIndex = 0;

        // Display the letters from our text one by one until we've made it through the entire text block
        while (charIndex < textToType.Length) {
            int lastCharIndex = charIndex;

            t += Time.deltaTime * textSpd;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++) {
                bool isLast = i >= textToType.Length - 1;

                textLabel.text = textToType.Substring(0, i + 1);

                // Pause when we reach a punctuation mark
                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i+1], out _)) {
                    pausingDialogue = true;
                    yield return new WaitForSeconds(waitTime);
                    pausingDialogue = false;
                }
            }

            yield return null;
        }

        isRunning = false;
    }

    private bool IsPunctuation(char character, out float waitTime) {
        foreach (KeyValuePair<HashSet<char>, float> punctuationCategory in punctuations) {
            if (punctuationCategory.Key.Contains(character)) {
                waitTime = punctuationCategory.Value;
                return true;
            }
        }
        waitTime = default;
        return false;
    }


}
