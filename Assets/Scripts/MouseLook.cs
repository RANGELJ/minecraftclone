using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start() {
        // Hide and lock the cursor on center
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        this.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        this.playerBody.Rotate(Vector3.up * mouseX);
    }
}
