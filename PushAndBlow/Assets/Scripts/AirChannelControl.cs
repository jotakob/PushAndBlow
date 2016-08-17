using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirChannelControl : MonoBehaviour {

	public enum AirChannelMode{
		IMPULSE,
		CHANNEL
	}

	public GameObject start_object;
	public GameObject end_object;
	private GameObject air_collider;
	private BoxCollider air_box_collider;
	private Vector3 start ;
	private Vector3 end;

	private List<Rigidbody> moveing = new List<Rigidbody> ();

	public float size = 1;
	public float strength = 1;
	public AirChannelMode mode;

	public ParticleSystem base_system_channel;

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

		if (base_system_channel) {
			ParticleSystem my_particles = (ParticleSystem)GameObject.Instantiate (base_system_channel);
			my_particles.transform.parent = transform;
			my_particles.transform.position = start_object.transform.position;
			my_particles.transform.rotation = air_collider.transform.rotation;
			my_particles.startSpeed = strength;
			my_particles.startLifetime = length/strength;
			if (mode == AirChannelMode.IMPULSE) {
				my_particles.startLifetime = length / strength;
				var vol = my_particles.velocityOverLifetime;
				vol.enabled = true;

				var col = my_particles.colorOverLifetime;
				col.enabled = true;
			}
			var sh = my_particles.shape;
			sh.box = new Vector3 (size, size, 0.2f);
		}
	}

	void add_object(Collider col){
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
		if (mode == AirChannelMode.IMPULSE)
			return;
		
		var velo = end - start;
		velo.Normalize ();
		foreach (var body in moveing) {
			var pro_velo = Vector3.Project (body.transform.position - start, velo)-(body.transform.position - start);
			body.velocity = velo * strength + pro_velo;
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
		Gizmos.matrix = Matrix4x4.identity; 

	}
}
