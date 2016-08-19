using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour {
    
    public AudioClip soundOnEnable;
    public AudioClip soundOnCollision;
    public AudioSource rollingAudio;
    public AudioSource otherAudio;
    public float rollingVolume = 1;
    public float groundedTimer = 0.2f;
    float airTime = 0;
    public bool isGrounded = true;

    float distToGround;

    // Use this for initialization
    void Start ()
    {
        distToGround = GetComponent<SphereCollider>().radius;

    }
	
	// Update is called once per frame
	void Update ()
    {
        float v = GetComponent<Rigidbody>().velocity.magnitude * rollingVolume;
        v -= 0.3f;

        if (isOnGround())
        {
            rollingAudio.volume = v;
            airTime = 0;
            isGrounded = true;
        }
        else
        {
            rollingAudio.volume = v * (1 - airTime / groundedTimer);
            airTime += Time.deltaTime;
            if (airTime > groundedTimer)
            {
                isGrounded = false;
            }
        }
	}

    void OnEnable()
    {
        otherAudio.PlayOneShot(soundOnEnable);
    }

    void OnCollisionEnter (Collision col)
    {
        
        if (col.relativeVelocity.magnitude >= 10)
        {
            otherAudio.PlayOneShot(soundOnCollision, (col.relativeVelocity.magnitude - 10) / 10);
        }
    }

    bool isOnGround()
    {
        return (Physics.Raycast(transform.position, Vector3.down, distToGround + 0.2f ) || Physics.Raycast(transform.position, new Vector3(1, -2 , 0), distToGround + 0.2f) || Physics.Raycast(transform.position, new Vector3(-1, -2, 0), distToGround + 0.2f));
    }
}

