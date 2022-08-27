using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    // Singleton
    static private GameManager instance;
    static public GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("There is no GameManager instance in the scene.");
            }
            return instance;
        }
    }
    #endregion

    #region Note holders

    public Lane[] lanes;
    
    public JudgementText JudgementText;
    public PlayerActionSwipeRight playerActionSwipeRight;
    public PlayerActionSwipeLeft playerActionSwipeLeft;

    #endregion

    #region In-Game Data
    private int numOfNote;
    public int NumOfNote
    {
        get
        {
            return numOfNote;
        }
    }


    public int maximumScore = 1000000; // same across the levels
    private float scorePerNote; // scorePerNote = maximumScore / numOfNote;

    private float currentScore = 0;
    public float CurrentScore
    {
        get
        {
            return currentScore;
        }
    }

    private float highestScore;
    public float HighestScore
    {
        get
        {
            return highestScore;
        }
    }
    
    private int currentCombo = 0;
    public int CurrentCombo
    {
        get
        {
            return currentCombo;
        }
    }

    private int maxCombo;
    public int MaxCombo
    {
        get
        {
            return maxCombo;
        }
    }


    public int perfectHit;
    public int goodHit;
    public int normalHit;
    public int miss;

    private int currentHealth = 100;
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
    }

    public int damagePerMiss = 10; // i assume that this should depends on the number of notes for each song.
    public int healthRecover = 1;

    public double marginOfError; // in seconds
    public double perfectOffset;
    public double goodOffset;

    public double mouseFlickMOE;
    public float mouseSensitivity;

    public int inputDelayInMilliseconds;

    public float noteTime;

    public float noteSpawnY;

    public float noteTapY; // This should be the position of judgement line in default, and allows user to change offset
    public float noteDespawnY
    {
        get
        {
            return noteTapY - (noteSpawnY - noteTapY);
        }
    }

    private float gameOverTimer;

    private float gameStartTimer;
    #endregion

    #region State Flags
    private int numOfEndedLane = 0; // This should be the position of judgement line in default, and allows user to change offset
    public int NumOfEndedLane
    {
        get
        {
            return numOfEndedLane;
        }
        set
        {
            numOfEndedLane ++;
        }
    }


    private bool isGameOver = false;
    public bool IsGameOver
    {
        get
        {
            return isGameOver;
        }
    }

    public bool isStarted;

    #endregion

    #region Linkage

    private SongManager songManager;
    private SettingsManager settingsManager;
    public static MidiFile thisStage;
    private UIManager uiManager;
    public GameObject spawner;
    #endregion


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
        }
    }

    void Start()
    {
        GameObject songManagerObject = GameObject.FindWithTag("AudioManager");
        if (songManagerObject != null)
            songManager = songManagerObject.GetComponent<SongManager>();
        if (songManager == null)
            Debug.Log("Cannot find 'songManager' script");

        thisStage = songManager.GetCurrentStage(); // load the state
        GetDataFromMidi(thisStage); // load the timestamps

        GameObject settingsManagerObject = GameObject.FindWithTag("SettingsManager");
        if (settingsManagerObject != null)
            settingsManager = settingsManagerObject.GetComponent<SettingsManager>();
        if (settingsManager == null)
            Debug.Log("Cannot find 'settingsManager' script");

        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        noteSpawnY = spawner.transform.localPosition.y;
        noteTime = (1f / settingsManager.noteSpeed) * 5f;

        inputDelayInMilliseconds = settingsManager.offset;

        gameStartTimer = 0f;
        gameOverTimer = 12f;

        currentScore = 0; 
        currentCombo = 0;
        currentHealth = 100;
        
        highestScore = PlayerPrefs.GetInt("Highest Score" + songManager.getCurrentSong().title); // load the highest score in last session

        StartCoroutine(GameStart());

    }

    void Update()
    {
        GameLose();
        GameOver();
        checkMaxCombo();
    }

    public void GetDataFromMidi(MidiFile stage)
    {
        var notes = stage.GetNotes();
        var array = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        notes.CopyTo(array, 0);

        numOfNote = notes.Count;

        foreach (var note in array)
        {
            if (note.Length > 64)
            {
                numOfNote++; // including the tail of holds
            }
        }
        
         // Pass total number of notes to GM

        scorePerNote = maximumScore / numOfNote;

        foreach (var lane in lanes) lane.SetTimeStamps(array); // pass midi data to lane script
        playerActionSwipeRight.SetTimeStamps(array);
        playerActionSwipeLeft.SetTimeStamps(array);

    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(gameStartTimer);
        Debug.Log("Game Start");
        isStarted = true;
        yield return songManager.StartSong();
    }

    void checkMaxCombo()
    {
        if (maxCombo < currentCombo)
        {
            maxCombo = currentCombo;
            PlayerPrefs.SetInt("Max Combo", maxCombo);
        }
    }

    public void RestoreHealth()
    {
        if (currentCombo >= 5 && currentHealth < 100)
        {
            currentHealth += healthRecover;
        }
    }
    public void Hit()
    {
        currentCombo += 1;
        currentScore += (int)(scorePerNote * 0.5);
        AudioManager.Instance.PlayHitSFX(); // play hit SFX
        JudgementText.Hmm();
        normalHit++;
        //Debug.Log("Normal hit scores " + (int)(scorePerNote * 0.5));
        RestoreHealth();
    }

    public void GoodHit()
    {
        currentCombo += 1;
        currentScore += (int)(scorePerNote * 0.8);
        JudgementText.Good();
        AudioManager.Instance.PlayHitSFX(); // play hit SFX
        goodHit++;
        //Debug.Log("Good hit scores " + (int)(scorePerNote * 0.8));
        RestoreHealth();
    }

    public void PerfectHit()
    {
        currentCombo += 1;
        currentScore += scorePerNote;
        JudgementText.Disco();
        AudioManager.Instance.PlayHitSFX(); // play hit SFX
        perfectHit++;
        //Debug.Log("Perfect hit scores " + scorePerNote);
        RestoreHealth();
    }

    public void PerfectHoldHit()
    {
        currentCombo += 1;
        currentScore += scorePerNote;
        //AudioManager.Instance.PlayHitSFX(); // play hit SFX
        perfectHit++;
        RestoreHealth();
    }

    public void Miss()
    {
        currentCombo = 0; // reset the combo
        currentScore += 0; // no score added
        currentHealth -= damagePerMiss; // loss health when miss a note
        JudgementText.Miss();
        AudioManager.Instance.PlayMissSFX(); // play miss SFX
        miss++;
    }

    public void GameLose()
    {
        if (currentHealth <= 0)
        {
            isStarted = false;
            Time.timeScale = 0;
            StartCoroutine(songManager.StopSong());
            uiManager.Lose();
        }
    }

    private bool checkGameOver()
    {
        if (numOfEndedLane == 5) // 3 lanes plus two swipe actions (every lanes have reached to the end)
        {
            isGameOver = true;
        }
        return isGameOver;
    }

    public void GameOver()
    {
        if (checkGameOver())
        {
            if (highestScore < currentScore)
            {
                highestScore = currentScore;
            }

            PlayerPrefs.SetInt("Highest Score" + songManager.getCurrentSong().title, (int)highestScore);

            PlayerPrefs.SetInt("Total Score", (int)currentScore);

            gameOverTimer -= Time.deltaTime; // start counting

            if (gameOverTimer <= 0)
            {
                songManager.audioSource.Stop(); // stop music (TBC)
                
            }
            
        }
    }


    public void GamePause()
    {
        Time.timeScale = 0;
        isStarted = false;
        StartCoroutine(songManager.PauseSong());
    }

    public void GameResume()
    {
        Time.timeScale = 1;
        isStarted = true;
        StartCoroutine(songManager.ResumeSong());
    }

    public void GameRestart()
    {
        SceneManager.LoadScene("GameBase", LoadSceneMode.Single);
        thisStage = songManager.GetCurrentStage(); // reload this stage
        GetDataFromMidi(thisStage); // reload the timestamps
        Time.timeScale = 1;
        currentHealth = 100;
        StartCoroutine(songManager.RestartSong());
        
    }
}
