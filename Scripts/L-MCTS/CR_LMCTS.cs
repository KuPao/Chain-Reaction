using ActionBool = System.Action<bool>;
using ActionGoal = System.Action<System.Tuple<bool, float>>;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using ChainReaction;
using System.Linq;
using LGS;

 public static class AppHelper
 {
     #if UNITY_WEBPLAYER
     public static string webplayerQuitURL = "http://google.com";
     #endif
     public static void Quit()
     {
         #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
         #elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
         #else
         Application.Quit();
         #endif
     }
 }
public class CR_LMCTS : MonoBehaviour {
    public LSystem.CR_LSystem lSystem = new LSystem.CR_LSystem();
    public List<List<GeoParam.IGeoParam>> geoParams;
    public ChainReaction.ChainReaction crInstance;

    public MCTSNode root;
    public List<MCTSNode> candidates = new List<MCTSNode>();
    public List<MCTSNode> result = new List<MCTSNode>();
    public bool finish = false;
    public bool useCamera = false;
    public RecordTransformHierarchy recorder;
    public List<System.Tuple<AnimationClip, float>> clips = new List<System.Tuple<AnimationClip, float>>();

    public float target = 5.0f;
    public float startTime, endTime;
    public int good;
    public int better;
    public int best;
    public bool inverse;

    public string folder;
    public string date;

    public GameObject camera;
    public List<GameObject> cameras;

    public void Awake() {
        
    }

