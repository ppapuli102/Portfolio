using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterCheck : MonoBehaviour
{
    private Collection filterRef;
    [SerializeField] string filterCheck;

    // Start is called before the first frame update
    void Start()
    {
        filterRef = FindObjectOfType<Collection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (filterRef.filteringByTypes.Contains(filterCheck)) {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        } else{
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }


}
