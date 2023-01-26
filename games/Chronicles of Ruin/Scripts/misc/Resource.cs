using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// public enum resourceType {CRYSTAL, COAL, GOLD, LOYALTY, MOONSHINE, TIMBER}

public class Resource : MonoBehaviour
{

    // private int initialAmount = 0;

    // public resourceType resource;
    
    // private void Start() {
    //     UpdateResourceAmount(0);
    // }

    // public void UpdateResourceAmount(int delta) {
    //     switch (resource)
    //     {
    //         case resourceType.CRYSTAL: 
    //             Crystal.amount += delta;
    //             IUpdateableResource crystal = FindObjectOfType<Crystal>();
    //             OverworldManager.instance.UpdateResourceText(crystal, Coal.amount.ToString());
                
    //         break;

    //         case resourceType.COAL:
    //             Coal.amount += delta;
    //             IUpdateableResource coal = FindObjectOfType<Coal>();
    //             OverworldManager.instance.UpdateResourceText(coal, Coal.amount.ToString());
    //         break;

    //         case resourceType.GOLD:
    //             Gold.amount += delta;
    //             IUpdateableResource gold = FindObjectOfType<Gold>();
    //             OverworldManager.instance.UpdateResourceText(gold, Coal.amount.ToString());
    //         break;

    //         case resourceType.LOYALTY:
    //             Loyalty.amount += delta;
    //             IUpdateableResource loyalty = FindObjectOfType<Loyalty>();
    //             OverworldManager.instance.UpdateResourceText(loyalty, Coal.amount.ToString());
    //         break;

    //         case resourceType.MOONSHINE:
    //             Moonshine.amount += delta;
    //             IUpdateableResource moonshine = FindObjectOfType<Moonshine>();
    //             OverworldManager.instance.UpdateResourceText(moonshine, Coal.amount.ToString());
    //         break;

    //         case resourceType.TIMBER:
    //             Timber.amount += delta;
    //             IUpdateableResource timber = FindObjectOfType<Timber>();
    //             OverworldManager.instance.UpdateResourceText(timber, Coal.amount.ToString());
    //         break;

    //         default: break;
    //     };

    // }

    // // public void UpdateText(string text) {
    // //     if (textLabel != null) {
    // //         textLabel.text = text;
    // //     }
    // // }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (other.CompareTag("Caravan")) {
    //         OverworldManager.instance.AddResource(this.GetComponent<IUpdateableResource>(), 5);
    //         // Destroy(this.gameObject);
    //     }
    // }
    
}