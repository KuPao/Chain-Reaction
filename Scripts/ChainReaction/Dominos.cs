using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class Dominos : ChainPrimitive, IForceReceiver {

        public GameObject dominoUnit;
        public int dominoN;
        public float gap;
        public float time;
        public int type;
        
        public List<DominoUnit> dominos;

        public void clearDominos() {
            foreach (DominoUnit domino in dominos) {
                GameObject.DestroyImmediate(domino.gameObject);
            }
            dominos.Clear();
        }

        public void genDominos() {
            clearDominos();

            float height_offset = 0.05f;
            
            for (int i = 0; i < dominoN; i++) {
                Vector3 pos = transform.position + transform.forward * gap * i;
                if (type == 1) {
                    pos += transform.up * height_offset * i;
                }
                if (type == 2) {
                    pos -= transform.up * height_offset * i;
                }
                GameObject domino = GameObject.Instantiate(dominoUnit, transform);
                domino.name = "domino" + i.ToString();
                domino.transform.position = pos;
                domino.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f); // to make dominos facing +x direction
                dominos.Add(domino.GetComponent<DominoUnit>());
            }
        }

        public override void reset() {
            genDominos();
            hitTarget = false;
        }

        public override GameObject genFloor(FloorCreator floorCreator) {
            GameObject floor = GameObject.Instantiate(floorCreator.floorPrefab);
            
            Vector3 scale = floor.transform.localScale;
            scale.x = (dominoN - 1) * gap + floorCreator.floorExtend;
            floor.transform.localScale = scale;

            Vector3 center = Vector3.zero;
            foreach (DominoUnit domino in dominos) {
                center += (domino.bottom.position + Vector3.up * -floorCreator.floorHeightOffset); // offset a little
            }
            
            center /= dominoN;
            if (type == 1) {
                center.y = dominos[0].bottom.position.y - floorCreator.floorHeightOffset;
            }
            if (type == 2) {
                center.y = dominos[dominoN - 1].bottom.position.y - floorCreator.floorHeightOffset;
            }

            floor.transform.position = center;

            if (type == 1) {
                for (int i = 1; i < dominoN; i++) {
                    GameObject sub_floor = GameObject.Instantiate(floorCreator.floorPrefab);

                    scale = sub_floor.transform.localScale;
                    scale.x = (dominoN - 1 - i) * gap + floorCreator.floorExtend;
                    sub_floor.transform.localScale = scale;

                    center = Vector3.zero;
                    for (int j = i; j < dominoN; j++) {
                        DominoUnit domino = dominos[j];
                        center += (domino.bottom.position + Vector3.up * -floorCreator.floorHeightOffset); // offset a little
                    }
                    
                    center /= (dominoN - i);
                    center.y = dominos[i].bottom.position.y - floorCreator.floorHeightOffset;

                    sub_floor.transform.position = center;
                    sub_floor.transform.parent = floor.transform;
                }
            }
            if (type == 2) {
                for (int i = dominoN - 2; i >= 0; i--) {
                    GameObject sub_floor = GameObject.Instantiate(floorCreator.floorPrefab);

                    scale = sub_floor.transform.localScale;
                    scale.x = (i) * gap + floorCreator.floorExtend;
                    sub_floor.transform.localScale = scale;

                    center = Vector3.zero;
                    for (int j = i; j >= 0; j--) {
                        DominoUnit domino = dominos[j];
                        center += (domino.bottom.position + Vector3.up * -floorCreator.floorHeightOffset); // offset a little
                    }

                    center /= i + 1;
                    center.y = dominos[i].bottom.position.y - floorCreator.floorHeightOffset;

                    sub_floor.transform.position = center;
                    sub_floor.transform.parent = floor.transform;
                }
            }
            return floor;
        }
        
        public Rigidbody getTarget() {
            return dominos[0].GetComponent<Rigidbody>();
        }
        public Vector3 getPosition() {
            return dominos[0].transform.position; // tmp position
        }
        public Vector3 getDirection() {
            return Vector3.right; // tmp direction
        }
    }
}

