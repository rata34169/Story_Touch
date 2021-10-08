using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

/*////////////////////////////////////////////
 Default Value:
    Sensitivity: 70
////////////////////////////////////////////*/

public class KeyboardLook : MonoBehaviour
{
    //variable Keyboard (Head)
    public float sensitivity;
    public Transform playerBody;
    float xRotation;

    void Start()
    {
        Debug.Log("Hello Unity!!");
        Debug.Log("I'm in the scene " + SceneManager.GetActiveScene().buildIndex);

        sensitivity = 500f;
        xRotation = 0f;
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        /////////////////////////////////////////
        //Keyboard controll
        
        //fetch keyboard controll
        //  Horizontal: a(-1),d(+1)
        //  Vertical  : s(-1),w(+1)
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;//Use deltaTime to costomized PC's fps
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //limit vision angle between [-90, 90]
        xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);

        //rotate "camera"
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //rotate "character"
        playerBody.Rotate(Vector3.up * mouseX);
        /////////////////////////////////////////
        
        /////////////////////////////////////////
        //Mouse controll

        //Unlock the cursor
        if (Input.GetKey(KeyCode.H))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        /////////////////////////////////////////
    }
}
