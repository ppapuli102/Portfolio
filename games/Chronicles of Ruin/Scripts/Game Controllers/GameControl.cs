using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    
    public static GameControl instance;

    private Scene scene;

    [Header("Resources")]
    const string COAL = "Coal";
    const string CRYSTAL = "Crystal";
    const string GOLD = "Gold";
    const string LOYALTY = "Loyalty";
    const string MOONSHINE = "Moonshine";
    const string TIMBER = "Timber";
    private int COAL_AMOUNT;
    private int CRYSTAL_AMOUNT;
    private int GOLD_AMOUNT;
    private int LOYALTY_AMOUNT;
    private int MOONSHINE_AMOUNT;
    private int TIMBER_AMOUNT;

    private List<string> resourceTypes = new List<string> { COAL, CRYSTAL, GOLD, LOYALTY, MOONSHINE, TIMBER };
    private List<int> resourceAmounts;

    // Create a Singleton instance and destroy any non active controllers
    void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        resourceAmounts = new List<int> { COAL_AMOUNT, CRYSTAL_AMOUNT, GOLD_AMOUNT, LOYALTY_AMOUNT, MOONSHINE_AMOUNT, TIMBER_AMOUNT };
        
        GetResourcePrefs();
    }

    private void GetResourcePrefs() {
        for (int i = 0; i < resourceAmounts.Count; i ++) {
            resourceAmounts[i] = PlayerPrefs.GetInt(resourceTypes[i]);
        }
    }

    public void SaveResources(List<int> values) {
        for (int i = 0; i < resourceTypes.Count; i ++) {
            resourceAmounts[i] = values[i];
            PlayerPrefs.SetInt(resourceTypes[i], resourceAmounts[i]);
        }
    }

    public List<int> LoadResources() {
        // Debugging Resource Values ////////////////////
        // foreach (int amount in resourceAmounts) {
        //     Debug.Log(amount);
        // }

        return resourceAmounts;
    }


}

public enum Scene { MAIN_MENU, TOWN, MAP, ENCOUNTER }
