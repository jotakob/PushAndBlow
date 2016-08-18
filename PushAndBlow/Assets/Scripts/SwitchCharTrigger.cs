using UnityEngine;
using System.Collections;

public class SwitchCharTrigger : MonoBehaviour {

	public Animator switch_anim;

	void OnTriggerStay(Collider other){
		if (other.CompareTag ("Player")) {
			if (Input.GetButtonDown ("Fire2")) {
				switch_anim.SetBool ("on", !switch_anim.GetBool ("on"));
			}
		}
	}
}
