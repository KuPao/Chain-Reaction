using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChainReaction {
    [CustomEditor(typeof(UWire))]
    public class UWireEditor : Editor {
        public override void OnInspectorGUI() {
            UWire uWire = (UWire)target;
            
            uWire.width = EditorGUILayout.FloatField("width", uWire.width);

            if (GUILayout.Button("Update")) {
                uWire.updateUWire();
            }

            DrawDefaultInspector();
        }
    }
}
