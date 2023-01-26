using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Config Params
    [SerializeField] float runSpeed = 5f; 
    [SerializeField] float climbSpeed = 5f; 
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] Vector2 deathKick;

    // State
    bool isAlive = true;
    
    // Cached component refs
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeet;

    float gravityScaleAtStart;

    // Message then methods
    private void Start() {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        myFeet = GetComponent<BoxCollider2D>();
    }

    private void Update() {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        Jump();
        ClimbLadder();
    }

    private void Run() 
    {
        float controlThrow = Input.GetAxis("Horizontal"); // value between -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", hasHorizontalSpeed);
    }

    private void FlipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * Mathf.Sign(myRigidBody.velocity.x), transform.localScale.y);
        }
    }

    private void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (Input.GetButtonDown("Jump") && isAlive)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")) && isAlive) 
        { 
            myAnimator.SetBool("isClimbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return; 
        }

        float controlThrow = Input.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Enemy" || other.tag == "Hazards")
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isAlive) { return; }
        isAlive = false;
        myAnimator.SetTrigger("isDead");
        myRigidBody.velocity = deathKick;
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }

}
