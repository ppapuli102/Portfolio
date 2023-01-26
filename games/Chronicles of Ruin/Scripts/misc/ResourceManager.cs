using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{

    [Header("Meta Resources")]
    public TMP_Text coalText;
    public TMP_Text crystalText;
    public TMP_Text goldText;
    public TMP_Text loyaltyText; 
    public TMP_Text moonshineText;
    public TMP_Text timberText;
    private int coal = 0;
    private int crystal = 1;
    private int gold = 2;
    private int loyalty = 3;
    private int moonshine = 4;
    private int timber = 5;

    [SerializeField] GameObject resourceHUD;
    private List<int> resourceAmounts;
    private List<TMP_Text> resourceTexts;

    private void Awake() {
        InitializeValues();
    }

    private void Start() {
        LoadResourceValues();
    }

    private void Update() {
        ReadResetResourceValues();
    }

    private void InitializeValues()
    {
        resourceAmounts =  new List<int> { 0, 0, 0, 0, 0, 0 };
        resourceTexts = new List<TMP_Text> { coalText, crystalText, goldText, loyaltyText, moonshineText, timberText };

        foreach (TMP_Text text in resourceTexts)
        {
            text.text = "0";
        }
    }

    private void LoadResourceValues() {
        List<int> values = GameControl.instance.LoadResources();

        for (int i = 0; i < values.Count; i ++) {
            resourceAmounts[i] = values[i];
            resourceTexts[i].text = resourceAmounts[i].ToString();
        }
    }

    public void AddResource(string resourceName, int amount) {
        switch (resourceName)
        {
            case "coal":
                resourceAmounts[coal] += amount;
                coalText.text = resourceAmounts[coal].ToString();
            break;

            case "crystal":
                resourceAmounts[crystal] += amount;
                crystalText.text = resourceAmounts[crystal].ToString();
            break;

            case "gold":
                resourceAmounts[gold] += amount;
                goldText.text = resourceAmounts[gold].ToString();
            break;

            case "loyalty":
                resourceAmounts[loyalty] += amount;
                loyaltyText.text = resourceAmounts[loyalty].ToString();
            break;

            case "moonshine":
                resourceAmounts[moonshine] += amount;
                moonshineText.text = resourceAmounts[moonshine].ToString();
            break;

            case "timber":
                resourceAmounts[timber] += amount;
                timberText.text = resourceAmounts[timber].ToString();
            break;

            default:
            break;
        }

    }

    public void SaveResources() {
        GameControl.instance.SaveResources(resourceAmounts);
        LoadResourceValues();
    }

    private void ReadResetResourceValues() {
        if (Input.GetKeyDown("x")) {
            for (int i = 0; i < resourceAmounts.Count; i ++) {
                resourceAmounts[i] = 0;
            }
            SaveResources();
        }
    }

    public void ToggleHUD(){
        resourceHUD.SetActive(!resourceHUD.activeInHierarchy);
    }



}
