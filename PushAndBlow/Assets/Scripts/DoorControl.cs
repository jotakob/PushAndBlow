using UnityEngine;
using System.Collections;

public class DoorControl : MonoBehaviour {

	public Animator door_animator;
	public bool initalstate;
	void Start(){
		door_animator.SetBool ("close", initalstate);
	}

	void switch_on(){
		door_animator.SetBool ("close", false);
	}

	void switch_off(){
		door_animator.SetBool ("close", true);
	}
}
