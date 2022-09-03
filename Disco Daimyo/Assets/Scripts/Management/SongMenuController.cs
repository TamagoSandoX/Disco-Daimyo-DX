using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SongMenuController : MonoBehaviour
{
	public TextMeshProUGUI MetaDataName;
	public TextMeshProUGUI MetaDataArtist;
	public TextMeshProUGUI MetaDataCharter;

	public RawImage BannerBG;
	public RawImage Background;
	public Image difficultySelector;
	public Image black;
	public Animator anim;

	public TextMeshProUGUI highScore;

	public GameObject flipbook;

	public GameObject informationPanel;
	public GameObject interactionPanel;

	private SongManager songManager;
	private ShowDifficulty showDifficulty;
	private bool holdKey;
	private bool isStarted;
	private bool isFliping;

	private int currentInfoPage;
	private int currentInteractPage;


	void Start()
	{
		isStarted = false;
		GameObject songManagerObject = GameObject.FindWithTag("AudioManager");
		if (songManagerObject != null)
			songManager = songManagerObject.GetComponent<SongManager>();
		if (songManager == null)
			Debug.Log("Cannot find 'songManager' script");
		showDifficulty = difficultySelector.GetComponent<ShowDifficulty>();
		holdKey = false;
		loadSong();
		flipbook.GetComponent<AutoFlip>().FlipRightPage();
		currentInfoPage = 1;
		currentInteractPage = 2;
	}

    private void Update()
    {
		if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ConfirmSelection", "Enter")))) StartCoroutine(EnterGame());
		// menu navigational checks
		if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateLeft", "A")))) StartCoroutine(shiftSong(false));
		if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateRight", "D")))) StartCoroutine(shiftSong(true));
		if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateUp", "W")))) StartCoroutine(shiftDifficulty(false));
		if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateDown", "W")))) StartCoroutine(shiftDifficulty(true));
		if (Input.GetKeyDown(KeyCode.Escape)) StartCoroutine(ReturnToGameMenu()); // Buggy!! You can still press the key even the coroutine of other has started.
	}

	IEnumerator shiftSong(bool right)
	{
		holdKey = true;

		if (isFliping == false)
        {
			isFliping = true;

			if (right == true)
			{
				flipbook.GetComponent<AutoFlip>().FlipRightPage();
				informationPanel.transform.SetParent(GameObject.Find("Page" + (currentInfoPage + 2)).transform, false);
				interactionPanel.transform.SetParent(GameObject.Find("Page" + (currentInteractPage + 2)).transform, false);
				currentInfoPage += 2;
				currentInteractPage += 2;
				yield return new WaitForSeconds(1f);
				isFliping = false;
			}
			else
			{
				flipbook.GetComponent<AutoFlip>().FlipLeftPage();
				informationPanel.transform.SetParent(GameObject.Find("Page" + (currentInfoPage - 2)).transform, false);
				interactionPanel.transform.SetParent(GameObject.Find("Page" + (currentInteractPage - 2)).transform, false);
				currentInfoPage -= 2;
				currentInteractPage -= 2;
				yield return new WaitForSeconds(1f);
				isFliping = false;
			}
			songManager.shiftSong(right);
			loadSong();
		}

		yield return new WaitForSeconds(.3f);
		holdKey = false;
		
			
	}

	IEnumerator shiftDifficulty(bool down)
	{
		holdKey = true;
		songManager.shiftDifficulties(down);
		setDifficultyShow();
		yield return new WaitForSeconds(.3f);
		holdKey = false;
	}

	void loadSong()
	{
		try
		{
			if (!songManager.audioSource.isPlaying) songManager.audioSource.Stop();
			MetaData current = songManager.getCurrentSong();
			MetaDataName.text = current.title;
			MetaDataArtist.text = current.artist;
			MetaDataCharter.text = "Mapped by: " + current.charter;

			highScore.text = "Highest Score: " + PlayerPrefs.GetInt("Highest Score" + current.title).ToString();

			showDifficulty.SetDifficulty(current);
			setDifficultyShow();
			if (current.musicPath != "")
				StartCoroutine(songManager.playSampleAudio());
			StartCoroutine(loadCover(current.bannerPath));
			StartCoroutine(loadBackground(current.backgroundPath));
		}
		catch (Exception e)
		{
			MetaDataName.text = "No songs are loaded.";
			Debug.Log(e);
		}
	}

	void setDifficultyShow()
	{
		showDifficulty.SetSelectorPosition(songManager.getCurrentDifficulty());
	}

	IEnumerator loadCover(string path)
	{
		Color imageShow = BannerBG.color;
		if (path == "")
		{
			imageShow.a = 0;
			BannerBG.color = imageShow;
		}
		else
		{
			BannerBG.color = imageShow;
			Texture2D tex;
			tex = new Texture2D(4, 4, TextureFormat.DXT5, false);
			WWW www = new WWW("file://" + path.Replace("\\", "/"));
			yield return www;
			www.LoadImageIntoTexture(tex);
			BannerBG.texture = tex;
			imageShow.a = 1;
			BannerBG.color = imageShow;
		}
	}

	IEnumerator loadBackground(string path)
	{
		Color imageShow = Background.color;
		if (path == "")
		{
			imageShow.a = 0;
			Background.color = imageShow;
		}
		else
		{
			Background.color = imageShow;
			Texture2D tex;
			tex = new Texture2D(4, 4, TextureFormat.DXT5, false);
			WWW www = new WWW("file://" + path.Replace("\\", "/"));
			yield return www;
			www.LoadImageIntoTexture(tex);
			Background.texture = tex;
			imageShow.a = 1;
			Background.color = imageShow;
		}
	}
	IEnumerator EnterGame()
	{
		isStarted = true;
		songManager.audioSource.Stop();
		songManager.audioSource.time = 0;
		yield return new WaitForSeconds(1);
		anim.SetBool("Fade", true);
		yield return new WaitUntil(() => black.color.a == 1);
		SceneManager.LoadScene("GameBase", LoadSceneMode.Single);
	}

	IEnumerator ReturnToGameMenu()
	{
		songManager.audioSource.Stop();
		songManager.audioSource.time = 0;
		yield return new WaitForSeconds(1);
		anim.SetBool("Fade", true);
		yield return new WaitUntil(() => black.color.a == 1);
		SceneManager.LoadScene("GameMenu", LoadSceneMode.Single);
	}
}