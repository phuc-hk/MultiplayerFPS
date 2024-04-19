using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxController : MonoBehaviour
{
    public float jumpForce = 30f;
    public float moveAmount = 3f;
    public float moveSpeed = 6f;

    private Rigidbody rigidBody;
    private Animator animator;
    private bool isGrounded = true;
    Vector2 touchStartPosition;
    int currentPosition = 0;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Run();
        // Swipe detection
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Record touch position
                touchStartPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 swipeDelta = touch.position - touchStartPosition;

                // Determine swipe direction
                if (swipeDelta.magnitude > 100)
                {
                    // Calculate swipe direction
                    //Vector2 swipeDelta = touch.position - touch.deltaPosition;
                    Vector2 swipeDirection = swipeDelta.normalized;

                    // Determine swipe direction
                    float angle = Vector2.SignedAngle(Vector2.right, swipeDirection);

                    if (angle > 45 && angle < 135) // Swipe up
                    {
                        Jump();
                    }
                    else if (angle > 135 || angle < -135) // Swipe left
                    {
                        currentPosition--;
                        //if (currentPosition == -1)
                        //    gameObject.transform.localScale = new Vector3(1, 1, 1);
                        if (currentPosition >= -1)
                            MoveSideways(-1);
                        else
                            currentPosition = -1;
                    }
                    else if (angle > -135 && angle < -45) // Swipe down
                    {
                        Slide();
                    }
                    else if (angle > -45 && angle < 45) // Swipe right
                    {
                        currentPosition++;
                        //if (currentPosition == 1)
                        //    gameObject.transform.localScale = new Vector3(-1, 1, 1);
                        if (currentPosition <= 1)
                            MoveSideways(1);
                        else
                            currentPosition = 1;
                    }

                }
            }
        }
    }

    private void Run()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded)
        {
            animator.SetTrigger("jump");
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void Slide()
    {
        animator.SetTrigger("slide");
    }

    //void StopSlide()
    //{
    //    isSliding = false;
    //    // Revert sliding animation or change character collider size back
    //}

    void MoveSideways(int direction)
    {
        Vector3 moveDirection = new Vector3(direction, 0, 0);
        transform.Translate(moveDirection * moveAmount);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Va cham ne");
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Cham dat ne");
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
