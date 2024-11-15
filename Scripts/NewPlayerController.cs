using System; // Importing the System namespace
using System.Collections; // Importing the System.Collections namespace
using UnityEngine;
using UnityEngine.InputSystem; // Importing the UnityEngine namespace

public class NewPlayerController : MonoBehaviour // Define the NewPlayerController class that inherits from MonoBehaviour
{
// VARIABLES

[Header("Variables")] // Creates a header in the Unity Inspector
public Animator anim; // Public variable for the Animator component
public Rigidbody2D theRB; // Public variable for the Rigidbody2D component
[SerializeField] public float speed; // Serialized public variable for the player's speed
[SerializeField] private float jumpForce = 12; // Serialized private variable for the player's jump force

private bool canDoubleJump; // Private variable to check if the player can double jump
private bool canWallSlide; // Private variable to check if the player can wall slide
private bool isWallSliding; // Private variable to check if the player is currently wall sliding
private bool canMove; // Private variable to check if the player can move

public float runSpeed; // Public variable for the player's run speed
private float activeSpeed; // Private variable for the player's current active speed

private bool facingRight = true; // Private variable to check if the player is facing right
private float movingInput; // Private variable for the player's movement input

private int facingDirection = 1; // Private variable for the player's facing direction
[SerializeField] private Vector2 wallJumpDirection; // Serialized private variable for the direction of the wall jump

[Header("Collision Info")] // Creates a header in the Unity Inspector for collision information
[SerializeField] private Transform groundCheck; // Serialized private variable for the ground check transform
[SerializeField] private float groundCheckDistance; // Serialized private variable for the ground check distance
[SerializeField] private LayerMask whatIsGround; // Serialized private variable for the ground layer mask
private bool isGrounded; // Private variable to check if the player is grounded

[SerializeField] private Transform wallCheck; // Serialized private variable for the wall check transform
[SerializeField] private float wallCheckDistance; // Serialized private variable for the wall check distance

// [SerializeField] private float fHorizontalDampingWhenStopping = 0.1f;
// [SerializeField] private float fHorizontalDampingWhenTurning = 0.2f;
// [SerializeField] private float fHorizontalDampingBasic = 0.05f;

[SerializeField] private float accelerationForce = 10f;
[SerializeField] private float decelerationForce = 5f;

[SerializeField] private float maxSpeed = 6f;
[SerializeField] private float accelerationTime = 0.2f;
[SerializeField] private float decelerationTime = 0.1f;
[SerializeField] private float stopDamping = 0.05f;
private float currentSpeed;
private float velocitySmoothing;

private bool isWallDetected; // Private variable to check if a wall is detected

private float wallSlideStartTime; // Private variable to store the start time of the wall slide

public float disableDoubleJumpTimer = 0.5f; // Public variable for the timer to disable double jump

public float knockbackLength, knockbackSpeed; // Public variables for the knockback length and speed
private float knockbackCounter; // Private variable for the knockback counter

private float coyoteTime = 0.2f; // Private variable for the coyote time duration
private float coyoteTimeCounter; // Private variable for the coyote time counter

private float jumpBufferTime = 0.2f; // Private variable for the jump buffer time
private float jumpBufferCounter; // Private variable for the jump buffer counter

private bool isDucking; // Private variable to check if the player is ducking

[SerializeField] private float initialWallSlideUpwardVelocity = 2f;

[SerializeField]
private Transform grabPoint;

[SerializeField]
private Transform rayPoint;

[SerializeField]
private float rayDistance;

private GameObject grabbedObject;

private int layerIndex;

private bool isMovingObject = false;

private DistanceJoint2D distanceJoint; // The DistanceJoint2D component



private void Start() // Start method called before the first frame update
{
    layerIndex = LayerMask.NameToLayer("Objects");
    anim = GetComponent<Animator>(); // Get the Animator component attached to the game object
    theRB = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the game object
    distanceJoint = gameObject.AddComponent<DistanceJoint2D>(); // Add a DistanceJoint2D component to the player
        distanceJoint.enabled = false; // Initially disable the joint
}

void Update() // Update method called once per frame
{
    CollisionCheck(); // Call the CollisionCheck method
    FlipController(); // Call the FlipController method
    AnimatorController(); // Call the AnimatorController method

    isDucking = false; // Reset the isDucking variable

    if (Input.GetButtonUp("Jump") && theRB.velocity.y > 0f) // If the jump button is released and the player's y-velocity is positive
    {
        theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y * 0.5f); // Reduce the y-velocity by half
    }

