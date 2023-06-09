using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour{
    public Canvas canvas;
    public Button buttonMainMenu, buttonOptions, buttonMuteBGM, buttonExit, buttonResume, buttonSwitchPOV;
    void Start(){
        buttonMainMenu.onClick.AddListener(() => TaskOnClick(buttonMainMenu));
        buttonOptions.onClick.AddListener(() => TaskOnClick(buttonOptions));
        buttonResume.onClick.AddListener(() => TaskOnClick(buttonResume));
        buttonMuteBGM.onClick.AddListener(() => TaskOnClick(buttonMuteBGM));
        buttonExit.onClick.AddListener(() => TaskOnClick(buttonExit));
        buttonSwitchPOV.onClick.AddListener(() => TaskOnClick(buttonSwitchPOV));
        canvas.gameObject.SetActive(false);
        buttonMuteBGM.gameObject.SetActive(false);
        buttonExit.gameObject.SetActive(false);
        buttonSwitchPOV.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    
    void FixedUpdate(){
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (canvas.gameObject.activeSelf == false){
                Cursor.visible = true;
                enableStartMenu();
                Time.timeScale = 0;
            } else {
                canvas.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
    
    void TaskOnClick(Button buttonClicked){
        if(buttonClicked == buttonMainMenu){
            Time.timeScale = 1;
            Invoke("LoadGameScene", 1f);
        }else if(buttonClicked == buttonOptions){
            enableOptions();
            disableStartMenu();
        }else if(buttonClicked == buttonMuteBGM){
            BGM.instance.startPauseMusic();
        }else if(buttonClicked == buttonExit){
            disableOptions();
            enableStartMenu();
        }else if(buttonClicked == buttonResume){
            Cursor.visible = false;
            Time.timeScale = 1;
            canvas.gameObject.SetActive(false);
        }else if(buttonClicked == buttonSwitchPOV){
            switchPOV();
        }
    }

    void enableStartMenu(){
        canvas.gameObject.SetActive(true);
        buttonMainMenu.gameObject.SetActive(true);
        buttonOptions.gameObject.SetActive(true);
        buttonResume.gameObject.SetActive(true);
    }

    void disableStartMenu(){
        buttonMainMenu.gameObject.SetActive(false);
        buttonOptions.gameObject.SetActive(false);
        buttonResume.gameObject.SetActive(false);
    }
    void enableOptions(){
        buttonMuteBGM.gameObject.SetActive(true);
        buttonExit.gameObject.SetActive(true);
        buttonSwitchPOV.gameObject.SetActive(true);
    }

    void disableOptions(){
        buttonMuteBGM.gameObject.SetActive(false);
        buttonExit.gameObject.SetActive(false);
        buttonSwitchPOV.gameObject.SetActive(false);
    }

    void LoadGameScene(){
        SceneManager.LoadSceneAsync("mainMenu");
    }

    void switchPOV(){
        if(PlayerPrefs.GetInt("firstPerson") == 0){
            PlayerPrefs.SetInt("firstPerson", 1);
        }else if(PlayerPrefs.GetInt("firstPerson") == 1){
            PlayerPrefs.SetInt("firstPerson", 0);
        }
    }
}