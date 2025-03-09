using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLocalConstraints : MonoBehaviour {
    [Header("Freeze Local Position")]
    [SerializeField]
    bool x;
    [SerializeField]
    bool y;
    [SerializeField]
    bool z;

    Vector3 localPosition0;    //original local position

    private void Start() {
        SetOriginalLocalPosition();
    }

    private void Update() {
        float x, y, z;


        if (this.x)
            x = localPosition0.x;
        else
            x = transform.localPosition.x;

        if (this.y)
            y = localPosition0.y;
        else
            y = transform.localPosition.y;

        if (this.z)
            z = localPosition0.z;
        else
            z = transform.localPosition.z;


        transform.localPosition = new Vector3(x, y, z);

    }

    public void SetOriginalLocalPosition() {
        localPosition0 = transform.localPosition;
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag != "Player") {
            //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //this.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionX;
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            this.x = true;
            this.z = true;
        }
    }
    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag != "Player") {
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            this.x = false;
            this.z = false;
        }
    }
}
