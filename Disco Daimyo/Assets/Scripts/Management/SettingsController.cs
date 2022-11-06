using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{

    public GameObject gameMenuPanel;
    public GameObject settingsPanel;

    public Toggle screenModeToggle;


    public Slider noteSpeedSlider;
    public TextMeshProUGUI noteSpeedText;

    public Slider offsetSlider;
    public TextMeshProUGUI offsetText;

    public Slider main_volumeSlider;
    //private float mainVolume;

    public Slider song_volumeSlider;
    //private float songVolume;

    public Slider sfx_volumeSlider;
    //private float sfxVolume;


    public GameObject titleImage;
    public GameObject controlPanel;

    private SettingsManager settingsManager;

    // Start is called before the first frame update
    void Start()
    {
        settingsManager = GameObject.FindGameObjectWithTag("SettingsManager").GetComponent<SettingsManager>();
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Initialization()
    {
        if (PlayerPrefs.GetInt("ScreenMode") == 0)
        {
            screenModeToggle.isOn = true;
        }
        else
        {
            screenModeToggle.isOn = false;
        }

        noteSpeedSlider.value = settingsManager.noteSpeed;
        offsetSlider.value = settingsManager.offset;

        float mainVolume;
        float songVolume;
        float sfxVolume;
        settingsManager.mixer.GetFloat("Master", out mainVolume);
        settingsManager.mainVolume = mainVolume / 2f;
        main_volumeSlider.value = PlayerPrefs.GetFloat("MainVolume");
        settingsManager.mixer.GetFloat("Song", out songVolume);
        settingsManager.songVolume = songVolume / 2f;
        song_volumeSlider.value = PlayerPrefs.GetFloat("SongVolume");
        settingsManager.mixer.GetFloat("SFX", out sfxVolume);
        settingsManager.sfxVolume = sfxVolume / 2f;
        sfx_volumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");


    }

    public void Return()
    {
        gameMenuPanel.GetComponent<RawImage>().CrossFadeAlpha(1f, 1f, true); // fade in effect
        titleImage.SetActive(true); // display the title
        controlPanel.SetActive(true); // display the control panel
        settingsPanel.SetActive(false);
    }

    public void SetScreenMode()
    {
        if (screenModeToggle.isOn)
        {

            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            settingsManager.screenMode = 0;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            settingsManager.screenMode = 1;
        }
        PlayerPrefs.SetInt("ScreenMode", settingsManager.screenMode);

    }

    public void SetNoteSpeed()
    {
        settingsManager.noteSpeed = noteSpeedSlider.value;
        noteSpeedText.text = (noteSpeedSlider.value).ToString() + "x";
        PlayerPrefs.SetFloat("NoteSpeed", settingsManager.noteSpeed);
    }

    public void SetOffset()
    {
        settingsManager.offset = (int)offsetSlider.value;
        offsetText.text = offsetSlider.value.ToString() + "ms";
        PlayerPrefs.SetInt("Offset", settingsManager.offset);
    }


    public void SetMainVolume()
    {
        settingsManager.mainVolume = main_volumeSlider.value;
        settingsManager.mixer.SetFloat("Master", main_volumeSlider.value * 2f);
        if (main_volumeSlider.value == -10)
        {
            settingsManager.mixer.SetFloat("Master", -80f);
        }
        PlayerPrefs.SetFloat("MainVolume", settingsManager.mainVolume);
        //Debug.Log(PlayerPrefs.GetFloat("MainVolume"));
    }

    public void SetSongVolume()
    {
        settingsManager.songVolume = song_volumeSlider.value;
        settingsManager.mixer.SetFloat("Song", song_volumeSlider.value * 2f);
        if (song_volumeSlider.value == -10)
        {
            settingsManager.mixer.SetFloat("Song", -80f);
        }
        PlayerPrefs.SetFloat("SongVolume", settingsManager.songVolume);
        //Debug.Log(PlayerPrefs.GetFloat("SongVolume"));
    }

    public void SetSFXVolume()
    {
        settingsManager.sfxVolume = sfx_volumeSlider.value;
        settingsManager.mixer.SetFloat("SFX", sfx_volumeSlider.value * 2f);
        if (sfx_volumeSlider.value == -10)
        {
            settingsManager.mixer.SetFloat("SFX", -80f);
        }
        PlayerPrefs.SetFloat("SFXVolume", settingsManager.sfxVolume);
        //Debug.Log(PlayerPrefs.GetFloat("SFXVolume"));
    }

    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
        
    }

    //added purely to clean up the menu settings UI 
    public GameObject hotkeysTab, visualTab; // link these to menu objects and call the functions with buttons
    public void ToggleHotkeys()
    {
        hotkeysTab.SetActive(true);
        visualTab.SetActive(false);
        //accessibility.SetActive(false);
    }
    public void ToggleVisual()
    {
        hotkeysTab.SetActive(false);
        visualTab.SetActive(true);
        //accessibility.SetActive(false);
    }
    //public void ToggleAccessibilty()
    //{
    //    hotkeys.SetActive(false);
    //    visual.SetActive(false);
    //    accessibility.SetActive(true);
    //}
}
