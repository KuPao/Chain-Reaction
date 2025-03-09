using UnityEngine;
using UnityEditor.Animations;

public class RecordTransformHierarchy : MonoBehaviour {
    public AnimationClip clip;
    public bool record = false;
    public bool first_snap = false;

    public GameObjectRecorder m_Recorder;

    public float last_time = 0; 

    void Start() {
        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);
        //clip = new AnimationClip();
        //AnimationCurve translateX = AnimationCurve.Linear(0.0f, 0.0f, 2.0f, 2.0f);
        //clip.legacy = true;
        //clip.SetCurve("", typeof(Transform), "localPosition.x", translateX);

        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }

    void FixedUpdate() {
        if (clip == null)
            return;
        if (record) {
            // Take a snapshot and record all the bindings values for this frame.
            float curr_time = Time.time;
            m_Recorder.TakeSnapshot(curr_time - last_time);
            last_time = curr_time;
        }
        else if (m_Recorder.isRecording) {
            // Save the recorded session to the clip.
            m_Recorder.SaveToClip(clip);
            m_Recorder.ResetRecording();
        }
    }
}