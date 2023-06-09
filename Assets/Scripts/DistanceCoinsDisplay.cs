using UnityEngine;
using UnityEngine.UI;

public class DistanceCoinsDisplay : MonoBehaviour{
    public GameObject ship;
    public Text distanceTextfield, coinTextfield, boosterTextfield, shieldTextfield;

    private void Start(){
        int initalShieldDisplay = PlayerPrefs.GetInt("Shield") == 1 ? 5 : 0;
        PlayerPrefs.SetInt("ShieldCountdown", initalShieldDisplay);
    }

    void FixedUpdate(){
        float z = ship.transform.position.z;
        int roundedZ = Mathf.RoundToInt(z);

        distanceTextfield.text = "Distance: " + roundedZ.ToString();
        PlayerPrefs.SetInt("distance", roundedZ);
        
        coinTextfield.text = "Coins: " + PlayerPrefs.GetInt("tempCoins").ToString();

        boosterTextfield.text = "Booster: " + PlayerPrefs.GetInt("BoosterCountdown");


        shieldTextfield.text = "Shield: " + PlayerPrefs.GetInt("ShieldCountdown");
    }
}
