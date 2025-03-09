using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChainReaction {
    [CustomEditor(typeof(BallOnSlope))]
    public class BallOnSlopeEditor : Editor {
        public override void OnInspectorGUI() {
            BallOnSlope ballOnSlope = (BallOnSlope)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Update")) {
                ballOnSlope.updateSlope();
            }
        }
    }
}