    if (isGrounded) // If the player is grounded
    {
        coyoteTimeCounter = coyoteTime; // Reset the coyote time counter
    }
    else // If the player is not grounded
    {
        coyoteTimeCounter -= Time.deltaTime; // Decrease the coyote time counter
    }

    if (Input.GetButtonDown("Jump")) // If the jump button is pressed
    {
        jumpBufferCounter = jumpBufferTime; // Reset the jump buffer counter
    }
    else // If the jump button is not pressed
    {
        jumpBufferCounter -= Time.deltaTime; // Decrease the jump buffer counter
    }

    if (isGrounded) // If the player is grounded
    {
        canMove = true; // Set canMove to true
        canDoubleJump = true; // Set canDoubleJump to true
    }

    if (isWallDetected && canWallSlide)
{
    if (!isWallSliding)
    {
        isWallSliding = true;
        wallSlideStartTime = Time.time;
        theRB.velocity = new Vector2(theRB.velocity.x, initialWallSlideUpwardVelocity); // Initial upward velocity
    }

    float elapsedTime = Time.time - wallSlideStartTime;
    float velocityMultiplier = Mathf.Lerp(0.6f, 1f, elapsedTime / 0.5f);
    theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y * velocityMultiplier); // Gradually pull down
}

    Move(); // Call the Move method

    Grab();
}

// private void Grab()
// {
//     RaycastHit2D hitInfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
//     if (hitInfo.collider != null && hitInfo.collider.gameObject.layer == layerIndex)
//     {
//         // Grab Object
//         if (Input.GetKeyDown(KeyCode.F) && grabbedObject == null)
//         {
//             grabbedObject = hitInfo.collider.gameObject;
//             Rigidbody2D grabbedRB = grabbedObject.GetComponent<Rigidbody2D>();

//             distanceJoint.connectedBody = grabbedRB; // Connect the joint to the grabbed object
//             distanceJoint.autoConfigureDistance = false;
//             distanceJoint.distance = Vector2.Distance(grabPoint.position, grabbedObject.transform.position);
//             distanceJoint.anchor = Vector2.zero; // Set the anchor to the player's position
//             distanceJoint.connectedAnchor = grabbedRB.transform.InverseTransformPoint(grabPoint.position);
//             distanceJoint.enabled = true; // Enable the joint
//         }
//         // Release Object
//         else if (Input.GetKeyDown(KeyCode.F) && grabbedObject != null)
//         {
//             distanceJoint.enabled = false; // Disable the joint
//             grabbedObject = null;
//         }
//     }

//     Debug.DrawRay(rayPoint.position, transform.right * rayDistance, Color.red);
// }
   

   private bool isObjectGrabbed = false;

