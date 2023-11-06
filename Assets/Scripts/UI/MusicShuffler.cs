using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicShuffler : MonoBehaviour
{
    public AudioClip[] Tracks;

    public List<int> songQueue;

    public float volume = 0.7F;
    private int randomNumber;

    public AudioSource mainAudioSource;

    public SongDisplay songDisplay;
    // Start is called before the first frame update
    void Start()
    {
        mainAudioSource = GetComponent<AudioSource>();
        randomNumber = Random.Range(0, 13);
        PopulateSongQueue();
        SelectNextSong();
    }

    public void SelectNextSong()
    {
        mainAudioSource.PlayOneShot(Tracks[songQueue[0]], volume);
        SetSongDisplay(Tracks[songQueue[0]]);
        songQueue.RemoveAt(0);
    }
    // Update is called once per frame
    void Update()
    {
        if (!mainAudioSource.isPlaying)
        {
            if (songQueue.Count == 0) {
                PopulateSongQueue();
            }
            SelectNextSong();
        }
    }

    void PopulateSongQueue() {
        // our list is empty
        // so we need to fill it up
        // roll random number between 1 & 13
        // songnumber = Random.Range(0, 12);
        // songQueue.Add(Tracks[songnumber]);

        for (int i = 0; i < Tracks.Length; i++) 
        {
            randomNumber = Random.Range(0, 14);
            while (songQueue.Contains(randomNumber))
            {
                randomNumber = Random.Range(0, 14);
            }

            songQueue.Add(randomNumber);
        }

        // assign each rolled number a number in the list format
    }

    void SetSongDisplay(AudioClip track) {
        var split = track.name.Split(" - ");
        songDisplay.SetDisplay(split[1], split[0]);
    }
    
}