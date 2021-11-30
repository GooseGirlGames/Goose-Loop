using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {
    public const float MOUSE_SPEED = 300.0f;
    public const float MOVE_SPEED = 6.0f;
    private Transform cameraTransform;
    private CharacterController cc;
    private float pitch = 0.0f;

    // Start is called before the first frame update
    void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = GetComponentInChildren<Camera>().transform;
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Look();
        Move();
    }

    private void Look() {
        float dx = Input.GetAxis("Mouse X");
        float dy = -Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(dy + pitch, -90.0f, 90.0f);
        transform.RotateAround(transform.position, Vector3.up, dx * MOUSE_SPEED * Time.deltaTime);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void Move() {
        float r = Time.deltaTime * MOVE_SPEED * Input.GetAxis("Horizontal");
        float f = Time.deltaTime * MOVE_SPEED * Input.GetAxis("Vertical");

        r = Mathf.Sign(r) * Mathf.Pow(Mathf.Abs(r), 1f);
        f = Mathf.Sign(f) * Mathf.Pow(Mathf.Abs(f), 1f);

        Vector3 move = f * transform.forward + r * transform.right;
        cc.Move(move);

        if (Input.GetButtonDown("Jump")) {
            // TODO
        }
    }
}
