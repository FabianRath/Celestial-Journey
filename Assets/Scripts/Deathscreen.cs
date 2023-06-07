using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Deathscreen : MonoBehaviour
{
    public Button buttonMainMenu, buttonContinue;
    public Text text1, text2, text3, text4;

    void Start(){
        Cursor.visible = true;
        buttonMainMenu.onClick.AddListener(() => TaskOnClick(buttonMainMenu));
        buttonContinue.onClick.AddListener(() => TaskOnClick(buttonContinue));

        int distance = PlayerPrefs.GetInt("distance");
        text1.text = "Distance: " + distance.ToString();

        if(PlayerPrefs.GetInt("maxDistance") < distance){
            PlayerPrefs.SetInt("maxDistance", distance);
        }
        text4.text = "Max Distance: " + PlayerPrefs.GetInt("maxDistance").ToString();

        int tempCoins = PlayerPrefs.GetInt("tempCoins");
        text2.text = "Coins: " + tempCoins.ToString();

        int totalCoins = PlayerPrefs.GetInt("totalCoins");
        int newTotalCoins = totalCoins+tempCoins;
        PlayerPrefs.SetInt("totalCoins", newTotalCoins);
        text3.text = "Total Coins: " + newTotalCoins.ToString();
    }

    void TaskOnClick(Button buttonClicked){
        if(buttonClicked == buttonMainMenu){
            SceneManager.LoadScene("mainMenu");
        }
        else if(buttonClicked == buttonContinue){
            Cursor.visible = false;
            PlayerPrefs.SetInt("distance", 0);
            PlayerPrefs.SetInt("tempCoins", 0);            
            SceneManager.LoadScene("gameRun");
        }
    }
}