using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionSwipeLeft : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction_Left;
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction_Left_Alt;
    
    public List<Note> notes = new List<Note>();

    public Note SwipeLeftNotePrefabs;
    public Note overlapNotePrefab; // only the swipe sign

    public Lane lane_mid;
    public Lane lane_left;
    public Lane lane_right;

    public List<double> timeStamps = new List<double>();

    public GameObject judgementLine;
    public GameObject judgementLine_Left;

    public int spawnIndex = 0;

    public int inputIndex = 0;

    private float startPos;
    private bool startCounting;


    private bool isPlaying;

    // Start is called before the first frame update
    void Start()
    {
        isPlaying = true;
        spawnIndex = 0;
        inputIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isStarted)
        {
            Spawn();

            InputManager();
        }
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array) // Convert midi file tempo map to timestamp
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction_Left || note.NoteName == noteRestriction_Left_Alt)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, GameManager.thisStage.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    private void Spawn()
    {
        if (spawnIndex < timeStamps.Count) // song hasnt ended yet
        {
            if (SongManager.GetAudioSourceTime() >= (timeStamps[spawnIndex] - GameManager.Instance.noteTime)) // maybe lead to the bug at the beginning
            {
                SpawnManager();
            }
        }
        else
        {

            if (isPlaying)
            {
                // This lane is ended
                GameManager.Instance.NumOfEndedLane++;
                //Debug.Log("This lane is over");
                isPlaying = false;
            }


        }

    }

    private void RegularSpawn()
    {
        var note = Instantiate(SwipeLeftNotePrefabs, judgementLine.transform);
        note.name = "Swipe Note";
        notes.Add(note.GetComponent<Note>());
        note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
        spawnIndex++;
        //Debug.Log("Spawned");
    }

    private void OverlapSpawn()
    {
        var overlapNote = Instantiate(overlapNotePrefab, judgementLine_Left.transform);
        overlapNote.name = "OverLap Swipe Note";
        notes.Add(overlapNote.GetComponent<Note>());
        overlapNote.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
        spawnIndex++;
    }

    private void SpawnManager()
    {
        if (LaneStillPlaying(lane_mid) || LaneStillPlaying(lane_left) || LaneStillPlaying(lane_right))
        {
            if (OnSameBeat(lane_mid) && IsHold() || StillWithinHold(lane_mid))
            {
                OverlapSpawn();
            }
            else
            {
                RegularSpawn();
            }
        }
        else
        {
            RegularSpawn();
        }

    }

    private bool StillWithinHold(Lane lane)
    {
        if (lane.holdSpawned)
        {
            return lane.HoldNoteEndTime() - timeStamps[spawnIndex] > 0;
        }
        return false;
    }

    private bool LaneStillPlaying(Lane lane)
    {
        return lane.spawnIndex < lane.timeStamps.Count;
    }
    private bool OnSameBeat(Lane lane)
    {
        if (spawnIndex < timeStamps.Count && LaneStillPlaying(lane))
        {
            return timeStamps[spawnIndex] == lane.timeStamps[lane.spawnIndex];
        }
        return false;
    }

    private bool IsHold()
    {
        return lane_mid.noteLength[lane_mid.spawnIndex] > lane_mid.regularNoteLength;
    }

    private void Hit()
    {
        GameManager.Instance.PerfectHit(); // swipe action only have perfect
    }

    private void Miss()
    {
        GameManager.Instance.Miss();
    }

    private void InputManager()
    {
        if (inputIndex < timeStamps.Count)
        {
            double timeStamp = timeStamps[inputIndex];
            double marginOfError = GameManager.Instance.mouseFlickMOE;
            double audioTime = SongManager.GetAudioSourceTime() - (GameManager.Instance.inputDelayInMilliseconds / 1000.0);

            float currentPosition = transform.position.x;

            if (Math.Abs(audioTime - timeStamp) < marginOfError)
            {
                if (startCounting)
                {
                    startPos = currentPosition;
                    startCounting = false;
                }

                
                if ((currentPosition - startPos) < -(GameManager.Instance.mouseSensitivity)) // Swipe Left (check if the deltaPostion is smaller than certain number (0.5f)
                {                                                                            // and whether the coming note is the left note: SwipeLeftNotePrefabs
                    Hit();
                    Destroy(notes[inputIndex].gameObject);
                    inputIndex++;
                    startCounting = true;
                    //print($"Hit on {inputIndex} note at " + this.name);
                }

                //Debug.Log((currentPosition - startPos));

            }

            if (timeStamp + marginOfError <= audioTime)
            {
                Miss();
                inputIndex++;
                startCounting = true;
                //print($"Missed {inputIndex} note at " + this.name);
            }
        }
    }
}
