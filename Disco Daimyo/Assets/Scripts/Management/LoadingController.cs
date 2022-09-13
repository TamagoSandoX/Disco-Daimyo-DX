using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

	public Image black;
	public Animator anim;

	public GameObject gameMenuPanel;

	public GameObject loadingText;

	private SongManager songManager;

	private bool isDisplayed = false;

	void Start () {
		GameObject songManagerObject = GameObject.FindWithTag("AudioManager");
		if(songManagerObject != null)
			songManager = songManagerObject.GetComponent<SongManager>();
		if(songManager == null)
			Debug.Log("Cannot find 'SongManager' script");
	}

	void Update () {
		if(songManager._loadComplete && !isDisplayed)
			StartCoroutine (loadingEnd ());
	}

	IEnumerator loadingEnd(){ 
		yield return new WaitForSeconds (1);
		anim.SetBool("Fade", true);
		yield return new WaitUntil(() => black.color.a == 1);
		gameMenuPanel.SetActive(true);
		isDisplayed = true;

		// Hide fade image and the blinking loading text
		loadingText.SetActive(false);
		black.gameObject.SetActive(false);
	}
}
