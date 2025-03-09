using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class Billiard : ChainPrimitive {
        public GameObject ball;
        public GameObject slope;
        public List<BilliardBall> balls;

        public float ballOffset;
        public float slopeLength;
        public List<Vector3> outPoints;
        public float time;
        public float maxVelocity = 0;
        public bool hitTarget0 = false;
        public bool hitTarget1 = false;
        public bool hitTarget2 = false;

        public void clearBalls() {
            foreach (BilliardBall b in balls) {
                GameObject.DestroyImmediate(b.gameObject);
            }
            balls.Clear();
        }
        public void updateSlope() {
            clearBalls();
            ball.active = true;
            Vector3 scale = slope.transform.localScale;
            scale.x = slopeLength;
            slope.transform.localScale = scale;

            Vector3 localPosition = new Vector3(0.0f, ballOffset, 0.0f);
            Vector3 worldPosition = slope.transform.TransformPoint(localPosition);
            ball.transform.position = worldPosition;
            BilliardBall ballComponent = ball.GetComponent<BilliardBall>();
            ballComponent.direction = new Vector2(transform.localScale.x, 0);
            for (int i = 0; i < outPoints.Count; i++) {
                int zOffset = i % 2 == 1 ? 1 : -1;
                GameObject newBall = Instantiate(ball, new Vector3(worldPosition.x - 0.075f*2 * transform.localScale.x, worldPosition.y, worldPosition.z + 0.075f/3*2*zOffset), Quaternion.identity, this.transform);
                Rigidbody rb = newBall.GetComponent<Rigidbody>();
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                newBall.name = "ball" + this.transform.childCount.ToString();
                BilliardBall b = newBall.GetComponent<BilliardBall>();
                b.direction = new Vector2(transform.localScale.x * 0.05f * Mathf.Sqrt(8), zOffset * 0.05f);
                b.direction = b.direction.normalized;
                balls.Add(b);
            }
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
