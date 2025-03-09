using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChainReaction {
    [CustomEditor(typeof(Dominos))]
        public class DominosEditor : Editor {

        public override void OnInspectorGUI() {
            Dominos dominos = (Dominos)target;

            DrawDefaultInspector();

            if (GUILayout.Button("GenDominos")) {
                dominos.genDominos();
            }

            if (GUILayout.Button("ClearDominos")) {
                dominos.clearDominos();
            }
        }
    }
}

