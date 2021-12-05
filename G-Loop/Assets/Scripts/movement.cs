using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public const float SPEED = 12f;
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
        controller.Move(move * SPEED * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump")) {
            velocity.y = Mathf.Sqrt(JUMP_HEIGHT * 2f * -GRAVITY);
        }
        
        velocity.y += GRAVITY * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    bool IsGrounded() {
        return Physics.CheckSphere(groundCheck.position, GROUND_CHECK_RADIUS, groundLayer);
    }
}
