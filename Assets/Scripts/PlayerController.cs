using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.02f;

    // movement condition
    private bool isRunning = false;

    private Animator anim;

    // movement
    private CharacterController characterController;
    private float jumpForce = 5.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLane = 1; // 0 = left, 1 = middle, 2 = right

    // speed modifier
    private float originalSpeed = 6.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    // Sound
    private AudioSource playerAudio;
    public AudioClip crashSound;

    // Start is called before the first frame update
    void Start()
    {
        speed = originalSpeed;
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
            return;

        if (Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        // Get input on which lane to be
        if (MobileInput.Instance.SwipeLeft)
        {
            MoveLane(false);
        }
        if (MobileInput.Instance.SwipeRight)
        {
            MoveLane(true);
        }

        // calculate wher player should be
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        // calculate move vector
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;
        


        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        //calculate Y
        if (isGrounded) //if grounded
        {
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp)
            {
                // jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                // slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            // Fast falling mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //move pingu
        characterController.Move(moveVector * Time.deltaTime);

        // Rotate pingu to where it is going
        Vector3 dir = characterController.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }
    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(
            new Vector3(
            characterController.bounds.center.x, 
            (characterController.bounds.center.y - characterController.bounds.extents.y) + 0.2f, 
            characterController.bounds.center.z), 
            Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.red, 1.0f);
        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }

    public void StartSliding()
    {
        anim.SetBool("Sliding", true);
        characterController.height /= 2;
        characterController.center = new Vector3(characterController.center.x,
            characterController.center.y / 2, characterController.center.z);
    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        characterController.height *= 2;
        characterController.center = new Vector3(characterController.center.x,
            characterController.center.y * 2, characterController.center.z);
    }

    private void Crash()
    {
        anim.SetTrigger("Death");
        playerAudio.PlayOneShot(crashSound, 1.0f);
        isRunning = false;
        GameManager.Instance.OnDeath();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
            break;
        }
    }
}
