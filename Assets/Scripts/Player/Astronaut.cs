using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of character movement
    public Animator animator; // Reference to the Animator component

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        // Get the input from arrow keys
        //float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction
        Vector3 movement = new Vector3(0f, 0f, verticalInput) * moveSpeed * Time.deltaTime;
      
        // Move the character
        transform.Translate(movement);

 
        // Calculate the magnitude of input to determine if the character is moving
        float moveMagnitude = new Vector2(0f, verticalInput).sqrMagnitude;

        // Update the "Speed" parameter in the animator controller
        animator.SetFloat("Speed", moveMagnitude);

    }
}
