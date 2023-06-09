using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVHandling : MonoBehaviour{
    public List<GameObject> objectsToDelete;
    public Camera firstPersonCamera, thirdPersonCamera;
    public Canvas pauseMenu;

    void Start(){
        List<GameObject> objectsToDestroy = new List<GameObject>(objectsToDelete);

        foreach (GameObject obj in objectsToDestroy){
            Destroy(obj);
            objectsToDelete.Remove(obj);
        }
    }
    void Update() {
        if(PlayerPrefs.GetInt("firstPerson") == 0){
            firstPersonCamera.gameObject.SetActive(false);
            thirdPersonCamera.gameObject.SetActive(true);
            switchPauseMenuThirdPerson();
        }else if(PlayerPrefs.GetInt("firstPerson") == 1){
            firstPersonCamera.gameObject.SetActive(true);
            thirdPersonCamera.gameObject.SetActive(false);
            switchPauseMenuFirstPerson();
        }
    }

    void switchPauseMenuThirdPerson(){
        pauseMenu.transform.localPosition = new Vector3(-14.05191f, 6.01f, -10.28f);
        pauseMenu.worldCamera = thirdPersonCamera;
    }

    void switchPauseMenuFirstPerson(){
        pauseMenu.transform.localPosition = new Vector3(-14.05191f, 4.25767f, 1.722192f);
        pauseMenu.worldCamera = firstPersonCamera;
    }
}