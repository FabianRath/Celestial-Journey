using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM : MonoBehaviour
{
    public AudioClip[] musicTracks;
    private AudioSource audioSource;
    public static BGM instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void PlayRandomTrack()
    {
        // Choose a random music track from the array
        int randomIndex = Random.Range(0, musicTracks.Length);
        AudioClip randomTrack = musicTracks[randomIndex];

        // Set the audio clip and play the music track
        audioSource.clip = randomTrack;
        audioSource.Play();
    }
}
