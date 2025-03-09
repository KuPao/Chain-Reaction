using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class FollowCamera : MonoBehaviour {
        //public List<ChainPrimitive> primitives;
        public ChainPrimitive current;
        public int currentId;
        public Vector3 target;
        public float speed = 0.125f;
        // Start is called before the first frame update

        // Update is called once per frame
        void Update() {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, target, speed);
            transform.position = smoothedPosition;
        }
    }
}
