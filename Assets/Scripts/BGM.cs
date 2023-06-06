using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM : MonoBehaviour{
    public static BGM instance;
    private AudioSource audioSource;

    private void Awake(){
        audioSource = GetComponent<AudioSource>();
        if (instance != null){
            Destroy(gameObject);
        }else{
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void startPauseMusic(){
        if (audioSource.isPlaying){
            audioSource.Pause();
            PlayerPrefs.SetInt("BGMMuted", 1);
        }else{
            audioSource.UnPause();
            PlayerPrefs.SetInt("BGMMuted", 0);
        }
    }
}
