using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public Button buttonNewRun, buttonShop, buttonOptions, buttonBack, buttonResetGameProgress, buttonGameEnd, buttonInspectShip;
    public TextMeshProUGUI totalCoinsText, textShield, textBooster, textLight;
    public CameraAnimationPlayer animPlayer;
    public ScrollRect scrollViewShop;
    public GameObject panelLeft;

    void Start(){
        buttonNewRun.onClick.AddListener(() => TaskOnClick(buttonNewRun));
        buttonShop.onClick.AddListener(() => TaskOnClick(buttonShop));
        buttonOptions.onClick.AddListener(() => TaskOnClick(buttonOptions));
        buttonBack.onClick.AddListener(() => TaskOnClick(buttonBack));
        buttonGameEnd.onClick.AddListener(() => TaskOnClick(buttonGameEnd));
        buttonResetGameProgress.onClick.AddListener(() => TaskOnClick(buttonResetGameProgress));
        buttonBack.gameObject.SetActive(false);
        buttonResetGameProgress.gameObject.SetActive(false);
        scrollViewShop.gameObject.SetActive(false);
    }

    void Update(){
        totalCoinsText.text = "Coins: " + (PlayerPrefs.GetInt("totalCoins")).ToString();
        textShield.text = "Shield: " + (PlayerPrefs.GetInt("Shield") == 1 ? "Activated":"Deactivated");
        textBooster.text = "Boost: " + (PlayerPrefs.GetInt("Booster") == 1 ? "Activated":"Deactivated");
        textLight.text = "Light: " + (PlayerPrefs.GetInt("Light") == 1 ? "Activated":"Deactivated");
    }

    void TaskOnClick(Button buttonClicked){
        if(buttonClicked == buttonNewRun){
            disableMainMenu();
            newRun();
        }
        else if(buttonClicked == buttonShop){
            disableMainMenu();
            enableShop();
        }
        else if(buttonClicked == buttonOptions){
            disableMainMenu();
            enableSettings();
        }
        else if(buttonClicked == buttonBack){
            buttonBack.gameObject.SetActive(false);
            enableMainMenu();
            disableSettings();
            disableShop();
        }
        else if(buttonClicked == buttonGameEnd){
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        else if(buttonClicked == buttonResetGameProgress){
            gameReset();
        }
    }

    void newRun(){
        totalCoinsText.gameObject.SetActive(false);
        textShield.gameObject.SetActive(false);
        textBooster.gameObject.SetActive(false);
        textLight.gameObject.SetActive(false);
        panelLeft.gameObject.SetActive(false);
        PlayerPrefs.SetInt("tempCoins", 0);
        PlayerPrefs.SetInt("distance", 0);
        animPlayer.PlayAnimation();
        Invoke("LoadGameScene", 1f);
    }

    void disableMainMenu(){
        buttonNewRun.gameObject.SetActive(false);
        buttonShop.gameObject.SetActive(false);
        buttonOptions.gameObject.SetActive(false);
        buttonGameEnd.gameObject.SetActive(false);
    }

    void enableMainMenu(){
        buttonNewRun.gameObject.SetActive(true);
        buttonShop.gameObject.SetActive(true);
        buttonOptions.gameObject.SetActive(true);
        buttonBack.GetComponent<RectTransform>().anchoredPosition = new Vector2(41.54635f, 185.6707f);
        buttonResetGameProgress.gameObject.SetActive(false);
        buttonGameEnd.gameObject.SetActive(true);
    }
 
    void enableSettings(){
        buttonOptions.gameObject.SetActive(false);
        buttonResetGameProgress.gameObject.SetActive(true);
        buttonBack.gameObject.SetActive(true);
    }

    void disableSettings(){
        buttonResetGameProgress.gameObject.SetActive(false);
        buttonBack.gameObject.SetActive(false);
    }

    void enableShop(){
        PlayerPrefs.SetInt("shop", 1);
        buttonBack.gameObject.SetActive(true);
        scrollViewShop.gameObject.SetActive(true);
        buttonBack.GetComponent<RectTransform>().anchoredPosition = new Vector2(41.54635f, 185.655f);
    }

    void disableShop(){
        buttonBack.gameObject.SetActive(false);
        scrollViewShop.gameObject.SetActive(false);
    }

    void gameReset(){
        PlayerPrefs.SetInt("totalCoins", 1000);
    }

    void LoadGameScene(){
        SceneManager.LoadScene("gameRun");
    }
}
