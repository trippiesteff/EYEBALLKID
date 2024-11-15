using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEditor.Animations;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{

 

    public GameObject player;
    [Header("Variables")]
    public float moveSpeed;
    public Rigidbody2D theRB;

    public float jumpForce;
    public float doubleJumpForce;
    public float runSpeed;
    private float activeSpeed;

    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float wallJumpCooldown = 0.2f; // Cooldown time for wall jumping
    public float wallSlideDelay = 0.5f; // Delay before wall sliding is allowed

    [Header("Layers")]
    public LayerMask groundLayer;

    public bool isGrounded;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public UnityEngine.Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    [Header("Booleans")]
    public bool canMove = true;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    public int side = 1;

    private bool canDoubleJump;
    private bool canWallJump = true; // Can the player wall jump?
    public Animator anim;

    public float knockbackLength, knockbackSpeed;
    private float knockbackCounter;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float wallSlideCounter; // Timer to track wall slide delay

    private UnityEngine.Vector2 dir;

    // private Vector2 wallJumpDirection = new Vector2(1, 1); // Adjust this vector to control the jump direction
    private int facingDirection = 1; // Assuming 1 for right and -1 for left
    [SerializeField] private UnityEngine.Vector2 wallJumpDirection;
    private bool facingRight = true;



    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Get Direction Inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        dir = new UnityEngine.Vector2(x, y);

        // Handle wall sliding
        if (onWall && !isGrounded && wallSlideCounter <= 0)
        {
            wallSlide = true;
            WallSlide();
        }
        else
        {
            wallSlide = false;
            if (isGrounded)
            {
                wallSlideCounter = wallSlideDelay; // Reset wall slide counter when grounded
            }
            else
            {
                wallSlideCounter -= Time.deltaTime;
            }
        }

        // Ground and Wall Detection
        isGrounded = Physics2D.OverlapCircle((UnityEngine.Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((UnityEngine.Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((UnityEngine.Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((UnityEngine.Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((UnityEngine.Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;

        // Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            canWallJump = true; // Reset wall jump when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Time.timeScale > 0f)
        {
            if (knockbackCounter <= 0)
            {
                HandleMovement();
                HandleJump();
            }
            else
            {
                HandleKnockback();
            }

            UpdateAnimations();
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            theRB.velocity = new UnityEngine.Vector2(Input.GetAxisRaw("Horizontal") * activeSpeed, theRB.velocity.y);
        }

        if (isGrounded)
        {
            canMove = true;
            canDoubleJump = true;
        
        }
        // if (onWall && canWallSlide)
        // {
        //     wallSlide();
        // }
        else if (!onWall)
        {
            
        }
    }

    void HandleMovement()
    {
        if (!canMove)
    {
        return; // Return early if the player cannot move
    }
        activeSpeed = moveSpeed;

        if (Input.GetButton("Fire3"))
        {
            activeSpeed = runSpeed;
        }

        theRB.velocity = new UnityEngine.Vector2(Input.GetAxisRaw("Horizontal") * activeSpeed, theRB.velocity.y);
        flip();
    }


    void HandleJump()
    {
        if (jumpBufferCounter > 0f && (isGrounded || coyoteTimeCounter > 0f))
        {
            Jump(UnityEngine.Vector2.up, jumpForce);
            canDoubleJump = true;
            jumpBufferCounter = 0f;
            anim.SetBool("isDoubleJumping", false);
        }

        else if (Input.GetButtonDown("Jump") && onRightWall && canWallJump)
        {
            WallJump();
            // WallJumpLeft();
        }

        else if (Input.GetButtonDown("Jump") && onLeftWall && canWallJump)
        {
            WallJump();
            // WallJumpRight();
        }

        else if (Input.GetButtonDown("Jump") && canDoubleJump && !isGrounded)
        {
            Jump(UnityEngine.Vector2.up, doubleJumpForce);
            canDoubleJump = false;
            canMove = true;
            anim.SetTrigger("doDoubleJump");
        }

        if (Input.GetButtonUp("Jump") && theRB.velocity.y > 0f)
        {
            theRB.velocity = new UnityEngine.Vector2(theRB.velocity.x, theRB.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    public void Jump(UnityEngine.Vector2 direction, float force)
    {
        theRB.velocity = new UnityEngine.Vector2(theRB.velocity.x, 0);
        theRB.velocity += direction * force;
        AudioManager.Instance.PlaySFXPitched(14);
    }


    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void HandleKnockback()
    {
        knockbackCounter -= Time.deltaTime;
        theRB.velocity = new UnityEngine.Vector2(knockbackSpeed * -transform.localScale.x, theRB.velocity.y);
    }

    public void KnockBack()
    {
        theRB.velocity = new UnityEngine.Vector2(0f, jumpForce * 0.5f);
        anim.SetTrigger("isKnockingBack");
        knockbackCounter = knockbackLength;
    }

    public void BouncePlayer(float bounceAmount)
    {
        theRB.velocity = new UnityEngine.Vector2(theRB.velocity.x, bounceAmount);
        canDoubleJump = true;
        anim.SetBool("isGrounded", true);
    }

    void UpdateAnimations()
    {
        anim.SetFloat("speed", Mathf.Abs(theRB.velocity.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("ySpeed", theRB.velocity.y);
        anim.SetBool("onWall", onWall);
        anim.SetBool("onRightWall", onRightWall);
        anim.SetBool("wallSlide", wallSlide);
        anim.SetBool("canMove", canMove);

        // Transition to falling animation if in the air
        if (!isGrounded && theRB.velocity.y < 0)
        {
            anim.SetBool("isFalling", true);
        }
        else
        {
            anim.SetBool("isFalling", false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((UnityEngine.Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((UnityEngine.Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((UnityEngine.Vector2)transform.position + leftOffset, collisionRadius);
    }

    void flip()
    {
        if (theRB.velocity.x > 0)
        {
            transform.localScale = UnityEngine.Vector3.one;
        }
        else if (theRB.velocity.x < 0)
        {
            transform.localScale = new UnityEngine.Vector3(-1f, 1f, 1f);
        }
    }

    void GroundTouch()
    {
        flip();
    }

private void WallJump()
{
    canMove = false;
    UnityEngine.Vector2 direction = new UnityEngine.Vector2(wallJumpDirection.x * -facingDirection, wallJumpDirection.y );
    // FlipForJump();
    theRB.AddForce(direction, ForceMode2D.Impulse);

}

 public void FlipForJump()
{
    facingDirection = facingDirection * -1;
    facingRight = !facingRight;
    transform.Rotate(0, 180, 0);
}


    private IEnumerator WallJumpCooldown()
    {
        // Wait for the cooldown period
        yield return new WaitForSeconds(wallJumpCooldown);
        canWallJump = true; // Re-enable wall jumping
    }

    private void WallSlide()
    {
        // if (wallSide != side)
        //     flip();

        // if (!canMove)
        //     return;

        // bool pushingWall = false;
        // if ((theRB.velocity.x > 0 && onRightWall) || (theRB.velocity.x < 0 && onLeftWall))
        // {
        //     pushingWall = true;
        // }
        // float push = pushingWall ? 0 : theRB.velocity.x;

        // theRB.velocity = new UnityEngine.Vector2(push, -slideSpeed);
    }

    private void Walk(UnityEngine.Vector2 dir)
    {
        if (!canMove)
            return;

        if (!wallJumped)
        {
            theRB.velocity = new UnityEngine.Vector2(dir.x * runSpeed, theRB.velocity.y);
        }
        else
        {
            theRB.velocity = UnityEngine.Vector2.Lerp(theRB.velocity, new UnityEngine.Vector2(dir.x * runSpeed, theRB.velocity.y), wallJumpLerp * Time.deltaTime);
        }
    }

    public void SetHorizontalMovement(float x, float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", x);
        anim.SetFloat("VerticalAxis", y);
        anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

}