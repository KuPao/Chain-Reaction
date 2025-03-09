using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

namespace LGS
{
    public class LGSCollisionData
    {
        public enum State { MOVING, IDLE};
        public enum ColType { BRICK, CUSHION, NONE };
        public enum ElementType { B, D, P, C, L, ROOT };
        
        public LGSCollisionData list_prev;
        public LGSCollisionData list_next;

        public LGSCollisionData targ;
        public LGSCollisionData next;
        public LGSCollisionData prev;

        public List<LGSCollisionData> childern;

        public ColType colType;
        public ElementType elementType;
        public char symbol;

        public int ballID;
        public int id;

        public bool alreadyConcern;

        public bool sameSide;
        public bool onTail;
        public bool onKick;

        public ChainReaction.ChainPrimitive primitive;
        // GeomParams
        public Vector3 position;
        public bool isEnd = false;
        public int dir;
        public float ballOffset;
        public float slopeLength;
        public int dominoN;
        public float dominoGap;
        public int dominoType;
        public float wireWidth;
        public float cupDepth;
        public float boardDepth;
        public bool boardFirst;
        public string name;
        public List<Vector3> outPoints;

        public int n = 0;
        public float w = 0.0f;

        // Counter
        public int branchCount = 0;

        // Generate Direction
        public bool backward = true;

        //public bool moving
        //{
        //    get
        //    {
        //        return prev != null;
        //    }
        //}

        public int layer = 0;

        public int horri = 0;
        //撞到磚塊反射角度
        public float ca = 90;
        //ca = ta or -ta
        public float ta = 90;

        //入射角等於反射角
        public float r = 1.0f;

        public float length;
        public float difficulty;

        public Vector3 point1;
        public Vector3 point2;

        public float minAngle = -5000000.0f;
        public float maxAngle = 1000000.0f;

        //更改成固定參數去隨機
        //public int brickDis = GameObject.Find("LGS").GetComponent<LevelGeneratorSystem>().brickDis;
        public int brickDis = 5;

        public LGSCollisionData(char c, int ballID)
        {
            this.ballID = ballID;

            switch (c)
            {
                case 'B':
                    this.colType = LGSCollisionData.ColType.NONE;
                    this.elementType = LGSCollisionData.ElementType.B;
                    break;
                case 'D':
                    this.colType = LGSCollisionData.ColType.NONE;
                    this.elementType = LGSCollisionData.ElementType.D;
                    break;
                case 'P':
                    this.colType = LGSCollisionData.ColType.NONE;
                    this.elementType = LGSCollisionData.ElementType.P;
                    break; 
                case 'C':
                    this.colType = LGSCollisionData.ColType.NONE;
                    this.elementType = LGSCollisionData.ElementType.C;
                    break;
                case 'L':
                    this.colType = LGSCollisionData.ColType.NONE;
                    this.elementType = LGSCollisionData.ElementType.L;
                    break;
            }

            if ( c == 'H' || c == 'c')
                onKick = true;
        }

        public LGSCollisionData(LGSCollisionData collision)
        {
            if (collision != null)
            {
                this.colType = collision.colType;
                this.ballID = collision.ballID;
                this.layer = collision.layer;
                this.horri = collision.horri;
                this.elementType = collision.elementType;
                this.ta = collision.ta;
                this.ca = collision.ca;
                this.position = collision.position;
                this.brickDis = collision.brickDis;
            }
        }

