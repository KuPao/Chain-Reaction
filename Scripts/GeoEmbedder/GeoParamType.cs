
namespace GeoParam {
    public enum GeoParamType {
        // Chain Reaction
        CR_POSITION,
        CR_DIRECTION,
        // BallOnSlope
            CR_B_BO, // ball offset
            CR_B_SL, // slope length
        // Dominos
            CR_D_N,   // dominos n
            CR_D_GAP, // dominos gap
            CR_D_TYPE, // dominos type
        // UWire(P)
        CR_P_W,     // wire width
            CR_P_CD,    // wire cup depth
            CR_P_BD,    // wire board depth
            CR_P_BF,    // board first?
        // Billiard
            CR_L_BO,    // ball offset
            CR_L_SL,    // slope length
            CR_L_OP     // billiard output point
    }
}

