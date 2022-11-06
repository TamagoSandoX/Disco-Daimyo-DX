using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{

    // Singleton
    static private SettingsManager instance;
    static public SettingsManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("There is no SettingsManager instance in the scene.");
            }
            return instance;
        }
    }

    public AudioMixer mixer;


    //float screenResolution;

    public int screenMode; // {0: Full screen, 1: Windowed}
    public int offset;
    public float noteSpeed;
    public float mainVolume;
    public float songVolume;
    public float sfxVolume;

    public int highScore;


    private SongManager songManager;

    void Awake()
    {
        if (instance != null)
        {
            // destroy duplicates
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        songManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<SongManager>();
        GetSettings();
        ApplySettings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetSettings()
    {
        //screenResolution = PlayerPrefs.GetFloat("ScreenResolution");
        screenMode = PlayerPrefs.GetInt("ScreenMode");
        noteSpeed = PlayerPrefs.GetFloat("NoteSpeed");
        if (noteSpeed <= 0)
        {
            noteSpeed = 1;
            PlayerPrefs.SetFloat("NoteSpeed", 1);
        }
        offset = PlayerPrefs.GetInt("Offset");
        mainVolume = PlayerPrefs.GetFloat("MainVolume");
        songVolume = PlayerPrefs.GetFloat("SongVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");



    }

    void ApplySettings()
    {
        if (screenMode == 0)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