private void Grab()
{
    RaycastHit2D hitInfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
    if (hitInfo.collider != null && hitInfo.collider.gameObject.layer == layerIndex)
    {
        // Grab Object
        if (Input.GetKeyDown(KeyCode.F) && grabbedObject == null)
        {
            grabbedObject = hitInfo.collider.gameObject;
            Rigidbody2D grabbedRB = grabbedObject.GetComponent<Rigidbody2D>();

            distanceJoint.connectedBody = grabbedRB; // Connect the joint to the grabbed object
            distanceJoint.autoConfigureDistance = false;
            distanceJoint.distance = Vector2.Distance(grabPoint.position, grabbedObject.transform.position);
            distanceJoint.anchor = grabPoint.localPosition; // Set the anchor to the grab point's local position on the player
            distanceJoint.connectedAnchor = Vector2.zero; // Connect to the center of the grabbed object
            distanceJoint.enabled = true; // Enable the joint
            isObjectGrabbed = true;
        }
        // Release Object
        else if (Input.GetKeyDown(KeyCode.F) && grabbedObject != null)
        {
            distanceJoint.enabled = false; // Disable the joint
            grabbedObject = null;
            isObjectGrabbed = false;
        }
    }

    Debug.DrawRay(rayPoint.position, transform.right * rayDistance, Color.red);
}




// private void Grab()
// {
// RaycastHit2D hitInfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
//         if (hitInfo.collider != null && hitInfo.collider.gameObject.layer == layerIndex)
//         {
//             // Grab Object
//             if (Input.GetKeyDown(KeyCode.F) && grabbedObject == null)
//             {
//                 grabbedObject = hitInfo.collider.gameObject;
//                 Rigidbody2D grabbedRB = grabbedObject.GetComponent<Rigidbody2D>();

//                 distanceJoint.connectedBody = grabbedRB; // Connect the joint to the grabbed object
//                 distanceJoint.connectedAnchor = grabbedRB.transform.InverseTransformPoint(grabPoint.position);
//                 distanceJoint.distance = Vector2.Distance(grabPoint.position, grabbedObject.transform.position);
//                 distanceJoint.autoConfigureDistance = false;
//                 distanceJoint.enabled = true; // Enable the joint
//             }
//             // Release Object
//             else if (Input.GetKeyDown(KeyCode.F) && grabbedObject != null)
//             {
//                 distanceJoint.enabled = false; // Disable the joint
//                 grabbedObject = null;
//             }
//         }

//         Debug.DrawRay(rayPoint.position, transform.right * rayDistance, Color.red);
//     }



    private void CheckInput() // Method to check player input
{
    movingInput = Input.GetAxisRaw("Horizontal"); // Get the horizontal movement input

    if (Input.GetAxis("Vertical") < 0) // If the vertical input is less than 0
    {
        canWallSlide = false; // Set canWallSlide to false
    }

    if (jumpBufferCounter > 0f) // If the jump buffer counter is greater than 0
    {
        JumpButton(); // Call the JumpButton method
        jumpBufferCounter = 0f; // Reset the jump buffer counter
    }
}

private void Move() // Method to handle player movement
{

    if (knockbackCounter <=0)
    {
        CheckInput(); // Call the CheckInput method

        if (isGrounded && movingInput == 0 && Input.GetAxisRaw("Vertical") < 0) // If the player is grounded, not moving horizontally, and pressing down
        {
            isDucking = true; // Set isDucking to true
        }

        activeSpeed = speed; // Set the active speed to the base speed

        if (Input.GetButton("Fire3")) // If the run button is pressed
        {
            activeSpeed = runSpeed; // Set the active speed to the run speed
        }

        if (canMove) // If the player can move
        {
            Walk(); // Call the Walk method
        }
    }
    else
    {
        knockbackCounter -= Time.deltaTime;
          theRB.velocity = new Vector2(knockbackSpeed * -movingInput, theRB.velocity.y); // Apply knockback velocity
    }
}


// private void Walk()
// {
//     float targetSpeed = movingInput * activeSpeed;
//     float currentSpeed = theRB.velocity.x;

//     if (Mathf.Abs(targetSpeed) < 0.01f) {
//         // Apply deceleration force when stopping
//         if (currentSpeed != 0) {
//             float deceleration = Mathf.Sign(currentSpeed) * decelerationForce;
//             theRB.AddForce(new Vector2(-deceleration, 0));

