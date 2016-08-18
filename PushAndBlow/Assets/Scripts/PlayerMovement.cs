using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

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
    public float rotationTime = 0.2f;
	public Vector3 air_force_1 = new Vector3 ();
	[HideInInspector]
	public float gravityStartTime;

    
    float deadzone = 0.2f;
    int facing = 0;
    float gravity = 0f;
    bool isJumping = false;
    float offGroundCounter = 0;
    float jumpStart;
    float lastMovement = 0;
    float lastMoveStart;
    float lastMoveStop;
    float decDirection;
    float rotationStart = 0;
    float rotationStartTime;

    CharacterController charController;
    public GameObject mesh;


    // Use this for initialization
    void Start () {
        charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        float dt = Time.deltaTime;
        float x = 0;
        float xMovement = Input.GetAxis("Horizontal");
        if (xMovement != 0)
        {
            if (lastMovement == 0 || Mathf.Sign(lastMovement) != Mathf.Sign(xMovement))
            {
                lastMoveStart = Time.time;
            }
            float accPoint = (Time.time - lastMoveStart) / accelerationTime;
            x = xMovement * accelerationCurve.Evaluate(accPoint) * dt *  moveSpeed; //
        }
        else
        {
            if (lastMovement != 0)
            {
                lastMoveStop = Time.time;
                decDirection = Mathf.Sign(lastMovement);
            }

            x = moveSpeed * decelerationCurve.Evaluate((Time.time - lastMoveStop) / decelerationTime) * dt * decDirection; 
        }


        if (xMovement < 0 && facing != 2)
            face(2);
        else if (xMovement > 0 && facing != 0)
            face(0);
        else if (xMovement == 0 && facing != 1)
            face(1);

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

		charController.Move(new Vector3(x, y)+air_force_1*dt);
		air_force_1 -= air_force_1*Mathf.Sqrt(air_force_1.magnitude) * dt;
        lastMovement = xMovement;
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
            Debug.Log(yRotation);
            mesh.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        }
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
}
