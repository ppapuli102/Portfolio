using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image[] images;
    [SerializeField] [Range(0,1)] float opacity = 0.65f;
    private int i = 2;

    private void Awake() {
        int numPlayerHealth = FindObjectsOfType<PlayerHealth>().Length;
        if (numPlayerHealth > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void AlterOpacity(Image image, float opacity)
    {
        var tempColor = image.color;
        tempColor.a = opacity;
        image.color = tempColor;        
    }

    public void ReduceHealthUI()
    {
        Debug.Log(images[0]);        
        StartCoroutine("FlashTwiceThenDestroy", i);
        i --;
    }

    private IEnumerator FlashTwiceThenDestroy(int i)
    {
        AlterOpacity(images[i], 0f);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], opacity);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], 0f);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], opacity);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], 0f);
    }
}