//             // Clamp velocity to zero if the deceleration would overshoot
//             if (Mathf.Abs(currentSpeed) < decelerationForce * Time.deltaTime) {
//                 theRB.velocity = new Vector2(0, theRB.velocity.y);
//             }
//         }
//     } else {
//         // Apply acceleration force when moving
//         float acceleration = movingInput * accelerationForce;
//         theRB.AddForce(new Vector2(acceleration, 0));
        
//         // Clamp the speed to max speed
//         if (Mathf.Abs(theRB.velocity.x) > maxSpeed) {
//             theRB.velocity = new Vector2(Mathf.Sign(theRB.velocity.x) * maxSpeed, theRB.velocity.y);
//         }
//     }
// }

private void Walk()
{
    // theRB.velocity = new Vector2(movingInput * activeSpeed, theRB.velocity.y); // Apply horizontal movement    

    float targetSpeed = movingInput * activeSpeed;
    float speedDifference = targetSpeed - currentSpeed;

    if (Mathf.Abs(targetSpeed) < 0.01f) {
        // Apply deceleration when stopping
        currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref velocitySmoothing, decelerationTime);
    } else {
        // Apply acceleration when moving
        if (Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed)) {
            // If changing direction, apply a faster deceleration first
            currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref velocitySmoothing, stopDamping);
        }
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocitySmoothing, accelerationTime);
    }

    theRB.velocity = new Vector2(currentSpeed, theRB.velocity.y); ;
}


private void JumpButton() // Method to handle jumping
{
    if (isWallSliding) // If the player is wall sliding
    {
        WallJump(); // Call the WallJump method
        Flip(); // Call the Flip method
        AudioManager.Instance.PlaySFXPitched(14);
    }
    else if (coyoteTimeCounter > 0f) // If the coyote time counter is greater than 0
    {
        Jump(); // Call the Jump method
        coyoteTimeCounter = 0f; // Reset the coyote time counter
        jumpBufferCounter = 0f; // Reset the jump buffer counter
        AudioManager.Instance.PlaySFXPitched(14);
    }
    else if (canDoubleJump) // If the player can double jump
    {
        canMove = true; // Set canMove to true
        canDoubleJump = false; // Set canDoubleJump to false
        anim.SetTrigger("doDoubleJump"); // Trigger the double jump animation
        DoubleJump(); // Call the DoubleJump method
        AudioManager.Instance.PlaySFXPitched(14);
    }

    canWallSlide = false; // Set canWallSlide to false
}

public void Jump() // Method to perform a jump
{
    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce); // Set the y-velocity to the jump force
}

private void DoubleJump() // Method to perform a double jump
{
    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce * 0.8f); // Set the y-velocity to 80% of the jump force
}

private void WallJump() // Method to perform a wall jump
{
    canMove = false; // Set canMove to false
    canDoubleJump = false; // Set canDoubleJump to false
    theRB.velocity = new Vector2(wallJumpDirection.x * -facingDirection, wallJumpDirection.y); // Set the velocity for the wall jump
    Invoke("EnableDoubleJump", disableDoubleJumpTimer); // Schedule EnableDoubleJump to be called after a delay
}

private void EnableDoubleJump() // Method to enable double jumping
{
    canDoubleJump = true; // Set canDoubleJump to true
}

private void AnimatorController() // Method to update animator parameters
{
    bool isMoving = movingInput != 0; // Check if the player is moving
    anim.SetBool("isMoving", isMoving); // Set the isMoving parameter in the animator
    anim.SetFloat("speed", Mathf.Abs(theRB.velocity.x)); // Set the speed parameter in the animator
    anim.SetBool("isGrounded", isGrounded); // Set the isGrounded parameter in the animator
    anim.SetFloat("ySpeed", theRB.velocity.y); // Set the ySpeed parameter in the animator
    anim.SetBool("isWallSliding", isWallSliding); // Set the isWallSliding parameter in the animator
    anim.SetBool("isWallDetected", isWallDetected); // Set the isWallDetected parameter in the animator
    anim.SetBool("isDucking", isDucking); // Set the isDucking parameter in the animator
}

