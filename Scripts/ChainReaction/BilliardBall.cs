using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliardBall : MonoBehaviour
{
    public float defaultMass, time = 0;
    public bool isEnd;
    public Vector2 direction = new Vector2(0,0);

    void Init() {
        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.isKinematic = true;
    }
    private void Update() {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) {
            if (rb.velocity.magnitude > 0.005f) {
                Vector2 horizon = new Vector2(rb.velocity.x, rb.velocity.z);
                Vector2 new_horizon = direction * horizon.magnitude;
                rb.velocity = new Vector3(new_horizon.x, rb.velocity.y, new_horizon.y);
            }
        }        
    }

    //void OnTriggerEnter(Collider other) {
    //    Rigidbody otherRB = other.GetComponent<Rigidbody>();

    //    Rigidbody rb = GetComponent<Rigidbody>();
    //    string othername = other.name.ToLower();
    //    Debug.Log(othername);
    //    if (other.name != this.name && !othername.Contains("cup") && !othername.Contains("board")) {
    //        if (rb != null) {
    //            //Debug.Log(other.gameObject.name);
    //            //rb.isKinematic = false;
    //            if (otherRB != null) {
    //                float value = otherRB.velocity.magnitude;
    //                ChainReaction.Billiard parent = this.GetComponentInParent<ChainReaction.Billiard>();
    //                parent.maxVelocity = value > parent.maxVelocity ? value : parent.maxVelocity;
    //                value = parent.maxVelocity;
    //                Vector3 direct = this.transform.position - other.transform.position;
    //                direct = direct.normalized;
    //                Debug.Log("direct:" + direct.ToString());
    //                rb.velocity = direct * value;
    //            }
    //        }
    //    }
    //}
}
