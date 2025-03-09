using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChainReaction {
    [CustomEditor(typeof(ChainReaction))]
    public class ChainReactionEditor : Editor {

        public override void OnInspectorGUI() {
            ChainReaction chainReaction = (ChainReaction)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Add Dominos")) {
                chainReaction.addDominios();
            }

            if (GUILayout.Button("Add Ball")) {
                chainReaction.addBall();
            }

            if (GUILayout.Button("Add U-Wire")) {
                chainReaction.addUWire();
            }

            if (GUILayout.Button("Add Cup")) {
                chainReaction.addCup();
            }

            if (GUILayout.Button("Gen Floors")) {
                chainReaction.genFloors();
            }

            if (GUILayout.Button("Clear Scene")) {
                chainReaction.clearScene();
            }
        }
    }
}

