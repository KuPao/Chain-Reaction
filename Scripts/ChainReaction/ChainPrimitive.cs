using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class ChainPrimitive : MonoBehaviour {
        public string target;
        public char targetSymbol;
        public List<string> targets;
        public List<bool> hitTargets;
        public bool hitTarget;
        public virtual void reset() {
            hitTarget = false;
        }
        public virtual GameObject genFloor(FloorCreator floorCreator) {
            return null;
        }
        void OnCollisionEnter(Collision collision) {
        }
    }
}

