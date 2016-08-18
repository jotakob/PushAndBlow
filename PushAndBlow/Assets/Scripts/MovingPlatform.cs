using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public enum MoveType{
		ONCE,
		PINGPONG,
		PINGPONG_ONCE,
	}

	public GameObject start;
	public GameObject end;

	public float travel_time = 3;
	[Range(0.0f, 1.0f)]
	public float state;
	public MoveType movement;
	public bool forward = true;

	private Vector3 start_pos;
	private Vector3 end_pos;
	private bool stop = false;
	private bool runeonce = false;

	// Use this for initialization
	void Start () {
		start_pos = start.transform.position;
		end_pos = end.transform.position;
	}

	void switch_on(){
		runeonce = false;
		stop = false;
	}
	void switch_off(){
		stop = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!stop) {
			state = Mathf.Clamp01 (state + ((forward ? Time.deltaTime : -Time.deltaTime)/travel_time));

			switch (movement) {
			case MoveType.ONCE:
				{
					if (state >= 1.0f) {
						forward = false;
						stop = true;
					} else if (state <= 0.0f) {
						forward = true;
						stop = true;
					}
				}
				break;
			case MoveType.PINGPONG:
				{
					if (state >= 1.0f) {
						forward = false;
					} else if (state <= 0.0f) {
						forward = true;
					}
				}
				break;
			case MoveType.PINGPONG_ONCE:
				{
					if (state >= 1.0f) {
						forward = false;
						if (runeonce) {
							stop = true;
						}
						runeonce = true;
					} else if (state <= 0.0f) {
						forward = true;
						if (runeonce) {
							stop = true;
						}
						runeonce = true;

					}
				}
				break;
			}

			transform.position = Vector3.Lerp (start_pos, end_pos, state);

			if (state >= 1.0f) {
				forward = false;
			} else if (state <= 0.0f) {
				forward = true;
			} 
		}
	}

	void OnGoizmoDraw(){
		Gizmos.DrawLine (start.transform.position, end.transform.position);
	}
}
