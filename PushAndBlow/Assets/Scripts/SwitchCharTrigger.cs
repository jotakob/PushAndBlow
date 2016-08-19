using UnityEngine;
using System.Collections;

public class SwitchCharTrigger : MonoBehaviour {

	public Animator switch_anim;

	void OnTriggerStay(Collider other){
		if (other.CompareTag ("Player")) {
			if (Input.GetButtonDown ("Interact")) {
				switch_anim.SetBool ("on", !switch_anim.GetBool ("on"));
			}
		}
	}
}
