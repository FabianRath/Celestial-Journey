
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CollisionDetector : MonoBehaviour
{
    private Rigidbody spaceshipRigidbody;
    private AudioSource audioSource;
    public AudioClip crashSound;
    public AudioClip ringSound;

    private bool boosterActive = false;
    private bool shieldActive = false;
    private bool shieldRunning = false;

    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    public Canvas canvas;

    private void Start(){
        audioSource = GetComponent<AudioSource>();
        spaceshipRigidbody = GetComponentInChildren<Rigidbody>();

        if(PlayerPrefs.GetInt("Booster") != 0){
            spaceshipRigidbody.isKinematic = true;
            boosterSwitch();
            PlayerPrefs.SetInt("Booster", 0);
            StartCoroutine(boosterTransform());
            Invoke("boosterSwitch",10);
        }
        if(PlayerPrefs.GetInt("Shield") != 0){
            PlayerPrefs.SetInt("Shield", 0);
            shieldActive = true;
        }
    }

    IEnumerator boosterTransform(){
        float timer = 100f;
        while (timer > 0f){
            timer--;
            StartCoroutine(transformToPosition());
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FixedUpdate(){
        if(transform.position.x < -60f || transform.position.x > 60f || transform.position.y < -50f || transform.position.y > 70f){
            loadDeathscreen();
        }
    }

    void OnTriggerEnter(Collider other){
        switch (other.gameObject.tag){
            case "Ring":
                StartCoroutine(playRingSound());
                int tempCoins = PlayerPrefs.GetInt("tempCoins");
                tempCoins = tempCoins + Mathf.RoundToInt(100f + 0.1f * transform.position.z);
                PlayerPrefs.SetInt("tempCoins", tempCoins);
                break;
            case "Rock":
                spaceshipRigidbody.isKinematic = true;
                audioSource.PlayOneShot(crashSound, 1f);
                if(boosterActive){
                    StartCoroutine(transformToPosition());
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
        StartCoroutine(playShieldAnimation());

        float timer = 50f;
        while (timer > 0f){
            timer--;
            PlayerPrefs.SetInt("ShieldCountdown", Mathf.RoundToInt(timer/10));
            StartCoroutine(transformToPosition());
            yield return new WaitForSeconds(0.1f);
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
        Camera camera;
        if(PlayerPrefs.GetInt("firstPerson") == 0){
            camera = thirdPersonCamera;
        }else{
            camera = firstPersonCamera;
        }
        float duration = 0.5f; // duration of the animation in seconds
        float magnitude = 0.2f; // magnitude of the shake
        Vector3 startPosition = camera.transform.position;

        float timeElapsed = 0.0f;
        while (timeElapsed < duration) {
            startPosition = camera.transform.position;
            float x = startPosition.x + Random.Range(-magnitude, magnitude);
            float y = startPosition.y + Random.Range(-magnitude, magnitude);
            float z = startPosition.z;

            camera.transform.position = new Vector3(x, y, z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        camera.transform.position = startPosition;
    }

    private void loadDeathscreen(){
        SceneManager.LoadScene("deathScreen");
    }

    IEnumerator transformToPosition(){
        transform.localPosition = new Vector3(-14.05191f, 3.116171f, -0.4671082f);
        if(PlayerPrefs.GetInt("firstPerson") == 1){
            canvas.transform.localPosition = new Vector3(-14.05241f, 4.07217f, 2.152892f);
            firstPersonCamera.transform.localPosition = new Vector3(3.634f, 2.33f, 0f);
        }else if(PlayerPrefs.GetInt("firstPerson") == 0){
            thirdPersonCamera.transform.localPosition = new Vector3(-20.366f, 5.83f, 0f);
        }
        yield return null;
    }
}