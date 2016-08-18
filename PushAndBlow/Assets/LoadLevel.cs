using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

	public LoadOperations load_fader;
	public int next_level = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
			load_fader.LevelToLoad = next_level;
			load_fader.FadeOut();
        }
    }
}
