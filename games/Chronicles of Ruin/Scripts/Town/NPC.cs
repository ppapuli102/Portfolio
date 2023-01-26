using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{

    // [SerializeField] private TownManager townManager;

    [Header("UI References")]
    public TMP_Text textLabel;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private GameObject clickToContinueMouse;

    [Header("Dialogue")]
    [SerializeField] private DialogueObject npcDialogue;
    [SerializeField] private GameObject dialogueBox;

    [Header("SFX")]
    [SerializeField] private AudioClip textSFX;
    [SerializeField] private float textSFXVolume = 0.8f;
    [SerializeField] private float textSFXDelay;

    private TypewriterEffect typewriterEffect;
        

    private void Start() {
        typewriterEffect = GetComponent<TypewriterEffect>();
        // CloseDialogueBox();
        StartDialogue(npcDialogue);
    }

    public void StartDialogue(DialogueObject dialogueObject) {
        dialogueBox.SetActive(true);
        TownManager.instance.ToggleSpeechBubble();
        clickToContinueMouse.SetActive(false);
        exitButton.interactable = false;
        interactionPanel.SetActive(false);

        // Display the dialogue with a delay
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject) {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i ++) {
            // Current dialogue text
            string dialogue = dialogueObject.Dialogue[i];
            
            yield return StartTypingEffect(dialogue);

            textLabel.text = dialogue;

            yield return null;
            clickToContinueMouse.SetActive(true);
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            clickToContinueMouse.SetActive(false);
        }

        CloseDialogueBox();
    }

    private IEnumerator StartTypingEffect(string dialogue) {
        typewriterEffect.RunEffect(dialogue, textLabel);

        while (typewriterEffect.isRunning) {
            if (!typewriterEffect.pausingDialogue) {
                yield return StartCoroutine(PlayDialogueAudio());
            }
            else if (typewriterEffect.pausingDialogue) {
                yield return null;
            }
        }
    }

    public IEnumerator PlayDialogueAudio() {
        AudioSource.PlayClipAtPoint(textSFX, Camera.main.transform.position, textSFXVolume);
        yield return new WaitForSeconds(textSFXDelay);
    }

    private void CloseDialogueBox() {
        dialogueBox.SetActive(false);
        exitButton.interactable = true;
        interactionPanel.SetActive(true);
        textLabel.text = string.Empty;
    }

}
