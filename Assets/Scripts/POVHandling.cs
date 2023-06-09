using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVHandling : MonoBehaviour{
    public List<GameObject> objectsToDelete;
    public Camera firstPersonCamera, thirdPersonCamera;
    public Canvas pauseMenu;
    public Canvas distanceCoinsDisplay;

    private bool firstPerson;

    void Start(){
        List<GameObject> objectsToDestroy = new List<GameObject>(objectsToDelete);

        foreach (GameObject obj in objectsToDestroy){
            Destroy(obj);
            objectsToDelete.Remove(obj);
        }

        if(PlayerPrefs.GetInt("firstPerson") == 0){
            switchPauseMenuThirdPerson();
        }else if(PlayerPrefs.GetInt("firstPerson") == 1){
            switchPauseMenuFirstPerson();
        }
    }
    void Update() {
        if(PlayerPrefs.GetInt("firstPerson") == 0){
            if(firstPerson == true){
                switchPauseMenuThirdPerson();
            }
        } else if(PlayerPrefs.GetInt("firstPerson") == 1){
            if(firstPerson == false){
                switchPauseMenuFirstPerson();
            }
        }
    }

    void switchPauseMenuThirdPerson(){
        firstPerson = false;

        firstPersonCamera.gameObject.SetActive(false);
        thirdPersonCamera.gameObject.SetActive(true);

        pauseMenu.transform.localPosition = new Vector3(-14.05191f, 6.01f, -10.28f);
        pauseMenu.worldCamera = thirdPersonCamera;

        distanceCoinsDisplay.transform.localPosition = new Vector3(-14.32f, 6.185f, -10.28f);
        distanceCoinsDisplay.GetComponent<RectTransform>().localScale = new Vector3(0.0002f, 0.0002f, 0f);
        distanceCoinsDisplay.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        distanceCoinsDisplay.worldCamera = thirdPersonCamera;
    }

    void switchPauseMenuFirstPerson(){
        firstPerson = true;

        firstPersonCamera.gameObject.SetActive(true);
        thirdPersonCamera.gameObject.SetActive(false);

        pauseMenu.transform.localPosition = new Vector3(-14.05191f, 4.25767f, 1.722192f);
        pauseMenu.worldCamera = firstPersonCamera;

        distanceCoinsDisplay.transform.localPosition = new Vector3(-14.0412f, 4.0699f, 2.1453f);
        distanceCoinsDisplay.GetComponent<RectTransform>().localScale = new Vector3(0.00035f, 0.00035f, 0f);
        distanceCoinsDisplay.GetComponent<RectTransform>().rotation = Quaternion.Euler(35f, 1.5f, 0f);
        distanceCoinsDisplay.worldCamera = firstPersonCamera;
    }
}