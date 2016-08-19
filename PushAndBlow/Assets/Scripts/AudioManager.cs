using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public GameObject player;
    PlayerMovement playerChar;
    PlayerMovement.Masks lastMask;

    float[] musicMaxVolumes = new float[3];
    float[] ambientMaxVolumes = new float[3];
    float[] musicMinVolumes = new float[3];
    float[] ambientMinVolumes = new float[3];
    public float crossFadeTime = 0.5f;
    public float reduceMainTo = 0.8f;
    public AudioSource[] music;
    public AudioSource[] ambient;
    float fadeTime;
    int fadeOutMask;
    int fadeInMask;
    int globalFade = 0;
    float globalFadeTime;

    // Use this for initialization
    void Start () {
        playerChar = player.GetComponentInChildren<PlayerMovement>();
        for (int i = 0; i < music.Length; i++)
        {
            musicMaxVolumes[i] = music[i].volume;
            musicMinVolumes[i] = i == 0 ? reduceMainTo * music[i].volume : 0;
            music[i].volume = 0;
        }

        for (int i = 0; i < ambient.Length; i++)
        {
            ambientMaxVolumes[i] = ambient[i].volume;
            ambientMinVolumes[i] = i == 0 ? reduceMainTo * ambient[i].volume : 0;
            ambient[i].volume = 0;
        }
        globalFadeIn(2);
    }
	
    public void globalFadeOut(float time)
    {
        globalFade = 2;
        fadeTime = 0;
        globalFadeTime = time;
    }

    public void globalFadeIn(float time)
    {
        globalFade = 1;
        fadeTime = 0;
        globalFadeTime = time;
    }

    // Update is called once per frame
    void Update () {
        PlayerMovement.Masks mask = playerChar.current_mask;

        fadeTime += Time.deltaTime;
        if (fadeTime >= globalFadeTime)
        {
            globalFade = 0;
        }

        if (globalFade != 2)
        {

            if (mask != lastMask)
            {
                if (fadeTime <= crossFadeTime)
                {
                    music[fadeOutMask].volume = 0;
                    ambient[fadeOutMask].volume = 0;
                }
                fadeOutMask = (int)lastMask;
                fadeInMask = (int)mask;
                fadeTime = 0;
                music[fadeInMask].mute = false;
                ambient[fadeInMask].mute = false;
            }

            if (fadeTime <= crossFadeTime)
            {
                music[fadeOutMask].volume = Mathf.Lerp(musicMaxVolumes[fadeOutMask], musicMinVolumes[fadeOutMask], fadeTime / crossFadeTime);
                music[fadeInMask].volume = Mathf.Lerp(musicMinVolumes[fadeInMask], musicMaxVolumes[fadeInMask], fadeTime / crossFadeTime);
                ambient[fadeOutMask].volume = Mathf.Lerp(ambientMaxVolumes[fadeOutMask], ambientMinVolumes[fadeOutMask], fadeTime / crossFadeTime);
                ambient[fadeInMask].volume = Mathf.Lerp(ambientMinVolumes[fadeInMask], ambientMaxVolumes[fadeInMask], fadeTime / crossFadeTime);
            }

            lastMask = mask;
        }   
        if (globalFade == 1)
        {
            music[0].volume = Mathf.Lerp(0, musicMaxVolumes[0], fadeTime / globalFadeTime);
            ambient[0].volume = Mathf.Lerp(0, musicMaxVolumes[0], fadeTime / globalFadeTime);
        }
        else if (globalFade == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                music[i].volume = Mathf.Lerp(musicMaxVolumes[i], 0, fadeTime / globalFadeTime);
                ambient[i].volume = Mathf.Lerp(musicMaxVolumes[i], 0, fadeTime / globalFadeTime);
            }
        }
	}
}
