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

    public GameObject titleImage;
    public GameObject controlPanel;

    // Start is called before the first frame update
    void Start()
    {
        loadingController = GameObject.Find("Main Camera").GetComponent<LoadingController>();
        background = gameObject.GetComponent<RawImage>();
        
        alpha = background.color.a;
        if (settingsPanel.active == false)
        {
            FadeIn();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SongSelectionMenu", LoadSceneMode.Single);
    }

    public void Settings()
    {
        gameMenuPanel.GetComponent<RawImage>().CrossFadeAlpha(0.5f, 1f, true); // fade out effect
        titleImage.SetActive(false); // hide the title
        controlPanel.SetActive(false); // hide the control panel
        settingsPanel.SetActive(true);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    void FadeIn()
    {
        background.CrossFadeAlpha(1f, 1f, false); // fade in effect
    }
}
