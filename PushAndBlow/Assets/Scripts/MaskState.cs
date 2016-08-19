using UnityEngine;
using System.Collections;

public class MaskState : MonoBehaviour {

	public ArrayList availableMasks = new ArrayList();


	private static MaskState holder;
	public static MaskState getMaskState(){
		if (!holder) {
			var go = new GameObject ("MaskInfo");
			holder = go.AddComponent<MaskState> ();
		}

		return holder;
	}


	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
