using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Note : MonoBehaviour
{

    double timeInstantiated;
    double timeReached;

    public float assignedTime; // the time when it spawns
    public float assignedEndedTime;

    public float multiplier;

    public GameObject pivot;

    private float deltaT;

    private bool startTracking;

    private Lane lane;

    private float newSpawnY;
    private float newDespawnY;

    private float currentSpawnY;
    private float currentDespawnY;

    private float gapBetweenSpawnAndDespawn;

    private bool canExtend;

    void Start()
    {
        lane = gameObject.GetComponentInParent<Lane>();
        timeInstantiated = SongManager.GetAudioSourceTime();
        if (this.name == "Hold Note")
        {
            pivot = transform.GetChild(0).gameObject;
        }
        else
        {
            pivot = transform.gameObject;
        }

        currentSpawnY = GameManager.Instance.noteSpawnY;
        currentDespawnY = GameManager.Instance.noteDespawnY;
        gapBetweenSpawnAndDespawn = 60f;
        startTracking = false; // this is a flag for tracking reached bottom times
        canExtend = false;
    }

    // Update is called once per frame
    void Update()
    {
        NoteMovement();
        
    }

    void NoteMovement()
    {

        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated; // this has impact on the starting few notes
        
        
        deltaT = (float)(timeSinceInstantiated / (GameManager.Instance.noteTime * 2)); // divided by 2 means when this value has reached 0.5 it will be right on the judgement line

        if (transform.localPosition == Vector3.up * GameManager.Instance.noteDespawnY)
        {
            canExtend = true;
        }

        if (gameObject.name == "Hold Note")
        { 

            if (canExtend)
            {
                if (transform.localPosition == Vector3.up * currentDespawnY)
                {
                    canExtend = true;
                    startTracking = true;
                }

                ExtendHoldsReachBottom();

                if (SongManager.GetAudioSourceTime() > assignedEndedTime) 
                {
                    
                    Destroy(gameObject);
                    lane.holdSpawned = false;
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(Vector3.up * GameManager.Instance.noteSpawnY, Vector3.up * GameManager.Instance.noteDespawnY, deltaT);
                DisplayRenderer();
            }
            
        }
        if (deltaT > 1 && gameObject.name != "Hold Note")
        {
            Destroy(gameObject);
        }
        else
        {
            if (gameObject.name != "Hold Note")
            {
                transform.localPosition = Vector3.Lerp(Vector3.up * GameManager.Instance.noteSpawnY, Vector3.up * GameManager.Instance.noteDespawnY, deltaT);
                DisplayRenderer();
            }
            
        }

    }


    void ExtendHoldsReachBottom()
    {
        
        if (startTracking)
        {
            timeReached = SongManager.GetAudioSourceTime();
            newSpawnY = currentSpawnY - gapBetweenSpawnAndDespawn;
            newDespawnY = currentDespawnY - gapBetweenSpawnAndDespawn;
            startTracking = false;
        }
        double timeSinceReached = SongManager.GetAudioSourceTime() - timeReached;
        float newDeltaT = (float)(timeSinceReached / (GameManager.Instance.noteTime * 2));
        transform.localPosition = Vector3.Lerp(Vector3.up * newSpawnY, Vector3.up * newDespawnY, newDeltaT);
        currentSpawnY = newSpawnY;
        currentDespawnY = newDespawnY;
    }

    void DisplayRenderer()
    {
        if (this.name == "Regular Note" || this.name == "Overlap Tap Note" || this.name == "Hold Note") // Regular Note, Overlap tap note, hold note
        {
            GetComponentInChildren<MeshRenderer>().enabled = true; // Make sure the note will be shown only when it starts moving
        }
        if (this.name == "Swipe Note" || this.name == "OverLap Swipe Note")// Swipe Note && overlap swipe note
        {
            GetComponentInChildren<SpriteRenderer>().enabled = true; // Make sure the note will be shown only when it starts moving
        }
    }
}
