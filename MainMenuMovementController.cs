

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MainMenuMovementController : MonoBehaviour
{
    private float speed = 100f;
    private float jumpForce = 5f;
    private float gravity = 9.81f;

    private Rigidbody rb;
    private bool isGrounded;
    private float lastXIncrement = 0f;
    public Camera camera;

    private float mouseSensitivity = 2f;

    private void Awake(){
        rb = GetComponent<Rigidbody>();
    }

        private void FixedUpdate(){
        if (PlayerPrefs.GetInt("inspectShip") == 1){
            float mouseInputX = Input.GetAxis("Mouse X");
            float mouseInputY = Input.GetAxis("Mouse Y");

            lastXIncrement = mouseInputX;
            Vector3 currentRotation = camera.transform.eulerAngles;

            currentRotation.y += mouseInputX * mouseSensitivity;

            Debug.Log(currentRotation.x);

            currentRotation.x += (mouseInputY * -1) * mouseSensitivity;
            ClampRotation(-40f, 40f, 0f);
            currentRotation.x += 180f;
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

            isGrounded = IsGrounded();

            if (!isGrounded)
            {
                rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
            }

            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                Debug.Log("awd");
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void ClampRotation(float minAngle, float maxAngle, float clampAroundAngle = 0){
        //clampAroundAngle is the angle you want the clamp to originate from
        //For example a value of 90, with a min=-45 and max=45, will let the angle go 45 degrees away from 90

        //Adjust to make 0 be right side up
        clampAroundAngle += 180;

        //Get the angle of the z axis and rotate it up side down
        float x = camera.transform.rotation.eulerAngles.x - clampAroundAngle;

        x = WrapAngle(x);

        //Move range to [-180, 180]
        x -= 180;

        //Clamp to desired range
        x = Mathf.Clamp(x, minAngle, maxAngle);

        //Move range back to [0, 360]
        x += 180;

        //Set the angle back to the transform and rotate it back to right side up
        camera.transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles.x, camera.transform.rotation.eulerAngles.y, x + clampAroundAngle);
    }

    float WrapAngle(float angle){
        //If its negative rotate until its positive
        while (angle < 0)
            angle += 360;

        //If its to positive rotate until within range
        return Mathf.Repeat(angle, 360);
    }

    private bool IsGrounded(){
        float raycastOffset = 0.1f;
        return Physics.Raycast(transform.position + Vector3.up * raycastOffset, Vector3.down, raycastOffset + 0.1f);
    }
}
