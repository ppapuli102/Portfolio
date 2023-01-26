using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothing;
    public Vector2 maxPosition;
    public Vector2 minPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate() {
        if (transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3( // target position for our camera
                target.position.x, 
                target.position.y, 
                transform.position.z // maintain camera z position
            );
            targetPosition.x = Mathf.Clamp(
                targetPosition.x, // thing to clamp
                minPosition.x, // minimum
                maxPosition.x // maximum
            );
            targetPosition.y = Mathf.Clamp(
                targetPosition.y, // thing to clamp
                minPosition.y, // minimum
                maxPosition.y // maximum
            );
            transform.position = Vector3.Lerp( // Move camera to target
                transform.position, 
                targetPosition, 
                smoothing
            );

        }
    }
}
