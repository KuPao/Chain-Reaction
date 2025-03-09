using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class SimpleForce : MonoBehaviour {
        public GameObject target;
        public float forceMagnitude;

        public void addForce(IForceReceiver forceReceiver) {
            if (forceReceiver == null)
                return;

            Vector3 force = forceReceiver.getDirection() * forceMagnitude;
            forceReceiver.getTarget().AddForceAtPosition(force, forceReceiver.getPosition());
        }
    }
}

