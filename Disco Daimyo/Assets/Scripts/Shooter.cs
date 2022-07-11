using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;

    public Bullet projectilePrefab;

    List<Note> notes = new List<Note>();
    public List<double> timeStamps = new List<double>();

    Vector3 projectilePos;
    Quaternion projectileRotation;

    private int spawnIndex = 0;

    //private int inputIndex = 0; //Maybe mouse click to eliminate the bullet or just dodge it 

    // Start is called before the first frame update
    void Start()
    {
        
    }
   
    // Update is called once per frame
    void Update()
    {
        Spawn();
    }

    void Spawn()
    {
        if (spawnIndex < timeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - GameManager.Instance.noteTime)
            {
                Bullet bullet = Instantiate(projectilePrefab);
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.transform.parent = transform;
                bullet.name = "Bullet" + spawnIndex;
                spawnIndex++;
            }
        }
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == noteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, GameManager.thisStage.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }
}
