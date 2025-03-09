using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGS;

namespace GeoParam {
    using LSystem;
    public class CR_GeoEmbedder : IGeoEmbedder {
        public override List<List<IGeoParam>> geoEmbbed(ILString lString, bool backward) {
            List<List<IGeoParam>> rt = new List<List<IGeoParam>>();
            for(int i = 0; i < lString.symbols.Count; i++) {
                rt.Add(new List<IGeoParam>());
            }

            // hard coded... bad...
            Vector3 refPoint = new Vector3(5.0f, 0.0f, 0.0f);
            Vector3 billiardRefPoint = new Vector3(5.0f, 0.0f, 0.0f);
            List<Vector3> branchRefPoint = new List<Vector3>();
            int branchCounter = 0;
            int currentDir = 0;
            int branchDir = 0;
            for (int i = lString.symbols.Count - 1; i >= 0;) {
                if (backward) {
                    ISymbol symbol = lString.symbols[i];
                    List<IGeoParam> geoParams = rt[i];
                    if (symbol.alphabet == 'B') {
                        int dir;
                        float ballOffset, slopeLength;
                        getRandomBallParam(i == 0, out dir, out ballOffset, out slopeLength);
                        if (currentDir != 0 && (lString.symbols[i + 1].alphabet == 'D' || lString.symbols[i + 1].alphabet == 'L')) {
                            dir = currentDir;
                        }
                        List<IGeoParam> ballParam = wrapBallParam(dir, ballOffset, slopeLength);
                        geoParams.AddRange(ballParam);

                        Vector3 pos, newRefPoint;
                        bool cPulley = false;
                        if (i > 0)
                            if (lString.symbols[i - 1].alphabet == 'P') {
                                cPulley = true;
                            }
                        getBallGeoParam(dir, ballOffset, slopeLength, refPoint, i == 0, backward, cPulley, out pos, out newRefPoint);
                        geoParams.Add(new CR_Position(pos));

                        refPoint = newRefPoint;
                        currentDir = dir;
                    }
                    else if (symbol.alphabet == 'D') {
                        int dir, dominosN, dominoType;
                        float dominosGap;
                        getRandomDominosParam(out dir, out dominosN, out dominosGap, out dominoType);
                        if (currentDir != 0 && (lString.symbols[i + 1].alphabet == 'D' || lString.symbols[i + 1].alphabet == 'L')) {
                            dir = currentDir;
                        }
                        List<IGeoParam> dominosParam = wrapDominosParam(dir, dominosN, dominosGap, dominoType);
                        geoParams.AddRange(dominosParam);

                        Vector3 pos, newRefPoint;
                        getDominosGeoParam(dir, dominosN, dominosGap, dominoType, refPoint, backward, out pos, out newRefPoint);
                        geoParams.Add(new CR_Position(pos));

                        refPoint = newRefPoint;
                        currentDir = dir;
                    }
                    else if (symbol.alphabet == 'P') {
                        int dir;
                        float wireWidth, wireCupDepth, wireBoardDepth;
                        getRandomWireParam(out dir, out wireWidth, out wireCupDepth, out wireBoardDepth);
                        //if (currentDir != 0 && lString.symbols[i + 1].alphabet == 'D') {
                        //    dir = currentDir;
                        //}
                        List<IGeoParam> wireParam = wrapWireParam(dir, wireWidth, wireCupDepth, wireBoardDepth);
                        geoParams.AddRange(wireParam);

                        Vector3 pos, newRefPoint;
                        bool boardFirst;
                        getWireGeoParam(dir, wireWidth, wireCupDepth, wireBoardDepth, refPoint, backward, out boardFirst, out pos, out newRefPoint);
                        geoParams.Add(new CR_P_BoardtFirst(boardFirst));
                        geoParams.Add(new CR_Position(pos));

                        refPoint = newRefPoint;
                        currentDir = dir;
                    }
                    else if (symbol.alphabet == 'L') {
                        int dir;
                        float ballOffset, slopeLength;
                        getRandomBilliardParam(i == 0, out dir, out ballOffset, out slopeLength);
                        dir = currentDir;
                        List<IGeoParam> billiardParam = wrapBilliardParam(dir, ballOffset, slopeLength);
                        geoParams.AddRange(billiardParam);

                        Vector3 pos, newRefPoint;
                        List<Vector3> newBranchRefPoints;
                        int branchCount = 0;
                        for (int j = i; j < lString.symbols.Count; j++) {
                            if (lString.symbols[j].alphabet == ']') {
                                branchCount += 1;
                                if (lString.symbols[j + 1].alphabet != '[') {
                                    //branchCount += 1;
                                    break;
                                }
                            }
                        }
                        getBilliardGeoParam(dir, branchCount, slopeLength, refPoint, i == 0, backward, out pos, out newRefPoint, out newBranchRefPoints);
                        geoParams.Add(new CR_Position(pos));
                        geoParams.Add(new CR_L_OutPoint(newBranchRefPoints));

                        branchRefPoint = newBranchRefPoints;
                        refPoint = newRefPoint;
                        billiardRefPoint = newRefPoint;
                        branchDir = dir;
                        currentDir = dir;
                        branchCounter = 0;
                        backward = false;
                        i++;
                        continue;
                    }
                    else if (symbol.alphabet == 'C') {
                        Vector3 pos, newRefPoint;
                        getCupGeoParam(refPoint, backward, out pos, out newRefPoint);
                        geoParams.Add(new CR_Position(pos));

                        refPoint = newRefPoint;
                    }
                    else if (symbol.alphabet == ']') {
                        for (int j = i; j >= 0; j--) {
                            if (lString.symbols[j].alphabet == 'L') {
                                i = j + 1;
                                break;
                            }
                        }
                    }
                    i--;
                }
                else {
                    ISymbol symbol = lString.symbols[i];
                    List<IGeoParam> geoParams = rt[i];
                    if (symbol.alphabet == 'B') {
                        int dir;
                        float ballOffset, slopeLength;
                        getRandomBallParam(i == 0, out dir, out ballOffset, out slopeLength);
                        
                        List<IGeoParam> ballParam = wrapBallParam(dir, ballOffset, slopeLength);
                        geoParams.AddRange(ballParam);

                        Vector3 pos, newRefPoint;
                        bool cPulley = false;
                        if (i > 0)
                            if (lString.symbols[i - 1].alphabet == 'P') {
                                cPulley = true;
                            }
                        getBallGeoParam(dir, ballOffset, slopeLength, refPoint, i == 0, backward, cPulley, out pos, out newRefPoint);
                        geoParams.Add(new CR_Position(pos));

                        refPoint = newRefPoint;
                        currentDir = dir;
                    }
                    else if (symbol.alphabet == 'D') {
                        int dir, dominosN, dominoType;
                        float dominosGap;
                        getRandomDominosParam(out dir, out dominosN, out dominosGap, out dominoType);
                        if (currentDir != 0 && (lString.symbols[i - 1].alphabet == 'B' || lString.symbols[i - 1].alphabet == 'D')) {
                            dir = currentDir;
                        }
                        List<IGeoParam> dominosParam = wrapDominosParam(dir, dominosN, dominosGap, dominoType);
                        geoParams.AddRange(dominosParam);

                        Vector3 pos, newRefPoint;
                        getDominosGeoParam(dir, dominosN, dominosGap, dominoType, refPoint, backward, out pos, out newRefPoint);
                        geoParams.Add(new CR_Position(pos));

                        refPoint = newRefPoint;
                        currentDir = dir;
                    }
                    else if (symbol.alphabet == 'P') {
                        if (lString.symbols[i - 1].alphabet != '[') {
                            int dir;
                            float wireWidth, wireCupDepth, wireBoardDepth;
                            getRandomWireParam(out dir, out wireWidth, out wireCupDepth, out wireBoardDepth);
                            List<IGeoParam> wireParam = wrapWireParam(dir, wireWidth, wireCupDepth, wireBoardDepth);
                            geoParams.AddRange(wireParam);

                            Vector3 pos, newRefPoint;
                            bool boardFirst;
                            getWireGeoParam(dir, wireWidth, wireCupDepth, wireBoardDepth, refPoint, backward, out boardFirst, out pos, out newRefPoint);
                            geoParams.Add(new CR_P_BoardtFirst(boardFirst));
                            geoParams.Add(new CR_Position(pos));

                            refPoint = newRefPoint;
                            currentDir = dir;
                        }
                        else {
                            int dir;
                            float wireWidth, wireCupDepth, wireBoardDepth;
                            getRandomWireParam(out dir, out wireWidth, out wireCupDepth, out wireBoardDepth);
                            List<IGeoParam> wireParam = wrapWireParam(dir, wireWidth, wireCupDepth, wireBoardDepth);
                            geoParams.AddRange(wireParam);

                            Vector3 pos, newRefPoint;
                            bool boardFirst;
                            getWireGeoParam(dir, wireWidth, wireCupDepth, wireBoardDepth, branchRefPoint[branchCounter], backward, out boardFirst, out pos, out newRefPoint);
                            branchCounter++;
                            geoParams.Add(new CR_P_BoardtFirst(boardFirst));
                            geoParams.Add(new CR_Position(pos));

                            refPoint = newRefPoint;
                            currentDir = dir;
                        }
                    }
                    else if (symbol.alphabet == 'C') {
                        Vector3 pos, newRefPoint;
                        getCupGeoParam(refPoint, backward, out pos, out newRefPoint);
                        geoParams.Add(new CR_Position(pos));

                        refPoint = newRefPoint;
                    }
                    else if (symbol.alphabet == ']') {
                        if (lString.symbols[i + 1].alphabet != '[') {
                            refPoint = billiardRefPoint;
                            currentDir = branchDir;
                            for (int j = i; j >= 0; j--) {
                                if (lString.symbols[j].alphabet == 'L') {
                                    backward = true;
                                    i = j - 1;
                                    i--;
                                    break;
                                }
                            }
                        }
                    }
                    i++;
                }
                
            }

            return rt;
        }

