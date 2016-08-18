using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	private ParticleSystem system;

	// Use this for initialization
	void Start () {
		system = GetComponent<ParticleSystem> ();
	}

	public void activate(){
		system.Play ();
	}
	public void deactivate(){
		system.Stop ();
	}

	void OnTriggerEnter(Collider other){
		other.SendMessage ("checkpoint", this);
	}
}
