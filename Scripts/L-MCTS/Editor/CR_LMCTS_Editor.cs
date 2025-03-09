using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LGS;

[CustomEditor(typeof(CR_LMCTS))]
public class CR_LMCTS_Edtiro : Editor {
    bool showLString, finish;
    List<List<List<GeoParam.IGeoParam>>> geoParamsList;

    public override async void OnInspectorGUI() {
        CR_LMCTS lmcts = (CR_LMCTS)target;

        lmcts.crInstance = (ChainReaction.ChainReaction)EditorGUILayout.ObjectField(lmcts.crInstance, typeof(ChainReaction.ChainReaction), true);
        lmcts.camera = (GameObject)EditorGUILayout.ObjectField(lmcts.camera, typeof(GameObject), true);
        lmcts.useCamera = (bool)EditorGUILayout.Toggle(lmcts.useCamera);

        string showLStringText = "LSystem";
        if (!showLString) {
            showLStringText = showLStringText + " (" + lmcts.lSystem.lString.toString() + ")";
        }
        showLString = EditorGUILayout.Foldout(showLString, showLStringText);
        if (showLString) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current String: " + lmcts.lSystem.lString.toString());
            if (GUILayout.Button("Reset")) {
                lmcts.lSystem.lString = lmcts.lSystem.getAxiom();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Possible Candidates:");
            EditorGUI.indentLevel += 2;
            {
                List<LSystem.ILString> candidates = lmcts.lSystem.getExtendCandidates();
                for (int i = 0; i < candidates.Count; i++) {
                    //Debug.Log(i);
                    //Debug.Log(candidates[i].toString());
                    for (int j = i + 1; j < candidates.Count; j++) {
                        if (candidates[i].toString() == candidates[j].toString()) {
                            candidates.RemoveAt(j--);
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(candidates[i].toString());
                    if (GUILayout.Button("Choose this!")) {
                        lmcts.lSystem.lString = candidates[i];
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel -= 2;
        }
        if (GUILayout.Button("Init Scene")) {
            lmcts.initScene();
        }
        if (GUILayout.Button("Embedd GeoParams")) {
            float startTime = Time.time;
            lmcts.genRandomGeoParams();
            lmcts.applyGeoParams();
            LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
            lmcts.setMCTS(solution);
            lmcts.startCamera();
            Debug.Log(lmcts.RBMScore());
            lmcts.setCheckpoint();
            lmcts.setTarget();

        }
        if (GUILayout.Button("Replay Current")) {
            lmcts.initScene();
            lmcts.applyGeoParams();
            LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
            lmcts.setMCTS(solution);
            lmcts.startCamera();
            Debug.Log(lmcts.RBMScore());
            lmcts.setCheckpoint();
            lmcts.setTarget();

        }
        //if (GUILayout.Button("Embedd GeoParams")) {
        //    float startTime = Time.time;
        //    lmcts.genRandomGeoParams();
        //    lmcts.applyGeoParams();
        //    LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
        //    lmcts.setMCTS(solution);
        //    lmcts.startCamera();
        //    Debug.Log(lmcts.RBMScore());
        //    lmcts.setCheckpoint();
        //    lmcts.setTarget();

        //}
        List<List<GeoParam.IGeoParam>> geoParams = lmcts.geoParams;
        if (GUILayout.Button("Solution Test")) {
            LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
            //solution.lstring = lmcts.lSystem.lString;
            //lmcts.setMCTS(solution);
            //var chosen = 
            solution.printSolution();
        }
        if (GUILayout.Button("Record Test")) {
            lmcts.genRandomGeoParams();
            lmcts.applyGeoParams();
            geoParams = lmcts.geoParams;
            RecordTransformHierarchy recorder = lmcts.crInstance.primitiveRoot.GetComponent<RecordTransformHierarchy>();
            recorder.clip = new AnimationClip();
            recorder.m_Recorder.BindComponentsOfType<Transform>(lmcts.crInstance.primitiveRoot.gameObject, true);
            recorder.last_time = Time.time;
            recorder.m_Recorder.TakeSnapshot(0);
            recorder.record = true;
            lmcts.setCheckpoint();
            lmcts.setTarget();
        }
        if (GUILayout.Button("Replay")) {
            lmcts.geoParams = geoParams;
            lmcts.applyGeoParams();
            lmcts.disablePhysic();
            RecordTransformHierarchy recorder = lmcts.crInstance.primitiveRoot.GetComponent<RecordTransformHierarchy>();
            Animation animation = lmcts.crInstance.primitiveRoot.GetComponent<Animation>();
            var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(recorder.clip);
            Debug.Log(curveBindings.Length);
            recorder.clip.legacy = true;
            animation.clip = recorder.clip;
            animation.AddClip(animation.clip, "test");
            animation.Play("test");
        }
        lmcts.target = EditorGUILayout.FloatField("Time", lmcts.target);
        //lmcts.target = EditorGUILayout.FloatField("Time", lmcts.target);
        if (GUILayout.Button("Random")) {
            UnityEditor.EditorApplication.isPlaying = false;
            LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
            solution.lstring = lmcts.lSystem.lString;
            lmcts.setMCTS(solution);
            lmcts.RandomGen(5, true, 1000);
        }
        if (GUILayout.Button("Random 10 Times")) {
            lmcts.target = 10.0f;
            string folder = @"D:\L_MCTS\Random\" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + lmcts.target.ToString() + "_g8" + "\\";
            System.IO.Directory.CreateDirectory(folder);
            for (int i = 0; i < 10; i++) {
                UnityEditor.EditorApplication.isPlaying = false;
                lmcts.lSystem.lString = new LSystem.CR_LString(new LSystem.ISymbol[] { LSystem.CR_LSystem.Ball_Symbol, LSystem.CR_LSystem.Cup_Symbol });
                LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
                solution.lstring = lmcts.lSystem.lString;
                lmcts.setMCTS(solution);
                
                await lmcts.RandomGen(8, true, 1000);

                if (lmcts.finish) {
                    string fileName = i.ToString() + ".txt";
                    string fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        sw.WriteLine(lmcts.root.rootSolution.SelectList().Count.ToString());
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].fitness.ToString());
                        }
                    }
                    fileName = i.ToString() + "_strings.txt";
                    fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].solution.lstring.toString());
                        }
                    }
                    lmcts.finish = false;
                }
            }
            folder = @"D:\L_MCTS\Random\" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + lmcts.target.ToString() + "_g9" + "\\";
            System.IO.Directory.CreateDirectory(folder);
            for (int i = 0; i < 10; i++) {
                UnityEditor.EditorApplication.isPlaying = false;
                lmcts.lSystem.lString = new LSystem.CR_LString(new LSystem.ISymbol[] { LSystem.CR_LSystem.Ball_Symbol, LSystem.CR_LSystem.Cup_Symbol });
                LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
                solution.lstring = lmcts.lSystem.lString;
                lmcts.setMCTS(solution);

                await lmcts.RandomGen(9, true, 1000);

                if (lmcts.finish) {
                    string fileName = i.ToString() + ".txt";
                    string fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        sw.WriteLine(lmcts.root.rootSolution.SelectList().Count.ToString());
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].fitness.ToString());
                        }
                    }
                    fileName = i.ToString() + "_strings.txt";
                    fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].solution.lstring.toString());
                        }
                    }
                    lmcts.finish = false;
                }
            }
        }

        if (GUILayout.Button("Generate")) {
            UnityEditor.EditorApplication.isPlaying = false;
            LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
            solution.lstring = lmcts.lSystem.lString;
            lmcts.setMCTS(solution);
            await lmcts.Search(true);
        }
        if (GUILayout.Button("Generate 10 Times")) {
            lmcts.target = 10.0f;
            lmcts.date = System.DateTime.Now.ToString("MM-dd-yyyy");
            string folder = @"D:\L_MCTS\MCTS_inverse\" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + lmcts.target.ToString() + "\\";
            //System.IO.Directory.CreateDirectory(folder);
            //for (int i = 0; i < 1; i++) {
            //    UnityEditor.EditorApplication.isPlaying = false;
            //    //lmcts.lSystem.lString = new LSystem.CR_LString(new LSystem.ISymbol[] { LSystem.CR_LSystem.Ball_Symbol, LSystem.CR_LSystem.Cup_Symbol });
            //    lmcts.lSystem.lString = lmcts.lSystem.stringToLString("BC");
            //    LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
            //    solution.lstring = lmcts.lSystem.lString;
            //    lmcts.setMCTS(solution);

            //    await lmcts.Search(true);

            //    if (lmcts.finish) {
            //        string fileName = i.ToString() + ".txt";
            //        string fullPath = folder + fileName;
            //        using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
            //            sw.WriteLine(lmcts.root.rootSolution.SelectList().Count.ToString());
            //            for (int j = 0; j < lmcts.result.Count; j++) {
            //                sw.WriteLine(lmcts.result[j].fitness.ToString());
            //            }
            //        }
            //        fileName = i.ToString() + "_strings.txt";
            //        fullPath = folder + fileName;
            //        using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
            //            for (int j = 0; j < lmcts.result.Count; j++) {
            //                sw.WriteLine(lmcts.result[j].solution.lstring.toString());
            //            }
            //        }
            //        lmcts.finish = false;
            //    }
            //}
            lmcts.date = System.DateTime.Now.ToString("MM-dd-yyyy");

            lmcts.target = 10.0f;
            folder = @"D:\L_MCTS\Random_inverse\" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + lmcts.target.ToString() + "\\";
            System.IO.Directory.CreateDirectory(folder);
            lmcts.date = System.DateTime.Now.ToString("MM-dd-yyyy");
            for (int i = 0; i < 1; i++) {
                UnityEditor.EditorApplication.isPlaying = false;
                //lmcts.lSystem.lString = new LSystem.CR_LString(new LSystem.ISymbol[] { LSystem.CR_LSystem.Ball_Symbol, LSystem.CR_LSystem.Cup_Symbol });
                lmcts.lSystem.lString = lmcts.lSystem.stringToLString("BC");
                LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
                solution.lstring = lmcts.lSystem.lString;
                lmcts.setMCTS(solution);

                await lmcts.RandomGen(12, true, 1000);

                if (lmcts.finish) {
                    string fileName = i.ToString() + ".txt";
                    string fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        sw.WriteLine(lmcts.root.rootSolution.SelectList().Count.ToString());
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].fitness.ToString());
                        }
                    }
                    fileName = i.ToString() + "_strings.txt";
                    fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].solution.lstring.toString());
                        }
                    }
                    lmcts.finish = false;
                }
            }

            lmcts.target = 10.0f;
            folder = @"D:\L_MCTS\MCTS_random\" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + lmcts.target.ToString() + "\\";
            System.IO.Directory.CreateDirectory(folder);
            lmcts.date = System.DateTime.Now.ToString("MM-dd-yyyy");
            for (int i = 0; i < 1; i++) {
                UnityEditor.EditorApplication.isPlaying = false;
                //lmcts.lSystem.lString = new LSystem.CR_LString(new LSystem.ISymbol[] { LSystem.CR_LSystem.Ball_Symbol, LSystem.CR_LSystem.Cup_Symbol });
                lmcts.lSystem.lString = lmcts.lSystem.stringToLString("BC");
                LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
                solution.lstring = lmcts.lSystem.lString;
                lmcts.setMCTS(solution);

                await lmcts.Search(false);

                if (lmcts.finish) {
                    string fileName = i.ToString() + ".txt";
                    string fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        sw.WriteLine(lmcts.root.rootSolution.SelectList().Count.ToString());
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].fitness.ToString());
                        }
                    }
                    fileName = i.ToString() + "_strings.txt";
                    fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].solution.lstring.toString());
                        }
                    }
                    lmcts.finish = false;
                }
            }

            lmcts.target = 1.0f;
            folder = @"D:\L_MCTS\Random_random\" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + lmcts.target.ToString() + "\\";
            System.IO.Directory.CreateDirectory(folder);
            lmcts.date = System.DateTime.Now.ToString("MM-dd-yyyy");
            for (int i = 0; i < 1; i++) {
                UnityEditor.EditorApplication.isPlaying = false;
                //lmcts.lSystem.lString = new LSystem.CR_LString(new LSystem.ISymbol[] { LSystem.CR_LSystem.Ball_Symbol, LSystem.CR_LSystem.Cup_Symbol });
                lmcts.lSystem.lString = lmcts.lSystem.stringToLString("BC");
                LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
                solution.lstring = lmcts.lSystem.lString;
                lmcts.setMCTS(solution);

                await lmcts.RandomGen(12, false, 1000);

                if (lmcts.finish) {
                    string fileName = i.ToString() + ".txt";
                    string fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        sw.WriteLine(lmcts.root.rootSolution.SelectList().Count.ToString());
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].fitness.ToString());
                        }
                    }
                    fileName = i.ToString() + "_strings.txt";
                    fullPath = folder + fileName;
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
                        for (int j = 0; j < lmcts.result.Count; j++) {
                            sw.WriteLine(lmcts.result[j].solution.lstring.toString());
                        }
                    }
                    lmcts.finish = false;
                }
            }
            //lmcts.target = 5.0f;
            //folder = @"D:\L_MCTS\MCTS\" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + lmcts.target.ToString() + "\\";
            //System.IO.Directory.CreateDirectory(folder);
            //for (int i = 0; i < 10; i++) {
            //    UnityEditor.EditorApplication.isPlaying = false;
            //    lmcts.lSystem.lString = new LSystem.CR_LString(new LSystem.ISymbol[] { LSystem.CR_LSystem.Ball_Symbol, LSystem.CR_LSystem.Cup_Symbol });
            //    LGSSolution solution = new LGSSolution(lmcts.lSystem.lString.toString(), lmcts);
            //    solution.lstring = lmcts.lSystem.lString;
            //    lmcts.setMCTS(solution);

            //    await lmcts.Search();

            //    if (lmcts.finish) {
            //        string fileName = i.ToString() + ".txt";
            //        string fullPath = folder + fileName;
            //        using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
            //            sw.WriteLine(lmcts.root.rootSolution.SelectList().Count.ToString());
            //            for (int j = 0; j < lmcts.result.Count; j++) {
            //                sw.WriteLine(lmcts.result[j].fitness.ToString());
            //            }
            //        }
            //        fileName = i.ToString() + "_strings.txt";
            //        fullPath = folder + fileName;
            //        using (System.IO.StreamWriter sw = System.IO.File.CreateText(fullPath)) {
            //            for (int j = 0; j < lmcts.result.Count; j++) {
            //                sw.WriteLine(lmcts.result[j].solution.lstring.toString());
            //            }
            //        }
            //        lmcts.finish = false;
            //    }
            //}
        }

        if (lmcts.finish) {
            GUILayout.Label("Possible Candidates:");
            EditorGUI.indentLevel += 2;
            {
                List<CR_LMCTS.MCTSNode> candidates = lmcts.candidates;
                List<System.Tuple<AnimationClip, float>> clips = lmcts.clips;
                for (int i = 0; i < candidates.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("candidates" + i.ToString());
                    if (GUILayout.Button("Choose this!")) {
                        lmcts.lSystem.lString = candidates[i].solution.lstring;
                        lmcts.initScene();
                        candidates[i].solution.Init(lmcts);
                        candidates[i].Apply(lmcts.geoParams, lmcts.crInstance);
                        lmcts.setTarget();
                        lmcts.startCamera();
                        lmcts.disablePhysic();
                        lmcts.setAnimation(i);
                        Debug.Log(candidates[i].solution.high_score);
                        Debug.Log(candidates[i].fitness);
                        Debug.Log(candidates[i].score);
                        //lmcts.setTarget();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel -= 2;
        }
    }
}