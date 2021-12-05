using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public const float SPEED = 12f;
    private float speedFactor = 1f;
    public const float GRAVITY = -30f;
    public const float GROUND_CHECK_RADIUS = 0.5f;
    public const float JUMP_HEIGHT = 2f;

    // 0  = instant accelleration (infinetely snappy)
    // 1  = default
    // >1 = slower (not snappy)
    public const float MOVEMENT_SNAPINESS = 0.3f;
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Vector3 velocity;
    private bool isGrounded;
    public Animator anim;
    public GhoostlingRecorder rec;
    public GhoostlingAction crouchAction;
    public GhoostlingAction uncrouchAction;
    
    private static float Snap(float d) {
        return Mathf.Sign(d) * Mathf.Pow(Mathf.Abs(d), MOVEMENT_SNAPINESS);
    }

    void Update() {
        isGrounded = IsGrounded();
        float x = Snap(Input.GetAxis("Horizontal"));
        float z = Snap(Input.GetAxis("Vertical"));

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * SPEED * speedFactor * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump")) {
            velocity.y = Mathf.Sqrt(JUMP_HEIGHT * 2f * -GRAVITY);
        }
        
        velocity.y += GRAVITY * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Crouch")) {
            rec.ExecuteAndRecordAction(crouchAction);
        } else if (Input.GetButtonUp("Crouch")) {
            rec.ExecuteAndRecordAction(uncrouchAction);
        }
    }

    bool IsGrounded() {
        return Physics.CheckSphere(groundCheck.position, GROUND_CHECK_RADIUS, groundLayer);
    }

    public void Crouch(bool crouch = true) {
        anim.SetBool("Crouch", crouch);
        speedFactor = crouch ? 0.5f : 1f;
    }
}
