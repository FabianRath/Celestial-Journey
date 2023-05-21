
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour{
    public Canvas canvas;
    public Button exit, options, resume;

    void Start(){
        exit.onClick.AddListener(() => TaskOnClick(exit));
        options.onClick.AddListener(() => TaskOnClick(options));
        resume.onClick.AddListener(() => TaskOnClick(resume));
        canvas.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    
    void FixedUpdate(){
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (canvas.gameObject.activeSelf == false){
                canvas.gameObject.SetActive(true);
                Time.timeScale = 0;
            } else {
                canvas.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
        Debug.Log("wad");
    }
    
    void TaskOnClick(Button buttonClicked){
        if(buttonClicked == exit){
            Time.timeScale = 1;
            Invoke("LoadGameScene", 1f);
        }else if(buttonClicked == options){
            
        }else if(buttonClicked == resume){
            Time.timeScale = 1;
            canvas.gameObject.SetActive(false);
        }
    }

    void LoadGameScene(){
        SceneManager.LoadSceneAsync("mainMenu");
    }
}