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
    private int scorePerPerfectNote = 300; // scorePerNote = maximumScore / numOfNote;
    private int scorePerGoodNote = 150;
    private int scorePerOkNote = 100;

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

        setHealthLoss();

        StartCoroutine(GameStart());
    }
    
    // Moved all health related values together so it was easily viewable during any future tuning, if you wanna move it back to the top you can - Jacob 
    // All these values are placeholders, as they're all set during setHealthLoss() based on the ingame difficulty. any tuning to difficulty. Change those numbers, not these
    private int currentHealth = 100;
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
    }
    private int damagePerMiss = 10; // the damage the player takes per note miss
    private int healthRecover = 10; // the damage the player heals upon heal
    private int notesTillHeal = 1; // the notes it takes till the player starts healing

    // Tuning for difficulty
    void setHealthLoss()
    {
        int scenario = SongManager.Instance.getCurrentDifficulty();

        switch (scenario)
        {
            case 0:
                damagePerMiss = 0;
                healthRecover = 0;
                break;
            case 1:
                damagePerMiss = 5;
                healthRecover = 5;
                notesTillHeal = 2;
                break;
            case 2:
                damagePerMiss = 5;
                healthRecover = 1;
                notesTillHeal = 5;
                break;
            case 3:
                damagePerMiss = 5;
                healthRecover = 1;
                notesTillHeal = 5;
                break;
            case 4:
                damagePerMiss = 10;
                healthRecover = 1;
                notesTillHeal = 10;
                break;
        }
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

        // sets the % based on the player getting a perfect score on each note in the song
        numOfNote = notes.Count;
        maximumScore = numOfNote * scorePerPerfectNote;

        foreach (var note in array)
        {
            if (note.Length > 64)
            {
                numOfNote++; // including the tail of holds
            }
        }

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
        if (currentCombo >= notesTillHeal && currentHealth < 100)
        {
            currentHealth += healthRecover;
        }
    }
    public void Hit()
    {
        currentCombo += 1;
        currentScore += (int)(scorePerOkNote);
        AudioManager.Instance.PlayHitSFX(); // play hit SFX
        JudgementText.Hmm();
        normalHit++;
        //Debug.Log("Normal hit scores " + (int)(scorePerNote * 0.5));
        RestoreHealth();
    }

    public void GoodHit()
    {
        currentCombo += 1;
        currentScore += (int)(scorePerGoodNote);
        JudgementText.Good();
        AudioManager.Instance.PlayHitSFX(); // play hit SFX
        goodHit++;
        //Debug.Log("Good hit scores " + (int)(scorePerNote * 0.8));
        RestoreHealth();
    }

    public void PerfectHit()
    {
        currentCombo += 1;
        currentScore += scorePerPerfectNote;
        JudgementText.Disco();
        AudioManager.Instance.PlayHitSFX(); // play hit SFX
        perfectHit++;
        //Debug.Log("Perfect hit scores " + scorePerNote);
        RestoreHealth();
    }

    public void PerfectHoldHit()
    {
        currentCombo += 1;
        currentScore += scorePerPerfectNote;
        //AudioManager.Instance.PlayHitSFX(); // play hit SFX
        perfectHit++;
        RestoreHealth();
    }

    public void Miss()
    {
        currentCombo = 0; // reset the combo
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
