using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool hadCollision = false, success = false;
    private float collisionTime = -1;
    public float endTime = 0;
    public System.Tuple<bool, float> goal = new System.Tuple<bool, float>(false, 0);
    private RecordTransformHierarchy recorder;
    private Animation animation;

    private void Update() {
        if (recorder == null) {
            recorder = gameObject.GetComponentInParent<RecordTransformHierarchy>();
            animation = gameObject.GetComponentInParent<Animation>();
        }
        if (hadCollision && (Time.time - collisionTime) > 3) {
            if (!success) {
                endTime = Time.time;
                goal = new System.Tuple<bool, float>(true, endTime - 3);
                //Debug.Log(endTime - 3);
                //recorder.record = false;
                //recorder.m_Recorder.TakeSnapshot(Time.time - recorder.last_time);
                //recorder.m_Recorder.SaveToClip(recorder.clip);
                //var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(recorder.clip);
                //Debug.Log("End clip");
                //Debug.Log(curveBindings.Length);
                //recorder.m_Recorder.ResetRecording();
            }
            success = true;
        }
    }
    void OnCollisionEnter(Collision target) {
        if (target.gameObject.tag.Equals("Target") == true) {
            collisionTime = Time.time;
            hadCollision = true;
        }
    }

    void OnCollisionExit(Collision target) {
        if (target.gameObject.tag.Equals("Target") == true) {
            hadCollision = false;
        }
    }
}
