using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// In this script, I'm let book be trigger to read if it touch the blanket

public class ReadZoneInteract : MonoBehaviour
{
    public Transform readTrigger;
    public Transform blanketCenter;
    public Transform player;
    public GameObject book;
    public float triggerDistance = 0.2f;
    public LayerMask readZone;

    bool nextreadable, readable;

    Vector3 triggerScale = new Vector3(5f, 5f, 5f);
    Vector3 normalScale = new Vector3(1.5f, 1.5f, 1.5f);
    Vector3 positionOffset = new Vector3(0f, 0.152f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        readable = false;
        nextreadable = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Distence: book -- Read Zone(Layer)
        nextreadable = Physics.CheckSphere(readTrigger.position, triggerDistance, readZone);

        if(nextreadable && (readable != nextreadable))
        {
            readable = nextreadable;
            book.gameObject.transform.localScale = triggerScale;
            book.gameObject.transform.position = blanketCenter.transform.position;
            book.gameObject.transform.position += positionOffset;
            book.gameObject.transform.rotation = player.transform.rotation;
            book.gameObject.transform.Rotate(0f, 180f, 0f, Space.Self);
            book.GetComponent<Rigidbody>().isKinematic = true;
            book.GetComponent<Rigidbody>().useGravity = false;
        }

        else if(!nextreadable && (readable != nextreadable))
        {
            readable = nextreadable;
            book.gameObject.transform.localScale = normalScale;
        }
    }
}
