using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Config Params
    [SerializeField] float moveSpeed = 1f;

    // Cached refs
    Rigidbody2D myRigidBody;
    BoxCollider2D myBoxCollider;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFacingRight())
        {
            myRigidBody.velocity = new Vector2(moveSpeed, 0);
        }
        else
        {
            myRigidBody.velocity = new Vector2(-moveSpeed, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        FlipSprite();
    }

    private void FlipSprite()
    {
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -Mathf.Sign(myRigidBody.velocity.x), transform.localScale.y);
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }



}
