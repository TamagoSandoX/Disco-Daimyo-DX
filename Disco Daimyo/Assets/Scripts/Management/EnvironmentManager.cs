using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject[] scenes;
    
    private GameObject activeScene;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject scene in scenes) 
        {
            scene.SetActive(false);
            setScene(scene);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setScene(GameObject newscene)
    {
        if (SongManager.Instance.getCurrentSong().environment == newscene.name) // if the name of scene matches environment in .sm file, it shows up
        {
            activeScene = newscene;
            activeScene.SetActive(true);
        }
    }
}
