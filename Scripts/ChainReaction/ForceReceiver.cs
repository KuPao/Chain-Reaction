using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public interface IForceReceiver {
        Rigidbody getTarget();
        Vector3 getPosition();
        Vector3 getDirection();
    }
}


