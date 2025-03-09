using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainReaction {
    public class UWire : ChainPrimitive {
        public float width;
        public GameObject partPrefab, boardPrefab, cupPrefab, platformPrefab, connectPrefab;
        public Transform leftEnd, leftJoint, rightJoint, rightEnd, parentObject, board, cup, leftConnectFront, leftConnectBack, rightConnectFront, rightConnectBack;
        public Vector3 cupInit = new Vector3(0, 0, 0), boardInit = new Vector3(0, 0, 0);
        public Rigidbody leftRigid, rightRigid;
        public float totalLength, wireWidth, boardDepth = 1.0f, cupDepth, partDistance = 0.21f;
        public bool boardFirst;
        public bool guideByLeft, animation = false;
        public bool hasDrop = false;

        public float time;
        private Vector3[] connectInit;

        // Start is called before the first frame update
        void Start() {
            this.hitTarget = true;
            //Init();

            //Cup cup = rightRigid.GetComponent<Cup>();
            //cup.triggerCallback = new Cup.TriggerCallback(Trigger);
        }

        void Update() {
            if (cup != null && board != null) {
                float cupDrop = (cup.position - cupInit).magnitude;

                if (cupDrop >= 0.5) {
                    hasDrop = true;
                }

                if(cup.GetComponent<Rigidbody>().isKinematic && cup.GetComponent<MeshCollider>().convex) {
                    MeshCollider mc = cup.GetComponent<MeshCollider>();
                    mc.convex = false;
                }

                if (!animation) {
                    if (board != null && board.hasChanged && cupDrop < 0.01) {
                        board.localPosition = new Vector3(0, 0, 0);
                        //Debug.Log(board.localPosition);
                        //Debug.Log(board.position);
                        board.hasChanged = false;
                    }


                    //cup.GetComponent<Rigidbody>().useGravity = false;
                    //cup.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    if (cupDrop - boardDepth >= 0.0) {
                        cup.position = cupInit - new Vector3(0.0f, boardDepth, 0.0f);
                        cup.GetComponent<Rigidbody>().useGravity = false;
                        cup.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        cup.GetComponent<Rigidbody>().isKinematic = true;
                        cup.GetComponent<MeshCollider>().convex = false;
                        //cup.GetComponent<Rigidbody>().isKinematic = fa;
                        board.position = boardInit + new Vector3(0.0f, boardDepth, 0.0f);
                    }
                    else {
                        board.position = boardInit + new Vector3(0.0f, cupDrop, 0.0f);
                    }
                    if (leftConnectBack != null) {
                        leftConnectFront.position = new Vector3(this.board.position.x, (this.board.position.y + leftJoint.position.y) * 0.5f, cup.transform.position.z - 0.6f);
                        leftConnectFront.localScale = new Vector3(0.05f, (leftJoint.position.y - this.board.position.y) * 0.5f, 0.05f);
                        leftConnectBack.position = new Vector3(this.board.position.x, (this.board.position.y + leftJoint.position.y) * 0.5f, cup.transform.position.z + 0.6f);
                        leftConnectBack.localScale = new Vector3(0.05f, (leftJoint.position.y - this.board.position.y) * 0.5f, 0.05f);
                        rightConnectFront.position = new Vector3(this.cup.position.x, (this.cup.position.y + rightJoint.position.y + 0.5f) * 0.5f, cup.transform.position.z - 0.6f);
                        rightConnectFront.localScale = new Vector3(0.05f, (rightJoint.position.y - this.cup.position.y - 0.5f) * 0.5f, 0.05f);
                        rightConnectBack.position = new Vector3(this.cup.position.x, (this.cup.position.y + rightJoint.position.y + 0.5f) * 0.5f, cup.transform.position.z + 0.6f);
                        rightConnectBack.localScale = new Vector3(0.05f, (rightJoint.position.y - this.cup.position.y - 0.5f) * 0.5f, 0.05f);
                    }
                }
            }
            else {
                if (board == null)
                    board = Instantiate(boardPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform).transform;
                if (cup == null)
                    cup = Instantiate(cupPrefab, new Vector3(transform.position.x - wireWidth * transform.localScale.x, transform.position.y + boardDepth - cupDepth - 0.7f, transform.position.z), Quaternion.identity, parentObject.transform).transform;
            }
        }

        //void FixedUpdate() {
        //    float guideLength;
        //    if (guideByLeft) {
        //        guideLength = (leftEnd.position - leftJoint.position).magnitude;
        //    }else {
        //        guideLength = (rightEnd.position - rightJoint.position).magnitude;
        //    }
        //    float remainLength = totalLength - guideLength;
        //    if (remainLength <= 0.0) {
        //        if (guideByLeft) {
        //            leftEnd.position = (leftEnd.position - leftJoint.position).normalized * totalLength + leftJoint.position;
        //        }else {
        //            rightEnd.position = (rightEnd.position - rightJoint.position).normalized * totalLength + rightJoint.position;
        //        }
        //        leftRigid.useGravity = false;
        //        leftRigid.velocity = Vector3.zero;
        //        rightRigid.useGravity = false;
        //        rightRigid.velocity = Vector3.zero;
        //    }
        //    else {
        //        if (guideByLeft) {
        //            rightEnd.position = (rightEnd.position - rightJoint.position).normalized * remainLength + rightJoint.position;
        //            rightRigid.velocity = Vector3.zero;
        //        }
        //        else {
        //            leftEnd.position = (leftEnd.position - leftJoint.position).normalized * remainLength + leftJoint.position;
        //            leftRigid.velocity = Vector3.zero;
        //        }
        //    }
        //}

        public void Init() {
            totalLength = (leftEnd.position - leftJoint.position).magnitude + (rightEnd.position - rightJoint.position).magnitude;
        }

        public void updateUWire() {
            if (boardFirst) {
                parentObject = this.transform.GetChild(0);
                foreach (Transform child in parentObject.transform) {
                    GameObject.Destroy(child.gameObject);
                }

                GameObject boardObject;
                boardObject = Instantiate(boardPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                boardObject.transform.parent = parentObject.transform;
                boardObject.transform.localPosition = Vector3.zero;
                //board.transform.localRotation = Quaternion.Euler(0,180, 0);
                //Debug.Log(boardObject.transform.localPosition);
                //Debug.Log(boardObject.transform.position);

                boardObject.name = "board" + parentObject.transform.childCount.ToString();
                boardObject.transform.hasChanged = false;
                Destroy(boardObject.GetComponent<CharacterJoint>());
                Destroy(boardObject.GetComponent<Rigidbody>());
                this.board = boardObject.transform;


                int boardDepthCount = (int)(boardDepth / partDistance);

                int widthCount = (int)(wireWidth / partDistance);

                int cupDepthCount = (int)(cupDepth / partDistance);

                GameObject cup;
                cup = Instantiate(cupPrefab, new Vector3(transform.position.x - wireWidth * transform.localScale.x, transform.position.y + boardDepth - cupDepth - 0.7f, transform.position.z), Quaternion.identity, parentObject.transform);
                //cup.transform.eulerAngles = new Vector3(180, 0, 180);
                cup.name = "cup" + parentObject.transform.childCount.ToString();

                cup.GetComponent<Rigidbody>().isKinematic = true;

                this.cup = cup.transform;
                //Debug.Log(this.cup.localPosition);
                //Debug.Log(this.cup.position);
                //transform.position += new Vector3(1* wireWidth * transform.localScale.x, 0, 0);
                boardInit = this.board.position;
                cupInit = this.cup.position;

                GameObject pulleyObject;
                pulleyObject = Instantiate(partPrefab, new Vector3(transform.position.x - 0.05f * transform.localScale.x, transform.position.y + boardDepth + 0.25f, transform.position.z), Quaternion.Euler(90, 0, 0));
                pulleyObject.name = "pulley" + parentObject.transform.childCount.ToString();
                pulleyObject.transform.parent = parentObject.transform;
                leftJoint = pulleyObject.transform;
                pulleyObject = Instantiate(partPrefab, new Vector3(cup.transform.position.x + 0.05f * transform.localScale.x, leftJoint.position.y, cup.transform.position.z), Quaternion.Euler(90, 0, 0));
                pulleyObject.name = "pulley" + parentObject.transform.childCount.ToString();
                pulleyObject.transform.parent = parentObject.transform;
                rightJoint = pulleyObject.transform;
                GameObject connect = Instantiate(connectPrefab, new Vector3((leftJoint.position.x + rightJoint.position.x) / 2, leftJoint.position.y + 0.05f, cup.transform.position.z + 0.6f), Quaternion.Euler(0, 0, 90));
                connect.transform.localScale = new Vector3(0.05f, (leftJoint.position - rightJoint.position).magnitude * 0.5f, 0.05f);
                connect.name = "connect" + parentObject.transform.childCount.ToString();
                connect.transform.parent = parentObject.transform;
                connect = Instantiate(connectPrefab, new Vector3((leftJoint.position.x + rightJoint.position.x) / 2, leftJoint.position.y + 0.05f, cup.transform.position.z - 0.6f), Quaternion.Euler(0, 0, 90));
                connect.transform.localScale = new Vector3(0.05f, (leftJoint.position - rightJoint.position).magnitude * 0.5f, 0.05f);
                connect.name = "connect" + parentObject.transform.childCount.ToString();
                connect.transform.parent = parentObject.transform;

                connect = Instantiate(connectPrefab, new Vector3(this.board.position.x, (this.board.position.y + leftJoint.position.y) * 0.5f, cup.transform.position.z - 0.6f), Quaternion.identity);
                connect.transform.localScale = new Vector3(0.05f, (leftJoint.position.y - this.board.position.y) * 0.5f, 0.05f);
                connect.name = "connect" + parentObject.transform.childCount.ToString();
                connect.transform.parent = parentObject.transform;
                leftConnectFront = connect.transform;
                connect = Instantiate(connectPrefab, new Vector3(this.board.position.x, (this.board.position.y + leftJoint.position.y) * 0.5f, cup.transform.position.z + 0.6f), Quaternion.identity);
                connect.transform.localScale = new Vector3(0.05f, (leftJoint.position.y - this.board.position.y) * 0.5f, 0.05f);
                connect.name = "connect" + parentObject.transform.childCount.ToString();
                connect.transform.parent = parentObject.transform;
                leftConnectBack = connect.transform;
                connect = Instantiate(connectPrefab, new Vector3(this.cup.position.x, (this.cup.position.y + rightJoint.position.y + 0.5f) * 0.5f, cup.transform.position.z - 0.6f), Quaternion.identity);
                connect.transform.localScale = new Vector3(0.05f, (rightJoint.position.y - this.cup.position.y - 0.5f) * 0.5f, 0.05f);
                connect.name = "connect" + parentObject.transform.childCount.ToString();
                connect.transform.parent = parentObject.transform;
                rightConnectFront = connect.transform;
                connect = Instantiate(connectPrefab, new Vector3(this.cup.position.x, (this.cup.position.y + rightJoint.position.y + 0.5f) * 0.5f, cup.transform.position.z + 0.6f), Quaternion.identity);
                connect.transform.localScale = new Vector3(0.05f, (rightJoint.position.y - this.cup.position.y - 0.5f) * 0.5f, 0.05f);
                connect.name = "connect" + parentObject.transform.childCount.ToString();
                connect.transform.parent = parentObject.transform;
                rightConnectBack = connect.transform;
                //Debug.Log(boardObject.transform.localPosition);
                //Debug.Log(boardObject.transform.position);
                //GameObject last;
                //GameObject board;
                //board = Instantiate(boardPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, parentObject.transform);
                //board.transform.eulerAngles = new Vector3(180, 0, 180);
                //board.name = "board" + parentObject.transform.childCount.ToString();
                //board.GetComponent<CharacterJoint>().anchor = new Vector3(0, 0.7f, 0);
                //board.GetComponent<CharacterJoint>().autoConfigureConnectedAnchor = false;
                //board.GetComponent<CharacterJoint>().connectedAnchor = new Vector3(0, 0, 0);
                //last = board;

                //int boardDepthCount = (int)(boardDepth / partDistance);
                //for (int i = 0; i < boardDepthCount; i++) {
                //    GameObject tmp;

                //    tmp = Instantiate(partPrefab, new Vector3(transform.position.x, transform.position.y + partDistance * (i), transform.position.z), Quaternion.identity, parentObject.transform);
                //    tmp.transform.eulerAngles = new Vector3(180, 0, 180);

                //    tmp.name = parentObject.transform.childCount.ToString();

                //    last.GetComponent<CharacterJoint>().connectedBody = tmp.GetComponent<Rigidbody>();
                //    last = tmp;
                //}

                //int widthCount = (int)(wireWidth / partDistance);
                //for (int i = 0; i < widthCount; i++) {
                //    GameObject tmp;

                //    tmp = Instantiate(partPrefab, new Vector3(transform.position.x - partDistance * (i + 0.5f), transform.position.y + partDistance * (boardDepthCount - 0.5f), transform.position.z), Quaternion.identity, parentObject.transform);
                //    tmp.transform.eulerAngles = new Vector3(180, 0, 90);

                //    tmp.name = parentObject.transform.childCount.ToString();

                //    last.GetComponent<CharacterJoint>().connectedBody = tmp.GetComponent<Rigidbody>();
                //    last = tmp;
                //}

                //int cupDepthCount = (int)(cupDepth / partDistance);
                //for (int i = 0; i < cupDepthCount; i++) {
                //    GameObject tmp;

                //    tmp = Instantiate(partPrefab, new Vector3(transform.position.x - partDistance * (widthCount), transform.position.y + partDistance * (boardDepthCount - i - 1), transform.position.z), Quaternion.identity, parentObject.transform);
                //    tmp.transform.eulerAngles = new Vector3(180, 0, 0);

                //    tmp.name = parentObject.transform.childCount.ToString();

                //    last.GetComponent<CharacterJoint>().connectedBody = tmp.GetComponent<Rigidbody>();
                //    last = tmp;
                //}

                //GameObject cup;
                //cup = Instantiate(cupPrefab, new Vector3(transform.position.x - partDistance * (widthCount), transform.position.y + partDistance * (boardDepthCount - cupDepthCount - 1) - 0.7f, transform.position.z), Quaternion.identity, parentObject.transform);
                //cup.transform.eulerAngles = new Vector3(180, 0, 180);
                //cup.name = "cup" + parentObject.transform.childCount.ToString();

                //cup.GetComponent<Rigidbody>().isKinematic = false;

                //last.GetComponent<CharacterJoint>().connectedBody = cup.GetComponent<Rigidbody>();

                //GameObject platform = Instantiate(platformPrefab, new Vector3(transform.position.x - partDistance * (widthCount + 1) / 2, transform.position.y + partDistance * (boardDepthCount - 1) - 0.1f, transform.position.z), Quaternion.identity, parentObject.transform);
                //platform.transform.localScale = new Vector3(partDistance * (widthCount - 1), 0.2f, 2);
            }
            else {
                updateUWireForward();
            }
            hasDrop = false;
        }

        public void updateUWireForward() {
            parentObject = this.transform.GetChild(0);
            foreach (Transform child in parentObject.transform) {
                GameObject.Destroy(child.gameObject);
            }

            GameObject cupObject;
            cupObject = Instantiate(cupPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            cupObject.transform.parent = parentObject.transform;
            cupObject.transform.localPosition = Vector3.zero;
            //board.transform.localRotation = Quaternion.Euler(0,180, 0);
            //Debug.Log(boardObject.transform.localPosition);
            //Debug.Log(boardObject.transform.position);

            cupObject.name = "cup" + parentObject.transform.childCount.ToString();
            cupObject.GetComponent<Rigidbody>().isKinematic = true;
            this.cup = cupObject.transform;



            GameObject board;
            board = Instantiate(boardPrefab, new Vector3(transform.position.x - wireWidth * transform.localScale.x, transform.position.y + cupDepth - boardDepth + 0.7f, transform.position.z), Quaternion.identity, parentObject.transform);
            //cup.transform.eulerAngles = new Vector3(180, 0, 180);
            board.name = "board" + parentObject.transform.childCount.ToString();

            board.transform.hasChanged = false;
            Destroy(board.GetComponent<CharacterJoint>());
            Destroy(board.GetComponent<Rigidbody>());

            this.board = board.transform;
            //Debug.Log(this.cup.localPosition);
            //Debug.Log(this.cup.position);
            //transform.position += new Vector3(1* wireWidth * transform.localScale.x, 0, 0);
            boardInit = this.board.position;
            cupInit = this.cup.position;

            GameObject pulleyObject;
            pulleyObject = Instantiate(partPrefab, new Vector3(board.transform.position.x - 0.05f * transform.localScale.x, board.transform.position.y + boardDepth + 0.25f, board.transform.position.z), Quaternion.Euler(90, 0, 0));
            pulleyObject.name = "pulley" + parentObject.transform.childCount.ToString();
            pulleyObject.transform.parent = parentObject.transform;
            leftJoint = pulleyObject.transform;
            pulleyObject = Instantiate(partPrefab, new Vector3(cup.transform.position.x + 0.05f * transform.localScale.x, leftJoint.position.y, cup.transform.position.z), Quaternion.Euler(90, 0, 0));
            pulleyObject.name = "pulley" + parentObject.transform.childCount.ToString();
            pulleyObject.transform.parent = parentObject.transform;
            rightJoint = pulleyObject.transform;
            GameObject connect = Instantiate(connectPrefab, new Vector3((leftJoint.position.x + rightJoint.position.x) / 2, leftJoint.position.y + 0.05f, cup.transform.position.z + 0.6f), Quaternion.Euler(0, 0, 90));
            connect.transform.localScale = new Vector3(0.05f, (leftJoint.position - rightJoint.position).magnitude * 0.5f, 0.05f);
            connect.name = "connect" + parentObject.transform.childCount.ToString();
            connect.transform.parent = parentObject.transform;
            connect = Instantiate(connectPrefab, new Vector3((leftJoint.position.x + rightJoint.position.x) / 2, leftJoint.position.y + 0.05f, cup.transform.position.z - 0.6f), Quaternion.Euler(0, 0, 90));
            connect.transform.localScale = new Vector3(0.05f, (leftJoint.position - rightJoint.position).magnitude * 0.5f, 0.05f);
            connect.name = "connect" + parentObject.transform.childCount.ToString();
            connect.transform.parent = parentObject.transform;

            connect = Instantiate(connectPrefab, new Vector3(this.board.position.x, (this.board.position.y + leftJoint.position.y) * 0.5f, cup.transform.position.z - 0.6f), Quaternion.identity);
            connect.transform.localScale = new Vector3(0.05f, (leftJoint.position.y - this.board.position.y) * 0.5f, 0.05f);
            connect.name = "connect" + parentObject.transform.childCount.ToString();
            connect.transform.parent = parentObject.transform;
            leftConnectFront = connect.transform;
            connect = Instantiate(connectPrefab, new Vector3(this.board.position.x, (this.board.position.y + leftJoint.position.y) * 0.5f, cup.transform.position.z + 0.6f), Quaternion.identity);
            connect.transform.localScale = new Vector3(0.05f, (leftJoint.position.y - this.board.position.y) * 0.5f, 0.05f);
            connect.name = "connect" + parentObject.transform.childCount.ToString();
            connect.transform.parent = parentObject.transform;
            leftConnectBack = connect.transform;
            connect = Instantiate(connectPrefab, new Vector3(this.cup.position.x, (this.cup.position.y + rightJoint.position.y + 0.5f) * 0.5f, cup.transform.position.z - 0.6f), Quaternion.identity);
            connect.transform.localScale = new Vector3(0.05f, (rightJoint.position.y - this.cup.position.y - 0.5f) * 0.5f, 0.05f);
            connect.name = "connect" + parentObject.transform.childCount.ToString();
            connect.transform.parent = parentObject.transform;
            rightConnectFront = connect.transform;
            connect = Instantiate(connectPrefab, new Vector3(this.cup.position.x, (this.cup.position.y + rightJoint.position.y + 0.5f) * 0.5f, cup.transform.position.z + 0.6f), Quaternion.identity);
            connect.transform.localScale = new Vector3(0.05f, (rightJoint.position.y - this.cup.position.y - 0.5f) * 0.5f, 0.05f);
            connect.name = "connect" + parentObject.transform.childCount.ToString();
            connect.transform.parent = parentObject.transform;
            rightConnectBack = connect.transform;
            cupObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        public override void reset() {
            //updateUWire();
            hitTarget = true;
            //cupPrefab.active = true;
            //Rigidbody rb = cupPrefab.GetComponent<Rigidbody>();
            //if (rb != null)
            //    rb.velocity = Vector3.zero;
        }

        //public void SetGuide(bool byLeft) {
        //    guideByLeft = byLeft;
        //    leftRigid.useGravity = guideByLeft;
        //    rightRigid.useGravity = !guideByLeft;
        //}

        //public void Trigger() {
        //    SetGuide(false);
        //}
    }
}

