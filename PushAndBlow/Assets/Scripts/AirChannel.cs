using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class AirChannel : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		SendMessageUpwards ("add_object", other);
	}
	void OnTriggerExit(Collider other){
		SendMessageUpwards ("remove_object", other);
	}
}
