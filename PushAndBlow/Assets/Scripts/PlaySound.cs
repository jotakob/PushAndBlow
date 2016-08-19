using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    public int collisions = 1;
    public AudioClip soundOnEnable;
    public AudioClip soundOnCollision;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Enable()
    {
        GetComponent<AudioSource>().PlayOneShot(soundOnEnable);
    }

    void OnCollision (Collider other)
    {
        if (collisions != 0)
        {
            GetComponent<AudioSource>().PlayOneShot(soundOnCollision);
        }
    }
}
