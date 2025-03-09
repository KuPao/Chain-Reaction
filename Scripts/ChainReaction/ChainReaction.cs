using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class ChainReaction : MonoBehaviour {
        public GameObject dominosPrefab;
        public GameObject ballPrefab;
        public GameObject uWirePrefab;
        public GameObject cupPrefab;
        public GameObject billiardPrefab;
        public GameObject emptyPrefab;
        public FloorCreator floorCreator;

        public GameObject primitiveRoot;
        public List<ChainPrimitive> primitives;

        private void ensurePrimitiveRoot() {
            string rootName = "PrimitiveRoot";
            if (primitiveRoot == null) {
                Transform child = transform.Find(rootName);
                if (child == null) {
                    primitiveRoot = new GameObject(rootName);
                    primitiveRoot.transform.parent = transform;
                }else {
                    primitiveRoot = child.gameObject;
                }
            }
        }

        public void addDominios() {
            ensurePrimitiveRoot();
            GameObject newObj = GameObject.Instantiate(dominosPrefab, primitiveRoot.transform);
            Dominos dominos = newObj.GetComponent<Dominos>();
            primitives.Add(dominos);
        }

        public void addBall() {
            ensurePrimitiveRoot();
            GameObject newObj = GameObject.Instantiate(ballPrefab, primitiveRoot.transform);
            BallOnSlope ball = newObj.GetComponent<BallOnSlope>();
            primitives.Add(ball);
        }

        public void addUWire() {
            ensurePrimitiveRoot();
            GameObject newObj = GameObject.Instantiate(uWirePrefab, primitiveRoot.transform);
            UWire uWire = newObj.GetComponent<UWire>();
            primitives.Add(uWire);
        }

        public void addBilliard() {
            ensurePrimitiveRoot();
            GameObject newObj = GameObject.Instantiate(billiardPrefab, primitiveRoot.transform);
            Billiard billiard = newObj.GetComponent<Billiard>();
            primitives.Add(billiard);
        }

        public void addCup() {
            ensurePrimitiveRoot();
            GameObject newObj = GameObject.Instantiate(cupPrefab, primitiveRoot.transform);
            Cup cup = newObj.GetComponent<Cup>();
            primitives.Add(cup);
        }
        public void addEmpty() {
            ensurePrimitiveRoot();
            GameObject newObj = GameObject.Instantiate(emptyPrefab, primitiveRoot.transform);
            Empty empty = newObj.GetComponent<Empty>();
            primitives.Add(empty);
        }

        public void clearScene() {
            for (var i = primitives.Count - 1; i > -1; i--) {
                if (primitives[i] == null)
                    primitives.RemoveAt(i);
            }
            foreach (ChainPrimitive primitive in primitives) {
                GameObject.DestroyImmediate(primitive.gameObject);
            }
            primitives.Clear();
            int childs = primitiveRoot.transform.childCount;
            for (int i = childs - 1; i > -1; i--) {
                GameObject.Destroy(primitiveRoot.transform.GetChild(i).gameObject);
            }
            primitives = new List<ChainPrimitive>();

            floorCreator.clearFloors();
        }

        public void genFloors() {
            floorCreator.primitives = primitives;
            floorCreator.genFloors();
        }
    }
}

