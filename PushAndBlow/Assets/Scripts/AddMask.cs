using UnityEngine;
using System.Collections;

public class AddMask : MonoBehaviour {

	public PlayerMovement.Masks mask_to_add;

	void OnTriggerEnter(Collider other){
		other.SendMessage ("addMask", mask_to_add, SendMessageOptions.DontRequireReceiver);
		GameObject.Destroy (gameObject);
	}
}
