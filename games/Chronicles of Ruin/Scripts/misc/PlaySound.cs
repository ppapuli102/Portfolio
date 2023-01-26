using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{

    [SerializeField] AudioClip sfx;
    [SerializeField] float vol;
    [SerializeField] float wait;
    [SerializeField] int repeat;

    private Vector3 cameraPos = Camera.main.transform.position;

    public void StartPlaySFX() {
        StartCoroutine(PlaySFX());
    }

    private IEnumerator PlaySFX() {
        for (int i = 0; i < repeat; i++) {
            AudioSource.PlayClipAtPoint(sfx, cameraPos, vol);
            yield return new WaitForSeconds(wait);
        }
        yield return null;

        // SceneLoader.LoadScene("Overworld");
    }
}
