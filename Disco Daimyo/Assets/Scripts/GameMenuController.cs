using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    // (Introduction text/video/animation)

    // Title

    // (Player Statistics)
    // Start button
    // Settings button
    // (Recent Update Corner)
    // End Game button

    public GameObject gameMenuPanel;
    public GameObject settingsPanel;

    private RawImage background;
    private float alpha;

    private LoadingController loadingController;

    // Start is called before the first frame update
    void Start()
    {
        loadingController = GameObject.Find("Main Camera").GetComponent<LoadingController>();
        background = gameObject.GetComponent<RawImage>();
        background.color = new Color (255,255,255,0); // Transparent at start
        alpha = background.color.a;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FadeIn();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SongSelectionMenu", LoadSceneMode.Single);
    }

    public void Settings()
    {
        settingsPanel.SetActive(true);
        gameMenuPanel.SetActive(false);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    void FadeIn()
    {
        if (alpha < 255f)
        {
            alpha += 0.005f;
            background.color = new Color(255, 255, 255, alpha);
        }
        else
        {
            alpha = 255f;
        }

    }
}
