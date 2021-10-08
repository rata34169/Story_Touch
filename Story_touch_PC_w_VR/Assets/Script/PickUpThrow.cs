using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.XR;

public class PickUpThrow : MonoBehaviour
{
    //variables Mouse (Handle)
    public Transform objectHolder;
    public float throwForce = 1000.0f;
    public bool carryObject;
    public GameObject item;
    public bool isThrowable;
    public bool reading;
    public int newPage = 0;
    RaycastHit hit;
    int MAX_PAGE = 5;

    void start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + " " + item.characteristics);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Left click: grab / throw object
        if (Input.GetMouseButtonDown(0))
        {
            if(!carryObject)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Object: main object that need to grab or interact
                if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Object") && !reading)
                {
                    carryObject = true;
                    isThrowable = true;
                    if (carryObject)
                    {
                        item = hit.collider.gameObject;
                        Debug.Log( "Object: " + item.name );
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
                else if(Physics.Raycast(ray, out hit) && (hit.collider.tag == "Sub Object"))
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
                        else if(item.name == "Flip_Back")
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
                    }
                }

                else
                    return;
            }
            else
            {
                carryObject = false;
                isThrowable = false;
                objectHolder.DetachChildren();
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<Rigidbody>().useGravity = true;
                item.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * throwForce);
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
