using UnityEngine;
using System.Collections;

public class EanbleEntity : MonoBehaviour {
	public GameObject target;
	public int triggertimes = 1;

	void switch_on(){
		if(triggertimes <=0)
			target.SetActive (true);
		
		triggertimes--;
	}
	void switch_off(){
		//target.SetActive (false);
	}
}
