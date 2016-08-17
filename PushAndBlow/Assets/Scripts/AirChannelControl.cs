using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirChannelControl : MonoBehaviour {

	public GameObject start_object;
	public GameObject end_object;
	private GameObject air_collider;
	private BoxCollider air_box_collider;
	private Vector3 start ;
	private Vector3 end;

	private List<Rigidbody> moveing = new List<Rigidbody> ();

	public float size = 1;
	public float strength = 1;

	// Use this for initialization
	void Start () {
		if (!air_collider) {
			air_collider = new GameObject ("AirCollider");
			air_collider.transform.parent = transform;
			air_box_collider = air_collider.AddComponent<BoxCollider> ();
			air_collider.AddComponent<AirChannel> ();
			air_box_collider.isTrigger = true;
		}
		start = start_object.transform.position;
		end = end_object.transform.position;

		var center = (end - start) * 0.5f + start;
		air_collider.transform.position = center;
		var length = Vector3.Distance (start,end);
		air_collider.transform.rotation = Quaternion.LookRotation (end - start);
		air_box_collider.size = new Vector3 (size, size,length);
	}

	void add_object(Collider col){
		Debug.Log ("added");
		moveing.Add (col.attachedRigidbody);

		var start = start_object.transform.position;
		var end = end_object.transform.position;

		var velo = end - start;
		velo.Normalize ();
		col.attachedRigidbody.AddForce (velo * strength, ForceMode.Impulse);
	}

	void remove_object(Collider col){
		moveing.Remove (col.attachedRigidbody);
	}
	
	// Update is called once per frame
	void Update () {


		var velo = end - start;
		velo.Normalize ();


		foreach (var body in moveing) {
			body.AddForce (velo * strength, ForceMode.VelocityChange);
		}
	}

	void OnDrawGizmos(){
		var start = start_object.transform.position;
		var end = end_object.transform.position;

		var center = (end - start) * 0.5f + start;
		var length = Vector3.Distance (start,end);

		Gizmos.DrawLine (start, end);

		Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, Quaternion.LookRotation (end - start), Vector3.one);
		Gizmos.matrix = rotationMatrix; 
		Gizmos.DrawWireCube (Vector3.zero,new Vector3 (size, size,length));

	}
}
