using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private float baseSpeed = 5f;
    private float speedIncrease = 0.1f;
    private float currentSpeed;

    private float rollAngle = 5f;
    private float rollBackDelay = 0.1f;
    private float currentRollAngle = 0f;

    private float pitchAngle = 3f;
    private float pitchBackDelay = 0.1f;
    private float currentPitchAngle = 0f;

    float countdownTime = 10f;

    public ParticleSystem particleSystem;

    void Start() {
        currentSpeed = baseSpeed;
        if (PlayerPrefs.GetInt("Booster") != 0) {
            StartCoroutine(ActivateRocketBooster());
        }
    }

    IEnumerator ActivateRocketBooster() {
        particleSystem.Play();
        currentSpeed = 150;
        StartCoroutine(DisplayCountdown());
        StartCoroutine(boosterShake());
        yield return new WaitForSeconds(countdownTime);
        currentSpeed = baseSpeed;
        PlayerPrefs.SetInt("Booster", 0);
        particleSystem.Stop();
    }

    IEnumerator DisplayCountdown() {
        float remainingTime = countdownTime;
        while (remainingTime > 0) {
            PlayerPrefs.SetInt("BoosterCountdown", Mathf.RoundToInt(remainingTime));
            remainingTime -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator boosterShake() {
        float duration = 5f;
        float magnitude = 0.4f;
        Vector3 startPosition = transform.position;

        float timeElapsed = 0.0f;
        while (timeElapsed < duration) {
            startPosition = transform.position;
            float x = startPosition.x + Random.Range(-magnitude, magnitude);
            float y = startPosition.y + Random.Range(-magnitude/2, magnitude/2);
            float z = startPosition.z;

            transform.position = new Vector3(x, y, z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;
    }

    void Update() {
        currentSpeed += speedIncrease * Time.deltaTime;
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * currentSpeed;
        float y = Input.GetAxis("Vertical") * Time.deltaTime * currentSpeed;
        float z = Time.deltaTime * currentSpeed;
        transform.Translate(x, y * 0.75f, z);

        if (Input.GetKey(KeyCode.A)) {
            currentRollAngle += rollAngle * Time.deltaTime * 2f;
            currentRollAngle = Mathf.Clamp(currentRollAngle, -rollAngle, rollAngle);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRollAngle);
        } else if (Input.GetKey(KeyCode.D)) {
            currentRollAngle -= rollAngle * Time.deltaTime * 2f;
            currentRollAngle = Mathf.Clamp(currentRollAngle, -rollAngle, rollAngle);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRollAngle);
        } else {
            currentRollAngle = Mathf.Lerp(currentRollAngle, 0f, rollBackDelay);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRollAngle);
        }

        if (Input.GetKey(KeyCode.S)) {
            currentPitchAngle += pitchAngle * Time.deltaTime * 2f;
            currentPitchAngle = Mathf.Clamp(currentPitchAngle, -pitchAngle, pitchAngle);
            transform.rotation = Quaternion.Euler(currentPitchAngle, 0f, currentRollAngle);
        } else if (Input.GetKey(KeyCode.W)) {
            currentPitchAngle -= pitchAngle * Time.deltaTime * 2f;
            currentPitchAngle = Mathf.Clamp(currentPitchAngle, -pitchAngle, pitchAngle);
            transform.rotation = Quaternion.Euler(currentPitchAngle, 0f, currentRollAngle);
        } else {
            currentPitchAngle = Mathf.Lerp(currentPitchAngle, 0f, pitchBackDelay);
            transform.rotation = Quaternion.Euler(currentPitchAngle, 0f, currentRollAngle);
        }
    }
}