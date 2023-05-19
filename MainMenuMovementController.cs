

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MainMenuMovementController : MonoBehaviour
{
    private float speed = 100f;
    private float jumpForce = 130f;
    private float fallForce = 15f;
    private float timer = 100f;
    private float takeTimer = 0f;

    private Rigidbody rb;
    private bool isGrounded;
    public Camera camera;

    private float mouseSensitivity = 2f;

    private void Awake(){
        rb = GetComponent<Rigidbody>();
    }

        private void FixedUpdate(){
        if (PlayerPrefs.GetInt("inspectShip") == 1){
            float mouseInputX = Input.GetAxis("Mouse X");
            float mouseInputY = Input.GetAxis("Mouse Y");

            Vector3 currentRotation = camera.transform.eulerAngles;

            int currentRotationXInt = (int)Math.Round(currentRotation.x);
            float currentRotationX = currentRotationXInt;
            
            if (currentRotationX <= 35 && currentRotationX >= 0){
                if ((mouseInputY * -1) * mouseSensitivity < 0f){
                    currentRotation.x += (mouseInputY * -1) * mouseSensitivity;
                }
                else{
                    currentRotation.x = Mathf.Min(currentRotation.x + (mouseInputY * -1) * mouseSensitivity, 35);
                }
            }
            else if (currentRotationX >= 330 && currentRotationX <= 360){
                if ((mouseInputY * -1) * mouseSensitivity > 0f){
                    currentRotation.x += (mouseInputY * -1) * mouseSensitivity;
                }
                else{
                    currentRotation.x = Mathf.Max(currentRotation.x + (mouseInputY * -1) * mouseSensitivity, 330);
                }
            }

            currentRotation.y += mouseInputX * mouseSensitivity;

            currentRotation.z = 0f;

            camera.transform.eulerAngles = currentRotation;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movementDirection = new Vector3(horizontalInput, 0f, verticalInput);
            movementDirection = Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f) * movementDirection;
            movementDirection.Normalize();

            Vector3 movement = movementDirection * speed * Time.fixedDeltaTime;
            movement.y = rb.velocity.y;
            rb.velocity = movement;


            if (Input.GetButtonDown("Jump")){
                if (timer - takeTimer > 100){
                    takeTimer = timer;
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            } 
            if(timer - takeTimer > 20){
                rb.AddForce(Vector3.down * fallForce, ForceMode.Impulse);
            }
            timer += 1f;
        }
    }
}
