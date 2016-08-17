using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {

	public GameObject target; 
	public bool invert = false;

	void switch_on(){
		if (invert) {
			target.SendMessage ("switch_off", SendMessageOptions.DontRequireReceiver);

		} else {
			target.SendMessage ("switch_on", SendMessageOptions.DontRequireReceiver);
		}
	}
	void switch_off(){
		if (invert) {
			target.SendMessage ("switch_on", SendMessageOptions.DontRequireReceiver);
		} else {
			target.SendMessage ("switch_off", SendMessageOptions.DontRequireReceiver);
		}
	}
	
}
