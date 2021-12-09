using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    public bool AcceptPlayerInput = true;
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
    public float pushPower = 10f;
    public float weight = 1f;
    
    private static float Snap(float d) {
        return Mathf.Sign(d) * Mathf.Pow(Mathf.Abs(d), MOVEMENT_SNAPINESS);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        Vector3 force;

        if (body == null || body.isKinematic){
            return;
        }
        //Debug.Log(hit.moveDirection);
        if (hit.moveDirection.y < -0.3){
            force = new Vector3(0f, 0.5f, 0f) * Movement.GRAVITY * weight;
            Debug.Log("IN THE AIR:" + force);
        }
        else{
            Debug.Log(hit.controller.velocity.magnitude);
            Debug.Log(hit.normal);
            Debug.Log(pushPower);
            force = hit.controller.velocity.magnitude * hit.normal * -1 * pushPower * 100;
            Debug.Log("ON THE GROUND:" + force);
        }
        body.AddForceAtPosition(force, hit.point);
    }

    void Update() {
        isGrounded = IsGrounded();

        if (AcceptPlayerInput) {
            ProcessPlayerInput();
        }
    }

    bool IsGrounded() {
        return Physics.CheckSphere(groundCheck.position, GROUND_CHECK_RADIUS, groundLayer);
    }

    public void Crouch(bool crouch = true) {
        anim.SetBool("Crouch", crouch);
        speedFactor = crouch ? 0.5f : 1f;
    }

    private void ProcessPlayerInput() {

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
}
