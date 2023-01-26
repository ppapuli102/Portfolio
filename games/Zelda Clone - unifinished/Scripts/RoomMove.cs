using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class RoomMove : MonoBehaviour
{
    public Vector2 cameraChange;
    public Vector3 playerChange;
    public PolygonCollider2D newCollider;
    private CinemachineConfiner2D myCinemachineConfiner;
    public bool needText;
    public string placeName;
    public GameObject text;
    public Text placeText;

    // Start is called before the first frame update
    void Start()
    {
        myCinemachineConfiner = FindObjectOfType<CinemachineConfiner2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
        {
            other.transform.position += playerChange;
            myCinemachineConfiner.m_BoundingShape2D = newCollider;
            if (needText)
            {
                StartCoroutine(DisplayPlaceText());
            }
        }
    }

    private IEnumerator DisplayPlaceText()
    {
        text.SetActive(true);
        placeText.text = placeName;
        yield return new WaitForSeconds(4f);
        text.SetActive(false);
    }
}
