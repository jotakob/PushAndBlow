using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public enum Masks{
		NormalMask,
		StrongMask,
		AirMask
	}

    public float moveSpeed = 100;
    public float jumpHeight = 0.5f;
    public float jumpTime = 0.5f;
    public float airJumpThreshold = 0.1f;
    public AnimationCurve jumpCurve;
    public float fallTime = 1f;
    public float maxFallSpeed = 15f;
    public AnimationCurve fallCurve;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.2f;
    public AnimationCurve accelerationCurve;
    public AnimationCurve decelerationCurve;
    public float rotationDelay = 1f;
    public float rotationTime = 0.2f;
    public float xHoverSpace = 0.1f;
    public float yHoverSpace = 0.1f;
    public AnimationCurve xHoverCurve;
    public AnimationCurve yHoverCurve;
	public Vector3 air_force_1 = new Vector3 ();
	public Masks current_mask = Masks.NormalMask;

	[HideInInspector]
	public float gravityStartTime;

    int facing = 0;
    float gravity = 0f;
    bool isJumping = false;
    float offGroundCounter = 0;
    float jumpStart;
    float lastMovementSpeed = 0;
    float lastMoveInput = 0;
    float lastMoveStart;
    float lastMoveStop;
    float decSpeed;
    float rotationStart = 0;
    float rotationStartTime;
    float hoverTime = 0;

	bool pushing = false;

    CharacterController charController;
    public GameObject mesh;


    // Use this for initialization
    void Start () {
        charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        float dt = Time.deltaTime;

        //Horizontal movement

        float xAcceleration = 0;
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput != 0)
        {
            if (lastMoveInput == 0 || Mathf.Sign(lastMoveInput) != Mathf.Sign(moveInput))
            {
                lastMoveStart = Time.time;
            }
            float accPoint = (Time.time - lastMoveStart) / accelerationTime;
            xAcceleration = moveInput * accelerationCurve.Evaluate(accPoint) * moveSpeed;
        }
        else
        {
            if (lastMoveInput != 0)
            {
                lastMoveStop = Time.time;
                decSpeed = lastMovementSpeed;
            }

            // Curve value of                time difference         in relation to      reduced stopping time          times speed to slow down from
            float decPoint = Mathf.Clamp01((Time.time - lastMoveStop) / decelerationTime);
            Debug.Log("DecPoint: " + decPoint);
            xAcceleration = decelerationCurve.Evaluate(decPoint) * decSpeed;
            Debug.Log("Break: " + xAcceleration);
            if ((Time.time - lastMoveStop) > rotationDelay && facing != 1)
            {
                face(1);
            }
        }
        float x = xAcceleration * dt;


        if (moveInput < 0 && facing != 2)
            face(2);
        else if (moveInput > 0 && facing != 0)
            face(0);

        //Jumping and Falling

        gravity = fallCurve.Evaluate((Time.time - gravityStartTime) / fallTime) * dt * maxFallSpeed * -1;
        float y = gravity;
        if (offGroundCounter < airJumpThreshold)
        {
            if (Input.GetButtonDown("Jump"))
            {
                startJump();
            }
            offGroundCounter += dt;
        }

        if (isJumping)
        {
            float jumpPoint = (Time.time - jumpStart) / jumpTime;
            float lastJumpPoint = (Time.time - jumpStart - dt) / jumpTime;
            if (jumpPoint <= 1)
            {
                y = jumpCurve.Evaluate(jumpPoint - lastJumpPoint) * jumpHeight;
            }
            else
            {
                isJumping = false;
                gravityStartTime = Time.time;
                y = 0;
            }
        }

		if (pushing) {
			x *= 0.5f;
			pushing = false;
		}

        //Applying movement
		charController.Move(new Vector3(x, y) + (air_force_1 * dt));
		air_force_1 -= air_force_1 * Mathf.Sqrt(air_force_1.magnitude) * dt;

        lastMovementSpeed = xAcceleration;
        lastMoveInput = moveInput;

        if (charController.isGrounded)
        {
            offGroundCounter = 0;
            gravityStartTime = Time.time;
        }

        //Character smooth rotation
        int angle = facing * 90;
        if (mesh.transform.localRotation.eulerAngles.y != angle)
        {
            float rSpeed = Mathf.Abs(rotationStart - angle) / 90;
            float rT = (Time.time - rotationStartTime) / (rSpeed * rotationTime);
            float yRotation = Mathf.SmoothStep(rotationStart, angle, rT);
            mesh.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        }

        //Character hovering
        doHover();
    }

    void doHover()
    {
        hoverTime = (hoverTime + Time.deltaTime) % 2f;
        float xHover = 0;
        float dAngle = Mathf.Abs(90 - mesh.transform.eulerAngles.y);
        if (dAngle < 10)
        {
            xHover = xHoverCurve.Evaluate(hoverTime) * xHoverSpace * (1 - (dAngle / 10));
        }
        mesh.transform.localPosition = new Vector3(  xHover, yHoverCurve.Evaluate(hoverTime) * yHoverSpace, 0);
       
    }

    void startJump()
    {
        jumpStart = Time.time;
        isJumping = true;
    }

    //Flips the character, 0 = Right, 1 = Front, 2 = Left
    void face(int direction)
    {
        facing = direction;
        rotationStartTime = Time.time;
        rotationStart = mesh.transform.localRotation.eulerAngles.y;
    }

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (current_mask != Masks.StrongMask)
			return;
		
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic)
			return;

		if (hit.moveDirection.y < -0.3F)
			return;

		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir * charController.velocity.x;
		pushing = true;
	}
}