        public override List<IGeoParam> charGeoEmbbed(LGSCollisionData current, LGSCollisionData next, bool backward, bool random) {
            // hard coded... bad...
            Vector3 refPoint;
            LGSCollisionData prev = current.list_prev;
            
            if (backward) {
                if (next == null) {
                    refPoint = new Vector3(5.0f, 0.0f, 0.0f);
                }
                else if (next.elementType == LGSCollisionData.ElementType.P) {
                    float BOARD_VERTICAL_OFFSET = 0.25f;
                    float CUP_VERTICAL_OFFSET = 0.5f;

                    //Debug.Log(refPoint);
                    Vector3 tmp = next.position;
                    tmp.y += BOARD_VERTICAL_OFFSET + next.boardDepth - next.cupDepth - CUP_VERTICAL_OFFSET;
                    tmp.x -= next.dir * next.wireWidth;

                    refPoint = tmp;
                }
                else if (next.elementType == LGSCollisionData.ElementType.C) {
                    refPoint = next.position;
                    refPoint.y += 0.5f;
                }
                else if (next.elementType == LGSCollisionData.ElementType.L) {
                    float SLOPE_ANGLE = 0f;
                    float OFFSET_RATIO = 1.0f;

                    float halfSlopeWidth = Mathf.Abs(next.slopeLength / 2.0f * Mathf.Cos(Mathf.Deg2Rad * SLOPE_ANGLE));
                    float virtical_offset = 0.2f;
                    refPoint = next.position;
                    refPoint.x -= next.dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth) * 0.8f;
                }
                //else if (next.elementType == LGSCollisionData.ElementType.D) {
                //    float MIN_VERTICAL_OFFSET = 0.2f;
                //    float MAX_VERTICAL_OFFSET = 1.0f;
                //    float FIXED_VERTICAL_OFFSET = 0.25f;
                //    float SLOPE_OFFSET = 0.05f;

                //    float FIXED_HORIZONTAL_OFFSET = 0.3f;

                //    refPoint = next.position;
                    
                //    float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                //    refPoint.y += (virtical_offset + FIXED_VERTICAL_OFFSET);

                //    float dominosWidth = (next.dominoN - 1) * next.dominoGap;
                //    refPoint.x -= next.dir * (dominosWidth + FIXED_HORIZONTAL_OFFSET);                    
                //}
                else {
                    //if(next.elementType == LGSCollisionData.ElementType.D) {
                    //    Debug.Log("domino pos: " + next.position.ToString());
                    //}
                    refPoint = next.position;
                }
            }
            else {
                if (next.elementType == LGSCollisionData.ElementType.P) {
                    float BOARD_VERTICAL_OFFSET = 0.25f;
                    float CUP_VERTICAL_OFFSET = 0.5f;

                    //Debug.Log(refPoint);
                    Vector3 tmp = next.position;
                    tmp.y += BOARD_VERTICAL_OFFSET + next.cupDepth - next.boardDepth + CUP_VERTICAL_OFFSET - 0.05f;
                    tmp.x -= next.dir * next.wireWidth;

                    refPoint = tmp;
                }
                else if (next.elementType == LGSCollisionData.ElementType.B) {
                    const float SLOPE_ANGLE = -28.241f;
                    const float OFFSET_RATIO = 1.0f;

                    const float MIN_VERTICAL_OFFSET = 0.2f;
                    const float MAX_VERTICAL_OFFSET = 1.0f;
                    const float FIXED_VERTICAL_OFFSET = 0.3f;
                    refPoint = next.position;
                    float halfSlopeHeight = Mathf.Abs(next.slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                    refPoint.y -= (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    float halfSlopeWidth = Mathf.Abs(next.slopeLength / 2.0f * Mathf.Cos(Mathf.Deg2Rad * SLOPE_ANGLE));
                    refPoint.x += next.dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
                else if (next.elementType == LGSCollisionData.ElementType.D) {
                    const float MIN_VERTICAL_OFFSET = 0.2f;
                    const float MAX_VERTICAL_OFFSET = 1.0f;
                    const float FIXED_VERTICAL_OFFSET = 0.25f;

                    const float FIXED_HORIZONTAL_OFFSET = 0.3f;
                    
                    refPoint = next.position;
                    //Debug.Log("refPoint:" + refPoint.ToString());
                    float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                    refPoint.y -= (virtical_offset + FIXED_VERTICAL_OFFSET);

                    float dominosWidth = (next.dominoN - 1) * next.dominoGap;
                    refPoint.x += next.dir * (dominosWidth + FIXED_HORIZONTAL_OFFSET);
                    
                }
                else if (next.elementType == LGSCollisionData.ElementType.L) {
                    if (next.branchCount >= next.outPoints.Count) {
                        if (current.list_next.elementType == LGSCollisionData.ElementType.L) {
                            next.branchCount = 0;
                        }
                        else {
                            next.branchCount = 1;
                        }
                    }
                    refPoint = next.outPoints[next.branchCount];
                }
                else {
                    refPoint = prev.position;
                }
            }
            if (random) {
                refPoint.x = Random.Range(-50.0f, 50.0f);
                refPoint.y = Random.Range(0.0f, 100.0f);
            }
            

            int currentDir = next == null ? 1 : next.dir;
            List<IGeoParam> geoParams = new List<IGeoParam>();
            ISymbol symbol = new CR_Symbol(' ');
            ISymbol nextSymbol = new CR_Symbol(' ');
            switch (current.elementType) {
                case LGSCollisionData.ElementType.B:
                    symbol.alphabet = 'B';
                    break;
                case LGSCollisionData.ElementType.D:
                    symbol.alphabet = 'D';
                    break;
                case LGSCollisionData.ElementType.P:
                    symbol.alphabet = 'P';
                    break;
                case LGSCollisionData.ElementType.L:
                    symbol.alphabet = 'L';
                    break;
                case LGSCollisionData.ElementType.C:
                    symbol.alphabet = 'C';
                    break;
            }
            if (next != null) {
                switch (next.elementType) {
                    case LGSCollisionData.ElementType.B:
                        nextSymbol.alphabet = 'B';
                        break;
                    case LGSCollisionData.ElementType.D:
                        nextSymbol.alphabet = 'D';
                        break;
                    case LGSCollisionData.ElementType.P:
                        nextSymbol.alphabet = 'P';
                        break;
                    case LGSCollisionData.ElementType.L:
                        nextSymbol.alphabet = 'L';
                        break;
                    case LGSCollisionData.ElementType.C:
                        nextSymbol.alphabet = 'C';
                        break;
                }
            }
            else {
                nextSymbol.alphabet = ' ';
            }
            if (backward) {
                if (symbol.alphabet == 'B') {
                    //Debug.Log(refPoint);
                    int dir;
                    float ballOffset, slopeLength;
                    getRandomBallParam(prev == null, out dir, out ballOffset, out slopeLength);
                    if (currentDir != 0 && (nextSymbol.alphabet == 'D' || nextSymbol.alphabet == 'L')) {
                        dir = currentDir;
                    }
                    List<IGeoParam> ballParam = wrapBallParam(dir, ballOffset, slopeLength);
                    geoParams.AddRange(ballParam);

                    Vector3 pos, newRefPoint;
                    bool cPulley = false;
                    if (current.list_prev != null && current.list_prev.elementType == LGSCollisionData.ElementType.P)
                        cPulley = true;
                    getBallGeoParam(dir, ballOffset, slopeLength, refPoint, prev == null, backward, cPulley, out pos, out newRefPoint);
                    //Debug.Log(pos);
                    geoParams.Add(new CR_Position(pos));
                }
                else if (symbol.alphabet == 'D') {
                    int dir, dominosN, dominoType;
                    float dominosGap;
                    getRandomDominosParam(out dir, out dominosN, out dominosGap, out dominoType);
                    List<IGeoParam> dominosParam = wrapDominosParam(dir, dominosN, dominosGap, dominoType);
                    geoParams.AddRange(dominosParam);

                    Vector3 pos, newRefPoint;
                    getDominosGeoParam(dir, dominosN, dominosGap, dominoType, refPoint, backward, out pos, out newRefPoint);
                    geoParams.Add(new CR_Position(pos));
                }
                else if (symbol.alphabet == 'P') {
                    int dir;
                    float wireWidth, wireCupDepth, wireBoardDepth;
                    getRandomWireParam(out dir, out wireWidth, out wireCupDepth, out wireBoardDepth);
                    //if (currentDir != 0 && lString.symbols[i + 1].alphabet == 'D') {
                    //    dir = currentDir;
                    //}
                    List<IGeoParam> wireParam = wrapWireParam(dir, wireWidth, wireCupDepth, wireBoardDepth);
                    geoParams.AddRange(wireParam);

                    Vector3 pos, newRefPoint;
                    bool boardFirst;
                    //Debug.Log(refPoint);
                    getWireGeoParam(dir, wireWidth, wireCupDepth, wireBoardDepth, refPoint, backward, out boardFirst, out pos, out newRefPoint);
                    geoParams.Add(new CR_P_BoardtFirst(boardFirst));
                    geoParams.Add(new CR_Position(pos));
                }
                else if (symbol.alphabet == 'L') {
                    int dir;
                    float ballOffset, slopeLength;
                    getRandomBilliardParam(false, out dir, out ballOffset, out slopeLength);
                    dir = currentDir;
                    List<IGeoParam> billiardParam = wrapBilliardParam(dir, ballOffset, slopeLength);
                    geoParams.AddRange(billiardParam);

                    Vector3 pos, newRefPoint;
                    List<Vector3> newBranchRefPoints;
                    LGSCollisionData list_prev = current.list_prev;
                    int branchCount = 0;
                    while(list_prev != null || list_prev.elementType != LGSCollisionData.ElementType.L) {
                        if (list_prev.elementType == LGSCollisionData.ElementType.C) {
                            branchCount += 1;
                        }
                        list_prev = list_prev.list_prev;
                        if (list_prev == null) {
                            break;
                        }
                    }
                    getBilliardGeoParam(dir, branchCount, slopeLength, refPoint, false, backward, out pos, out newRefPoint, out newBranchRefPoints);
                    geoParams.Add(new CR_Position(pos));
                    geoParams.Add(new CR_L_OutPoint(newBranchRefPoints));
                }
                else if (symbol.alphabet == 'C') {
                    Vector3 pos, newRefPoint;
                    getCupGeoParam(refPoint, backward, out pos, out newRefPoint);
                    geoParams.Add(new CR_Position(pos));
                }
            }
            else {
                if (symbol.alphabet == 'B') {
                    //Debug.Log(refPoint);
                    int dir;
                    float ballOffset, slopeLength;
                    getRandomBallParam(false, out dir, out ballOffset, out slopeLength);

                    List<IGeoParam> ballParam = wrapBallParam(dir, ballOffset, slopeLength);
                    geoParams.AddRange(ballParam);

                    Vector3 pos, newRefPoint;
                    bool cPulley = false;
                    if (current.list_next != null && current.list_next.elementType == LGSCollisionData.ElementType.P)
                        cPulley = true;
                    getBallGeoParam(dir, ballOffset, slopeLength, refPoint, false, backward, cPulley, out pos, out newRefPoint);
                    geoParams.Add(new CR_Position(pos));
                }
                else if (symbol.alphabet == 'D') {
                    int dir, dominosN, dominoType;
                    float dominosGap;
                    getRandomDominosParam(out dir, out dominosN, out dominosGap, out dominoType);
                    if (currentDir != 0 && nextSymbol.alphabet == 'B') {
                        dir = currentDir;
                    }
                    List<IGeoParam> dominosParam = wrapDominosParam(dir, dominosN, dominosGap, dominoType);
                    geoParams.AddRange(dominosParam);

                    Vector3 pos, newRefPoint;
                    getDominosGeoParam(dir, dominosN, dominosGap, dominoType, refPoint, backward, out pos, out newRefPoint);
                    geoParams.Add(new CR_Position(pos));
                }
                else if (symbol.alphabet == 'P') {
                    if (nextSymbol.alphabet != 'L') {
                        int dir;
                        float wireWidth, wireCupDepth, wireBoardDepth;
                        getRandomWireParam(out dir, out wireWidth, out wireCupDepth, out wireBoardDepth);
                        List<IGeoParam> wireParam = wrapWireParam(dir, wireWidth, wireCupDepth, wireBoardDepth);
                        geoParams.AddRange(wireParam);

                        Vector3 pos, newRefPoint;
                        bool boardFirst;
                        getWireGeoParam(dir, wireWidth, wireCupDepth, wireBoardDepth, refPoint, backward, out boardFirst, out pos, out newRefPoint);
                        geoParams.Add(new CR_P_BoardtFirst(boardFirst));
                        geoParams.Add(new CR_Position(pos));
                    }
                    else {
                        int dir;
                        float wireWidth, wireCupDepth, wireBoardDepth;
                        getRandomWireParam(out dir, out wireWidth, out wireCupDepth, out wireBoardDepth);
                        List<IGeoParam> wireParam = wrapWireParam(dir, wireWidth, wireCupDepth, wireBoardDepth);
                        geoParams.AddRange(wireParam);

                        Vector3 pos, newRefPoint;
                        bool boardFirst;
                        getWireGeoParam(dir, wireWidth, wireCupDepth, wireBoardDepth, refPoint, backward, out boardFirst, out pos, out newRefPoint);
                        geoParams.Add(new CR_P_BoardtFirst(boardFirst));
                        geoParams.Add(new CR_Position(pos));
                    }
                }
                else if (symbol.alphabet == 'C') {
                    Vector3 pos, newRefPoint;
                    getCupGeoParam(refPoint, backward, out pos, out newRefPoint);
                    geoParams.Add(new CR_Position(pos));
                }
            }
            
            return geoParams;
        }

        private void getRandomBallParam(bool isFirst, out int dir, out float ballOffset, out float slopeLength) {
            const float MIN_BALL_OFFSET = 10.0f;
            const float MAX_BALL_OFFSET = 20.0f;

            const float MIN_SLOPE_LENGTH = 3.0f;
            const float MAX_SLOPE_LENGTH = 5.0f;
            
            float randomDir = Random.Range(-1.0f, 1.0f);
            dir = 1;
            if (randomDir < 0.0f)
                dir = -1;

            ballOffset = 1.0f;
            if (isFirst) {
                ballOffset = Random.Range(MIN_BALL_OFFSET, MAX_BALL_OFFSET);
            }
            
            slopeLength = Random.Range(MIN_SLOPE_LENGTH, MAX_SLOPE_LENGTH);

            //Debug.Log("slope");
            //Debug.Log(slopeLength);
        }
        private List<IGeoParam> wrapBallParam(int dir, float ballOffset, float slopeLength) {
            List<IGeoParam> rt = new List<IGeoParam>();
            rt.Add(new CR_Direction(dir));
            rt.Add(new CR_B_BallOffset(ballOffset));
            rt.Add(new CR_B_SlopeLength(slopeLength));
            return rt;
        }
        private void getBallGeoParam(int dir, float ballOffset, float slopeLength, Vector3 refPoint, bool isFirst, bool backward, bool cPulley, out Vector3 position, out Vector3 outRefPoint) {
            const float SLOPE_ANGLE = -28.241f;
            const float MAX_SLOPE_LENGTH = 5.0f;
            const float OFFSET_RATIO = 1.0f;

            const float MIN_VERTICAL_OFFSET = 0.2f;
            const float MAX_VERTICAL_OFFSET = 1.0f;
            const float FIXED_VERTICAL_OFFSET = 0.3f;

            position = refPoint;

            if (backward) {
                if (isFirst) {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                    if (cPulley) {
                        virtical_offset /= 2;
                    }
                    position.y += (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    float halfSlopeWidth = Mathf.Abs(slopeLength / 2.0f * Mathf.Cos(Mathf.Deg2Rad * SLOPE_ANGLE));
                    position.x -= dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
                else {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                    if (cPulley) {
                        virtical_offset /= 2;
                    }
                    position.y += (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    float halfSlopeWidth = Mathf.Abs(slopeLength / 2.0f * Mathf.Cos(Mathf.Deg2Rad * SLOPE_ANGLE));
                    position.x -= dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
                outRefPoint = position;
            }
            else {
                outRefPoint = position;
                if (isFirst) {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                    if (cPulley) {
                        virtical_offset /= 2;
                    }
                    outRefPoint.y -= (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    float halfSlopeWidth = Mathf.Abs(slopeLength / 2.0f * Mathf.Cos(Mathf.Deg2Rad * SLOPE_ANGLE));
                    outRefPoint.x += dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
                else {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                    if (cPulley) {
                        virtical_offset /= 2;
                    }
                    outRefPoint.y -= (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    float halfSlopeWidth = Mathf.Abs(slopeLength / 2.0f * Mathf.Cos(Mathf.Deg2Rad * SLOPE_ANGLE));
                    outRefPoint.x += dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                    //Debug.Log("position:" + position.ToString());
                    //Debug.Log("out:"+outRefPoint.ToString());
                }
            }            
            //Debug.Log(outRefPoint);
        }

        private void getRandomDominosParam(out int dir, out int dominosN, out float dominosGap, out int dominoType) {
            const int MIN_DOMINOS_N = 4;
            const int MAX_DOMINOS_N = 10;

            const float MIN_DOMINOS_GAP = 0.20f;
            const float MAX_DOMINOS_GAP = 0.30f;

            float randomDir = Random.Range(-1.0f, 1.0f);
            dir = 1;
            if (randomDir < 0.0f)
                dir = -1;

            dominosN = Random.Range(MIN_DOMINOS_N, MAX_DOMINOS_N);

            dominosGap = Random.Range(MIN_DOMINOS_GAP, MAX_DOMINOS_GAP);

            dominoType = Random.Range(0, 3);
            dominoType = 0;
            //Debug.Log(dominosN);
            //Debug.Log(dominosGap);
        }
        private List<IGeoParam> wrapDominosParam(int dir, int dominosN, float dominosGap, int dominoType) {
            List<IGeoParam> rt = new List<IGeoParam>();
            rt.Add(new CR_Direction(dir));
            rt.Add(new CR_D_N(dominosN));
            rt.Add(new CR_D_GAP(dominosGap));
            rt.Add(new CR_D_TYPE(dominoType));
            return rt;
        }
        private void getDominosGeoParam(int dir, int dominosN, float dominosGap, int dominoType, Vector3 refPoint, bool backward, out Vector3 position, out Vector3 outRefPoint) {
            const float MIN_VERTICAL_OFFSET = 0.2f;
            const float MAX_VERTICAL_OFFSET = 1.0f;
            const float FIXED_VERTICAL_OFFSET = 0.25f;
            const float SLOPE_OFFSET = 0.05f;

            const float FIXED_HORIZONTAL_OFFSET = 0.3f;

            position = refPoint;
            if (backward) {
                float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                position.y += (virtical_offset + FIXED_VERTICAL_OFFSET);
                if (dominoType == 1) {
                    position.y -= (dominosN - 1) * SLOPE_OFFSET;
                }
                if (dominoType == 2) {
                    position.y += (dominosN - 1) * SLOPE_OFFSET;
                }

                float dominosWidth = (dominosN - 1) * dominosGap;
                position.x -= dir * (dominosWidth + FIXED_HORIZONTAL_OFFSET);
                outRefPoint = position;
            }
            else {
                outRefPoint = position;
                //Debug.Log("refPoint:" + refPoint.ToString());
                float virtical_offset = Random.Range(MIN_VERTICAL_OFFSET, MAX_VERTICAL_OFFSET);
                outRefPoint.y -= (virtical_offset + FIXED_VERTICAL_OFFSET);
                if (dominoType == 1) {
                    outRefPoint.y -= (dominosN - 1) * SLOPE_OFFSET;
                }
                if (dominoType == 2) {
                    outRefPoint.y += (dominosN - 1) * SLOPE_OFFSET;
                }

                float dominosWidth = (dominosN - 1) * dominosGap;
                outRefPoint.x += dir * (dominosWidth + FIXED_HORIZONTAL_OFFSET);
            }            
            
        }
        private void getRandomWireParam(out int dir, out float wireWidth, out float wireCupDepth, out float wireBoardDepth) {
            const float MIN_WIRE_WIDTH = 2.0f;
            const float MAX_WIRE_WIDTH = 5.0f;

            const float MIN_WIRE_DEPTH = 0.5f;
            const float MAX_WIRE_DEPTH = 1.0f;

            float randomDir = Random.Range(-1.0f, 1.0f);
            dir = 1;
            if (randomDir < 0.0f)
                dir = -1;

            wireWidth = Random.Range(MIN_WIRE_WIDTH, MAX_WIRE_WIDTH);

            wireCupDepth = Random.Range(MIN_WIRE_DEPTH, MAX_WIRE_DEPTH / 2);

            wireBoardDepth = Random.Range(MIN_WIRE_DEPTH, MAX_WIRE_DEPTH);
        }
        private List<IGeoParam> wrapWireParam(int dir, float wireWidth, float wireCupDepth, float wireBoardDepth) {
            List<IGeoParam> rt = new List<IGeoParam>();
            rt.Add(new CR_Direction(dir));
            rt.Add(new CR_P_Width(wireWidth));
            rt.Add(new CR_P_CupDepth(wireCupDepth));
            rt.Add(new CR_P_BoardtDepth(wireBoardDepth));
            return rt;
        }
        private void getWireGeoParam(int dir, float wireWidth, float wireCupDepth, float wireBoardDepth, Vector3 refPoint, bool backward, out bool boardFirst, out Vector3 position, out Vector3 outRefPoint) {
            const float BOARD_VERTICAL_OFFSET = 0.25f;
            const float CUP_VERTICAL_OFFSET = 0.5f;

            //Debug.Log(refPoint);
            position = refPoint;
            Vector3 tmp = position;

            if (backward) {
                tmp.y += BOARD_VERTICAL_OFFSET + wireBoardDepth - wireCupDepth - CUP_VERTICAL_OFFSET;
                tmp.x -= dir * wireWidth;
            }
            else {
                position.y -= 0.5f;
                tmp = position;
                tmp.y += BOARD_VERTICAL_OFFSET + wireCupDepth - wireBoardDepth + CUP_VERTICAL_OFFSET - 0.05f;
                tmp.x -= dir * wireWidth;
            }

            boardFirst = backward;

            outRefPoint = tmp;
        }
        private void getRandomBilliardParam(bool isFirst, out int dir, out float ballOffset, out float slopeLength) {
            const float MIN_BALL_OFFSET = 10.0f;
            const float MAX_BALL_OFFSET = 20.0f;

            const float MIN_SLOPE_LENGTH = 1.0f;
            const float MAX_SLOPE_LENGTH = 5.0f;

            float randomDir = Random.Range(-1.0f, 1.0f);
            dir = 1;
            if (randomDir < 0.0f)
                dir = -1;

            ballOffset = 1.5f;
            if (isFirst) {
                ballOffset = Random.Range(MIN_BALL_OFFSET, MAX_BALL_OFFSET);
            }

            slopeLength = 3.0f;

            //Debug.Log("slope");
            //Debug.Log(slopeLength);
        }
        private List<IGeoParam> wrapBilliardParam(int dir, float ballOffset, float slopeLength) {
            List<IGeoParam> rt = new List<IGeoParam>();
            rt.Add(new CR_Direction(dir));
            rt.Add(new CR_L_BallOffset(ballOffset));
            rt.Add(new CR_L_SlopeLength(slopeLength));
            return rt;
        }
        private void getBilliardGeoParam(int dir, int branchCount, float slopeLength, Vector3 refPoint, bool isFirst, bool backward, out Vector3 position, out Vector3 outRefPoint, out List<Vector3> outBranchRefPoints) {
            const float SLOPE_ANGLE = 0f;
            const float OFFSET_RATIO = 1.0f;

            const float MIN_VERTICAL_OFFSET = 0.2f;
            const float MAX_VERTICAL_OFFSET = 1.0f;
            const float FIXED_VERTICAL_OFFSET = 0.3f;
            const float BALL_OFFSET = 0.15f;

            position = refPoint;
            float halfSlopeWidth = Mathf.Abs(slopeLength / 2.0f * Mathf.Cos(Mathf.Deg2Rad * SLOPE_ANGLE));
            float virtical_offset = 0.2f;
            if (backward) {
                if (isFirst) {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    position.y += (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    position.x -= dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
                else {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    position.y += (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    position.x -= dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
            }
            else {
                if (isFirst) {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    position.y -= (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    position.x += dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
                else {
                    float halfSlopeHeight = Mathf.Abs(slopeLength / 2.0f * Mathf.Sin(Mathf.Deg2Rad * SLOPE_ANGLE));
                    position.y -= (virtical_offset + halfSlopeHeight + FIXED_VERTICAL_OFFSET);

                    position.x += dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth);
                }
            }
            outBranchRefPoints = new List<Vector3>();
            for (int i = 0; i < branchCount; i++) {
                float offset = 1;
                for (int j = 0; j < (i + 1); j++) {
                    offset *= -1;
                }
                Vector3 tmp = refPoint;
                //tmp.y -= 0.5f;
                tmp.x -= dir * (slopeLength / 2 + BALL_OFFSET - 11f * 0.05f * Mathf.Sqrt(8));
                tmp.z += 0.8f * offset;
                outBranchRefPoints.Add(tmp);
            }
            outRefPoint = position;
            outRefPoint.x -= dir * (virtical_offset * OFFSET_RATIO + halfSlopeWidth) * 0.8f;
            //Debug.Log(outRefPoint);
        }
        private void getCupGeoParam(Vector3 refPoint, bool backward, out Vector3 position, out Vector3 outRefPoint) {
            const float FIXED_VERTICAL_OFFSET = 0.5f;

            if (backward) {
                position = refPoint;
                position.y -= FIXED_VERTICAL_OFFSET;
                outRefPoint = refPoint;
            }
            else {
                position = refPoint;
                position.y -= FIXED_VERTICAL_OFFSET;
                outRefPoint = refPoint;
            }  
            
        }
    }
}
