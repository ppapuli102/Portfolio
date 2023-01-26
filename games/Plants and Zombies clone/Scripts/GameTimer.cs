using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Tooltip("Time of Level in Seconds")]
    [SerializeField] float levelTime = 10f;
    bool triggeredLevelFinish = false;


    private void Update() 
    {
        if (triggeredLevelFinish) { return; }

        GetComponent<Slider>().value = Time.timeSinceLevelLoad / levelTime;

        bool timerFinished = (Time.timeSinceLevelLoad >= levelTime);
        if (timerFinished)
        {
            FindObjectOfType<LevelController>().LevelEnded();
            triggeredLevelFinish = true;
        }
    }

}
