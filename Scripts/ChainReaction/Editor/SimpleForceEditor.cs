using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChainReaction {
    [CustomEditor(typeof(SimpleForce))]
    public class SimpleForceEditor : Editor {
        
        public override void OnInspectorGUI() {
            SimpleForce simpleForce = (SimpleForce)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Go!")) {
                simpleForce.target.GetComponent<Dominos>().genDominos();
                simpleForce.addForce(getReceiver());
            }
        }

        public IForceReceiver getReceiver() {
            SimpleForce simpleForce = (SimpleForce)target;
            return (IForceReceiver)simpleForce.target.GetComponent<Dominos>();
        }
    }

}

