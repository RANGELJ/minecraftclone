using UnityEngine;

public class PlayerMovementScript : MonoBehaviour {
    public float speed = 1.9f;
    public CharacterController controller;

    void Update() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * this.speed * Time.deltaTime);
    }
}
