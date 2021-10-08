using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PlayerMovemoent : MonoBehaviour
{
    //Walk
    public CharacterController controller;
    public float speed;

    //Gravity
    public float gravity;
    Vector3 velocity;

    //Ground check
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    bool isGrounded;

    void Start()
    {
        speed = 3f;
        gravity = -9.8f;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded)
        {
            velocity.y = -2f;
        }

        float moveBtn = Input.GetAxis("Jump");
        Vector3 move = transform.forward * moveBtn;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
