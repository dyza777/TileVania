using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    [Header("State")]
    bool isAlive = true;

    [Header("Cached")]
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent <CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        Run();
        ClimbLadder();
        Jump();
        FlipSprite();
        Die();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        
    }

    private void Jump()
    {
        bool isOnGround = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (!isOnGround)
        {
            return;
        }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder()
    {
        bool isTouchingLadder = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));

        if (!isTouchingLadder)
        {
            myAnimator.SetBool("Climbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart;
            return;
        }

        float vertMove = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 playerVelocity = new Vector2(myRigidbody.velocity.x, vertMove * climbSpeed);
        myRigidbody.velocity = playerVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);

        myRigidbody.gravityScale = 0;
    }


    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            myAnimator.SetBool("Running", true);
            transform.localScale = new Vector3(Mathf.Sign(myRigidbody.velocity.x), transform.localScale.y, transform.localScale.z);
        } else
        {
            myAnimator.SetBool("Running", false);
        }
    }

    private void Die()
    {
       if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
       {
            myAnimator.SetTrigger("Dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
            isAlive = false;

            FindObjectOfType<GameSession>().ProcessPlayerDeath();
       }
    }
}
