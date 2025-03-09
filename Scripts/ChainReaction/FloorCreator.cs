using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class FloorCreator : MonoBehaviour {
        public GameObject floorPrefab;
        public float floorExtend;
        public float floorHeightOffset;

        public List<ChainPrimitive> primitives;

        public GameObject floorRoot;
        public List<GameObject> floors;

        private void ensureFloorRoot() {
            string rootName = "FloorRoot";
            if (floorRoot == null) {
                Transform child = transform.Find(rootName);
                if (child == null) {
                    floorRoot = new GameObject(rootName);
                    floorRoot.transform.parent = transform;
                }
                else {
                    floorRoot = child.gameObject;
                }
            }
        }

        public void clearFloors() {
            foreach (GameObject floor in floors) {
                GameObject.DestroyImmediate(floor);
            }
            floors.Clear();
        }

        public void genFloors() {
            clearFloors();

            ensureFloorRoot();

            foreach (ChainPrimitive primitive in primitives) {
                GameObject floor = primitive.genFloor(this);
                if (floor == null)
                    continue;

                floor.transform.parent = floorRoot.transform;
                floors.Add(floor);
            }
        }
    }
}
