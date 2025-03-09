using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChainReaction;

public class Checkpoint : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        ChainPrimitive primitive;

        primitive = transform.parent.GetComponent<BallOnSlope>();
        if (primitive != null) {
            switch (primitive.targetSymbol) {
                case 'D':
                    if (collision.transform.parent.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
                case 'P':
                    if (collision.transform.parent.parent.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
                case 'C':
                    if (collision.gameObject.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
                case 'L':
                    if (collision.transform.parent.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
            }
        }

        primitive = transform.parent.GetComponent<Dominos>();
        if (primitive != null) {
            switch (primitive.targetSymbol) {
                case 'P':
                    if (collision.transform.parent.parent.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
                case 'D':
                    if (collision.transform.parent.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
                case 'C':
                    if (collision.gameObject.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
            }
        }

        primitive = transform.parent.parent.GetComponent<UWire>();
        if (primitive != null) {
            switch (primitive.targetSymbol) {
                case 'B':
                    if (collision.transform.parent.name.Equals(primitive.target) == true) {
                        primitive.hitTarget = true;
                    }
                    break;
            }
        }

        primitive = transform.parent.GetComponent<Billiard>();
        if (primitive != null) {
            switch (primitive.targetSymbol) {
                case 'P':
                    if (primitive.targets.Contains(collision.transform.parent.parent.name)) {
                        int index = primitive.targets.FindIndex(a => a.Contains(collision.transform.parent.parent.name));
                        //Debug.Log(index);
                        if(index >= 0) {
                            primitive.hitTargets[index] = true;
                            Billiard b = (Billiard)primitive;
                            switch (index) {
                                case 0:
                                    b.hitTarget0 = true;
                                    break;
                                case 1:
                                    b.hitTarget1 = true;
                                    break;
                                case 2:
                                    b.hitTarget2 = true;
                                    break;
                            }
                        }
                        if (!primitive.hitTargets.Contains(false)) {
                            primitive.hitTarget = true;
                        }
                    }
                    else if (collision.transform.name.Contains("bottom")) {
                        if (primitive.targets.Contains(collision.transform.parent.parent.parent.name)) {
                            int index = primitive.targets.FindIndex(a => a.Contains(collision.transform.parent.parent.parent.name));
                            //Debug.Log(index);
                            if (index >= 0) {
                                primitive.hitTargets[index] = true;
                                Billiard b = (Billiard)primitive;
                                switch (index) {
                                    case 0:
                                        b.hitTarget0 = true;
                                        break;
                                    case 1:
                                        b.hitTarget1 = true;
                                        break;
                                    case 2:
                                        b.hitTarget2 = true;
                                        break;
                                }
                            }
                            if (!primitive.hitTargets.Contains(false)) {
                                primitive.hitTarget = true;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
