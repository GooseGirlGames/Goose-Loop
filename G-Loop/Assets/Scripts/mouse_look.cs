using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse_look : MonoBehaviour
{

    public float mouseSensitivity = 160f;
    public Transform playerBody;
    public float xRotation = 0f;
    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ProcessInputs(GhoostlingData.UserInputs inputs) {
        float mouseX = inputs.mouseX * mouseSensitivity * Time.deltaTime;
        float mouseY = inputs.mouseY * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        playerBody.Rotate(Vector3.up * mouseX);

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
    }
}
