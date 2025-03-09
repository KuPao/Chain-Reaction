using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoParam {
    public class CR_GeoParam : IGeoParam {
        public CR_GeoParam(GeoParamType type) : base(type) {
        }
    }

    public class CR_Position : CR_GeoParam {
        public Vector3 pos;
        public CR_Position(Vector3 pos) : base(GeoParamType.CR_POSITION) {
            this.pos = pos;
        }
        public CR_Position(CR_Position clone) : base(GeoParamType.CR_POSITION) {
            this.pos = clone.pos;
        }
    }
    public class CR_Direction : CR_GeoParam {
        public int dir;
        public CR_Direction(int dir) : base(GeoParamType.CR_DIRECTION) {
            this.dir = dir;
        }
        public CR_Direction(CR_Direction clone) : base(GeoParamType.CR_DIRECTION) {
            this.dir = clone.dir;
        }
    }

    public class CR_B_BallOffset : CR_GeoParam {
        public float offset;
        public CR_B_BallOffset(float offset) : base(GeoParamType.CR_B_BO) {
            this.offset = offset;
        }
        public CR_B_BallOffset(CR_B_BallOffset clone) : base(GeoParamType.CR_B_BO) {
            this.offset = clone.offset;
        }
    }
    public class CR_B_SlopeLength : CR_GeoParam {
        public float length;
        public CR_B_SlopeLength(float length) : base(GeoParamType.CR_B_SL) {
            this.length = length;
        }
        public CR_B_SlopeLength(CR_B_SlopeLength clone) : base(GeoParamType.CR_B_SL) {
            this.length = clone.length;
        }
    }

    public class CR_D_N : CR_GeoParam {
        public int n;
        public CR_D_N(int n) : base(GeoParamType.CR_D_N) {
            this.n = n;
        }
        public CR_D_N(CR_D_N clone) : base(GeoParamType.CR_D_N) {
            this.n = clone.n;
        }
    }
    public class CR_D_GAP : CR_GeoParam {
        public float gap;
        public CR_D_GAP(float gap) : base(GeoParamType.CR_D_GAP) {
            this.gap = gap;
        }
        public CR_D_GAP(CR_D_GAP clone) : base(GeoParamType.CR_D_GAP) {
            this.gap = clone.gap;
        }
    }
    public class CR_D_TYPE : CR_GeoParam {
        public int type;
        public CR_D_TYPE(int type) : base(GeoParamType.CR_D_TYPE) {
            this.type = type;
        }
        public CR_D_TYPE(CR_D_TYPE clone) : base(GeoParamType.CR_D_TYPE) {
            this.type = clone.type;
        }
    }
    public class CR_P_Width : CR_GeoParam {
        public float width;
        public CR_P_Width(float width) : base(GeoParamType.CR_P_W) {
            this.width = width;
        }
        public CR_P_Width(CR_P_Width clone) : base(GeoParamType.CR_P_W) {
            this.width = clone.width;
        }
    }
    public class CR_P_CupDepth : CR_GeoParam {
        public float cupDepth;
        public CR_P_CupDepth(float cupDepth) : base(GeoParamType.CR_P_CD) {
            this.cupDepth = cupDepth;
        }
        public CR_P_CupDepth(CR_P_CupDepth clone) : base(GeoParamType.CR_P_CD) {
            this.cupDepth = clone.cupDepth;
        }
    }
    public class CR_P_BoardtDepth : CR_GeoParam {
        public float boardDepth;
        public CR_P_BoardtDepth(float boardDepth) : base(GeoParamType.CR_P_BD) {
            this.boardDepth = boardDepth;
        }
        public CR_P_BoardtDepth(CR_P_BoardtDepth clone) : base(GeoParamType.CR_P_BD) {
            this.boardDepth = clone.boardDepth;
        }
    }
    public class CR_P_BoardtFirst : CR_GeoParam {
        public bool boardFirst;
        public CR_P_BoardtFirst(bool boardFirst) : base(GeoParamType.CR_P_BF) {
            this.boardFirst = boardFirst;
        }
        public CR_P_BoardtFirst(CR_P_BoardtFirst clone) : base(GeoParamType.CR_P_BF) {
            this.boardFirst = clone.boardFirst;
        }
    }
    public class CR_L_BallOffset : CR_GeoParam {
        public float offset;
        public CR_L_BallOffset(float offset) : base(GeoParamType.CR_L_BO) {
            this.offset = offset;
        }
        public CR_L_BallOffset(CR_L_BallOffset clone) : base(GeoParamType.CR_L_BO) {
            this.offset = clone.offset;
        }
    }
    public class CR_L_SlopeLength : CR_GeoParam {
        public float length;
        public CR_L_SlopeLength(float length) : base(GeoParamType.CR_L_SL) {
            this.length = length;
        }
        public CR_L_SlopeLength(CR_L_SlopeLength clone) : base(GeoParamType.CR_L_SL) {
            this.length = clone.length;
        }
    }
    public class CR_L_OutPoint : CR_GeoParam {
        public List<Vector3> out_points;
        public CR_L_OutPoint(List<Vector3> out_points) : base(GeoParamType.CR_L_OP) {
            this.out_points = out_points;
        }
        public CR_L_OutPoint(CR_L_OutPoint clone) : base(GeoParamType.CR_L_OP) {
            out_points = new List<Vector3>();
            for (int i = 0; i < clone.out_points.Count; i++) {
                out_points.Add(new Vector3(clone.out_points[i].x, clone.out_points[i].y, clone.out_points[i].z));
            }
        }
    }
}