private void CollisionCheck() // Method to check collisions
{
    isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround); // Check if the player is grounded
    isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround); // Check if a wall is detected

    if (isWallDetected && theRB.velocity.y < 0) // If a wall is detected and the player's y-velocity is negative
    {
        canWallSlide = true; // Set canWallSlide to true
    }

    if (isWallDetected) // If a wall is detected
    {
        Debug.Log("WallDetection"); // Log wall detection
    }

    if (!isWallDetected) // If no wall is detected
    {
        canWallSlide = false; // Set canWallSlide to false
        isWallSliding = false; // Set isWallSliding to false
    }
}

private void OnDrawGizmos() // Method to draw gizmos in the editor
{
    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance)); // Draw a line for the ground check
    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + wallCheckDistance, transform.position.y)); // Draw a line for the wall check
}

private void Flip() // Method to flip the player's direction
{
    facingDirection *= -1; // Invert the facing direction
    facingRight = !facingRight; // Toggle the facingRight variable
    transform.Rotate(0, 180, 0); // Rotate the player by 180 degrees
}

private void FlipController() // Method to control flipping based on movement
{
    if (isGrounded && isWallDetected) // If the player is grounded and a wall is detected
    {
        if (facingRight && movingInput < 0) // If the player is facing right and moving left
            Flip(); // Flip the player's direction
        else if (!facingRight && movingInput > 0) // If the player is facing left and moving right
            Flip(); // Flip the player's direction
    }

    if (movingInput > 0 && !facingRight) // If the player is moving right and not facing right
    {
        Flip(); // Flip the player's direction
    }
    else if (movingInput < 0 && facingRight) // If the player is moving left and facing right
    {
        Flip(); // Flip the player's direction
    }
}

void HandleKnockback() // Method to handle knockback
{
    knockbackCounter -= Time.deltaTime; // Decrease the knockback counter
    theRB.velocity = new Vector2(knockbackSpeed * -transform.localScale.x, theRB.velocity.y); // Apply knockback velocity
}

public void KnockBack() // Method to initiate knockback
{
    theRB.velocity = new Vector2(0f, jumpForce * 0.5f); // Set the y-velocity for knockback
    anim.SetTrigger("isKnockingBack"); // Trigger the knockback animation
    knockbackCounter = knockbackLength; // Set the knockback counter
    Debug.Log("Knockback is Happening" + knockbackLength); // Log the knockback event
}

public void BouncePlayer(float bounceAmount) // Method to bounce the player
{
    theRB.velocity = new Vector2(theRB.velocity.x, bounceAmount); // Set the y-velocity for the bounce
    canDoubleJump = true; // Set canDoubleJump to true
    anim.SetBool("isGrounded", true); // Set the isGrounded parameter in the animator to true
}


// void FixedUpdate()
// {
//     if (grabbedObject != null && isMovingObject)
//     {
//         Rigidbody2D grabbedRB = grabbedObject.GetComponent<Rigidbody2D>();
//         Vector2 direction = (grabPoint.position - grabbedObject.transform.position).normalized;
//         float distance = Vector2.Distance(grabPoint.position, grabbedObject.transform.position);
        
//         // Desired velocity towards the grab point
//         float desiredSpeed = 100f; // Adjust this to control the speed
//         Vector2 desiredVelocity = direction * desiredSpeed;
        
//         // Calculate the force needed to change the current velocity to the desired velocity
//         Vector2 currentVelocity = grabbedRB.velocity;
//         Vector2 force = (desiredVelocity - currentVelocity) * grabbedRB.mass / Time.fixedDeltaTime;
        
//         // Apply the calculated force
//         grabbedRB.AddForce(force);
//     }
// }


}