using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource hitSFX;
    public AudioSource missSFX;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayHitSFX()
    {
        hitSFX.Play();
    }

    public void PlayMissSFX()
    {
        missSFX.Play();
    }
}
