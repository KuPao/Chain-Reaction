using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class Cup : ChainPrimitive {

        public delegate void TriggerCallback();
        public TriggerCallback triggerCallback;

        public Transform bottom;

        public float defaultMass, time = 0;
        public bool isEnd;

        void Init() {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.mass = defaultMass;

            MeshCollider mc = GetComponent<MeshCollider>();
            mc.convex = false;
        }

        void OnTriggerEnter(Collider other) {
            MeshCollider mc = GetComponent<MeshCollider>();
            mc.convex = true;

            Rigidbody otherRB = other.GetComponent<Rigidbody>();

            Rigidbody rb = GetComponent<Rigidbody>();
            string othername = other.name.ToLower();
            if (other.name != this.name && !othername.Contains("cup") && !othername.Contains("board")) {
                    if (rb != null) {
                    //Debug.Log(other.gameObject.name);
                    rb.isKinematic = false;
                    if (otherRB != null) {
                        rb.velocity = otherRB.velocity;
                        rb.mass += otherRB.mass;
                    }
                }
                //Debug.Log(other.name + "  " + this.name);
            
                other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                other.gameObject.active = false;
            }

            if (triggerCallback != null)
                triggerCallback();
        }

        void setTriggerCallback(TriggerCallback cb) {
            triggerCallback = cb;
        }

        public override void reset() {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.mass = defaultMass;
            }

            MeshCollider mc = GetComponent<MeshCollider>();
            mc.convex = false;
        }

        public override GameObject genFloor(FloorCreator floorCreator) {
            if (!isEnd)
                return null;

            GameObject floor = GameObject.Instantiate(floorCreator.floorPrefab);
            floor.transform.position = bottom.position;

            return floor;
        }
    }
}