    private void Update() {
        if (!useCamera) {
            for (int i = 0; i < cameras.Count; i++) {
                cameras[i].SetActive(false);
            }
        }
        if (useCamera) {
            for (int i = 0; i < cameras.Count; i++) {
                cameras[i].SetActive(true);
            }
        }
        float offset = 5;
        float y_offset = 1.5f;
        for (int i = 0; i < cameras.Count; i++) {
            FollowCamera c = cameras[i].GetComponent<FollowCamera>();
            LSystem.ISymbol symbol = lSystem.lString.symbols[c.currentId];
            if (symbol.alphabet == 'B') {
                if (c.current.hitTarget) {
                    LSystem.ISymbol nextSymbol = lSystem.lString.symbols[c.currentId + 1];
                    c.currentId++;
                    Vector3 next = crInstance.primitives[c.currentId].transform.position;
                    if(nextSymbol.alphabet != 'P') {
                        c.target = new Vector3(next.x, next.y + y_offset, next.z - offset);
                        //cameras[i].transform.position = new Vector3(next.x, next.y, next.z - offset);
                    }
                    c.current = crInstance.primitives[c.currentId];
                }
            }
            else if (symbol.alphabet == 'D') {
                if (c.current.hitTarget) {
                    LSystem.ISymbol nextSymbol = lSystem.lString.symbols[c.currentId + 1];
                    c.currentId++;
                    Vector3 next = crInstance.primitives[c.currentId].transform.position;
                    if (nextSymbol.alphabet != 'P') {
                        c.target = new Vector3(next.x, next.y + y_offset, next.z - offset);
                        //cameras[i].transform.position = new Vector3(next.x, next.y, next.z - offset);
                    }
                    c.current = crInstance.primitives[c.currentId];
                }
            }
            else if (symbol.alphabet == 'P') {
                UWire current = (UWire)c.current;
                Vector3 newPos = new Vector3();
                if (current.boardFirst) {
                    Vector3 leftJoint = new Vector3(current.transform.position.x - 0.05f * current.transform.localScale.x, current.transform.position.y + current.boardDepth + 0.25f, current.transform.position.z);
                    Vector3 rightJoint = new Vector3(current.transform.position.x - current.wireWidth * current.transform.localScale.x + 0.05f * current.transform.localScale.x, leftJoint.y, current.transform.position.z);
                    newPos = new Vector3((leftJoint.x + rightJoint.x) / 2, leftJoint.y - 0.5f + y_offset, current.transform.position.z - offset);
                }
                else {
                    Vector3 board = new Vector3(current.transform.position.x - current.wireWidth * current.transform.localScale.x, current.transform.position.y + current.cupDepth - current.boardDepth + 0.7f, current.transform.position.z);
                    Vector3 cup = new Vector3(current.transform.position.x, current.transform.position.y, current.transform.position.z);
                    Vector3 leftJoint = new Vector3(board.x - 0.05f * current.transform.localScale.x, board.y + current.boardDepth + 0.25f, board.z);
                    Vector3 rightJoint = new Vector3(cup.x + 0.05f * current.transform.localScale.x, leftJoint.y, cup.z);
                    newPos = new Vector3((leftJoint.x + rightJoint.x) / 2, leftJoint.y - 0.5f + y_offset, current.transform.position.z - offset);
                }
                c.target = newPos;
                //cameras[i].transform.position = newPos;
                if (current.hasDrop) {
                    c.currentId++;
                    Vector3 next = crInstance.primitives[c.currentId].transform.position;
                    c.target = new Vector3(next.x, next.y + y_offset, next.z - offset);
                    //cameras[i].transform.position = new Vector3(next.x, next.y, next.z - offset);
                    c.current = crInstance.primitives[c.currentId];
                }
            }
            else if (symbol.alphabet == 'L') {
                Billiard b = (Billiard)c.current;
                int branchCount = b.outPoints.Count + 1;
                if (cameras.Count < branchCount) {
                    for (int j = 1; j < branchCount; j++) {
                        GameObject newCam = Instantiate(camera, camera.transform.parent);
                        cameras.Add(newCam);
                    }
                }
                if (b.hitTarget0 && i == 1) {
                    c.currentId = c.currentId + 2;
                    Vector3 next = crInstance.primitives[c.currentId].transform.position;
                    //cameras[i].transform.position = new Vector3(next.x, next.y, next.z - offset);
                    c.current = crInstance.primitives[c.currentId];
                }
                else if (i == 0 && ((branchCount == 3 && b.hitTarget2) || (branchCount == 2 && b.hitTarget1))) {
                    int counter = 0;
                    for (int j = c.currentId; j < lSystem.lString.symbols.Count; j++) {
                        if (lSystem.lString.symbols[j].alphabet == ']') {
                            counter++;
                        }
                        if (counter == branchCount - 1) {
                            c.currentId = j + 1;
                            Vector3 next = crInstance.primitives[c.currentId].transform.position;
                            //cameras[i].transform.position = new Vector3(next.x, next.y, next.z - offset);
                            c.current = crInstance.primitives[c.currentId];
                            break;
                        }
                    }
                }
                else if (i == 2) {
                    if (b.hitTarget1) {
                        for (int j = c.currentId; j < lSystem.lString.symbols.Count; j++) {
                            if (lSystem.lString.symbols[j].alphabet == ']') {
                                c.currentId = j + 2;
                                Vector3 next = crInstance.primitives[c.currentId].transform.position;
                                //cameras[i].transform.position = new Vector3(next.x, next.y, next.z - offset);
                                c.current = crInstance.primitives[c.currentId];
                                break;
                            }
                        }
                    }
                }
            }
            else if (symbol.alphabet == 'C') {
                Goal g = c.current.GetComponent<Goal>();
                if (g.success) {
                    if (i > 0) {
                        bool onBilliard = false;
                        for (int j = 0; j < cameras.Count; j++) {
                            FollowCamera f = cameras[j].GetComponent<FollowCamera>();
                            if (lSystem.lString.symbols[f.currentId].alphabet == 'L') {
                                onBilliard = true;
                            }
                        }
                        if (!onBilliard) {
                            GameObject currCamera = cameras[i];
                            cameras.RemoveAt(i);
                            Destroy(currCamera);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < cameras.Count; i++) {
            Camera cam = cameras[i].GetComponent<Camera>();
            cam.rect = new Rect(0 + i / (float)cameras.Count, 0, 1.0f / (float)cameras.Count, 1);
        }
    }

    public void startCamera() {
        for (int i = cameras.Count - 1; i >= 0; i--) {
            GameObject currCamera = cameras[i];
            cameras.RemoveAt(i);
            if(i > 0) {
                Destroy(currCamera);
            }
        }
        cameras.Clear();
        if (useCamera) {
            camera.active = true;
            Vector3 first = crInstance.primitives[0].transform.position;
            camera.transform.position = new Vector3(first.x, first.y + 1.5f, first.z - 5);
            camera.GetComponent<FollowCamera>().current = crInstance.primitives[0];
            camera.GetComponent<FollowCamera>().currentId = 0;
            camera.GetComponent<FollowCamera>().target = camera.transform.position;
            cameras.Add(camera);
        }
        else {
            camera.SetActive(false);
        }
    }
    public void writeGeoParamToFile(bool mcts, int t, MCTSNode node) {
        if (mcts) {
            if (this.inverse) {
                folder = @"D:\L_MCTS\MCTS_inverse\" + date + "_" + target.ToString() + "\\";
                string fileName = "MCTS_inverse" + ".txt";
                string fullPath = folder + fileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, true)) {
                    sw.WriteLine("Turn " + t.ToString());
                    sw.WriteLine("Fitness: " + node.fitness.ToString());
                    for (int i = 0; i < crInstance.primitives.Count; i++) {

                        
                        LSystem.ISymbol symbol = lSystem.lString.symbols[i];
                        if (symbol.alphabet == 'B') {
                            sw.WriteLine("B" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {

                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                                    GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                                    GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'D') {
                            sw.WriteLine("D" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_N) {
                                    GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParam;
                                    sw.WriteLine("dominoN: " + dParam.n.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_GAP) {
                                    GeoParam.CR_D_GAP dParam = (GeoParam.CR_D_GAP)geoParam;
                                    sw.WriteLine("gap: " + dParam.gap.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'P') {
                            sw.WriteLine("P" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_W) {
                                    GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParam;
                                    sw.WriteLine("wireWidth: " + pParam.width.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_CD) {
                                    GeoParam.CR_P_CupDepth pParam = (GeoParam.CR_P_CupDepth)geoParam;
                                    sw.WriteLine("cupDepth: " + pParam.cupDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BD) {
                                    GeoParam.CR_P_BoardtDepth pParam = (GeoParam.CR_P_BoardtDepth)geoParam;
                                    sw.WriteLine("boardDepth: " + pParam.boardDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BF) {
                                    GeoParam.CR_P_BoardtFirst pParam = (GeoParam.CR_P_BoardtFirst)geoParam;
                                    sw.WriteLine("boardFirst: " + pParam.boardFirst.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'L') {
                            sw.WriteLine("L" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_BO) {
                                    GeoParam.CR_L_BallOffset bParam = (GeoParam.CR_L_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_SL) {
                                    GeoParam.CR_L_SlopeLength bParam = (GeoParam.CR_L_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_OP) {
                                    GeoParam.CR_L_OutPoint bParam = (GeoParam.CR_L_OutPoint)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.out_points.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'C') {
                            sw.WriteLine("C" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                            }
                        }
                    }
                    sw.WriteLine(" ");
                }
            }
            else {
                folder = @"D:\L_MCTS\MCTS_random\" + date + "_" + target.ToString() + "\\";
                string fileName = "MCTS_random" + ".txt";
                string fullPath = folder + fileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, true)) {
                    sw.WriteLine("Turn " + t.ToString());
                    sw.WriteLine("Fitness: " + node.fitness.ToString());
                    for (int i = 0; i < crInstance.primitives.Count; i++) {
                        ChainPrimitive primitive = crInstance.primitives[i];
                        primitive.reset();
                        LSystem.ISymbol symbol = lSystem.lString.symbols[i];
                        if (symbol.alphabet == 'B') {
                            sw.WriteLine("B" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {

                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                                    GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                                    GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'D') {
                            sw.WriteLine("D" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_N) {
                                    GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParam;
                                    sw.WriteLine("dominoN: " + dParam.n.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_GAP) {
                                    GeoParam.CR_D_GAP dParam = (GeoParam.CR_D_GAP)geoParam;
                                    sw.WriteLine("gap: " + dParam.gap.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'P') {
                            sw.WriteLine("P" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_W) {
                                    GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParam;
                                    sw.WriteLine("wireWidth: " + pParam.width.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_CD) {
                                    GeoParam.CR_P_CupDepth pParam = (GeoParam.CR_P_CupDepth)geoParam;
                                    sw.WriteLine("cupDepth: " + pParam.cupDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BD) {
                                    GeoParam.CR_P_BoardtDepth pParam = (GeoParam.CR_P_BoardtDepth)geoParam;
                                    sw.WriteLine("boardDepth: " + pParam.boardDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BF) {
                                    GeoParam.CR_P_BoardtFirst pParam = (GeoParam.CR_P_BoardtFirst)geoParam;
                                    sw.WriteLine("boardFirst: " + pParam.boardFirst.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'L') {
                            sw.WriteLine("L" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_BO) {
                                    GeoParam.CR_L_BallOffset bParam = (GeoParam.CR_L_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_SL) {
                                    GeoParam.CR_L_SlopeLength bParam = (GeoParam.CR_L_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_OP) {
                                    GeoParam.CR_L_OutPoint bParam = (GeoParam.CR_L_OutPoint)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.out_points.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'C') {
                            sw.WriteLine("C" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                            }
                        }
                    }
                    sw.WriteLine(" ");
                }
            }
        }
        else {
            if (this.inverse) {
                folder = @"D:\L_MCTS\Random_inverse\" + date + "_" + target.ToString() + "\\";
                string fileName = "Random_inverse" + ".txt";
                string fullPath = folder + fileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, true)) {
                    sw.WriteLine("Turn " + t.ToString());
                    sw.WriteLine("Fitness: " + node.fitness.ToString());
                    for (int i = 0; i < crInstance.primitives.Count; i++) {
                        ChainPrimitive primitive = crInstance.primitives[i];
                        primitive.reset();
                        LSystem.ISymbol symbol = lSystem.lString.symbols[i];
                        if (symbol.alphabet == 'B') {
                            sw.WriteLine("B" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {

                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                                    GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                                    GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'D') {
                            sw.WriteLine("D" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_N) {
                                    GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParam;
                                    sw.WriteLine("dominoN: " + dParam.n.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_GAP) {
                                    GeoParam.CR_D_GAP dParam = (GeoParam.CR_D_GAP)geoParam;
                                    sw.WriteLine("gap: " + dParam.gap.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'P') {
                            sw.WriteLine("P" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_W) {
                                    GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParam;
                                    sw.WriteLine("wireWidth: " + pParam.width.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_CD) {
                                    GeoParam.CR_P_CupDepth pParam = (GeoParam.CR_P_CupDepth)geoParam;
                                    sw.WriteLine("cupDepth: " + pParam.cupDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BD) {
                                    GeoParam.CR_P_BoardtDepth pParam = (GeoParam.CR_P_BoardtDepth)geoParam;
                                    sw.WriteLine("boardDepth: " + pParam.boardDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BF) {
                                    GeoParam.CR_P_BoardtFirst pParam = (GeoParam.CR_P_BoardtFirst)geoParam;
                                    sw.WriteLine("boardFirst: " + pParam.boardFirst.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'L') {
                            sw.WriteLine("L" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_BO) {
                                    GeoParam.CR_L_BallOffset bParam = (GeoParam.CR_L_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_SL) {
                                    GeoParam.CR_L_SlopeLength bParam = (GeoParam.CR_L_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_OP) {
                                    GeoParam.CR_L_OutPoint bParam = (GeoParam.CR_L_OutPoint)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.out_points.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'C') {
                            sw.WriteLine("C" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                            }
                        }
                    }
                    sw.WriteLine(" ");
                }
            }
            else {
                folder = @"D:\L_MCTS\Random_random\" + date + "_" + target.ToString() + "\\";
                string fileName = "Random_random" + ".txt";
                string fullPath = folder + fileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, true)) {
                    sw.WriteLine("Turn " + t.ToString());
                    sw.WriteLine("Fitness: " + node.fitness.ToString());
                    for (int i = 0; i < crInstance.primitives.Count; i++) {
                        ChainPrimitive primitive = crInstance.primitives[i];
                        primitive.reset();
                        LSystem.ISymbol symbol = lSystem.lString.symbols[i];
                        if (symbol.alphabet == 'B') {
                            sw.WriteLine("B" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {

                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                                    GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                                    GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'D') {
                            sw.WriteLine("D" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_N) {
                                    GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParam;
                                    sw.WriteLine("dominoN: " + dParam.n.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_D_GAP) {
                                    GeoParam.CR_D_GAP dParam = (GeoParam.CR_D_GAP)geoParam;
                                    sw.WriteLine("gap: " + dParam.gap.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'P') {
                            sw.WriteLine("P" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_W) {
                                    GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParam;
                                    sw.WriteLine("wireWidth: " + pParam.width.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_CD) {
                                    GeoParam.CR_P_CupDepth pParam = (GeoParam.CR_P_CupDepth)geoParam;
                                    sw.WriteLine("cupDepth: " + pParam.cupDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BD) {
                                    GeoParam.CR_P_BoardtDepth pParam = (GeoParam.CR_P_BoardtDepth)geoParam;
                                    sw.WriteLine("boardDepth: " + pParam.boardDepth.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_P_BF) {
                                    GeoParam.CR_P_BoardtFirst pParam = (GeoParam.CR_P_BoardtFirst)geoParam;
                                    sw.WriteLine("boardFirst: " + pParam.boardFirst.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'L') {
                            sw.WriteLine("L" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                    GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                    sw.WriteLine("dir: " + dirParam.dir.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_BO) {
                                    GeoParam.CR_L_BallOffset bParam = (GeoParam.CR_L_BallOffset)geoParam;
                                    sw.WriteLine("ballOffset: " + bParam.offset.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_SL) {
                                    GeoParam.CR_L_SlopeLength bParam = (GeoParam.CR_L_SlopeLength)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.length.ToString());
                                }
                                else if (geoParam.type == GeoParam.GeoParamType.CR_L_OP) {
                                    GeoParam.CR_L_OutPoint bParam = (GeoParam.CR_L_OutPoint)geoParam;
                                    sw.WriteLine("slopeLength: " + bParam.out_points.ToString());
                                }
                            }
                        }
                        else if (symbol.alphabet == 'C') {
                            sw.WriteLine("C" + i.ToString());
                            for (int j = 0; j < geoParams[i].Count; j++) {
                                GeoParam.IGeoParam geoParam = geoParams[i][j];
                                if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                    GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                    sw.WriteLine("position: " + posParam.pos.ToString());
                                }
                            }
                        }
                    }
                    sw.WriteLine(" ");
                }
            }
        }

    }

    public void reset() {
        crInstance.clearScene();
    }

    public void initScene() {
        crInstance.clearScene();

        recorder = crInstance.primitiveRoot.GetComponent<RecordTransformHierarchy>();
        int counter = 0;
        for (int i = 0; i < lSystem.lString.symbols.Count; i++) {
            LSystem.ISymbol symbol = lSystem.lString.symbols[i];
            if (symbol.alphabet == 'B') {
                crInstance.addBall();
                crInstance.primitives[crInstance.primitives.Count - 1].name = "BallOnSlope" + crInstance.primitiveRoot.transform.childCount.ToString();
            }
            else if (symbol.alphabet == 'D') {
                crInstance.addDominios();
                crInstance.primitives[crInstance.primitives.Count - 1].name = "Dominos" + crInstance.primitiveRoot.transform.childCount.ToString();
            }
            else if (symbol.alphabet == 'P') {
                crInstance.addUWire();
                crInstance.primitives[crInstance.primitives.Count - 1].name = "Rope" + crInstance.primitiveRoot.transform.childCount.ToString();
            }
            else if (symbol.alphabet == 'L') {
                crInstance.addBilliard();
                crInstance.primitives[crInstance.primitives.Count - 1].name = "Billiard" + crInstance.primitiveRoot.transform.childCount.ToString();
            }
            else if (symbol.alphabet == 'C') {
                crInstance.addCup();
                crInstance.primitives[crInstance.primitives.Count - 1].name = "Cup" + crInstance.primitiveRoot.transform.childCount.ToString();
            }
            else if (symbol.alphabet == '[' || symbol.alphabet == ']') {
                crInstance.addEmpty();
            }
        }

        setName();
    }

    public void setName() {
        int counter = 0;
        for (int i = 0; i < lSystem.lString.symbols.Count; i++) {
            LSystem.ISymbol symbol = lSystem.lString.symbols[i];
            if (symbol.alphabet == 'B') {
                crInstance.primitives[i].name = "BallOnSlope" + i.ToString();
            }
            else if (symbol.alphabet == 'D') {
                crInstance.primitives[i].name = "Dominos" + i.ToString();
            }
            else if (symbol.alphabet == 'P') {
                crInstance.primitives[i].name = "Rope" + i.ToString();
            }
            else if (symbol.alphabet == 'L') {
                crInstance.primitives[i].name = "Billiard" + i.ToString();
            }
            else if (symbol.alphabet == 'C') {
                crInstance.primitives[i].name = "Cup" + i.ToString();
            }
        }
    }

    public void setAnimation(int i) {
        Animation animation = crInstance.primitiveRoot.GetComponent<Animation>();
        clips[i].Item1.legacy = true;
        var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(clips[i].Item1);
        Debug.Log(clips.Count);
        Debug.Log(candidates.Count);
        animation.clip = clips[i].Item1;
        Debug.Log(clips.Count);
        Debug.Log(candidates.Count);
        animation.RemoveClip("candidate");
        Debug.Log(clips.Count);
        Debug.Log(candidates.Count);
        animation.AddClip(clips[i].Item1, "candidate");
        Debug.Log(clips.Count);
        Debug.Log(candidates.Count);
        animation.Play("candidate");
    }

    public void disablePhysic() {
        
        for (int i = 0; i < lSystem.lString.symbols.Count; i++) {
            Component[] rigibodys;
            LSystem.ISymbol symbol = lSystem.lString.symbols[i];
            if (symbol.alphabet == 'B') {
                rigibodys = crInstance.primitives[i].GetComponentsInChildren<Rigidbody>();
            }
            else if (symbol.alphabet == 'D') {
                rigibodys = crInstance.primitives[i].GetComponentsInChildren<Rigidbody>();
            }
            else if (symbol.alphabet == 'P') {
                rigibodys = crInstance.primitives[i].GetComponentsInChildren<Rigidbody>();
                UWire uwire = crInstance.primitives[i].GetComponent<UWire>();
                uwire.animation = true;
                //Destroy(uwire);
            }
            else if (symbol.alphabet == 'L') {
                rigibodys = crInstance.primitives[i].GetComponentsInChildren<Rigidbody>();
            }
            else if (symbol.alphabet == 'C') {
                rigibodys = crInstance.primitives[i].GetComponentsInChildren<Rigidbody>();
            }
            else {
                rigibodys = crInstance.primitiveRoot.GetComponentsInChildren<Rigidbody>();
            }
            foreach (Rigidbody rigidbody in rigibodys) {
                Destroy(rigidbody);
            }
        }
    }

    public void setCheckpoint() {
        var stack = new Stack<int>();
        int id = 0;
        int last = -1;
        string sentence = lSystem.lString.toString();
        bool inside = false;
        for (int i = 0; i < sentence.Length; i++) {
            char c = sentence[i];
            int curr = -1;

            switch (c) {
                case '[':
                    stack.Push(last);
                    inside = true;
                    break;
                case ']':
                    last = stack.Pop();
                    inside = false;
                    break;
                default:
                    curr = id;
                    break;
            }
            id++;

            if (curr != -1) {
                if (last != -1) {
                    ChainPrimitive primitive = crInstance.primitives[last];
                    ChainPrimitive nextPrimitive = crInstance.primitives[curr];
                    LSystem.ISymbol symbol = lSystem.lString.symbols[last];
                    LSystem.ISymbol nextSymbol = lSystem.lString.symbols[curr];
                    primitive.targetSymbol = nextSymbol.alphabet;
                    switch (symbol.alphabet) {
                        case 'B':
                            primitive.target = nextPrimitive.gameObject.name;
                            Transform ball = primitive.transform.GetChild(0);
                            ball.gameObject.AddComponent<Checkpoint>();
                            break;
                        case 'D':
                            primitive.target = nextPrimitive.gameObject.name;
                            int count = primitive.transform.childCount;
                            Transform d = primitive.transform.GetChild(count - 1);
                            d.gameObject.AddComponent<Checkpoint>();
                            break;
                        case 'P':
                            primitive.target = nextPrimitive.gameObject.name;
                            primitive.hitTarget = true;
                            int children = primitive.transform.GetChild(0).childCount;
                            Transform board = primitive.transform.GetChild(0).GetChild(children - 10);
                            board.gameObject.AddComponent<Checkpoint>();
                            break;
                        case 'L':
                            primitive.targets.Add(nextPrimitive.gameObject.name);
                            primitive.hitTargets.Add(false);
                            var billiard = (Billiard)primitive;
                            Transform billiardBall;
                            //Debug.Log(curr);
                            if (inside) {
                                billiardBall = primitive.transform.GetChild(1 + billiard.targets.Count);
                                billiardBall.gameObject.AddComponent<Checkpoint>();
                            }
                            else {
                                billiardBall = primitive.transform.GetChild(0);
                                billiardBall.gameObject.AddComponent<Checkpoint>();
                            }
                            break;
                    }
                }
                last = curr;
            }
            
        }
        //for (int i = 0; i < crInstance.primitives.Count - 1; i++) {
        //    ChainPrimitive primitive = crInstance.primitives[i];
        //    ChainPrimitive nextPrimitive = crInstance.primitives[i + 1];
        //    LSystem.ISymbol symbol = lSystem.lString.symbols[i];
        //    LSystem.ISymbol nextSymbol = lSystem.lString.symbols[i + 1];
        //    primitive.target = nextPrimitive.gameObject.name;
        //    primitive.targetSymbol = nextSymbol.alphabet;
        //    switch (symbol.alphabet) {
        //        case 'B':
        //            Transform ball = primitive.transform.GetChild(0);
        //            ball.gameObject.AddComponent<Checkpoint>();
        //            break;
        //        case 'D':
        //            int count = primitive.transform.childCount;
        //            Transform last = primitive.transform.GetChild(count - 1);
        //            last.gameObject.AddComponent<Checkpoint>();
        //            break;
        //        case 'P':
        //            int c = primitive.transform.GetChild(0).childCount;
        //            Transform board = primitive.transform.GetChild(0).GetChild(c-10);
        //            board.gameObject.AddComponent<Checkpoint>();
        //            break;
        //        case 'L':
        //            Transform billiard = primitive.transform.GetChild(0);
        //            billiard.gameObject.AddComponent<Checkpoint>();
        //            break;
        //    }
        //}
    }

    public void genRandomGeoParams() {
        GeoParam.CR_GeoEmbedder embedder = new GeoParam.CR_GeoEmbedder();
        geoParams = embedder.geoEmbbed(this.lSystem.lString, true);
    }

    public void applyGeoParams() {
        for (int i = 0; i < crInstance.primitives.Count; i++) {
            ChainPrimitive primitive = crInstance.primitives[i];
            primitive.reset();
            LSystem.ISymbol symbol = lSystem.lString.symbols[i];
            if (symbol.alphabet == 'B') {
                applyGeoParams((BallOnSlope)primitive, geoParams[i]);
            }
            else if (symbol.alphabet == 'D') {
                applyGeoParams((Dominos)primitive, geoParams[i]);
            }
            else if (symbol.alphabet == 'P') {
                applyGeoParams((UWire)primitive, geoParams[i]);
            }
            else if (symbol.alphabet == 'L') {
                applyGeoParams((Billiard)primitive, geoParams[i]);
            }
            else if (symbol.alphabet == 'C') {
                applyGeoParams((Cup)primitive, geoParams[i]);
            }
        }
        crInstance.genFloors();
    }

    private void applyGeoParams(BallOnSlope b, List<GeoParam.IGeoParam> geoParams) {
        for (int i = 0; i < geoParams.Count; i++) {
            GeoParam.IGeoParam geoParam = geoParams[i];
            if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                b.transform.position = posParam.pos;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                Vector3 scale = b.transform.localScale;
                scale.x = dirParam.dir;
                b.transform.localScale = scale;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                b.ballOffset = bParam.offset;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                b.slopeLength = bParam.length;
                b.time = 0.36f * bParam.length;
            }
        }
        b.updateSlope();
    }
    private void applyGeoParams(Dominos d, List<GeoParam.IGeoParam> geoParams) {
        for (int i = 0; i < geoParams.Count; i++) {
            GeoParam.IGeoParam geoParam = geoParams[i];
            if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                d.transform.position = posParam.pos;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                if (dirParam.dir < 0) {
                    d.transform.localRotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                }
                else {
                    d.transform.localRotation = Quaternion.Euler(0.0f, 90, 0.0f);
                }
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_D_N) {
                GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParam;
                d.dominoN = dParam.n;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_D_GAP) {
                GeoParam.CR_D_GAP dParam = (GeoParam.CR_D_GAP)geoParam;
                d.gap = dParam.gap;
                d.time = 0.785f * dParam.gap * (d.dominoN - 1);
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_D_TYPE) {
                GeoParam.CR_D_TYPE dParam = (GeoParam.CR_D_TYPE)geoParam;
                d.type = dParam.type;
            }
        }
        d.genDominos();
    }
    private void applyGeoParams(UWire p, List<GeoParam.IGeoParam> geoParams) {
        for (int i = 0; i < geoParams.Count; i++) {
            GeoParam.IGeoParam geoParam = geoParams[i];
            if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                p.transform.position = posParam.pos;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                Vector3 scale = p.transform.localScale;
                scale.x = dirParam.dir;
                p.transform.localScale = scale;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_P_W) {
                GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParam;
                p.wireWidth = pParam.width;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_P_CD) {
                GeoParam.CR_P_CupDepth pParam = (GeoParam.CR_P_CupDepth)geoParam;
                p.cupDepth = pParam.cupDepth;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_P_BD) {
                GeoParam.CR_P_BoardtDepth pParam = (GeoParam.CR_P_BoardtDepth)geoParam;
                p.boardDepth = pParam.boardDepth;
                p.time = 0.2f;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_P_BF) {
                GeoParam.CR_P_BoardtFirst pParam = (GeoParam.CR_P_BoardtFirst)geoParam;
                p.boardFirst = pParam.boardFirst;
            }
        }
        p.updateUWire();
    }
    private void applyGeoParams(Billiard l, List<GeoParam.IGeoParam> geoParams) {
        for (int i = 0; i < geoParams.Count; i++) {
            GeoParam.IGeoParam geoParam = geoParams[i];
            if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                l.transform.position = posParam.pos;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                Vector3 scale = l.transform.localScale;
                scale.x = dirParam.dir;
                l.transform.localScale = scale;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_L_BO) {
                GeoParam.CR_L_BallOffset bParam = (GeoParam.CR_L_BallOffset)geoParam;
                l.ballOffset = bParam.offset;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_L_SL) {
                GeoParam.CR_L_SlopeLength bParam = (GeoParam.CR_L_SlopeLength)geoParam;
                l.slopeLength = bParam.length;
                l.time = 0.36f * bParam.length;
            }
            else if (geoParam.type == GeoParam.GeoParamType.CR_L_OP) {
                GeoParam.CR_L_OutPoint bParam = (GeoParam.CR_L_OutPoint)geoParam;
                l.outPoints = bParam.out_points;
            }
        }
        l.updateSlope();
    }
    private void applyGeoParams(Cup c, List<GeoParam.IGeoParam> geoParams) {
        for (int i = 0; i < geoParams.Count; i++) {
            GeoParam.IGeoParam geoParam = geoParams[i];
            if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                c.transform.position = posParam.pos;
            }
        }
        c.isEnd = true;
    }

    public List<int> setTarget() {
        List<int> ends = new List<int>();
        for (int i = 0; i < crInstance.primitives.Count; i++) {
            if (lSystem.lString.symbols[i].alphabet == 'C') {
                ChainPrimitive primitive = crInstance.primitives[i - 1];
                ChainPrimitive last = crInstance.primitives[i];
                foreach (Transform t in primitive.transform) {
                    t.gameObject.tag = "Target";
                }
                primitive.transform.gameObject.tag = "Target";

                Goal other = last.transform.gameObject.GetComponent<Goal>();
                if (other != null) {
                    Destroy(other);
                }
                last.transform.gameObject.AddComponent<Goal>();
                ends.Add(i);
            }
        }
        //ChainPrimitive primitive = crInstance.primitives[crInstance.primitives.Count - 2];
        //ChainPrimitive last = crInstance.primitives[crInstance.primitives.Count - 1];
        //foreach (Transform t in primitive.transform) {
        //    t.gameObject.tag = "Target";
        //}
        //primitive.transform.gameObject.tag = "Target";

        //Goal other = last.transform.gameObject.GetComponent<Goal>();
        //if (other != null) {
        //    Destroy(other);
        //}
        //last.transform.gameObject.AddComponent<Goal>();
        return ends;
    }

    public IEnumerator checkCheckpoint(int i, float delay, ActionBool continueWith) {
        yield return new WaitForSeconds(i * delay);
        continueWith(crInstance.primitives[i].hitTarget);
    }

    public IEnumerator checkTarget(float delay, ActionGoal continueWith) {
        yield return new WaitForSeconds(delay);
        int c = crInstance.primitives.Count;
        Goal tmp = crInstance.primitives[c - 1].transform.GetComponent<Goal>();
        continueWith(tmp.goal);
    }

    public async Task<bool> AsyncCheckCheckpoint(int layer, int i, float delay, CancellationToken token) {
        await Task.Delay((int)((layer+1) * delay * 1000));
        return crInstance.primitives[i].hitTarget;
    }

    public async Task<System.Tuple<bool, float>> AsyncCheckTarget(int id, float delay, CancellationToken token) {
        await Task.Delay((int)(delay * 1000));
        int c = crInstance.primitives.Count;
        Goal tmp = crInstance.primitives[id].transform.GetComponent<Goal>();
        return tmp.goal;
    }

    #region MCTS
    public void setMCTS(LGSSolution solution) {
        var rootCollision = new LGSCollisionData(' ', 0);
        rootCollision.list_prev = solution.collisions.Last();
        root = new MCTSNode(solution, rootCollision, null, solution);
        root.elementType = LGSCollisionData.ElementType.ROOT;
    }

    public void setMCTS(LGSSolution solution, LGSSolution rootSolution) {
        var rootCollision = new LGSCollisionData(' ', 0);
        rootCollision.list_prev = solution.collisions.Last();
        root = new MCTSNode(solution, rootCollision, null, rootSolution);
        root.elementType = LGSCollisionData.ElementType.ROOT;
        solution.root = root;
        this.lSystem.lString = solution.lstring;
    }

    public static void PrintTree(LGSSolution tree, System.String indent, bool last) {
        Debug.Log(indent + "+- " + tree.lstring.toString() + ": " +tree.high_score.ToString());
        indent += last ? "   " : "|  ";

        for (int i = 0; i < tree.children.Count; i++) {
            PrintTree(tree.children[i], indent, i == tree.children.Count - 1);
        }
    }

    public static void PrintTree(MCTSNode tree, System.String indent, bool last) {
        Debug.Log(indent + "+- " + tree.elementType.ToString() + ": " + tree.Score().ToString());
        indent += last ? "   " : "|  ";

        for (int i = 0; i < tree.children.Count; i++) {
            PrintTree(tree.children[i], indent, i == tree.children.Count - 1);
        }
    }

    public void SetN(LGSSolution tree, LGSSolution sol) {
        if (sol != tree) {
            if (sol.lstring.toString() == tree.lstring.toString()) {
                if (tree.n >= sol.n) {
                    sol.n = tree.n + 1;
                    sol.w += tree.w;
                    sol.total_scroe += tree.total_scroe;
                }
                tree.n = sol.n;
                tree.w = sol.w;
                tree.total_scroe = sol.total_scroe;
            }
        }
        for (int i = 0; i < tree.children.Count; i++) {
            SetN(tree.children[i], sol);
        }
    }

    public void reCalScore(LGSSolution tree) {
        float score = tree.Score();
        tree.high_score = System.Single.IsNaN(score) ? tree.high_score : score;
        for (int i = 0; i < tree.children.Count; i++) {
            reCalScore(tree.children[i]);
        }
    }

    public List<int> layerAmount(int layer) {
        List<int> amountEachLayer = new List<int>();
        amountEachLayer.Add(1);
        for (int i = 1; i < layer + 1; i++) {
            amountEachLayer.Add(amountEachLayer[amountEachLayer.Count - 1] * (i + 1));
        }
        return amountEachLayer;
    }

    public int layerProb(List<int> amountList) {
        int sum = amountList.Take(amountList.Count - 1).Sum();
        int randomNum = Random.Range(0, sum);
        int layerSum = 0;
        int layer = 0;
        for (int i = 0; i < amountList.Count; i++) {
            layerSum += amountList[i];
            if (randomNum > layerSum - 1) {
                layer++;
            }
            else {
                break;
            }
        }
        return layer;
    }

    public async Task<bool> RandomGen(int l, bool inverse, int j) {
        Random.InitState(System.Guid.NewGuid().GetHashCode());
        candidates = new List<MCTSNode>();
        result = new List<MCTSNode>();
        clips = new List<System.Tuple<AnimationClip, float>>();

        var jump = 300000 / 300;
        jump = j;
        var verify = jump / 20;
        int goodEnough = 0;
        int betterAmount = 0;
        int bestAmount = 0;
        //verify = jump * 2;
        verify = 1;
        int generateLayer = l;
        this.inverse = inverse;


        List<List<LGSSolution>> layerList = this.root.rootSolution.genChildren(this, generateLayer);
        List<int> amountList = layerAmount(generateLayer);

        var startList = root.rootSolution.SelectList();
        int startLayer = layerProb(amountList);
        var startSolution = layerList[startLayer][Random.Range(0, layerList[startLayer].Count)];
        Debug.Log("solutionList: " + startList.Count.ToString());
        this.lSystem.lString = startSolution.lstring;
        if (startSolution.root == null)
            setMCTS(startSolution, root.rootSolution);
        else
            root = startSolution.root;
        

        for (int i = 1; i <= 300000; i++) {
            recorder = crInstance.primitiveRoot.GetComponent<RecordTransformHierarchy>();
            var chosenList = root.SelectList();
            var chosen = chosenList[Random.Range(0, chosenList.Count)];
            Debug.Log("Good: " + goodEnough.ToString());
            Debug.Log("Turn: ");
            Debug.Log(i);
            chosen = chosen.Expansion(this);
            MCTSNode node = null;
            node = (await chosen.SimulationNoAnimation(this)).Item2;
            chosen.Backpropagation(node.fitness);

            if (node.fitness > 0) {
                if (!candidates.Contains(node) && node.fitness != 0) {
                    int ts = 0;
                    for (int k = 0; k < candidates.Count; k++) {
                        if (candidates[k].fitness == node.fitness) {
                            if (candidates[k].id >= node.id)
                                ts = 1;
                            else
                                candidates[k] = node;
                            break;
                        }
                    }
                    if (ts == 0) {
                        if (node.fitness >= 0.95f) {
                            goodEnough++;
                        }
                        if (node.fitness >= 0.975f) {
                            betterAmount++;
                        }
                        if (node.fitness >= 0.99f) {
                            bestAmount++;
                        }
                        candidates.Add(node);
                        Destroy(recorder.clip);
                        //clips.Add(new System.Tuple<AnimationClip, float>(recorder.clip, node.fitness));
                        //var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(recorder.clip);

                        //if (clips.Count > 10) {
                        //    clips.Sort((x, y) => -x.Item2.CompareTo(y.Item2));
                        //    clips = clips.Take(10).ToList();
                        //}
                    }
                }
            }
            writeGeoParamToFile(false, i, node);
            if (i % verify == 0) {
                float score = 0;
                if (node.fitness != 0) {
                    score = endTime - startTime;
                }
                else {
                    if (root.solution.n == 0) {
                        if(root.solution.parent != null) {
                            if (root.solution.parent.n != null) {
                                if (root.solution.parent.n != 0) {
                                    float parent_score = root.solution.parent.total_scroe / root.solution.parent.n;
                                    score = parent_score + (root.solution.maxLayer - root.solution.parent.maxLayer);
                                }
                                else {
                                    score = root.solution.maxLayer;
                                }
                            }
                            else {
                                score = root.solution.maxLayer;
                            }
                        }
                        else {
                            score = root.solution.maxLayer;
                        }
                    }
                    else {
                        score = root.solution.total_scroe / root.solution.n;
                    }
                }
                root.solution.total_scroe += score;
                root.solution.n += 1;
                root.solution.w += node.fitness;
                SetN(root.rootSolution, root.solution);
                LGSSolution.total += 1;
                reCalScore(root.rootSolution);

                int layer = layerProb(amountList);
                var newSolution = layerList[layer][Random.Range(0, layerList[layer].Count)];
                Debug.Log(newSolution.high_score);
                Debug.Log(newSolution.lstring.toString());
                this.lSystem.lString = newSolution.lstring;
                float avg_score = newSolution.total_scroe / newSolution.n;
                Debug.Log("solutionList: " + startList.Count.ToString());
                this.lSystem.lString = newSolution.lstring;
                if (newSolution.root == null)
                    setMCTS(newSolution, root.rootSolution);
                else
                    root = newSolution.root;
            }
            
            if (i % jump == 0) {
                candidates.Sort((x, y) => -x.fitness.CompareTo(y.fitness));
                clips.Sort((x, y) => -x.Item2.CompareTo(y.Item2));
                result.AddRange(candidates);
                good = goodEnough;
                better = betterAmount;
                best = bestAmount;
                candidates = candidates.Take(10).ToList();
                clips = clips.Take(10).ToList();
                break;
            }
            //writeGeoParamToFile(false, i, node);
        }

        var final = candidates.FirstOrDefault();

        if (final != null) {
            this.lSystem.lString = final.solution.lstring;
            this.initScene();
            final.solution.Init(this);
            final.Apply(this.geoParams, this.crInstance);
            Debug.Log("Final");
            Debug.Log(final.fitness);
            this.finish = true;
            return this.finish;
        }
        else {
            return this.finish;
        }
    }

    public async Task<bool> Search(bool inverse) {
        Random.InitState(System.Guid.NewGuid().GetHashCode());
        candidates = new List<MCTSNode>();
        result = new List<MCTSNode>();
        clips = new List<System.Tuple<AnimationClip, float>>();

        var jump = 300000 / 300;
        var verify = jump / 20;
        int goodEnough = 0;
        int betterAmount = 0;
        int bestAmount = 0;
        //verify = jump * 2;
        verify = 1;
        this.inverse = inverse;
        //jump = 1;

        for (int i = 1; i <= 300000; i++) {
            recorder = crInstance.primitiveRoot.GetComponent<RecordTransformHierarchy>();
            var chosen = root.Select(0);
            Debug.Log("chosen: " + chosen.id.ToString());
            Debug.Log("Good: " + goodEnough.ToString());
            Debug.Log("Turn: ");
            Debug.Log(i);
            //Debug.Log(chosen.elementType);
            //Debug.Log(chosen.id);
            chosen = chosen.Expansion(this);
            MCTSNode node = null;
            node = (await chosen.Simulation(this)).Item2;
            chosen.Backpropagation(node.fitness);

            if (node.fitness > 0) {
                if (!candidates.Contains(node) && node.fitness != 0) {
                    int ts = 0;
                    for (int k = 0; k < candidates.Count; k++) {
                        if (candidates[k].fitness == node.fitness) {
                            if (candidates[k].id >= node.id)
                                ts = 1;
                            else
                                candidates[k] = node;
                            break;
                        }
                    }
                    if (ts == 0) {
                        if (node.fitness >= 0.95f) {
                            goodEnough++;
                        }
                        if (node.fitness >= 0.975f) {
                            betterAmount++;
                        }
                        if (node.fitness >= 0.99f) {
                            bestAmount++;
                        }
                        candidates.Add(node);
                        clips.Add(new System.Tuple<AnimationClip, float>(recorder.clip, node.fitness));
                        var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(recorder.clip);

                        if (clips.Count > 15) {
                            clips.Sort((x, y) => -x.Item2.CompareTo(y.Item2));
                            GameObject.Destroy(clips[15].Item1);
                            clips = clips.Take(15).ToList();
                        }
                        //Debug.Log("there's a clip");
                        //Debug.Log(curveBindings.Length);
                    }
                }
            }
            //writeGeoParamToFile(true, i, node);
            if (i % verify == 0) {
                float score = 0;
                if (node.fitness != 0) {
                    //score = endTime - startTime;
                    score = node.score;
                    //root.solution.w += node.fitness;
                }
                else {
                    if (root.solution.n == 0) {
                        //RBMScore();
                        //float parent_score = root.solution.parent.total_scroe / root.solution.parent.n;
                        //score = parent_score + (root.solution.maxLayer - root.solution.parent.maxLayer);
                        if (root.solution.parent != null) {
                            if (root.solution.parent.n != null) {
                                if (root.solution.parent.n != 0) {
                                    float parent_score = root.solution.parent.total_scroe / root.solution.parent.n;
                                    score = parent_score + (root.solution.maxLayer - root.solution.parent.maxLayer);
                                }
                                else {
                                    score = root.solution.maxLayer;
                                }
                            }
                            else {
                                score = root.solution.maxLayer;
                            }
                        }
                        else {
                            score = root.solution.maxLayer;
                        }
                        //score = RBMScore();
                        //if (parent_time > target) {
                        //    time = target * 2;
                        //}
                        //else {
                        //    time = parent_time + (root.solution.collisions.Count - root.solution.parent.collisions.Count);
                        //}
                        //root.solution.w += node.fitness;
                    }
                    else {
                        score = root.solution.total_scroe / root.solution.n;
                        //root.solution.w += root.solution.w / root.solution.n;
                    }
                }
                //float time = node.fitness != 0 ? (endTime - startTime) : (root.solution.n == 0 ? root.solution.parent.total_scroe / root.solution.parent.n : root.solution.total_scroe / root.solution.n);
                root.solution.total_scroe += score;

                //root.solution.n += 1;
                //root.solution.w += node.fitness;

                root.solution.Backpropagation(node.fitness);

                //SetN(root.rootSolution, root.solution);
                LGSSolution.total += 1;
                reCalScore(root.rootSolution);
                var newSolution = root.rootSolution.Select(root.rootSolution.high_score);
                Debug.Log(newSolution.high_score);
                Debug.Log(newSolution.lstring.toString());
                this.lSystem.lString = newSolution.lstring;
                float avg_score = newSolution.total_scroe / newSolution.n;

                int layer = 0;
                var tmpSolution = newSolution;
                bool longEnough = false;
                while (tmpSolution != null) {
                    layer++;
                    tmpSolution = tmpSolution.parent;
                    if (target < 6 && layer >= 5) {
                        longEnough = true;
                    }
                }
                if (!longEnough && avg_score <= target * 1.1 && newSolution.high_score != 0 && newSolution.children.Count == 0) {
                    newSolution.GetChildSolution(this);
                    newSolution = root.rootSolution.Select(root.rootSolution.high_score);
                }
                this.lSystem.lString = newSolution.lstring;
                if (newSolution.root == null)
                    setMCTS(newSolution, root.rootSolution);
                else
                    root = newSolution.root;
                PrintTree(root.rootSolution, "", true);
            }

            if (goodEnough == 10) {
                candidates.Sort((x, y) => -x.fitness.CompareTo(y.fitness));
                clips.Sort((x, y) => -x.Item2.CompareTo(y.Item2));
                result.AddRange(candidates);
                good = goodEnough;
                better = betterAmount;
                best = bestAmount;
                candidates = candidates.Take(10).ToList();
                clips = clips.Take(10).ToList();
                break;
            }

            if (i % jump == 0) {
                candidates.Sort((x, y) => -x.fitness.CompareTo(y.fitness));
                clips.Sort((x, y) => -x.Item2.CompareTo(y.Item2));
                result.AddRange(candidates);
                good = goodEnough;
                better = betterAmount;
                best = bestAmount;
                candidates = candidates.Take(15).ToList();
                clips = clips.Take(15).ToList();
                break;
            }
            //PrintTree(this.root, "", true);

            //writeGeoParamToFile(true, i, node);
        }

        var final = candidates.FirstOrDefault();

        if (final != null) {
            this.lSystem.lString = final.solution.lstring;
            this.initScene();
            final.solution.Init(this);
            final.Apply(this.geoParams, this.crInstance);
            Debug.Log("Final");
            Debug.Log(final.fitness);
            this.finish = true;
            return this.finish;
        }
        else {
            return this.finish;
        }
    }

    public float RBMScore() {
        float score = 0.0f;
        float difficulty = 0.0f;
        float dirChange = 0.0f;
        float branchBalance = 0.0f;
        const float ballMax = 5.0f;
        const float dominoMax = 10.0f;
        const float wireMax = 5.0f;
        const float billiardMax = 3.0f;
        const float ballW = 0.2f; //0.75
        const float dominoW = 2.85f; //1.5
        const float dominoTypeW = 0.3f;
        const float wireW = 0.2f; //1
        const float billiardW = 2.85f;
        const float balanceW = 4.125f; //1.25
        const float layerW = 3f;
        const float directionW = 0.5f;
        const float localW = 5.5f;
        const float globalW = 1.5f;

        for (int i = 0; i < crInstance.primitives.Count; i++) {
            LSystem.ISymbol symbol = lSystem.lString.symbols[i];
            switch (symbol.alphabet) {
                case 'B':
                    GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParams[i][2];

                    difficulty += bParam.length / ballMax * ballW;
                    break;
                case 'D':
                    GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParams[i][1];
                    difficulty += dParam.n / dominoMax * dominoW;
                    GeoParam.CR_D_TYPE dType = (GeoParam.CR_D_TYPE)geoParams[i][3];
                    if (dType.type != 0) {
                        difficulty += dominoTypeW;
                    }
                    break;
                case 'P':
                    GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParams[i][1];
                    difficulty += pParam.width / wireMax * wireW;
                    break;
                case 'L':
                    GeoParam.CR_L_OutPoint lParam = (GeoParam.CR_L_OutPoint)geoParams[i][geoParams[i].Count - 1];
                    int k = i;
                    List<int> unitCounts = new List<int>();
                    for (int j = 0; j < lParam.out_points.Count; j++) {
                        int unitCount = 0;
                        for (; lSystem.lString.symbols[k].alphabet != ']'; k++) {
                            unitCount++;
                        }
                        unitCounts.Add(unitCount - 1);
                    }
                    unitCounts.Add(0);
                    for (; k < crInstance.primitives.Count; k++) {
                        if (lSystem.lString.symbols[k].alphabet == '[') {
                            break;
                        }
                        unitCounts[unitCounts.Count - 1]++;
                    }
                    int max = 0;
                    for (int j = 0; j < unitCounts.Count; j++) {
                        max = unitCounts[j] > max ? unitCounts[j] : max;
                    }
                    for (int j = 0; j < unitCounts.Count; j++) {
                        float point = ((float)unitCounts[j] / max) / (float)unitCounts.Count * 3 * balanceW;
                        branchBalance += point;
                    }
                    if (unitCounts.Count == 2) {
                        difficulty += 0.8f * billiardW;
                    }
                    else {
                        difficulty += unitCounts.Count / billiardMax * billiardW;
                    }
                    break;
                case 'C':
                    difficulty += 0.0f;
                    break;
            }
            if (symbol.alphabet != 'C' && symbol.alphabet != '[' && symbol.alphabet != ']' && i > 0) {
                GeoParam.CR_Direction currDir = (GeoParam.CR_Direction)geoParams[i][0];
                GeoParam.CR_Direction prevDir = null;
                if (lSystem.lString.symbols[i - 1].alphabet == '[' || lSystem.lString.symbols[i - 1].alphabet == ']') {
                    for (int j = i; j >= 0; j--) {
                        if (lSystem.lString.symbols[j].alphabet == 'L') {
                            prevDir = (GeoParam.CR_Direction)geoParams[j][0];
                            break;
                        }
                    }
                }
                else {
                    Debug.Log(i);
                    prevDir = (GeoParam.CR_Direction)geoParams[i - 1][0];
                }
                if (currDir.dir != prevDir.dir) {
                    dirChange += 1;
                }
            }
        }
        difficulty *= localW;
        float globalScore = globalW * (dirChange + layerW * root.solution.maxLayer + branchBalance);
        score = difficulty + globalScore;
        Debug.Log(difficulty);
        return score;
    }

    /*LGSCollisionData
    /*撞到的是誰 撞到該做甚麼 該往哪裡*/
    /*LGSSolution*/
    /*存取了很多CollisionData 可以用來儲存你選擇的結果  計算這個結果的一些資訊*/
    public class MCTSNode : LGSCollisionData {
        public LGSCollisionData curr;
        public LGSSolution solution;
        public LGSSolution rootSolution;
        public MCTSNode parent;

        public int n = 0;
        public float w = 0.0f;
        public float fitness;
        public float score;

        public List<MCTSNode> possibleStpes = new List<MCTSNode>();

        public List<MCTSNode> children = new List<MCTSNode>();

        public List<GeoParam.IGeoParam> geoParams;
        public List<GeoParam.IGeoParam> parentGeoParams;

        //public int count = GameObject.Find("LGS").GetComponent<LevelGeneratorSystem>().count;

        public MCTSNode(LGSSolution solution, LGSCollisionData collision, MCTSNode parent, LGSSolution rootSolution) : base(collision) {
            this.solution = solution;
            this.rootSolution = rootSolution;
            this.list_prev = collision.list_prev;
            this.list_next = collision.list_next;
            this.prev = collision.prev;
            this.next = collision.next;
            this.backward = collision.backward;
            this.id = collision.id;
            this.layer = collision.layer;
            this.elementType = collision.elementType;
            this.curr = this;
            this.n = collision.n;
            this.w = collision.w;
            

            if (parent != null) {
                layer = parent.layer + 1;
                this.parent = parent;
                if (parent.geoParams != null) {
                    parentGeoParams = new List<GeoParam.IGeoParam>();
                    for (int i = 0; i < parent.geoParams.Count; i++) {
                        if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_POSITION) {
                            parentGeoParams.Add(new GeoParam.CR_Position((GeoParam.CR_Position)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_DIRECTION) {
                            parentGeoParams.Add(new GeoParam.CR_Direction((GeoParam.CR_Direction)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_B_BO) {
                            parentGeoParams.Add(new GeoParam.CR_B_BallOffset((GeoParam.CR_B_BallOffset)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_B_SL) {
                            parentGeoParams.Add(new GeoParam.CR_B_SlopeLength((GeoParam.CR_B_SlopeLength)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_D_N) {
                            parentGeoParams.Add(new GeoParam.CR_D_N((GeoParam.CR_D_N)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_D_GAP) {
                            parentGeoParams.Add(new GeoParam.CR_D_GAP((GeoParam.CR_D_GAP)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_D_TYPE) {
                            parentGeoParams.Add(new GeoParam.CR_D_TYPE((GeoParam.CR_D_TYPE)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_P_W) {
                            parentGeoParams.Add(new GeoParam.CR_P_Width((GeoParam.CR_P_Width)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_P_CD) {
                            parentGeoParams.Add(new GeoParam.CR_P_CupDepth((GeoParam.CR_P_CupDepth)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_P_BD) {
                            parentGeoParams.Add(new GeoParam.CR_P_BoardtDepth((GeoParam.CR_P_BoardtDepth)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_P_BF) {
                            parentGeoParams.Add(new GeoParam.CR_P_BoardtFirst((GeoParam.CR_P_BoardtFirst)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_L_BO) {
                            parentGeoParams.Add(new GeoParam.CR_L_BallOffset((GeoParam.CR_L_BallOffset)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_L_SL) {
                            parentGeoParams.Add(new GeoParam.CR_L_SlopeLength((GeoParam.CR_L_SlopeLength)parent.geoParams[i]));
                        }
                        else if (parent.geoParams[i].type == GeoParam.GeoParamType.CR_L_OP) {
                            parentGeoParams.Add(new GeoParam.CR_L_OutPoint((GeoParam.CR_L_OutPoint)parent.geoParams[i]));
                        }
                    }
                }
                //this.parentGeoParams = parent.geoParams.ConvertAll(param => new Book(book.title));
            }
        }
        public object Clone() {
            return this.MemberwiseClone();
        }

        public float Score() {
            if(parent == null) {
                return w / n + Mathf.Sqrt(2 * Mathf.Log(n) / n);
            }
            return w / n + Mathf.Sqrt(2 * Mathf.Log(parent.n) / n);
        }

        public List<MCTSNode> SelectList() {
            var result = children.SelectMany(i => i.SelectList()).ToList();

            result.Add(this);

            return result.ToList();
            //var result = new List<MCTSNode>();
            //result.Add(this);
            //result.AddRange(children);
            //children.ForEach(x => result.AddRange(x.children));
            //return result;
        }

        public MCTSNode Select(float maxScore) {
            if (children.Count > 0) {
                MCTSNode chosen = null;

                foreach (var child in children) {
                    if (child.n > 0) {
                        var score = child.Score();

                        if (score >= maxScore) {
                            maxScore = score;
                            chosen = child;
                        }

                    }
                    else {
                        chosen = child;
                        break;
                    }

                }
                //try {
                //    return chosen.Select();
                //}
                //catch(System.NullReferenceException) {

                //}
                if (chosen == null) {
                    return this;
                }
                else {
                    return chosen.Select(maxScore);
                }
            }
            else {
                return this;
            }
        }

        public MCTSNode HighScore(float maxScore) {
            if (children.Count > 0) {
                MCTSNode chosen = null;

                foreach (var child in children) {
                    if (child.n > 0) {
                        var score = child.Score();

                        if (score >= maxScore) {
                            maxScore = score;
                            chosen = child;
                        }

                    }
                }
                if (chosen == null) {
                    return this;
                }
                else {
                    return chosen.Select(maxScore);
                }
            }
            else {
                return this;
            }
        }

        public List<MCTSNode> GetPossibleSteps(CR_LMCTS lmcts) {
            // result to return
            var next = this.list_prev;
            //if (next.next.id == this.id) {
            //    next.next = this;
            //}
            bool random = !lmcts.inverse;
            MCTSNode childParent = this;

            if (next != null) {
                possibleStpes = new List<MCTSNode>();
                if (possibleStpes.Count == 0) {
                    GeoParam.CR_GeoEmbedder embedder = new GeoParam.CR_GeoEmbedder();
                    List <GeoParam.IGeoParam> geoParams;
                    switch (next.elementType) {
                        case ElementType.B:
                            for (int x = 0; x < 1; x++) {
                                geoParams = embedder.charGeoEmbbed(next, next.next, next.backward, random);
                                if (next.next is MCTSNode) {
                                    childParent = (MCTSNode)next.next;
                                }
                                var child = new MCTSNode(solution, next, childParent, rootSolution);
                                child.geoParams = geoParams;
                                for (int i = 0; i < geoParams.Count; i++) {
                                    GeoParam.IGeoParam geoParam = geoParams[i];
                                    if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                        GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                        child.position = posParam.pos;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                        GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                        child.dir = dirParam.dir;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_B_BO) {
                                        GeoParam.CR_B_BallOffset bParam = (GeoParam.CR_B_BallOffset)geoParam;
                                        child.ballOffset = bParam.offset;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_B_SL) {
                                        GeoParam.CR_B_SlopeLength bParam = (GeoParam.CR_B_SlopeLength)geoParam;
                                        child.slopeLength = bParam.length;
                                    }
                                }
                                child.id = next.id;
                                possibleStpes.Add(child);
                            }
                            break;
                        case ElementType.D:
                            for (int x = 0; x < 1; x++) {
                                geoParams = embedder.charGeoEmbbed(next, next.next, next.backward, random);
                                if (next.next is MCTSNode) {
                                    childParent = (MCTSNode)next.next;
                                }
                                var child = new MCTSNode(solution, next, childParent, rootSolution);
                                child.geoParams = geoParams;
                                for (int i = 0; i < geoParams.Count; i++) {
                                    GeoParam.IGeoParam geoParam = geoParams[i];
                                    if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                        GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                        child.position = posParam.pos;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                        GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                        child.dir = dirParam.dir;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_D_N) {
                                        GeoParam.CR_D_N dParam = (GeoParam.CR_D_N)geoParam;
                                        child.dominoN = dParam.n;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_D_GAP) {
                                        GeoParam.CR_D_GAP dParam = (GeoParam.CR_D_GAP)geoParam;
                                        child.dominoGap = dParam.gap;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_D_TYPE) {
                                        GeoParam.CR_D_TYPE dParam = (GeoParam.CR_D_TYPE)geoParam;
                                        child.dominoType = dParam.type;
                                    }
                                }
                                child.id = next.id;
                                possibleStpes.Add(child);
                            }
                            break;
                        case ElementType.P:
                            for (int x = 0; x < 1; x++) {
                                geoParams = embedder.charGeoEmbbed(next, next.next, next.backward, random);
                                if (next.next is MCTSNode) {
                                    childParent = (MCTSNode)next.next;
                                }
                                var child = new MCTSNode(solution, next, childParent, rootSolution);
                                child.geoParams = geoParams;
                                for (int i = 0; i < geoParams.Count; i++) {
                                    GeoParam.IGeoParam geoParam = geoParams[i];
                                    if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                        GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                        child.position = posParam.pos;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                        GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                        child.dir = dirParam.dir;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_P_W) {
                                        GeoParam.CR_P_Width pParam = (GeoParam.CR_P_Width)geoParam;
                                        child.wireWidth = pParam.width;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_P_CD) {
                                        GeoParam.CR_P_CupDepth pParam = (GeoParam.CR_P_CupDepth)geoParam;
                                        child.cupDepth = pParam.cupDepth;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_P_BD) {
                                        GeoParam.CR_P_BoardtDepth pParam = (GeoParam.CR_P_BoardtDepth)geoParam;
                                        child.boardDepth = pParam.boardDepth;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_P_BF) {
                                        GeoParam.CR_P_BoardtFirst pParam = (GeoParam.CR_P_BoardtFirst)geoParam;
                                        child.boardFirst = pParam.boardFirst;
                                    }
                                }
                                child.id = next.id;
                                possibleStpes.Add(child);
                            }
                            if (next.next.elementType == LGSCollisionData.ElementType.L && !next.backward) {
                                next.next.branchCount++;
                            }
                            break;
                        case ElementType.L:
                            for (int x = 0; x < 1; x++) {
                                geoParams = embedder.charGeoEmbbed(next, next.next, next.backward, random);
                                if (next.next is MCTSNode) {
                                    childParent = (MCTSNode)next.next;
                                }
                                var child = new MCTSNode(solution, next, childParent, rootSolution);
                                child.geoParams = geoParams;
                                for (int i = 0; i < geoParams.Count; i++) {
                                    GeoParam.IGeoParam geoParam = geoParams[i];
                                    if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                        GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                        child.position = posParam.pos;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_DIRECTION) {
                                        GeoParam.CR_Direction dirParam = (GeoParam.CR_Direction)geoParam;
                                        child.dir = dirParam.dir;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_L_BO) {
                                        GeoParam.CR_L_BallOffset lParam = (GeoParam.CR_L_BallOffset)geoParam;
                                        child.ballOffset = lParam.offset;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_L_SL) {
                                        GeoParam.CR_L_SlopeLength lParam = (GeoParam.CR_L_SlopeLength)geoParam;
                                        child.slopeLength = lParam.length;
                                    }
                                    else if (geoParam.type == GeoParam.GeoParamType.CR_L_OP) {
                                        GeoParam.CR_L_OutPoint lParam = (GeoParam.CR_L_OutPoint)geoParam;
                                        child.outPoints = lParam.out_points;
                                    }
                                }
                                child.branchCount = 0;
                                child.id = next.id;
                                possibleStpes.Add(child);
                            }
                            break;
                        case ElementType.C:
                            for (int x = 0; x < 1; x++) {
                                geoParams = embedder.charGeoEmbbed(next, next.next, next.backward, random);
                                if (next.next is MCTSNode) {
                                    childParent = (MCTSNode)next.next;
                                }
                                var child = new MCTSNode(solution, next, childParent, rootSolution);
                                child.geoParams = geoParams;
                                for (int i = 0; i < geoParams.Count; i++) {
                                    GeoParam.IGeoParam geoParam = geoParams[i];
                                    if (geoParam.type == GeoParam.GeoParamType.CR_POSITION) {
                                        GeoParam.CR_Position posParam = (GeoParam.CR_Position)geoParam;
                                        child.position = posParam.pos;
                                    }
                                }
                                child.isEnd = true;
                                child.id = next.id;
                                possibleStpes.Add(child);
                            }
                            break;
                    }
                }
            }

            return possibleStpes;
        }

        public MCTSNode Expansion(CR_LMCTS lmcts) {
            if(this.position != null) {
                Debug.Log("curr pos: " + this.position.ToString());
            }
            children.AddRange(GetPossibleSteps(lmcts));

            if (children.Count > 0) {
                var child = children[children.Count - 1];
                var prev = child.list_prev;
                while (prev != null) {
                    if (prev.next.id == child.id) {
                        prev.next = child;
                    }
                    prev = prev.list_prev;
                }
                if (this.list_prev is MCTSNode) {
                    if (((MCTSNode)this.list_prev).childern != null) {
                        child.childern.AddRange(((MCTSNode)this.list_prev).childern);
                    }
                }
                //this.list_prev = child;
                child.list_next = this;
                return child;
            }
            else {
                this.parent.children.AddRange(this.parent.GetPossibleSteps(lmcts));
                if (this.parent.children.Count > 0) {
                    var child = this.parent.children[parent.children.Count - 1];
                    var prev = child.list_prev;
                    while (prev != null) {
                        if (prev.next.id == child.id) {
                            prev.next = child;
                        }
                        prev = prev.list_prev;
                    }
                    if (this.parent.list_prev is MCTSNode) {
                        if (((MCTSNode)this.parent.list_prev).childern != null) {
                            child.childern.AddRange(((MCTSNode)this.parent.list_prev).childern);
                        }
                    }
                    //this.parent.list_prev = child;
                    child.list_next = this.parent;
                    return child;
                }
                else {
                    return this;
                }
            }
        }

        public async Task<System.Tuple<bool, MCTSNode>> SimulationNoAnimation(CR_LMCTS lmcts) {
            bool success = false, first = true, finish = false;
            float timeScale = Time.timeScale;
            var step = this;
            var steps = step.GetPossibleSteps(lmcts);
            int counter = 0;
            int limit = 7;
            if (!lmcts.inverse) {
                limit = 1;
            }

            this.solution.Init(lmcts);

            while (steps.Count > 0) {
                var child = steps[steps.Count - 1];
                var prev = child.list_prev;
                while (prev != null) {
                    if (prev.next.id == child.id) {
                        prev.next = child;
                    }
                    prev = prev.list_prev;
                }

                //child.list_prev = step.list_prev.list_prev;
                //child.list_next = step.list_prev.list_next;
                //child.prev = step.list_prev.prev;
                //child.next = step.list_prev.next;
                //step.list_prev = child;
                child.list_next = step;
                lmcts.geoParams[child.id] = child.geoParams;
                step = child;
                steps = step.GetPossibleSteps(lmcts);
            }

            do {
                success = false;
                finish = false;

                if (!first) {
                    step = this;
                    this.possibleStpes = new List<MCTSNode>();
                    steps = step.GetPossibleSteps(lmcts);

                    while (steps.Count > 0) {
                        var child = steps[steps.Count - 1];
                        var prev = child.list_prev;
                        while (prev != null) {
                            if (prev.next.id == child.id) {
                                prev.next = child;
                            }
                            prev = prev.list_prev;
                        }
                        //step.list_prev = child;
                        child.list_next = step;
                        lmcts.geoParams[child.id] = child.geoParams;
                        step = child;
                        steps = step.GetPossibleSteps(lmcts);
                    }
                }

                UnityEditor.EditorApplication.isPlaying = true;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                if (Application.isPlaying) {
                    source.Cancel();
                    List<Task<bool>> unitTasks = new List<Task<bool>>();
                    List<Task<System.Tuple<bool, float>>> goalTasks = new List<Task<System.Tuple<bool, float>>>();
                    Task<System.Tuple<bool, float>> goalTask;

                    lmcts.initScene();

                    step.Apply(lmcts.geoParams, lmcts.crInstance);
                    lmcts.startTime = Time.time;

                    lmcts.setCheckpoint();
                    List<int> ends = lmcts.setTarget();

                    //// 開始計時各個單元
                    //for (int i = 1; i < lmcts.crInstance.primitives.Count; i++) {
                    //    unitTasks.Add(lmcts.AsyncCheckCheckpoint(i, 2f / timeScale, token));
                    //}
                    //// 終點倒計時
                    //goalTask = lmcts.AsyncCheckTarget(((lmcts.crInstance.primitives.Count+1) * 2f + 3) / timeScale, token);
                    // 開始計時各個單元
                    var node = lmcts.root.solution.collisions.First();
                    int tasksCount = 0;
                    List<int> taskId = new List<int>();
                    while (node != null) {
                        if (node.elementType != LGSCollisionData.ElementType.C) {
                            unitTasks.Add(lmcts.AsyncCheckCheckpoint(node.layer, node.id, 3f / timeScale, token));
                            taskId.Add(node.id);
                            tasksCount++;
                        }

                        node = node.list_next;
                    }
                    // 終點倒計時
                    for (int i = 0; i < ends.Count; i++) {
                        goalTasks.Add(lmcts.AsyncCheckTarget(ends[i], ((lmcts.root.solution.maxLayer + 1) * 3f + 3) / timeScale, token));
                    }
                    //goalTask = lmcts.AsyncCheckTarget(((lmcts.root.solution.maxLayer + 1) * 2f + 3) / timeScale, token);

                    // 依序等待單元回傳是否照順序碰撞
                    for (int i = 0; i < tasksCount; i++) {
                        bool unitSuccess = await unitTasks[i];
                        if (!unitSuccess) {
                            success = false;
                            finish = true;
                            source.Cancel();
                            counter++;
                            Debug.Log("fail");
                            Debug.Log(taskId[i]);
                            break;
                        }
                    }
                    // 確認是否到達終點
                    if (!finish) {
                        for (int i = 0; i < ends.Count; i++) {
                            System.Tuple<bool, float> reachGoal = await goalTasks[i];
                            if (!reachGoal.Item1) {
                                success = false;
                                finish = true;
                                source.Cancel();
                                counter++;
                                Debug.Log("no goal");
                            }
                            else {
                                if (i > 0 && (success && finish)) {
                                    success = true;
                                    finish = true;
                                    if (lmcts.endTime < reachGoal.Item2) {
                                        lmcts.endTime = reachGoal.Item2;
                                    }
                                }
                                else if (i == 0) {
                                    success = true;
                                    finish = true;
                                    lmcts.endTime = reachGoal.Item2;
                                }

                                source.Cancel();
                            }
                        }
                    }
                }
                source.Cancel();
                first = false;
                // 若沒有照順序或到達終點，重新生成一組幾何參數
            } while (finish && !success && counter < limit);


            lmcts.recorder.record = false;
            lmcts.recorder.m_Recorder.ResetRecording();

            float time = success ? (lmcts.endTime - lmcts.startTime) : 0;
            step.score = lmcts.endTime - lmcts.startTime;
            if (time > lmcts.target * 2) {
                time = lmcts.target * 2;
            }
            step.fitness = 1 - Mathf.Abs(time - lmcts.target) / lmcts.target;

            //float customScore = success ? lmcts.RBMScore() : 0;
            //step.score = customScore;
            //if (customScore > lmcts.target * 2) {
            //    customScore = lmcts.target * 2;
            //}
            //step.fitness = 1 - Mathf.Abs(customScore - lmcts.target) / lmcts.target;

            Debug.Log(step.score);
            Debug.Log(step.fitness);

            return new System.Tuple<bool, MCTSNode>(success, step);
        }

        public async Task<System.Tuple<bool, MCTSNode>> Simulation(CR_LMCTS lmcts) {
            bool success = false, first = true, finish = false;
            float timeScale = Time.timeScale;
            var step = this;
            var steps = step.GetPossibleSteps(lmcts);
            int counter = 0;
            int limit = 7;
            if (!lmcts.inverse) {
                limit = 1;
            }

            this.solution.Init(lmcts);

            while (steps.Count > 0) {
                var child = steps[steps.Count - 1];
                var prev = child.list_prev;
                while (prev != null) {
                    if (prev.next.id == child.id) {
                        prev.next = child;
                    }
                    prev = prev.list_prev;
                }

                //child.list_prev = step.list_prev.list_prev;
                //child.list_next = step.list_prev.list_next;
                //child.prev = step.list_prev.prev;
                //child.next = step.list_prev.next;
                //step.list_prev = child;
                child.list_next = step;
                lmcts.geoParams[child.id] = child.geoParams;
                step = child;
                steps = step.GetPossibleSteps(lmcts);
            }

            do {
                success = false;
                finish = false;

                if (!first) {
                    step = this;
                    this.possibleStpes = new List<MCTSNode>();
                    steps = step.GetPossibleSteps(lmcts);

                    while (steps.Count > 0) {
                        var child = steps[steps.Count - 1];
                        var prev = child.list_prev;
                        while (prev != null) {
                            if (prev.next.id == child.id) {
                                prev.next = child;
                            }
                            prev = prev.list_prev;
                        }
                        //step.list_prev = child;
                        child.list_next = step;
                        lmcts.geoParams[child.id] = child.geoParams;
                        step = child;
                        steps = step.GetPossibleSteps(lmcts);
                    }
                }

                UnityEditor.EditorApplication.isPlaying = true;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                lmcts.recorder.clip = new AnimationClip();
                if (Application.isPlaying) {
                    source.Cancel();
                    List<Task<bool>> unitTasks = new List<Task<bool>>();
                    List<Task<System.Tuple<bool, float>>> goalTasks = new List<Task<System.Tuple<bool, float>>>();
                    Task<System.Tuple<bool, float>> goalTask;

                    lmcts.initScene();
                    //for (int i = 0; i < lmcts.crInstance.primitives.Count; i++) {
                    //    Debug.Log(lmcts.crInstance.primitives[i].name);
                    //}
                    step.Apply(lmcts.geoParams, lmcts.crInstance);
                    lmcts.startTime = Time.time;
                    lmcts.recorder.m_Recorder.ResetRecording();
                    lmcts.recorder.m_Recorder.BindComponentsOfType<Transform>(lmcts.crInstance.primitiveRoot.gameObject, true);
                    for (int i = 0; i < lmcts.crInstance.primitives.Count; i++) {
                        if (lmcts.lSystem.lString.symbols[i].alphabet == 'P') {
                            Transform[] children;
                            if (lmcts.lSystem.lString.symbols[i - 1].alphabet == '[') {
                                if (lmcts.lSystem.lString.symbols[i - 2].alphabet == 'L') {
                                    children = lmcts.crInstance.primitives[i - 2].GetComponentsInChildren<Transform>();
                                }
                                else {
                                    break;
                                }
                            }
                            else {
                                children = lmcts.crInstance.primitives[i - 1].GetComponentsInChildren<Transform>();
                            }
                            foreach (Transform child in children) {
                                string path = UnityEditor.AnimationUtility.CalculateTransformPath(child, lmcts.crInstance.primitiveRoot.transform);
                                //Debug.Log(path);
                                UnityEditor.EditorCurveBinding binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(MeshRenderer), "m_Enabled");
                                lmcts.recorder.m_Recorder.Bind(binding);
                            }
                        }
                    }
                    for (int i = 0; i < lmcts.crInstance.primitives.Count; i++) {
                        LSystem.ISymbol symbol = lmcts.lSystem.lString.symbols[i];
                        if (symbol.alphabet == 'B') {
                            string path = UnityEditor.AnimationUtility.CalculateTransformPath(lmcts.crInstance.primitives[i].transform, lmcts.crInstance.primitiveRoot.transform);
                            UnityEditor.EditorCurveBinding binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(BallOnSlope), "hitTarget");
                            //Debug.Log("Bind B: " + i.ToString());
                            lmcts.recorder.m_Recorder.Bind(binding);
                        }
                        else if (symbol.alphabet == 'D') {
                            string path = UnityEditor.AnimationUtility.CalculateTransformPath(lmcts.crInstance.primitives[i].transform, lmcts.crInstance.primitiveRoot.transform);
                            UnityEditor.EditorCurveBinding binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(Dominos), "hitTarget");
                            //Debug.Log("Bind D: " + i.ToString());
                            lmcts.recorder.m_Recorder.Bind(binding);
                        }
                        else if (symbol.alphabet == 'P') {
                            string path = UnityEditor.AnimationUtility.CalculateTransformPath(lmcts.crInstance.primitives[i].transform, lmcts.crInstance.primitiveRoot.transform);
                            UnityEditor.EditorCurveBinding binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(UWire), "hitTarget");
                            lmcts.recorder.m_Recorder.Bind(binding);
                            binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(UWire), "cupInit");
                            //Debug.Log("Bind P: " + i.ToString());
                            lmcts.recorder.m_Recorder.Bind(binding);
                        }
                        else if (symbol.alphabet == 'C') {
                            string path = UnityEditor.AnimationUtility.CalculateTransformPath(lmcts.crInstance.primitives[i].transform, lmcts.crInstance.primitiveRoot.transform);
                            UnityEditor.EditorCurveBinding binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(Goal), "success");
                            //Debug.Log("Bind C: " + i.ToString());
                            lmcts.recorder.m_Recorder.Bind(binding);
                        }
                        else if (symbol.alphabet == 'L') {
                            string path = UnityEditor.AnimationUtility.CalculateTransformPath(lmcts.crInstance.primitives[i].transform, lmcts.crInstance.primitiveRoot.transform);
                            UnityEditor.EditorCurveBinding binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(Billiard), "hitTarget0");
                            lmcts.recorder.m_Recorder.Bind(binding);
                            binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(Billiard), "hitTarget1");
                            lmcts.recorder.m_Recorder.Bind(binding);
                            binding = UnityEditor.EditorCurveBinding.FloatCurve(path, typeof(Billiard), "hitTarget2");
                            //Debug.Log("Bind L: " + i.ToString());
                            lmcts.recorder.m_Recorder.Bind(binding);
                        }
                    }
                    lmcts.recorder.record = true;
                    lmcts.recorder.last_time = Time.time;
                    lmcts.recorder.m_Recorder.TakeSnapshot(0);
                    
                    lmcts.setCheckpoint();
                    List<int> ends = lmcts.setTarget();

                    //// 開始計時各個單元
                    //for (int i = 1; i < lmcts.crInstance.primitives.Count; i++) {
                    //    unitTasks.Add(lmcts.AsyncCheckCheckpoint(i, 2f / timeScale, token));
                    //}
                    //// 終點倒計時
                    //goalTask = lmcts.AsyncCheckTarget(((lmcts.crInstance.primitives.Count+1) * 2f + 3) / timeScale, token);
                    // 開始計時各個單元
                    var node = lmcts.root.solution.collisions.First();
                    int tasksCount = 0;
                    List<int> taskId = new List<int>();
                    while (node != null) {
                        if (node.elementType != LGSCollisionData.ElementType.C) {
                            unitTasks.Add(lmcts.AsyncCheckCheckpoint(node.layer, node.id, 3f / timeScale, token));
                            taskId.Add(node.id);
                            tasksCount++;
                        }
                        
                        node = node.list_next;
                    }
                    // 終點倒計時
                    for (int i = 0; i < ends.Count; i++) {
                        goalTasks.Add(lmcts.AsyncCheckTarget(ends[i], ((lmcts.root.solution.maxLayer + 1) * 3f + 3) / timeScale, token));
                    }
                    //goalTask = lmcts.AsyncCheckTarget(((lmcts.root.solution.maxLayer + 1) * 2f + 3) / timeScale, token);

                    // 依序等待單元回傳是否照順序碰撞
                    for (int i = 0; i < tasksCount; i++) {
                        bool unitSuccess = await unitTasks[i];
                        if (!unitSuccess) {
                            success = false;
                            finish = true;
                            source.Cancel();
                            counter++;
                            Debug.Log("fail");
                            Debug.Log(taskId[i]);
                            break;
                        }
                    }
                    // 確認是否到達終點
                    if (!finish) {
                        for (int i = 0; i < ends.Count; i++) {
                            System.Tuple<bool, float> reachGoal = await goalTasks[i];
                            if (!reachGoal.Item1) {
                                success = false;
                                finish = true;
                                source.Cancel();
                                counter++;
                                Debug.Log("no goal");
                            }
                            else {
                                if (i > 0 && (success && finish)) {
                                    success = true;
                                    finish = true;
                                    if (lmcts.endTime < reachGoal.Item2) {
                                        lmcts.endTime = reachGoal.Item2;
                                    }
                                }
                                else if (i == 0) {
                                    success = true;
                                    finish = true;
                                    lmcts.endTime = reachGoal.Item2;
                                }
                                
                                source.Cancel();
                            }
                        }
                        //System.Tuple<bool, float> reachGoal = await goalTask;
                        //if (!reachGoal.Item1) {
                        //    success = false;
                        //    finish = true;
                        //    source.Cancel();
                        //    counter++;
                        //    Debug.Log("no goal");
                        //}
                        //else {
                        //    success = true;
                        //    finish = true;
                        //    lmcts.endTime = reachGoal.Item2;
                        //    source.Cancel();
                        //}

                    }
                }
                source.Cancel();
                first = false;
                // 若沒有照順序或到達終點，重新生成一組幾何參數
            } while (finish && !success && counter < limit);


            lmcts.recorder.record = false;
            lmcts.recorder.m_Recorder.TakeSnapshot(Time.time - lmcts.recorder.last_time);
            lmcts.recorder.m_Recorder.SaveToClip(lmcts.recorder.clip);
            //var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(lmcts.recorder.clip);
            //Debug.Log("End clip");
            //Debug.Log(curveBindings.Length);

            lmcts.recorder.m_Recorder.ResetRecording();

            float time = success ? (lmcts.endTime - lmcts.startTime) : 0;
            step.score = lmcts.endTime - lmcts.startTime;
            if (time > lmcts.target * 2) {
                time = lmcts.target * 2;
            }
            step.fitness = 1 - Mathf.Abs(time - lmcts.target) / lmcts.target;

            //float customScore = success ? lmcts.RBMScore() : 0;
            //step.score = customScore;
            //if (customScore > lmcts.target * 2) {
            //    customScore = lmcts.target * 2;
            //}
            //step.fitness = 1 - Mathf.Abs(customScore - lmcts.target) / lmcts.target;

            //Debug.Log(lmcts.startTime);	
            //Debug.Log(lmcts.endTime);	
            //Debug.Log(customScore);
            Debug.Log(step.score);
            Debug.Log(step.fitness);

            return new System.Tuple<bool, MCTSNode>(success, step);
        }

        public void Backpropagation(float w) {
            this.n += 1;
            this.w += w;

            if (parent != null)
                parent.Backpropagation(w);
        }

        public void Apply(List<List<GeoParam.IGeoParam>> paramsList, ChainReaction.ChainReaction crInstance) {
            primitive = crInstance.primitives[id];
            //Debug.Log(id);
            primitive.reset();
            CR_LMCTS lmcts = new CR_LMCTS();
            if (elementType != ElementType.ROOT) {
                paramsList[id] = geoParams;
            }
            switch (elementType) {
                case ElementType.B:
                    lmcts.applyGeoParams((BallOnSlope)primitive, geoParams);
                    break;
                case ElementType.D:
                    lmcts.applyGeoParams((Dominos)primitive, geoParams);
                    break;
                case ElementType.P:
                    lmcts.applyGeoParams((UWire)primitive, geoParams);
                    break;
                case ElementType.L:
                    lmcts.applyGeoParams((Billiard)primitive, geoParams);
                    break;
                case ElementType.C:
                    lmcts.applyGeoParams((Cup)primitive, geoParams);
                    break;
            }

            if (parent != null) {
                var next_node = list_next;
                while (next_node != null) {
                    if (parent.id == next_node.id && next_node.elementType != ElementType.ROOT && next_node is MCTSNode) {
                        ((MCTSNode)next_node).geoParams = parentGeoParams;
                    }
                    next_node = next_node.list_next;
                }
            }
            if (list_next != null)
                ((MCTSNode)list_next).Apply(paramsList, crInstance);
            else {
                crInstance.genFloors();
            } 
        }
    }
    #endregion
}
