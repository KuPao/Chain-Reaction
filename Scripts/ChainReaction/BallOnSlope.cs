using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class BallOnSlope : ChainPrimitive {
        public GameObject ball;
        public GameObject slope;

        public float ballOffset;
        public float slopeLength;
        public float time;
        void Update() {
            if(hitTarget == true) {
                if(target.Contains("Domino") && ball.GetComponent<Rigidbody>().velocity.magnitude < 1) {
                    ball.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
        public void updateSlope() {
            ball.active = true;
            Vector3 scale = slope.transform.localScale;
            scale.x = slopeLength;
            slope.transform.localScale = scale;

            Vector3 localPosition = new Vector3(-0.5f, ballOffset, 0.0f);
            Vector3 worldPosition = slope.transform.TransformPoint(localPosition);
            ball.transform.position = worldPosition;
        }

        public override void reset() {
            updateSlope();
            hitTarget = false;
            ball.active = true;
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = Vector3.zero;
        }
    }
}

