using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System;

public class SongManager : MonoBehaviour
{
    // Singleton
    static private SongManager instance;
    static public SongManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("There is no SongManager instance in the scene.");
            }
            return instance;
        }
    }

    public bool _loadComplete;
    public AudioSource audioSource;
    public bool _isStarted;
    public float songDelayInSeconds;
    private string fileLocation;
    public List<MetaData> metaList;
    private int songIndex;
    private List<int> difficulties;
    private int difficultyIndex;
    private List<startBPM> startBPMs;
    private int nextBPMIndex;

    void Awake()
    {
        if (instance != null)
        {
            // destroy duplicates
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        string path = "Songs";

        _loadComplete = false;
        metaList = new List<MetaData>();
        DirectoryInfo baseDir = new DirectoryInfo(path);
        DirectoryInfo[] dirArray = baseDir.GetDirectories();

        FileAttributes attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
        {
            // Show the file.
            attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
            File.SetAttributes(path, attributes);
        }
        else
        {
            // Hide the file.
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
        }

        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            foreach (DirectoryInfo subDir in dirArray)
            {
                FileInfo[] smFiles = subDir.GetFiles("*.sm");
                if (smFiles.Length == 1)
                {
                    MetaData newSong = new MetaData(subDir, smFiles[0].FullName);
                    metaList.Add(newSong);
                    
                }
            }
            //ReadFromFile();
        }
        _loadComplete = true;
        _isStarted = false;
        songIndex = 0;
        difficulties = new List<int>();
        startBPMs = metaList[songIndex].bpms;
        nextBPMIndex = (startBPMs.Count > 1) ? 1 : 0;
        setDifficulties(metaList[songIndex]);
        
    }
    private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
    {
        return attributes & ~attributesToRemove;
    }

    void setDifficulties(MetaData map)
    {
        difficulties.Clear();
        if (map.beginnerExists) difficulties.Add(0);
        if (map.easyExists) difficulties.Add(1);
        if (map.mediumExists) difficulties.Add(2);
        if (map.hardExists) difficulties.Add(3);
        if (map.challengeExists) difficulties.Add(4);
        difficultyIndex = 0;
    }
    public List<MetaData> getSongList()
    {
        return metaList;
    }

    public List<int> getDifficultyList()
    {
        return difficulties;
    }

    public MetaData getCurrentSong()
    {
        return metaList[songIndex];
    }

    public int getCurrentDifficulty()
    {
        return difficulties[difficultyIndex];
    }

    public NoteData getCurrentStage()
    {
        switch (difficulties[difficultyIndex])
        {
            case 0:
                return metaList[songIndex].beginner;
            case 1:
                return metaList[songIndex].easy;
            case 2:
                return metaList[songIndex].medium;
            case 3:
                return metaList[songIndex].hard;
            case 4:
                return metaList[songIndex].challenge;
            default:
                return new NoteData();
        }
    }

    public MidiFile GetCurrentStage()
    {
        switch (difficulties[difficultyIndex])
        {
            case 0:
                return metaList[songIndex].beginner_alt;
            case 1:
                return metaList[songIndex].easy_alt;
            case 2:
                return metaList[songIndex].medium_alt;
            case 3:
                return metaList[songIndex].hard_alt;
            case 4:
                return metaList[songIndex].challenge_alt;
            default:
                
                return new MidiFile();
        }
    }

    public float getCurrentBPM(float gamePos)
    {
        if (startBPMs.Count == 1) return startBPMs[0].mapBPM;
        if (startBPMs[nextBPMIndex].mapPos <= gamePos)
            nextBPMIndex += 1;
        return startBPMs[nextBPMIndex - 1].mapBPM;
    }

    public float getCurrentTimeStampGap()
    {
        return metaList[songIndex].timestampGap;
    }
    public float getCurrentOffset()
    {
        return metaList[songIndex].offset;
    }

    public float getCurrentVolume()
    {
        return metaList[songIndex].volume;
    }

    public float getCurrentNoteLength()
    {
        return metaList[songIndex].regularNoteLength;
    }

    public void shiftSong(bool right)
    {
        int length = metaList.Count;
        songIndex += right ? 1 : -1;
        if (songIndex == -1) songIndex = length - 1;
        else if (songIndex == length) songIndex = 0;
        startBPMs = metaList[songIndex].bpms;
        setDifficulties(metaList[songIndex]);
    }

    public void shiftDifficulties(bool down)
    {
        int length = difficulties.Count;
        difficultyIndex += down ? 1 : -1;
        if (difficultyIndex == -1) difficultyIndex = length - 1;
        else if (difficultyIndex == length) difficultyIndex = 0;
    }

    void Update()
    {
        
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) // fixed the warnings
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    //midiFile = MidiFile.Read(stream);
                    //GetDataFromMidi(midiFile);
                }
            }
        }
    }

    public void ReadFromFile()
    {
        //midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
        //GetDataFromMidi(midiFile);
    }

    
    public IEnumerator StartSong()
    {
        _isStarted = true;
        yield return new WaitForSeconds(songDelayInSeconds);
        audioSource.Play();
        
    }
    public static double GetAudioSourceTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
        
    }

    public IEnumerator playSampleAudio()
    {
        string path = metaList[songIndex].musicPath;
        float timeStart = metaList[songIndex].sampleStart;
        float timeDuration = metaList[songIndex].sampleLength;
        int playingIndex = songIndex;

        
        WWW www = new WWW("file://" + path.Replace("\\", "/"));
        yield return www;
        audioSource.clip = (path.LastIndexOf(".mp3") != -1) ? NAudioPlayer.FromMp3Data(www.bytes) : www.GetAudioClip(false);
        

        //audioSource.clip = songList[songIndex]; // Maybe buggy

        while (playingIndex == songIndex)
        {
            audioSource.volume = getCurrentVolume();
            audioSource.time = timeStart;
            audioSource.Play();
            while (audioSource.time < (timeStart + timeDuration - 1))
            {
                yield return new WaitForSeconds(0.01f);
                if (playingIndex != songIndex) break;
            }
            float t = audioSource.volume;
            while (t > 0.0f)
            {
                if (playingIndex != songIndex) break; // Moved to next song -> cut current song
                t -= 0.01f;
                audioSource.volume = t;
                yield return new WaitForSeconds(0.01f);
            }
            if (playingIndex != songIndex) break;
            audioSource.Stop();
        }
    }

    public IEnumerator PauseSong()
    {
        _isStarted = true;
        yield return new WaitForSeconds(0);
        audioSource.Pause();
    }

    public IEnumerator ResumeSong()
    {
        _isStarted = true;
        yield return new WaitForSeconds(0);
        audioSource.UnPause();
    }

    public IEnumerator RestartSong()
    {
        _isStarted = true;
        audioSource.Stop();
        yield return new WaitForSeconds(songDelayInSeconds);

        audioSource.time = 0;
        audioSource.Play();

    }

    public IEnumerator StopSong()
    {
        _isStarted = false;
        audioSource.Stop();
        yield return new WaitForSeconds(songDelayInSeconds);
        audioSource.time = 0;
    }
}
