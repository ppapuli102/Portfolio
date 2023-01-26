using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthTracker : MonoBehaviour
{
    public Image[] images;
    [SerializeField] [Range(0,1)] float opacity = 0.65f;
    private int i = 2;

    private void AlterOpacity(Image image, float opacity)
    {
        var tempColor = image.color;
        tempColor.a = opacity;
        image.color = tempColor;        
    }

    public void reduceHealthUI(int damage)
    {
        Debug.Log(images[0]);
        int currentHealth = Player.health;
        if (damage == 200 && currentHealth >= 100)
        {
            StartCoroutine("FlashTwiceThenDestroyTWO", i);
            i -=2;
        }
        else
        {
            StartCoroutine("FlashTwiceThenDestroy", i);
            i --;
        }
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

    private IEnumerator FlashTwiceThenDestroyTWO()
    {
        AlterOpacity(images[i], 0f);
        AlterOpacity(images[i-1], 0f);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], opacity);
        AlterOpacity(images[i-1], opacity);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], 0f);
        AlterOpacity(images[i-1], 0f);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], opacity);
        AlterOpacity(images[i-1], opacity);
        yield return new WaitForSeconds(0.2f);
        AlterOpacity(images[i], 0f);
        AlterOpacity(images[i-1], 0f);
    }
}