        public void Init(List<GeoParam.IGeoParam> geoParams)
        {
            switch (elementType)
            {
                case ElementType.B:
                    for (int i = 0; i < geoParams.Count; i++) {
                        GeoParam.IGeoParam geoParam = geoParams[i];
                        if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                            GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                            position = posParam.pos;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                            GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                            dir = dirParam.dir;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                            GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                            ballOffset = bParam.offset;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                            GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                            slopeLength = bParam.length;
                        }
                    }
                    break;
                case ElementType.D:
                    for (int i = 0; i < geoParams.Count; i++) {
                        GeoParam.IGeoParam geoParam = geoParams[i];
                        if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                            GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                            position = posParam.pos;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                            GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                            dir = dirParam.dir;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_D_N) {
                            GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParam;
                            dominoN = dParam.n;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_D_GAP) {
                            GeoParam.CR_D_GAP dParam = (GeoParam.CR_D_GAP)geoParam;
                            dominoGap = dParam.gap;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_D_TYPE) {
                            GeoParam.CR_D_TYPE dParam = (GeoParam.CR_D_TYPE)geoParam;
                            dominoType = dParam.type;
                        }
                    }
                    break;
                case ElementType.P:
                    for (int i = 0; i < geoParams.Count; i++) {
                        GeoParam.IGeoParam geoParam = geoParams[i];
                        if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                            GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                            position = posParam.pos;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                            GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                            dir = dirParam.dir;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_P_W) {
                            GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParam;
                            wireWidth = pParam.width;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_P_CD) {
                            GeoParam.CR_P_CupDepth pParam = (GeoParam.CR_P_CupDepth)geoParam;
                            cupDepth = pParam.cupDepth;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_P_BD) {
                            GeoParam.CR_P_BoardtDepth pParam = (GeoParam.CR_P_BoardtDepth)geoParam;
                            boardDepth = pParam.boardDepth;
                        }
                    }
                    break;
                case ElementType.L:
                    for (int i = 0; i < geoParams.Count; i++) {
                        GeoParam.IGeoParam geoParam = geoParams[i];
                        if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                            GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                            position = posParam.pos;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                            GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                            dir = dirParam.dir;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                            GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                            ballOffset = bParam.offset;
                        }
                        else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                            GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                            slopeLength = bParam.length;
                        }
                    }
                    break;
                case ElementType.C:
                    for (int i = 0; i < geoParams.Count; i++) {
                        GeoParam.IGeoParam geoParam = geoParams[i];
                        if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                            GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                            position = posParam.pos;
                        }
                    }
                    isEnd = true;
                    break;
            }
        }

        //public void AssignElement()
        //{
        //    switch (colType)
        //    {
        //        case LGSCollisionData.ColType.BRICK:
        //            elementType = ElementType.BRICK;
        //            break;
        //        case LGSCollisionData.ColType.CUSHION:
        //            elementType = ElementType.CUSHION;
        //            break;
        //        case LGSCollisionData.ColType.NONE:
        //            if (moving == true)
        //                elementType = ElementType.CB_STOP;
        //            else
        //                elementType = ElementType.CB_START;
        //            break;
        //    }
        //}


        public bool Update()
        {
            bool success = true;

        if (elementType == ElementType.C)
            {
                //LGSCollisionData temp = prev;
                ////儲存現在點之後的磚塊
                //List<LGSCollisionData> brickSet = new List<LGSCollisionData>();
                //while (temp.elementType != ElementType.CB_START)
                //{
                //    if (temp.elementType == ElementType.BRICK)
                //    {
                //        brickSet.Add(temp);
                //    }
                //    else if (temp.elementType == ElementType.CUSHION)
                //    {
                //        LGSCollisionData pre_temp = temp.prev;

                //        for (int j = 0; j < brickSet.Count(); j++)
                //        {
                //            Vector3 brickHorn1 = new Vector3( (-1) *((temp.position.x - pre_temp.position.x) / (temp.position.y - pre_temp.position.y) * (temp.position.y - (brickSet[j].position.y - 0.5f)) - temp.position.x), (brickSet[j].position.y - 0.5f), 0.0f);
                //            Vector3 brickHorn2 = new Vector3((-1) * ((temp.position.x - pre_temp.position.x) / (temp.position.y - pre_temp.position.y) * (temp.position.y - (brickSet[j].position.y + 0.5f)) - temp.position.x), (brickSet[j].position.y + 0.5f), 0.0f);
                //            if (inBrick(brickHorn1, brickSet[j].position) == false)
                //            {
                //                return false;
                //            }
                //            if (inBrick(brickHorn2, brickSet[j].position) == false)
                //            {
                //                return false;
                //            }
                //        }

                //        Vector3 dir = new Vector3(temp.position.x - pre_temp.position.x, temp.position.y - pre_temp.position.y, 0.0f);

                //        for (int i = 1; i <= 10; i++)
                //        {
                //            Vector3 test = new Vector3(pre_temp.position.x + (i * dir.x / 10), pre_temp.position.y + (i * dir.y / 10), 0.0f);

                //            for (int j = 0; j < brickSet.Count(); j++)
                //            {
                //                if (inBrick(test, brickSet[j].position) == false)
                //                {
                //                    return false;
                //                }
                //            } 
                //        }
                //    }
                //    temp = temp.prev;
                //}
            }

            return success;
        }

        public bool inBrick(Vector3 point, Vector3 brickPos)
        {
            //範圍加上球的 再加上 因為磚塊會放在撞擊點上面下面0.5f
            if (point.x >= brickPos.x - 2f && point.x <= brickPos.x + 2f && point.y >= brickPos.y - 1.5f && point.y <= brickPos.y + 1.5f)
            {
                //在磚塊裡面  回傳false
                return false;
            }
            return true;
        }

        public bool brickInBrick(Vector3 brickPos1, Vector3 brickPos2)
        {
            Vector3 test = new Vector3(brickPos1.x + 1.0f, brickPos1.y + 0.5f, 0.0f);
            if (inBrick(test, brickPos2) == false)
            {
                return false;
            }

            test = new Vector3(brickPos1.x - 1.0f, brickPos1.y - 0.5f, 0.0f);
            if (inBrick(test, brickPos2) == false)
            {
                return false;
            }
            return true;
        }

        public Vector3 gc
        {
            get
            {
                return I.position - G.position;
            }
        }

        //白球終點
        public LGSCollisionData S
        {
            get
            {
                return next;
            }
        }
        //白球起點
        public LGSCollisionData I
        {
            get
            {
                return prev;
            }
        }
        //子球起點  (暫不列入 因為等於brick)
        
        public LGSCollisionData G
        {
            get
            {
                return this;
            }
        }

        public bool CushionCheck()
        {

            //判斷是否在範圍內
            if (position.x > 21.0f || position.x < -21.0f || position.y > 24.0f || position.y < -20.0f)
            {
                //Debug.Log("失敗的點為:"+ position);
                return false;
            }

            //判斷白求起始點到第一個點是否是垂直線 是就不行
            if (I.position.x == position.x || I.position.y == position.y)
            {
                return false;
            }
            

            if (next.colType == ColType.CUSHION) // next is cushion
            {
                //if (next.cushion == cushion) // could not be the same side as current cushion
                //{
                //    return false;
                //}
            }

            return true;
        }

        //public bool BrickCheck()
        //{

        //    //if ((I.position - position).magnitude < LGSUtility.BALL_DIAMETER * brickDis)
        //    //{
        //    //    return false;
        //    //}

        //    //磚塊不能和牆壁重疊 判斷是否在範圍內
        //    if (position.x >= 19.0f || position.x <= -19.0f || position.y >= 22.0f || position.y <= -17.0f)
        //    {
        //        return false;
        //    }


        //    //判斷白求起始點到第一個點是否是垂直線 是就不行
        //    if (I.position.x == position.x || I.position.y == position.y)
        //    {
        //        return false;
        //    }

        //    //判定磚塊是否重疊

        //    LGSCollisionData temp = prev;
        //    while (temp.elementType != ElementType.CB_START)
        //    {
        //        if (temp.elementType == ElementType.BRICK)
        //        {
        //            if (brickInBrick(temp.position, this.position) == false)
        //            {
        //                return false;
        //            }
        //        }
        //        if (temp.elementType == ElementType.CB_START)
        //        {
        //            break;
        //        }
        //        temp = temp.prev;
        //    }

        //    return true;
        //}


        public void Calculate()
        {
            length = 0;

            if (prev != null)
            {
                var dis = prev.position - position;

                length = dis.magnitude;

                switch (elementType)
                {
                    case ElementType.B:
                        difficulty = slopeLength * 0.2f;
                        break;
                    case ElementType.D:
                        difficulty = dominoN * 0.1f;
                        break;
                    case ElementType.P:
                        difficulty = cupDepth * cupDepth;
                        break;
                    case ElementType.L:
                        difficulty = slopeLength * 0.2f;
                        break;
                }
                if (elementType != ElementType.C || elementType != ElementType.ROOT) {
                    length = length * layer;
                }
            }
        }
    }
}