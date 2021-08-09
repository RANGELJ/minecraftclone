using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPlayer : MonoBehaviour {
    public RWorld world;
    private Transform cam;
    private Vector3 velocity;
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private float verticalMomentum = 0;
    private bool jumpIsRequested;
    private bool isGrounded;
    private bool isSprinting;

    private static readonly float walkSpeed = 3f;
    private static readonly float sprintSpeed = 6f;
    private static readonly float jumpForce = 5f;
    private static readonly float gravity = -9.8f;
    private static readonly float radius = 0.15f;

    void Start() {
        this.cam = GameObject.Find("Main Camera").transform;
    }

    void Update() {
        this.GetPlayerInput();
    }

    void FixedUpdate() {
        this.CalculateVelocity();

        if (this.jumpIsRequested) {
            this.Jump();
        }

        this.transform.Rotate(Vector3.up * mouseHorizontal);
        this.cam.Rotate(Vector3.right * -mouseVertical);
        this.transform.Translate(this.velocity, Space.World);
    }

    private void Jump() {
        this.verticalMomentum = RPlayer.jumpForce;
        this.isGrounded = false;
        this.jumpIsRequested = false;
    }

    private void CalculateVelocity() {
        if (this.verticalMomentum > RPlayer.gravity) {
            this.verticalMomentum += Time.fixedDeltaTime * RPlayer.gravity;
        }

        this.velocity = (transform.forward * this.vertical) + (transform.right * this.horizontal);

        if (this.isSprinting) {
            this.velocity *= Time.fixedDeltaTime * RPlayer.sprintSpeed;
        } else {
            this.velocity *= Time.fixedDeltaTime * RPlayer.walkSpeed;
        }

        this.velocity += Vector3.up * this.verticalMomentum * Time.fixedDeltaTime;

        if (
            (this.velocity.z > 0 && this.front)
            || (this.velocity.z < 0 && this.back)
        ) {
            this.velocity.z = 0;
        }

        if (
            (this.velocity.x > 0 && this.right)
            || (this.velocity.x < 0 && this.left)
        ) {
            this.velocity.x = 0;
        }

        if (this.velocity.y < 0) {
            this.velocity.y = this.CheckDownSpeed(this.velocity.y);
        } else if (this.velocity.y > 0) {
            this.velocity.y = this.CheckUpSpeed(this.velocity.y);
        }
    }

    private void GetPlayerInput() {
        this.horizontal = Input.GetAxis("Horizontal");
        this.vertical = Input.GetAxis("Vertical");
        this.mouseHorizontal = Input.GetAxis("Mouse X");
        this.mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint")) {
            this.isSprinting = true;
        } else if (Input.GetButtonUp("Sprint")) {
            this.isSprinting = false;
        }

        if (this.isGrounded && Input.GetButtonDown("Jump")) {
            this.jumpIsRequested = true;
        }
    }

    private Vector3[] GetCornerVectors(float speed) {
        Vector3 corner1 = new Vector3(
            transform.position.x - RPlayer.radius,
            transform.position.y + speed,
            transform.position.z - RPlayer.radius
        );
        Vector3 corner2 = new Vector3(
            transform.position.x + RPlayer.radius,
            transform.position.y + speed,
            transform.position.z - RPlayer.radius
        );
        Vector3 corner3 = new Vector3(
            transform.position.x + RPlayer.radius,
            transform.position.y + speed,
            transform.position.z + RPlayer.radius
        );
        Vector3 corner4 = new Vector3(
            transform.position.x - RPlayer.radius,
            transform.position.y + speed,
            transform.position.z + RPlayer.radius
        );

        return new Vector3[]{
            corner1,
            corner2,
            corner3,
            corner4
        };
    }

    private RBlock[] GetCornerBlocks(float downSpeed) {
        Vector3[] corners = this.GetCornerVectors(downSpeed);

        RBlock corner1Block = this.world.GetBlockFromPosition(
            x: corners[0].x,
            y: corners[0].y,
            z: corners[0].z
        );

        RBlock corner2Block = this.world.GetBlockFromPosition(
            x: corners[1].x,
            y: corners[1].y,
            z: corners[1].z
        );

        RBlock corner3Block = this.world.GetBlockFromPosition(
            x: corners[2].x,
            y: corners[2].y,
            z: corners[2].z
        );

        RBlock corner4Block = this.world.GetBlockFromPosition(
            x: corners[3].x,
            y: corners[3].y,
            z: corners[3].z
        );

        return new RBlock[]{
            corner1Block,
            corner2Block,
            corner3Block,
            corner4Block
        };
    }

    private float CheckDownSpeed(float downSpeed) {
        RBlock[] cornerBlocks = this.GetCornerBlocks(downSpeed);

        if (
            (cornerBlocks[0] != null && cornerBlocks[0].IsSolid())
            || (cornerBlocks[1] != null && cornerBlocks[1].IsSolid())
            || (cornerBlocks[2] != null && cornerBlocks[2].IsSolid())
            || (cornerBlocks[3] != null && cornerBlocks[3].IsSolid())
        ) {
            this.isGrounded = true;
            return 0;
        }

        isGrounded = false;
        return downSpeed;
    }

    private bool front {
        get {
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            float z = this.transform.position.z;

            RBlock feetBlock = this.world.GetBlockFromPosition(x, y, z + RPlayer.radius);
            RBlock headBlock = this.world.GetBlockFromPosition(x, y + 1f, z + RPlayer.radius);

            return (feetBlock != null && feetBlock.IsSolid()) || (headBlock != null && headBlock.IsSolid());
        }
    }

    private bool back {
        get {
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            float z = this.transform.position.z;

            RBlock feetBlock = this.world.GetBlockFromPosition(x, y, z - RPlayer.radius);
            RBlock headBlock = this.world.GetBlockFromPosition(x, y + 1f, z - RPlayer.radius);

            return (feetBlock != null && feetBlock.IsSolid()) || (headBlock != null && headBlock.IsSolid());
        }
    }

    private bool left {
        get {
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            float z = this.transform.position.z;

            RBlock feetBlock = this.world.GetBlockFromPosition(x - RPlayer.radius, y, z);
            RBlock headBlock = this.world.GetBlockFromPosition(x - RPlayer.radius, y + 1f, z);

            return (feetBlock != null && feetBlock.IsSolid()) || (headBlock != null && headBlock.IsSolid());
        }
    }

    private bool right {
        get {
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            float z = this.transform.position.z;

            RBlock feetBlock = this.world.GetBlockFromPosition(x + RPlayer.radius, y, z);
            RBlock headBlock = this.world.GetBlockFromPosition(x + RPlayer.radius, y + 1f, z);

            return (feetBlock != null && feetBlock.IsSolid()) || (headBlock != null && headBlock.IsSolid());
        }
    }

    private float CheckUpSpeed(float upSpeed) {
        // +2f because is the height of the character
        RBlock[] cornerBlocks = this.GetCornerBlocks(upSpeed + 2f);

        if (
            (cornerBlocks[0] != null && cornerBlocks[0].IsSolid())
            || (cornerBlocks[1] != null && cornerBlocks[1].IsSolid())
            || (cornerBlocks[2] != null && cornerBlocks[2].IsSolid())
            || (cornerBlocks[3] != null && cornerBlocks[3].IsSolid())
        ) {
            return 0;
        }

        return upSpeed;
    }
}
