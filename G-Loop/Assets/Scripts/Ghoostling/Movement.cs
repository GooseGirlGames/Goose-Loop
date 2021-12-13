using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    public bool AcceptPlayerInput = true;
    public const float SPEED = 12f;
    private float speedFactor = 1f;
    public const float GRAVITY = -30f;
    public const float JUMP_HEIGHT = 2f;

    // 0  = instant accelleration (infinetely snappy)
    // 1  = default
    // >1 = slower (not snappy)
    public const float MOVEMENT_SNAPINESS = 0.5f;
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Vector3 velocity;
    private bool isGrounded;
    public Animator anim;
    private float pushPower = 6f;
    private float weight = 5f;
    private GhoostlingManager gman;
    private GooseController gcon;
    private void Start() {
        gman = GhoostlingManager.GetInstance();
        gcon = GetComponentInChildren<GooseController>();
    }

    private static float Snap(float d) {
        return Mathf.Sign(d) * Mathf.Pow(Mathf.Abs(d), MOVEMENT_SNAPINESS);
    }

//------------------------------------------------------------------------------------------------------------------------------
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        /*
        TODO:
            If the object that was hit is a goose and that goose's id is
            higher than our id, push that goose out of the way.  Issue #17
        */

        GooseController other_goose = hit.gameObject.GetComponentInChildren<GooseController>();
        Rigidbody body = hit.collider.attachedRigidbody;


        if (other_goose!= null){ 
            if(other_goose.GetId() > gcon.GetId()){
                Debug.Log("I get push: " + gcon.GetId() + "   I push: " + other_goose.GetId());
                gcon.MakeInvulnerable(20);
                //apply
            }
            else{
            }
            /* if(gcon.GetState() == GooseController.GooseState.ACTIVE)
                Debug.Log("Collided Goose: " + other_goose.GetId()); */
        }


        if (body == null || body.isKinematic){
            return;
        }
        
        Vector3 pushDirection;
        if (hit.moveDirection.y < -0.3){
            return;
        } else {
            _gizmo_color = Color.magenta;
            pushDirection = hit.moveDirection;
            pushDirection.y = 0;  // don't push down
        }

        Vector3 force = pushPower * pushDirection * weight;

        _debug_ray = (hit.point, force, force.magnitude);
        body.AddForceAtPosition(force, hit.point);
    }
//------------------------------------------------------------------------------------------------------------------------------

    void FixedUpdate() {
        // Inputs are managed by GooseController, which invokes the ProcessInputs method
    }
    public void PerformGroundCheck() {
        isGrounded = Physics.CheckSphere(groundCheck.position,
                GroundCheck.GROUND_CHECK_RADIUS,
                groundLayer
        );
    }

    public void Crouch(bool crouch = true) {
        anim.SetBool("Crouch", crouch);
        speedFactor = crouch ? 0.5f : 1f;
    }

    public void ProcessInputs(GhoostlingData.UserInputs inputs) {
        PerformGroundCheck();

        float x = Snap(inputs.horizontal);
        float z = Snap(inputs.vertical);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * SPEED * speedFactor * Time.deltaTime);

        if (isGrounded && inputs.jumpButtonDown) {
            velocity.y = Mathf.Sqrt(JUMP_HEIGHT * 2f * -GRAVITY);
        }
        
        velocity.y += GRAVITY * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (inputs.crouchButtonDown) {
            // TODO crouch 
        } else if (inputs.crouchButtonUp) {
            // TODO uncrouch 
        }
    }
    
    public void yeet(float yeet_power){
        velocity.y = Mathf.Sqrt(yeet_power * 2f * -GRAVITY);
    }

    // DEBUG Stuff
    (Vector3, Vector3, float) _debug_ray = (Vector3.zero, Vector3.zero, 0);
    (Vector3, float) _debug_sphere = (Vector3.zero, 0);
    Color _gizmo_color = Color.gray;
    private void OnDrawGizmos() {
        Gizmos.color = _gizmo_color;
        Gizmos.DrawLine(
                _debug_ray.Item1,
                _debug_ray.Item1 + _debug_ray.Item3 * Vector3.Normalize(_debug_ray.Item2)
        );
        Gizmos.DrawSphere(_debug_sphere.Item1, _debug_sphere.Item2);
    }
}
