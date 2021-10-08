using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.XR;

public class Hand_Script : MonoBehaviour
{
    private InputDevice targetDevice;
    Vector2 axis;
    bool pBtn;

    //variables Mouse (Handle)
    public Transform objectHolder;
    public float throwForce = 10.0f;
    public bool carryObject;
    public GameObject item;
    public bool isThrowable;
    public bool reading;
    public int newPage = 0;
    RaycastHit hit;
    int MAX_PAGE = 5;
    int triggerBond = 0;

    public Transform player;
    public Transform pOneZero;

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

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + " " + item.characteristics);
        }

        if(devices.Count > 0)
        {
            targetDevice = devices[0];
        }

        Debug.Log(targetDevice.name);

        speed = 1f;
        gravity = -9.8f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded)
        {
            velocity.y = -2f;
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis) && axis != Vector2.zero)
        {
            Vector3 move = transform.forward * axis.y + transform.right * axis.x;

            controller.Move(move * speed * Time.deltaTime);

            //velocity.y += gravity * Time.deltaTime;
            //controller.Move(velocity * Time.deltaTime);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out pBtn) && pBtn)
        {
            triggerBond++;

            if(triggerBond > 8)
            {
                if (!carryObject)
                {

                    // Object: main object that need to grab or interact
                    if (Physics.Raycast(transform.position, forward, out hit) && (hit.collider.tag == "Object") && !reading)
                    {
                        carryObject = true;
                        isThrowable = true;
                        if (carryObject)
                        {
                            item = hit.collider.gameObject;
                            Debug.Log("Object: " + item.name);
                            Animator reset = item.GetComponent<Animator>();
                            bool ifReset = (reset.GetInteger("Page") == 0) ? false : true;
                            if (ifReset)
                            {
                                reset.SetTrigger("Reset");
                                reset.SetInteger("Page", 0);
                            }
                            item.transform.SetParent(objectHolder);
                            item.gameObject.transform.position = objectHolder.position;
                            item.gameObject.transform.rotation = objectHolder.rotation;
                            item.GetComponent<Rigidbody>().isKinematic = true;
                            item.GetComponent<Rigidbody>().useGravity = false;
                        }
                    }

                    // Sub Object: perform main objects' function (ex. book flip)
                    else if (Physics.Raycast(transform.position, forward, out hit) && (hit.collider.tag == "Sub Object"))
                    {
                        item = hit.collider.gameObject;
                        if (!bookIsTrigger(item.transform.parent))
                        {
                            //Debug.Log("Unrigger Sub Object: " + item.name + "   Parent: " + item.transform.parent.gameObject.name);
                            return;
                        }
                        else
                        {
                            //Debug.Log("Trigger Sub Object: " + item.name + "   Parent: " + item.transform.parent.gameObject.name);
                            Animator animator = item.transform.parent.gameObject.GetComponent<Animator>();
                            newPage = animator.GetInteger("Page");
                            if (item.name == "Flip_Next")
                            {
                                if (newPage == MAX_PAGE)
                                {
                                    Debug.Log("Can't flip next: maximum page number.");
                                    return;
                                }
                                else
                                {
                                    newPage++;
                                    animator.SetInteger("Page", newPage);
                                    animator.SetTrigger("Flip_Next");
                                    reading = (newPage == 5) ? false : true;
                                    Debug.Log("Flip next: page " + newPage);
                                }
                            }
                            else if (item.name == "Flip_Back")
                            {
                                if (newPage == 0)
                                {
                                    Debug.Log("Can't flip back: minimum page number.");
                                    return;
                                }
                                else
                                {
                                    newPage--;
                                    animator.SetInteger("Page", newPage);
                                    animator.SetTrigger("Flip_Back");
                                    reading = (newPage == 0) ? false : true;
                                    Debug.Log("Flip back: page " + newPage);
                                }
                            }

                            if(newPage == 4)
                            {
                                Debug.Log("Change!!!!");
                                Debug.Log(player.position + "to" + pOneZero.position);
                                player.position = pOneZero.position;
                            }
                        }
                    }

                    else
                        return;
                }
                else
                {
                    carryObject = false;
                    isThrowable = false;
                    item.transform.SetParent(null);
                    item.GetComponent<Rigidbody>().isKinematic = false;
                    item.GetComponent<Rigidbody>().useGravity = true;
                    item.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * throwForce);
                }

                triggerBond = 0;
            }
        }
    }

    bool bookIsTrigger(Transform cmpObject)
    {
        Vector3 bookUntriggerSize = new Vector3(1.5f, 1.5f, 1.5f);
        if (cmpObject.transform.localScale == bookUntriggerSize)
            return false;
        else
        {
            //Debug.Log("Object trigger with localScale" + cmpObject.transform.localScale);
            return true;
        }
    }
}
