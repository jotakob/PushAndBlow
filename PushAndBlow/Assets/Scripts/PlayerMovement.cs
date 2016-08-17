using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 100;
    public float g = 5f;
    public float jumpHeight = 0.5f;
    public float jumpTime = 0.5f;
    public float airJumpThreshold = 0.1f;
    public AnimationCurve jumpCurve;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.2f;
    public float breakForce = 3.5f;
    public CharacterController charController;
    public AnimationCurve accelerationCurve;
    public AnimationCurve decelerationCurve;
    
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
            if (lastMovement == 0 || Mathf.Sign(lastMovement) != xMovement)
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


        if (xMovement > 0 && facing != 2)
            face(2);
        else if (xMovement < 0 && facing != 1)
            face(1);
        else if (xMovement == 0 && facing != 0)
            face(0);

        gravity -= g * dt;
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
            if (jumpPoint <= 1)
            {
                y = jumpCurve.Evaluate(jumpPoint) * jumpHeight * dt;
            }
            else
            {
                isJumping = false;
                gravity = 0;
                y = 0;
            }
        }

        charController.Move(new Vector3(x, y));
        lastMovement = xMovement;
        if (charController.isGrounded)
        {
            offGroundCounter = 0;
            gravity = 0;
        }

    }

    void startJump()
    {
        jumpStart = Time.time;
        isJumping = true;
    }

    //Flips the character, 0 = Front, 1 = Left, 2 = Right
    void face(int direction)
    {
        
    }
}
