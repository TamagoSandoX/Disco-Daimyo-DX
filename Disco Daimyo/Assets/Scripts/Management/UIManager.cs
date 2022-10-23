using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{

    // Singleton
    static private UIManager instance;
    static public UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("There is no UIManager instance in the scene.");
            }
            return instance;
        }
    }

    // In-game

    public GameObject InGamePanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    public Slider healthBar;
    float lerpSpeed;


    // Song Information Panel
    public TextMeshProUGUI songTitleText;
    public TextMeshProUGUI songComposerText;
    public TextMeshProUGUI songBPMText;
    public RawImage songCoverImage;
    public TextMeshProUGUI songDifficultyText;

    // Pause Panel

    public GameObject pausePanel;


    // Lose Panel

    public GameObject losePanel;

    // Summary

    public GameObject summaryPanel;

    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI highestScoreText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI percentageText;
    public TextMeshProUGUI rankText;
    public RawImage songCoverImage_summary;
    public TextMeshProUGUI perfectHitText;
    public TextMeshProUGUI goodHitText;
    public TextMeshProUGUI normalHitText;
    public TextMeshProUGUI missText;

    public Image black;
    public Animator anim;

    private string[] rankTextList = { "E", "D", "C", "B", "A-", "A+", "AAA+", "S" };
    private float[] rankCompare = { 0.5f, 0.55f, 0.6f, 0.7f, 0.85f, 0.98f, 1.0f };
    private List<Color> rankTextColor = new List<Color>() { Color.red, Color.magenta, Color.blue, Color.blue, Color.green, Color.green, Color.cyan, Color.gray };


    private float summaryTimer;

    void Start()
    {
        InGamePanel.SetActive(true);
        pausePanel.SetActive(false);
        losePanel.SetActive(false);
        summaryTimer = 6f;
        StartCoroutine(loadCover(SongManager.Instance.getCurrentSong().bannerPath, songCoverImage));
        StartCoroutine(loadCover(SongManager.Instance.getCurrentSong().bannerPath, songCoverImage_summary));
        songTitleText.text = SongManager.Instance.getCurrentSong().title;
        songComposerText.text = SongManager.Instance.getCurrentSong().artist;
        songBPMText.text = "BPM: " + SongManager.Instance.getCurrentSong().bpm.ToString();
        songDifficultyText.text = SongManager.Instance.getCurrentDifficulty().ToString();
        setDifficultyColor();
    }


    void setDifficultyColor()
    {
        int key = SongManager.Instance.getCurrentDifficulty();

        switch (key)
        {
            case 0:
                songDifficultyText.text = "BEGINNER";
                songDifficultyText.color = Color.blue;
                break;
            case 1:
                songDifficultyText.text = "EASY";
                songDifficultyText.color = Color.green;
                break;
            case 2:
                songDifficultyText.text = "MEDIUM";
                songDifficultyText.color = Color.yellow;
                break;
            case 3:
                songDifficultyText.text = "HARD";
                songDifficultyText.color = Color.red;
                break;
            case 4:
                songDifficultyText.text = "CHALLENGE";
                songDifficultyText.color = Color.magenta;
                break;
        }
    }

    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;

        scoreText.text = GameManager.Instance.CurrentScore.ToString();  // display score text
        comboText.text = GameManager.Instance.CurrentCombo.ToString() + "X";  // display score text
        healthBar.value = GameManager.Instance.CurrentHealth; // display healthbar
        //healthBar.value = Mathf.Lerp(healthBar.value, GameManager.Instance.CurrentHealth, lerpSpeed); // display healthbar
        HealthColorChanger();

        if (Input.GetKeyDown(KeyCode.Escape) && losePanel.active == false)
        {
            Pause();
        }
        if (summaryPanel.active == true && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(enterMenu());
        }
    }

    void HealthColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (GameManager.Instance.CurrentHealth));
        healthBar.GetComponentInChildren<Image>().color = healthColor;
    }

    int getRank(float percent)
    {
        int rankNum = 0;
        for (int i = 0; i < rankCompare.Length; i++)
        {
            if (percent >= rankCompare[i]) rankNum = i;
        }
        return rankNum;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver)
        {
            summaryTimer -= Time.fixedDeltaTime;
            if (summaryTimer <= 0)
            {
                summaryPanel.SetActive(true);
                InGamePanel.SetActive(false);
                displaySummary();
                summaryTimer = 0;
            }  
        }
        
    }

    IEnumerator loadCover(string path, RawImage image)
    {
        Color imageShow = image.color;
        if (path == "")
        {
            imageShow.a = 0;
            image.color = imageShow;
        }
        else
        {
            image.color = imageShow;
            Texture2D tex;
            tex = new Texture2D(4, 4, TextureFormat.DXT5, false);
            WWW www = new WWW("file://" + path.Replace("\\", "/"));
            yield return www;
            www.LoadImageIntoTexture(tex);
            image.texture = tex;
            imageShow.a = 1;
            image.color = imageShow;
        }
    }
    IEnumerator enterMenu()
    {
        yield return new WaitForSeconds(0);
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        //SceneManager.LoadScene("GameMenu", LoadSceneMode.Single); // After the summary go back to game menu
        SceneManager.LoadScene("SongSelectionMenu", LoadSceneMode.Single); // After the summary go back to song selection menu
    }

    void displaySummary()
    {
        // when the song is over display the summary: including grades, total score, max combo, total number of perfect, good, normal, miss and ect
        songNameText.text = SongManager.Instance.getCurrentSong().title;
        if (PlayerPrefs.HasKey("Total Score"))
            totalScoreText.text = "Score: " + PlayerPrefs.GetInt("Total Score").ToString().PadLeft(8, '0');
        if (PlayerPrefs.HasKey("Highest Score" + SongManager.Instance.getCurrentSong().title))
            highestScoreText.text = "Highest Score: " + PlayerPrefs.GetInt("Highest Score" + SongManager.Instance.getCurrentSong().title).ToString().PadLeft(8, '0');
        if (PlayerPrefs.HasKey("Max Combo"))
            maxComboText.text = "Max Combo: " + PlayerPrefs.GetInt("Max Combo");
        
        int totalScore = PlayerPrefs.GetInt("Total Score");
        int maximum = GameManager.Instance.maximumScore;
        float percent = (float)totalScore / maximum;
        percentageText.text = (Mathf.Round(percent * 100.0f)).ToString() + "%";
        int rankNum = getRank(percent);
        rankText.text = rankTextList[rankNum];
        rankText.color = rankTextColor[rankNum];


        #region GameStatistics

        perfectHitText.text = "Perfect: " + GameManager.Instance.perfectHit.ToString();
        goodHitText.text = "Good: " + GameManager.Instance.goodHit.ToString();
        normalHitText.text = "Normal: " + GameManager.Instance.normalHit.ToString();
        missText.text = "Miss: " + GameManager.Instance.miss.ToString();

        #endregion

    }

    public void Lose()
    {
        losePanel.SetActive(true);
    }

    public void Pause()
    {
        GameManager.Instance.GamePause();
        pausePanel.SetActive(true);
        InGamePanel.SetActive(false);
    }

    public void Resume()
    {
        GameManager.Instance.GameResume();
        pausePanel.SetActive(false);
        InGamePanel.SetActive(true);
    }

    public void Restart()
    {
        GameManager.Instance.GameRestart();
        pausePanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void End()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SongSelectionMenu", LoadSceneMode.Single);
    }
}
