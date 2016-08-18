using UnityEngine;
using System.Collections;

public class Kill : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		other.SendMessage ("kill", SendMessageOptions.DontRequireReceiver);
	}
}
