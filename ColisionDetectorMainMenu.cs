using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColisionDetectorMainMenu : MonoBehaviour{
    public Light playerLight;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="InsideSpaceShip"){
            playerLight.gameObject.SetActive(false);
        } else if(other.gameObject.tag=="OutsideSpaceShip"){
            playerLight.gameObject.SetActive(true);
        }
    }
}
