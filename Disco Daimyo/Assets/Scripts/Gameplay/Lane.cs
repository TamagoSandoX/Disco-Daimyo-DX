using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;
    public Note notePrefab;
    public Note overlapNotePrefab;
    public Note holdNotePrefab;
    public GameObject glowStick;
    public Light glowLight;
    public List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();
    public List<double> noteLength = new List<double>();
    public int spawnIndex = 0;
    public int inputIndex = 0;
    public int noteLengthIndex = 0;
    public PlayerActionSwipeLeft playerActionSwipeLeft;
    public PlayerActionSwipeRight playerActionSwipeRight;
    private bool isPlaying = true;
    private bool isHoldNote = false;
    private bool canHold = false;
    public bool holdSpawned = false;
    public float regularNoteLength; // get this data from .sm file
    private float timeStampGap;

    public KeyCode input;
    //expandable list of lanes ((for if we ever add more than 3))
    public bool isLeftLane, isMiddleLane, isRightLane;

    void Start()
    {
        spawnIndex = 0;
        inputIndex = 0;
        noteLengthIndex = 0;
        regularNoteLength = SongManager.Instance.getCurrentNoteLength();
        timeStampGap = SongManager.Instance.getCurrentTimeStampGap();
        glowLight = glowStick.GetComponent<Light>();

        SetLaneHotkey();
    }

    void SetLaneHotkey(){
        if (isLeftLane){
            input = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftLane", "Q"));
        }
        else if (isMiddleLane){
            input = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MiddleLane", "W"));
        }
        else {
            input = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightLane", "E"));
        }
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
        double lastNoteTime = 0;

        foreach (var note in array)
        {

            if (note.NoteName == noteRestriction)
            {

                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, GameManager.thisStage.GetTempoMap());
                double convertedTimeStamp = (double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f;
                timeStamps.Add(convertedTimeStamp);


                timeStamps = timeStamps.Distinct().ToList();   // temporarily fixed the bug for duplicates

                if (convertedTimeStamp != lastNoteTime)
                {
                    noteLength.Add(note.Length);
                }

                lastNoteTime = convertedTimeStamp;

            }

        }


    }

    private void Spawn()
    {
        if (spawnIndex < timeStamps.Count) // song hasnt ended yet
        {
            if (SongManager.GetAudioSourceTime() > (timeStamps[spawnIndex] - GameManager.Instance.noteTime)) // if current time reached the timestamp to spawn the note
            {
                SpawnManager();
            }
        }
        else
        {

            if (isPlaying)
            {
                // This lane has ended
                GameManager.Instance.NumOfEndedLane++;
                //Debug.Log("This lane is over");
                isPlaying = false;
            }


        }
    }

    public double HoldNoteEndTime()
    {
        if (spawnIndex < timeStamps.Count)
        {
            return timeStamps[spawnIndex] + ((noteLength[spawnIndex] / regularNoteLength) * timeStampGap);
        }
        return 0;
    }
    private void RegularSpawn()
    {
        var note = Instantiate(notePrefab, transform);
        note.name = "Regular Note";
        notes.Add(note.GetComponent<Note>());
        note.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
        note.GetComponent<Note>().assignedEndedTime = (float)timeStamps[spawnIndex];
        spawnIndex++;
    }

    private void OverlapSpawn()
    {
        var overlapNote = Instantiate(overlapNotePrefab, transform);
        overlapNote.name = "Overlap Tap Note";
        notes.Add(overlapNote.GetComponent<Note>());
        overlapNote.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
        overlapNote.GetComponent<Note>().assignedEndedTime = (float)timeStamps[spawnIndex];
        spawnIndex++;
    }

    private float MathSucks(float multiplier) // current state: in x1 speed it is perfectly right, along the increaing speed the gap seem reducing  
    {
        return (multiplier) * (1 / GameManager.Instance.noteTime) * 3 + ((timeStampGap * multiplier));
    }
    private void HoldNoteSpawn()
    {
        holdSpawned = true;
        float multiplier = (float)(noteLength[spawnIndex] / regularNoteLength);
        var holdNote = Instantiate(holdNotePrefab, transform);
        holdNote.name = "Hold Note";

        holdNote.transform.Find("Pivot").transform.localScale = new Vector3(1, MathSucks(multiplier), 1);
        
        if (holdNote.transform.Find("Pivot").transform.localScale.y <= 1) 
        {
            holdNote.transform.Find("Pivot").transform.localScale = new Vector3(1, 1, 1);
        }
        
        notes.Add(holdNote.GetComponent<Note>());
        holdNote.GetComponent<Note>().assignedTime = (float)timeStamps[spawnIndex];
        holdNote.GetComponent<Note>().assignedEndedTime = (float)HoldNoteEndTime();
        holdNote.GetComponent<Note>().multiplier = multiplier;
        spawnIndex++;

    }

    private void SpawnManager()
    {
        if (SwipeStillPlaying_Left(playerActionSwipeLeft) && SwipeStillPlaying_Right(playerActionSwipeRight))
        {
            if (this.name == "Lane 2" && (OnSameBeat_Left() && !IsHold() || OnSameBeat_Right() && !IsHold()))
            {
                OverlapSpawn();

            }
            else if (IsHold())
            {
                HoldNoteSpawn();
            }
            else
            {
                RegularSpawn();
            }

        }
        else if (IsHold())
        {
            HoldNoteSpawn();
        }
        else
        {
            RegularSpawn();
        }


    }

    private bool IsHold()
    {
        return noteLength[spawnIndex] > regularNoteLength;
    }
    private bool OnSameBeat_Left()
    {
        return timeStamps[spawnIndex] == playerActionSwipeLeft.timeStamps[playerActionSwipeLeft.spawnIndex];
    }

    private bool OnSameBeat_Right()
    {
        return timeStamps[spawnIndex] == playerActionSwipeRight.timeStamps[playerActionSwipeRight.spawnIndex];
    }

    private bool SwipeStillPlaying_Left(PlayerActionSwipeLeft playerActionSwipeLeft)
    {
        return playerActionSwipeLeft.spawnIndex < playerActionSwipeLeft.timeStamps.Count;
    }

    private bool SwipeStillPlaying_Right(PlayerActionSwipeRight playerActionSwipeRight)
    {
        return playerActionSwipeRight.spawnIndex < playerActionSwipeRight.timeStamps.Count;
    }

    private void Hit(string typeOfHit)
    {
        if (typeOfHit == "Normal")
        {
            GameManager.Instance.Hit();
        }
        if (typeOfHit == "Good")
        {
            GameManager.Instance.GoodHit();
        }
        if (typeOfHit == "Perfect")
        {
            GameManager.Instance.PerfectHit();
        }
        if (typeOfHit == "PerfectHold")
        {
            GameManager.Instance.PerfectHoldHit();
        }

    }
    private void Miss()
    {
        GameManager.Instance.Miss();
    }

    private void InputManager()
    {
        if (inputIndex < timeStamps.Count)
        {
            // if note length is greater than 64 (1/8 note) = holds
            if (noteLength[noteLengthIndex] > regularNoteLength)
            {
                isHoldNote = true;
            }
            else
            {
                isHoldNote = false;
            }

            double timeStamp = timeStamps[inputIndex];
            double marginOfError = GameManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (GameManager.Instance.inputDelayInMilliseconds / 1000.0);

            double tailCalculation = timeStamps[inputIndex] + ((noteLength[inputIndex] / regularNoteLength) * timeStampGap);

            checkRegularNote(audioTime, timeStamp, marginOfError);
            checkHoldNote(audioTime, timeStamp, marginOfError, tailCalculation);

        }
    }

    private void checkHoldNote(double audioTime, double timeStamp, double marginOfError, double tailCalculation)
    {
        
        if (Input.GetKeyDown(input) && isHoldNote)
        {
            if (Math.Abs(audioTime - timeStamp) < marginOfError - GameManager.Instance.perfectOffset) // Perfect Hit
            {

                Hit("Perfect");
                Explosion();
                canHold = true;
                //print($"Perfect Hit on {inputIndex} note at " + this.name);


            }
            else if (Math.Abs(audioTime - timeStamp) < marginOfError - GameManager.Instance.goodOffset) // Good Hit
            {

                Hit("Good");
                Explosion();
                canHold = true;
                //print($"Good Hit on {inputIndex} note at " + this.name);

            }
            else if (Math.Abs(audioTime - timeStamp) < marginOfError) // Normal Hit
            {
                Hit("Normal");
                Explosion();
                canHold = true;
                //print($"Normal Hit on {inputIndex} note at " + this.name);
            }
            else
            {


            }
        }

        if (timeStamp + marginOfError <= audioTime && !canHold && isHoldNote) // if the note is passed 
        {
            Miss();
            inputIndex++;
            noteLengthIndex++;
            //print($"Missed {inputIndex} note at " + this.name);
        }

        if (canHold)
        {
            Holding(audioTime, tailCalculation);
        }

    }

    private void Holding(double audioTime, double tailCalculation)
    {
        if (Input.GetKey(input))
        {
            if (audioTime >= tailCalculation - GameManager.Instance.marginOfError)
            {
                inputIndex++;
                noteLengthIndex++;
                Hit("PerfectHold");
                canHold = false;
                holdSpawned = false;
            }
            
        }
        else if (Input.GetKeyUp(input) && audioTime < tailCalculation - GameManager.Instance.marginOfError)
        {
            inputIndex++;
            noteLengthIndex++;
            Miss();
            holdSpawned = false;
            canHold = false; // cannot hold again
        }
        else
        {
            
        }
    }

    private void checkRegularNote(double audioTime, double timeStamp, double marginOfError)
    {
        if (Input.GetKeyDown(input) && !isHoldNote)
        {
            if (Math.Abs(audioTime - timeStamp) < marginOfError - GameManager.Instance.perfectOffset) // Perfect Hit
            {

                Hit("Perfect");
                Explosion();
                Destroy(notes[inputIndex].gameObject);
                noteLengthIndex++;
                inputIndex++;
                //print($"Perfect Hit on {inputIndex} note at " + this.name);
            }


            else if (Math.Abs(audioTime - timeStamp) < marginOfError - GameManager.Instance.goodOffset) // Good Hit
            {

                Hit("Good");
                Explosion();
                Destroy(notes[inputIndex].gameObject);
                noteLengthIndex++;
                inputIndex++;
                //print($"Perfect Hit on {inputIndex} note at " + this.name);

            }
            else if (Math.Abs(audioTime - timeStamp) < marginOfError) // Normal Hit
            {

                Hit("Normal");
                Explosion();
                Destroy(notes[inputIndex].gameObject);
                noteLengthIndex++;
                inputIndex++;
                //print($"Perfect Hit on {inputIndex} note at " + this.name);

            }
            else
            {

            }
        }
        if (timeStamp + marginOfError <= audioTime && !canHold && !isHoldNote) // if the note is passed
        {
            Miss();
            inputIndex++;
            noteLengthIndex++;
            //print($"Missed {inputIndex} note at " + this.name);
        }

    }

    private void Explosion()
    {
        glowLight.intensity = Mathf.PingPong(Time.time, 300);
    }

}
