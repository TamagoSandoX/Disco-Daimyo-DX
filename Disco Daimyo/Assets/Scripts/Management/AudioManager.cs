using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource titleMusic;

    public AudioSource gameoverSFX;
    public AudioSource hitSFX;
    public AudioSource miss1SFX;
    public AudioSource miss2SFX;
    public AudioSource miss3SFX;
    public AudioSource miss4SFX;
    public AudioSource miss6SFX;
    public AudioSource miss5SFX;
    public AudioSource miss7SFX;
    public AudioSource miss8SFX;
    public AudioSource miss9SFX;
    public AudioSource miss10SFX;
    public AudioSource miss11SFX;
    public AudioSource miss12SFX;
    public AudioSource miss13SFX;
    public AudioSource miss14SFX;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayTitleMusic()
    {
        titleMusic.Play();
    }

    public void StopTitleMusic()
    {
        titleMusic.Stop();
    }

    public void PlayGameoverSFX()
    {
        gameoverSFX.Play();
    }

    public void PlayHitSFX()
    {
        hitSFX.Play();
    }

    public void PlayMissSFX()
    {
        miss1SFX.Play();
    }
}
