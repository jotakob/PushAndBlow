using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadOperations: MonoBehaviour {

    public AudioManager audio;
	public Image myImage;
	// Use this for initialization
	public bool UseAsync;
	private AsyncOperation async = null;
	public int LevelToLoad;

	public float FadeoutTime;
	public float fadeSpeed = 1.5f; 
	private bool fadeout;
	private bool fadein;    

	public bool change_level = false;

	void Start(){
		FadeIn ();
	}


	public void FadeOut(){
		fadein= false;
		fadeout = true;
        audio.globalFadeOut(fadeSpeed);
	}

	public void FadeIn(){
		myImage.color = Color.black;

		fadeout = false;
		fadein = true;
        audio.globalFadeIn(fadeSpeed);
    }

	void Update(){
		if (change_level) {
			FadeOut ();
			change_level = false;
		}

		if(async != null){
			Debug.Log(async.progress);
			//When the Async is finished, the level is done loading, fade in the screen
			if(async.progress >= 1.0){
				async = null;
				FadeIn();
			}
		}

		//Fade Out the screen to black
		if(fadeout){
			myImage.color = Color.Lerp(myImage.color, Color.black, fadeSpeed * Time.deltaTime);
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

			foreach(var player in players)
				player.SendMessage ("frezze", SendMessageOptions.DontRequireReceiver);
			//Once the Black image is visible enough, Start loading the next level
			if(myImage.color.a >= 0.999){
				StartCoroutine("LoadALevel");
				fadeout = false;
			}
		}

		if(fadein){
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

			foreach(var player in players)
				player.SendMessage ("frezze", SendMessageOptions.DontRequireReceiver);
			
			myImage.color = Color.Lerp(myImage.color, new Color(0,0,0,0), fadeSpeed * Time.deltaTime);

			if(myImage.color.a <= 0.01){
				fadein = false;
				foreach(var player in players)
					player.SendMessage ("unfrezze", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void LoadLevel(int index){
		if(UseAsync){
			LevelToLoad= index;
		}else{
			if (SceneManager.sceneCount <= index) {
				SceneManager.LoadScene (index, LoadSceneMode.Single);
			} else {
				SceneManager.SetActiveScene (SceneManager.GetSceneAt (index));
			}
		}
	}

	public IEnumerator LoadALevel() {
		if (SceneManager.sceneCount > LevelToLoad) {
			SceneManager.SetActiveScene (SceneManager.GetSceneAt (LevelToLoad));
			yield return true;
		}
		
		async = SceneManager.LoadSceneAsync(LevelToLoad,LoadSceneMode.Single);
		yield return async;
	}
}
