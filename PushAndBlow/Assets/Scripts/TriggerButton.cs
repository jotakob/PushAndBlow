using UnityEngine;
using System.Collections;

public class TriggerButton : MonoBehaviour {

	public Animator button_animator;

	void OnTriggerEnter(Collider other){
		button_animator.SetBool ("pressed", true);
        GetComponent<AudioSource>().Play();
	}
	void OnTriggerExit(Collider other){
		button_animator.SetBool ("pressed", false);
	}
}
