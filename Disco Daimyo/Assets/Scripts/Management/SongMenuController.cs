using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using PixelCrushers.DialogueSystem;

public class SongMenuController : MonoBehaviour
{
	public TextMeshProUGUI MetaDataName;
	public TextMeshProUGUI MetaDataArtist;
	public TextMeshProUGUI MetaDataCharter;

	public TextMeshProUGUI CharacterName;
	public TextMeshProUGUI ClubName;
	public TextMeshProUGUI CharacterQuote;

	public RawImage BannerBG;
	public RawImage Background;
	public RawImage Portrait;

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

	private int currentPanelPage;
	private int currentBGPage;

	private int flipCount;

	private float characterID;

	private bool shiftSongRightArrow;
	private bool shiftSongLeftArrow;

	private bool canInput;

	public GameObject NPC;

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
		flipCount++; // autoFlip at start

		// They are Page 2 & Page 1 after the first flip
		currentPanelPage = 2;
		currentBGPage = 1;
		characterID = 1;
		DialogueLua.SetVariable("CharacterID", characterID);
	}

    private void Update()
    {
		if ((Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ConfirmSelection", "Enter")))) && canInput) StartCoroutine(EnterGame());
		// menu navigational checks 
		if ((Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateLeft", "A")))) && canInput) StartCoroutine(shiftSong(false));
		if ((Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateRight", "D"))))  && canInput) StartCoroutine(shiftSong(true));
		if ((Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateUp", "W")))) && canInput) StartCoroutine(shiftDifficulty(false));
		if ((Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateDown", "W")))) && canInput) StartCoroutine(shiftDifficulty(true));
		if ((Input.GetKeyDown(KeyCode.Escape)) && canInput) StartCoroutine(ReturnToGameMenu()); // Buggy!! You can still press the key even the coroutine of other has started.
	}

	public void DisableInput()
    {
		canInput = false;
	}

	public void EnableInput()
	{
		canInput = true;
	}

	bool CheckCurrentPageReachedEnd() // Reach the end of the song list and hence prevent flip book fuction
    {
		if (flipCount == songManager.metaList.Count)
        {
			return true;
        }
		return false;
    }

	bool CheckCurrentPageReachedFront()
    {
		if (flipCount == 1)
		{
			return true;
		}
		return false;
	}

	IEnumerator shiftSong(bool right)
	{
		holdKey = true;
		

		if (isFliping == false)
        {
			
			if (right == true)
			{
				if (!CheckCurrentPageReachedEnd())
                {
					shiftSongRightArrow = false;
					isFliping = true;
					flipbook.GetComponent<AutoFlip>().FlipRightPage();
					currentPanelPage += 2;
					currentBGPage += 2;
					informationPanel.transform.SetParent(GameObject.Find("Page" + (currentPanelPage)).transform, false);
					interactionPanel.transform.SetParent(GameObject.Find("Page" + (currentPanelPage)).transform, false);
					Background.transform.SetParent(GameObject.Find("Page" + (currentBGPage)).transform, false);
					flipCount++;
					songManager.shiftSong(right);
					loadSong();
					yield return new WaitForSeconds(0.5f); // Wait for 0.5s flipping animation
					isFliping = false;
					DialogueLua.SetVariable("CharacterID", characterID);
					
				}
				
			}
			else
			{
				if (!CheckCurrentPageReachedFront())
                {
					shiftSongLeftArrow = false;
					isFliping = true;
					flipbook.GetComponent<AutoFlip>().FlipLeftPage();
					currentPanelPage -= 2;
					currentBGPage -= 2;
					informationPanel.transform.SetParent(GameObject.Find("Page" + (currentPanelPage)).transform, false);
					interactionPanel.transform.SetParent(GameObject.Find("Page" + (currentPanelPage)).transform, false);
					Background.transform.SetParent(GameObject.Find("Page" + (currentBGPage)).transform, false);
					flipCount--;
					songManager.shiftSong(right);
					loadSong();
					yield return new WaitForSeconds(0.5f); // Wait for 0.5s flipping animation
					isFliping = false;
					DialogueLua.SetVariable("CharacterID", characterID);
				}
				
			}
			

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

	public void shiftDifficultyLeftArrow()
	{
		shiftSongLeftArrow = true;
	}

	public void shiftDifficultyRightArrow()
	{
		shiftSongRightArrow = true;
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
			CharacterName.text = current.characterName;
			ClubName.text = current.clubName;
			CharacterQuote.text = current.characterQuote;

			highScore.text = "Highest Score: " + PlayerPrefs.GetInt("Highest Score" + current.title).ToString();

			characterID = current.characterID;

			showDifficulty.SetDifficulty(current);
			setDifficultyShow();
			if (current.musicPath != "")
				StartCoroutine(songManager.playSampleAudio());
			StartCoroutine(loadCover(current.bannerPath));
			StartCoroutine(loadBackground(current.backgroundPath));
			StartCoroutine(loadPortrait(current.portraitPath));
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

	IEnumerator loadPortrait(string path)
	{
		Color imageShow = Portrait.color;
		if (path == "")
		{
			imageShow.a = 0;
			Portrait.color = imageShow;
		}
		else
		{
			Portrait.color = imageShow;
			Texture2D tex;
			tex = new Texture2D(4, 4, TextureFormat.DXT5, false);
			WWW www = new WWW("file://" + path.Replace("\\", "/"));
			yield return www;
			www.LoadImageIntoTexture(tex);
			Portrait.texture = tex;
			imageShow.a = 1;
			Portrait.color = imageShow;
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
			imageShow.a = 1f;
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