using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CollisionDetector : MonoBehaviour
{
    private Rigidbody spaceshipRigidbody;
    private AudioSource audioSource;
    public AudioClip crashSound;
    public AudioClip ringSound;
    private GameObject spaceship;

    private bool boosterActive = false;
    private bool shieldActive = false;
    private bool shieldRunning = false;

    private void Start(){
        audioSource = GetComponent<AudioSource>();
        spaceshipRigidbody = GetComponentInChildren<Rigidbody>();
        spaceship = GameObject.Find("Transport Shuttle_fbx");

        if(PlayerPrefs.GetInt("Booster") != 0){
            spaceshipRigidbody.isKinematic = true;
            boosterSwitch();
            PlayerPrefs.SetInt("Booster", 0);
            Invoke("boosterSwitch", 10f);
        }
        if(PlayerPrefs.GetInt("Shield") != 0){
            PlayerPrefs.SetInt("Shield", 0);
            shieldActive = true;
        }
    }

    private void FixedUpdate(){
        if(transform.position.x < -50f || transform.position.x > 50f || transform.position.y < -0f || transform.position.y > 30f){
            loadDeathscreen();
        }
    }

    void OnTriggerEnter(Collider other){
        switch (other.gameObject.tag){
            case "Ring":
                StartCoroutine(playRingSound());
                GameObject spaceship = GameObject.Find("Transport Shuttle_fbx");
                int tempCoins = PlayerPrefs.GetInt("tempCoins");
                tempCoins = tempCoins + Mathf.RoundToInt(100f + 0.1f * spaceship.transform.position.z);
                PlayerPrefs.SetInt("tempCoins", tempCoins);
                break;
            case "Rock":
                audioSource.PlayOneShot(crashSound, 1f);
                if(boosterActive){
                    StartCoroutine(playShieldAnimation());
                }
                else{
                    if(!shieldRunning){
                        if(shieldActive){
                            PlayerPrefs.SetInt("Shield", 0);
                            shieldActive = false;
                            shieldRunning = true;
                            StartCoroutine(surviveCrashWithRockShield(false));
                        }
                        else{
                            loadDeathscreen();
                        }
                    }
                }
                break;
            case "Boundary":
                    loadDeathscreen();
                break;
            }
    }

    private void boosterSwitch(){
        if(boosterActive){
            spaceshipRigidbody.isKinematic = false;
            boosterActive = false;
            shieldRunning = true;
            StartCoroutine(surviveCrashWithRockShield(true));
        } else if(!boosterActive){
            spaceshipRigidbody.isKinematic = true;
            boosterActive = true;
        }
    }

    private IEnumerator surviveCrashWithRockShield(bool booster){
        spaceshipRigidbody.isKinematic = true;
        spaceship = GameObject.Find("Transport Shuttle_fbx");
        spaceship.transform.localPosition = new Vector3(0f, -2.33f, -1.817f);
        StartCoroutine(playShieldAnimation());

        float timer = 5f;
        while (timer > 0f){
            yield return new WaitForSeconds(1f);
            timer--;
            PlayerPrefs.SetInt("ShieldCountdown", Mathf.RoundToInt(timer));
        }
        shieldRunning = false;
        spaceshipRigidbody.isKinematic = false;
        if(booster){
            PlayerPrefs.SetInt("ShieldCountdown", 5);
        }
    }

    IEnumerator playRingSound(){
        yield return new WaitForSeconds(0.1f);
        audioSource.PlayOneShot(ringSound, 1f);
    }

    IEnumerator playShieldAnimation(){
        GameObject mainCamera = GameObject.Find("Main Camera");
        float duration = 0.5f; // duration of the animation in seconds
        float magnitude = 0.2f; // magnitude of the shake
        Vector3 startPosition = mainCamera.transform.position;

        float timeElapsed = 0.0f;
        while (timeElapsed < duration) {
            startPosition = mainCamera.transform.position;
            float x = startPosition.x + Random.Range(-magnitude, magnitude);
            float y = startPosition.y + Random.Range(-magnitude, magnitude);
            float z = startPosition.z;

            mainCamera.transform.position = new Vector3(x, y, z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = startPosition;
    }

    private void loadDeathscreen(){
        SceneManager.LoadScene("deathScreen");
    }
}
