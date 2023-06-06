using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGM : MonoBehaviour{
    public static BGM instance;
    private AudioSource audioSource;

    private void Awake(){
        if (instance != null){
            Destroy(gameObject);
        }else{
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    public void PauseMusic(){
        if (audioSource.isPlaying){
            audioSource.Pause();
        }else{
            audioSource.UnPause();
        }
    }
}
