using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public enum Masks {
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
    public float hoverSpeed = 0.5f;
    public AnimationCurve xHoverCurve;
    public AnimationCurve yHoverCurve;
    public Vector3 air_force_1 = new Vector3();
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
    
    Vector3 startPosition;

	bool pushing = false;

    CharacterController charController;
    GameObject mesh;
	GameObject[] channels = new GameObject[0];

	Checkpoint last_checkpoint;


    // Use this for initialization
    void Start () {
        charController = GetComponent<CharacterController>();
        startPosition = transform.position;
		MaskState.getMaskState().availableMasks.Add(Masks.NormalMask);
        mesh = transform.FindChild("Mesh").gameObject;
        equipMask(Masks.NormalMask);
		gravityStartTime = Time.time;
        rotationStartTime = Time.time;

		channels = GameObject.FindGameObjectsWithTag ("AirChannel");
		if (current_mask != Masks.AirMask) {
			foreach (var channel in channels) {
				channel.SetActive (false);
			}
		} else {
			foreach (var channel in channels) {
				channel.SetActive (true);
			}
		}
	}


	void checkpoint(Checkpoint new_checkpoint){
		if (last_checkpoint)
			last_checkpoint.deactivate ();
		last_checkpoint = new_checkpoint;
		last_checkpoint.activate ();
	}
	
	// Update is called once per frame
	void Update () {

        float dt = Time.deltaTime;
        
        // CHEATCODES!!

        if (Input.GetKeyDown("n"))
        {
			MaskState.getMaskState().availableMasks.Add(Masks.StrongMask); // TEMP !!
			MaskState.getMaskState().availableMasks.Add(Masks.AirMask); // TEMP !!
        }

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
            xAcceleration = decelerationCurve.Evaluate(decPoint) * decSpeed;
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
        
        //Z - lock
        float z = 0;
        if (transform.position.z != startPosition.z)
        {
            z = startPosition.z - transform.position.z;
        }
			

        //Applying movement
        air_force_1.z = 0;
		var result = charController.Move(new Vector3(x, y, z) + (air_force_1 * dt));
		pushing = result == CollisionFlags.Sides;
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
            float rSpeed = Mathf.Clamp(Mathf.Abs(rotationStart - angle) / 90, 0.0001f, 2);
            float rT = (Time.time - rotationStartTime) / (rSpeed * rotationTime);
            float yRotation = Mathf.SmoothStep(rotationStart, angle, rT);
            mesh.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        }

        //Masks

        if (Input.GetButtonDown("SwitchMask"))
        {
			if (MaskState.getMaskState().availableMasks.Count > 1)
            {
                switch (current_mask)
                {
                    case Masks.NormalMask:
					if (MaskState.getMaskState().availableMasks.Contains(Masks.StrongMask))
                        {
                            equipMask(Masks.StrongMask);
                        }
                        else
                        {
                            equipMask(Masks.AirMask);
                        }
                        break;
                    case Masks.StrongMask:
					if (MaskState.getMaskState().availableMasks.Contains(Masks.AirMask))
                        {
                            equipMask(Masks.AirMask);
                        }
                        else
                        {
                            equipMask(Masks.NormalMask);
                        }
                        break;
                    case Masks.AirMask:
					if (MaskState.getMaskState().availableMasks.Contains(Masks.StrongMask))
                        {
                            equipMask(Masks.StrongMask);
                        }
                        else
                        {
                            equipMask(Masks.NormalMask);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        //Character hovering
        doHover();

        
    }

    void doHover()
    {
        hoverTime = (hoverTime + (Time.deltaTime * hoverSpeed)) % 2f;
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
    
    public void kill()
    {
        face(1);
		if(last_checkpoint){
			transform.position = last_checkpoint.transform.position;
		} else {
        	this.transform.position = startPosition;
		}
    }

    public void addMask(Masks newMask)
    {
		if (!MaskState.getMaskState().availableMasks.Contains(newMask))
        {
			MaskState.getMaskState().availableMasks.Add(newMask);
        }
        equipMask(newMask);
    }

    public bool equipMask(Masks newMask)
    {
		if (!MaskState.getMaskState().availableMasks.Contains(newMask))
        {
            return false;
        }

		if (newMask == Masks.AirMask) {
			foreach (var channel in channels) {
				channel.SetActive (true);
			}
		} else {
			foreach (var channel in channels) {
				channel.SetActive (false);
			}
		}
        mesh.transform.FindChild(newMask.ToString()).gameObject.SetActive(true);
        mesh.transform.FindChild(current_mask.ToString()).gameObject.SetActive(false);
        current_mask = newMask;

        return true;
    }

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic)
			return;

		if (hit.moveDirection.y < -0.3F)
			return;

		if (current_mask != Masks.StrongMask) {
			Vector3 pushBackDir = new Vector3 (-hit.moveDirection.x, -hit.moveDirection.y, 0)*body.velocity.magnitude;
			charController.Move (pushBackDir);
			return;
		}
		
		Vector3 pushDir = new Vector3 (hit.moveDirection.x, hit.moveDirection.y, 0);
		body.velocity = pushDir * moveSpeed*0.1f;
	}
}
